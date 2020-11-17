using System;

namespace Parallel
{
    [Serializable]
    public struct ParallelRay
    {
        public Fix64Vec3 direction;
        public Fix64Vec3 origin;

        /// <summary>
        ///   <para>Creates a ray starting at origin along direction.</para>
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="direction"></param>
        public ParallelRay(Fix64Vec3 origin, Fix64Vec3 direction)
        {
            this.origin = origin;
            this.direction = direction.normalized;
        }

        /// <summary>
        ///   <para>Returns a point at distance units along the ray.</para>
        /// </summary>
        /// <param name="distance"></param>
        public Fix64Vec3 GetPoint(Fix64 distance)
        {
            return origin + direction * distance;
        }

        public override string ToString()
        {
            return $"direction={direction} origin={origin}";
        }
    }
}
