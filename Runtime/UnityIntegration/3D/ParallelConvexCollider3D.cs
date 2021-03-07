using UnityEngine;
using System;
using System.Collections.Generic;

namespace Parallel
{
    [RequireComponent(typeof(ParallelTransform))]
    public class ParallelConvexCollider3D : ParallelCollider3D
    {
        MeshFilter meshFilter;
        Mesh mesh;
        public Vector3[] verts = new Vector3[0];
        public int vertsCount = 0;

        public bool UpdateConvexData;
        public UInt32 limit = 24;

        public ParallelQHullData convexData;
        public ParallelQHullData2 convexData2;

        public bool draw2;

        Fix64Vec3 _currentSize = Fix64Vec3.one;

        public void BuildConvexData()
        {
            //stan hull
            Vector3[] vIn1 = new Vector3[vertsCount];
            for (int i = 0; i < vertsCount; i++)
            {
                vIn1[i] = verts[i];
            }
            Debug.Log("Build: " + limit);
            ParallelQHullData2 qhullData2 = Parallel3D.ConvextHull3D2(vIn1, (UInt32)vertsCount, (int)limit);

            convexData2 = qhullData2;

            ParallelIntTriangle[] t = new ParallelIntTriangle[convexData2.triCount];
            Array.Copy(convexData2.tris, 0, t, 0, convexData2.triCount);
            convexData2.tris = t;

            HashSet<int> triVertsIndex = new HashSet<int>();

            for (int i = 0; i < convexData2.triCount; i++)
            {
                ParallelIntTriangle tri = convexData2.tris[i];
                triVertsIndex.Add(tri.v1);
                triVertsIndex.Add(tri.v2);
                triVertsIndex.Add(tri.v3);
            }

            Vector3[] convex2Verts = new Vector3[triVertsIndex.Count];

            for (int i = 0; i < triVertsIndex.Count; i++)
            {
                convex2Verts[i] = convexData2.vertices[i];
            }

            convexData2.vertices = convex2Verts;

            //new convex hull
            Fix64Vec3[] vIn = new Fix64Vec3[triVertsIndex.Count];
            for (int i = 0; i < triVertsIndex.Count; i++)
            {
                vIn[i] = (Fix64Vec3)convexData2.vertices[i];
            }

            ParallelQHullData qhullData = Parallel3D.ConvextHull3D(vIn, (UInt32)triVertsIndex.Count, true, (Fix64)(0.1f));

            convexData = qhullData;

            Fix64Vec3[] v = new Fix64Vec3[convexData.vertexCount];
            Array.Copy(convexData.vertices, 0, v, 0, convexData.vertexCount);
            convexData.vertices = v;

            string output = "";
            output += $"b3Vec3 verts[{convexData.vertexCount}] = {{}};\n";
            //Debug.Log($"b3Vec3 verts[{convexData.vertexCount}] = {{}};");
            for (int i = 0; i < convexData.vertexCount; i++)
            {
                Vector3 vec3 = (Vector3)convexData.vertices[i];
                output += $"verts[{i}] = b3Vec3({vec3.x}, {vec3.y}, {vec3.z});\n";
                //Debug.Log($"verts[{i}] = b3Vec3({vec3.x}, {vec3.y}, {vec3.z});");
            }
            Debug.Log(output);

            ParallelEdge[] e = new ParallelEdge[convexData.edgeCount];
            Array.Copy(convexData.edges, 0, e, 0, convexData.edgeCount);
            convexData.edges = e;

            ParallelFace[] f = new ParallelFace[convexData.faceCount];
            Array.Copy(convexData.faces, 0, f, 0, convexData.faceCount);
            convexData.faces = f;

            ParallelPlane[] p = new ParallelPlane[convexData.faceCount];
            Array.Copy(convexData.planes, 0, p, 0, convexData.faceCount);
            convexData.planes = p;

            return;
        }

        void Reset()
        {
            MeshFilter meshFilter = GetComponent<MeshFilter>();

            Mesh mesh = meshFilter.sharedMesh;

            verts = mesh.vertices;
            vertsCount = verts.Length;

            BuildConvexData();
        }

        void DrawConvex2()
        {
            for (int i = 0; i < convexData2.triCount; i++)
            {
                ParallelIntTriangle t = convexData2.tris[i];
                Vector3 v1 = convexData2.vertices[t.v1];
                Vector3 v2 = convexData2.vertices[t.v2];
                Vector3 v3 = convexData2.vertices[t.v3];

                Gizmos.DrawLine(transform.TransformPoint(v1.x, v1.y, v1.z), transform.TransformPoint(v2.x, v2.y, v2.z));
                Gizmos.DrawLine(transform.TransformPoint(v1.x, v1.y, v1.z), transform.TransformPoint(v3.x, v3.y, v3.z));
                Gizmos.DrawLine(transform.TransformPoint(v3.x, v3.y, v3.z), transform.TransformPoint(v2.x, v2.y, v2.z));
            }
        }

        void DrawConvex1()
        {
            for (UInt32 i = 0; i < convexData.edgeCount; i += 2)
            {
                ParallelEdge edge1 = convexData.edges[i];
                ParallelEdge edge2 = convexData.edges[i + 1];

                Vector3 v1 = (Vector3)convexData.vertices[edge1.origin];
                Vector3 v2 = (Vector3)convexData.vertices[edge2.origin];

                Gizmos.DrawLine(transform.TransformPoint(v1.x, v1.y, v1.z), transform.TransformPoint(v2.x, v2.y, v2.z));
            }

            foreach (Fix64Vec3 fv in convexData.vertices)
            {
                Vector3 v = (Vector3)fv;
                Gizmos.DrawWireSphere(transform.TransformPoint(v.x, v.y, v.z), 0.01f);
            }
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = ParallelUtil.ColliderOutlineColor;

            if(draw2)
            {
                DrawConvex2();
            }
            else
            {
                DrawConvex1();
            }
        }

        Fix64Vec3 CalculateSize()
        {
            Fix64Vec3 s = pTransform.localScale;

            return s;
        }

        public void UpdateVertsLimit(UInt32 newLimit)
        {
            limit = newLimit;
            BuildConvexData();
        }

        public override void UpdateShape(GameObject root)
        {
            Fix64Vec3 s = CalculateSize();

            if (s == Fix64Vec3.zero)
            {
                return;
            }

            if (s == _currentSize)
            {
                return;
            }

            Fix64Vec3 scale = s / _currentSize;

            _currentSize = s;

            Parallel3D.UpdatePolyhedron(_shape, _fixture, scale);
        }

        public override PShape3D CreateShape(GameObject root)
        {
            Fix64Vec3 s = CalculateSize();

            if (s != Fix64Vec3.zero)
            {
                _currentSize = s;

                Fix64Vec3 center = Fix64Vec3.zero;
                Fix64Quat rotation = Fix64Quat.identity;

                if (gameObject != root)
                {
                    center = _pTransform.localPosition;
                    rotation = _pTransform.localRotation;
                }

                _shape = Parallel3D.CreatePolyhedron(convexData, s, center, rotation);

                if(createUnityPhysicsCollider)
                {
                    var collider = gameObject.AddComponent<MeshCollider>();
                    collider.convex = true;
                }
                return _shape;
            }
            else
            {
                return null;
            }
        }
    }
}