using UnityEngine;
using System;
using System.Collections.Generic;

namespace Parallel
{
    [RequireComponent(typeof(ParallelTransform))]
    [ExecuteInEditMode]
    public class ParallelTerrianCollider : ParallelCollider3D
    {
        public Terrain terrain;

        public float gizmoSize = 0.1f;

        public ParallelTerrianData terrianData;

        public FFloat resolution = FFloat.two;

        void Reset()
        {
            terrain = GetComponent<Terrain>();

            Vector3 size = terrain.terrainData.size;

            Vector3 pos = transform.position;

            FVector3 fPos = (FVector3)pos;

            FFloat xFloat = (FFloat)size.x + resolution;
            FFloat zFloat = (FFloat)size.z + resolution;

            FFloat xFloatSample = fPos.x;
            FFloat zFloatSample = fPos.z;

            List<FFloat> heights = new List<FFloat>();

            uint xCount = (uint)FMath.CeilToInt(xFloat / resolution);
            uint zCount = (uint)FMath.CeilToInt(zFloat / resolution);

            while(xFloatSample < xFloat)
            {
                while(zFloatSample < zFloat)
                {
                    FFloat height = (FFloat)terrain.SampleHeight(new Vector3((float)xFloatSample, 0, (float)zFloatSample));

                    heights.Add(height);

                    zFloatSample += resolution;
                }
                zFloatSample = fPos.x;
                xFloatSample += resolution;
            }

            terrianData.resolution = resolution;
            terrianData.xCount = xCount;
            terrianData.zCount = zCount;
            terrianData.vertexCount = (uint)heights.Count;
            terrianData.vertices = heights.ToArray();
        }

        public override PShape3D CreateShape(GameObject root)
        {
            _shape = Parallel3D.CreateTerrian(terrianData);

            return _shape;
            //FVector3 s = CalculateSize();

            //if (s != FVector3.zero)
            //{
            //    _currentSize = s;
            //    _shape = Parallel3D.CreateMesh(meshData, s);

            //    if (createUnityPhysicsCollider)
            //    {
            //        gameObject.AddComponent<MeshCollider>();
            //    }

            //    return _shape;
            //}
            //else
            //{
            //    return null;
            //}
        }

        public override void UpdateShape(GameObject root)
        {
            return;
        }
    }
}
