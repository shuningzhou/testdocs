using System;

namespace Parallel
{
    [Serializable]
    public struct ParallelRay
    {
        public FVector3 direction;
        public FVector3 origin;

        /// <summary>
        ///   <para>Creates a ray starting at origin along direction.</para>
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="direction"></param>
        public ParallelRay(FVector3 origin, FVector3 direction)
        {
            this.origin = origin;
            this.direction = direction.normalized;
        }

        /// <summary>
        ///   <para>Returns a point at distance units along the ray.</para>
        /// </summary>
        /// <param name="distance"></param>
        public FVector3 GetPoint(FFloat distance)
        {
            return origin + direction * distance;
        }

        public override string ToString()
        {
            return $"direction={direction} origin={origin}";
        }
    }
}
