using UnityEngine;
using System;

namespace Parallel
{
    [RequireComponent(typeof(ParallelTransform))]
    [ExecuteInEditMode]
    public class ParallelMeshCollider : ParallelCollider3D
    {
        MeshFilter meshFilter;
        Mesh mesh;

        public float gizmoSize = 0.005f;
        public Vector3[] verts = new Vector3[0];
        public int vertsCount = 0;
        public ParallelMeshData meshData;

        FVector3 _currentSize = FVector3.one;

        void Reset()
        {
            MeshFilter meshFilter = GetComponent<MeshFilter>();

            Mesh mesh = meshFilter.sharedMesh;

            verts = mesh.vertices;
            vertsCount = verts.Length;

            int[] triangles = mesh.triangles;
            int triangleCount = triangles.Length;

            meshData = new ParallelMeshData();
            meshData.vertexCount = (UInt32)vertsCount;
            meshData.vertices = new FVector3[meshData.vertexCount];
            meshData.triangleCount = (UInt32)(triangleCount / 3);
            meshData.triangles = new ParallelTriangle[meshData.triangleCount];

            for (int i = 0; i < vertsCount; i++)
            {
                meshData.vertices[i] = (FVector3)verts[i];
            }

            for (int i = 0; i < triangleCount; i++)
            {
                int vIndex = i % 3;
                int tIndex = i / 3;

                if (vIndex == 0)
                {
                    meshData.triangles[tIndex].v1 = (UInt32)triangles[i];
                }
                else if (vIndex == 1)
                {
                    meshData.triangles[tIndex].v2 = (UInt32)triangles[i];
                }
                else
                {
                    meshData.triangles[tIndex].v3 = (UInt32)triangles[i];
                }
            }
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = ParallelUtil.ColliderOutlineColor;

            foreach (Vector3 v in verts)
            {
                Gizmos.DrawWireSphere(transform.TransformPoint(v.x, v.y, v.z), 0.01f);
            }

            foreach (FVector3 fv in meshData.vertices)
            {
                Vector3 v = (Vector3)fv;
                Gizmos.DrawWireSphere(transform.TransformPoint(v.x, v.y, v.z), 0.01f);
            }
        }

        FVector3 CalculateSize()
        {
            FVector3 s = pTransform.localScale;

            if (s.x > FFloat.zero && s.y > FFloat.zero)
            {
                return s;
            }
            else
            {
                Debug.LogError("Invalid size");
                return FVector3.zero;
            }
        }

        public override void UpdateShape(GameObject root)
        {
            FVector3 s = CalculateSize();

            if (s == FVector3.zero)
            {
                return;
            }

            if (s == _currentSize)
            {
                return;
            }

            FVector3 scale = s / _currentSize;

            _currentSize = s;

            Parallel3D.UpdateMesh(_shape, _fixture, scale);
        }

        public override PShape3D CreateShape(GameObject root)
        {
            FVector3 s = CalculateSize();

            if (s != FVector3.zero)
            {
                _currentSize = s;
                _shape = Parallel3D.CreateMesh(meshData, s);

                if(createUnityPhysicsCollider)
                {
                    gameObject.AddComponent<MeshCollider>();
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
