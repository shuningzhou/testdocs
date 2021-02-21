using UnityEngine;
using System;
using System.Runtime.InteropServices;
using AOT;

namespace Parallel
{
    internal class NativeParallel3D
    {
        // Name of the plugin when using [DllImport]
#if !UNITY_EDITOR && UNITY_IOS
		const string PLUGIN_NAME = "__Internal";
#else
        const string PLUGIN_NAME = "parallel3D";
#endif
        internal static void Initialize()
        {
            RegisterDebugCallback3D(NativeParallelEventHandler.OnDebugCallback);
        }

        [DllImport(PLUGIN_NAME)]
        internal static extern void RegisterDebugCallback3D(debugCallback cb);

        //3D world
        [DllImport(PLUGIN_NAME)]
        internal static extern IntPtr CreateWorld3D(Fix64Vec3 gravity, bool allowSleep, bool warmStart, ContactEnterCallBack3D enterCallback, ContactExitCallBack3D exitCallback);

        [DllImport(PLUGIN_NAME)]
        internal static extern void GetWorldSize3D(IntPtr worldHandle, ref Fix64Vec3 lower, ref Fix64Vec3 upper);

        [DllImport(PLUGIN_NAME)]
        internal static extern IntPtr SetGravity3D(IntPtr worldHandle, Fix64Vec3 gravity);

        [DllImport(PLUGIN_NAME)]
        internal static extern void DestroyWorld3D(IntPtr worldHandle);

        [DllImport(PLUGIN_NAME)]
        internal static extern void Step3D(IntPtr worldHandle, Fix64 time, int velocityIterations, int positionIterations);

        [DllImport(PLUGIN_NAME)]
        internal static extern IntPtr Snapshot3D(IntPtr worldHandle);

        [DllImport(PLUGIN_NAME)]
        internal static extern void Restore3D(IntPtr worldHandle, IntPtr snapshotHandle);

        [DllImport(PLUGIN_NAME)]
        internal static extern void DestroySnapshot3D(IntPtr snapshotHandle);

        //3D body
        [DllImport(PLUGIN_NAME)]
        internal static extern IntPtr CreateBody3D(IntPtr worldHandle, 
            int bodyType, 
            Fix64Vec3 position, 
            Fix64Quat orientation,
            Fix64Vec3 linearDamping,
            Fix64Vec3 angularDamping,
            Fix64Vec3 gravityScale,
            bool fixedRotationX,
            bool fixedRotationY,
            bool fixedRotationZ,
            ref UInt16 bodyID);

        [DllImport(PLUGIN_NAME)]
        internal static extern void UpdateBodyTransform3D(IntPtr bodyHandle, Fix64Vec3 position, Fix64Quat orientation);

        [DllImport(PLUGIN_NAME)]
        internal static extern void UpdateBodyTransformForRollback3D(IntPtr bodyHandle, Fix64Vec3 position, Fix64Quat orientation, Fix64Quat orientation0);

        [DllImport(PLUGIN_NAME)]
        internal static extern void UpdateBodyVelocity3D(IntPtr bodyHandle, Fix64Vec3 linearVelocity, Fix64Vec3 angularVelocity);

        [DllImport(PLUGIN_NAME)]
        internal static extern void UpdateBodyProperties3D(IntPtr bodyHandle,
            int bodyType,
            Fix64Vec3 linearDamping,
            Fix64Vec3 angularDamping,
            Fix64Vec3 gravityScale,
            bool fixedRotationX,
            bool fixedRotationY,
            bool fixedRotationZ);

        [DllImport(PLUGIN_NAME)]
        internal static extern void ApplyForce3D(IntPtr bodyHandle, Fix64Vec3 point, Fix64Vec3 force);

        [DllImport(PLUGIN_NAME)]
        internal static extern void ApplyForceToCenter3D(IntPtr bodyHandle, Fix64Vec3 force);

        [DllImport(PLUGIN_NAME)]
        internal static extern void ApplyTorque3D(IntPtr bodyHandle, Fix64Vec3 torque);

        [DllImport(PLUGIN_NAME)]
        internal static extern void ApplyLinearImpulse3D(IntPtr bodyHandle, Fix64Vec3 point, Fix64Vec3 impulse);

        [DllImport(PLUGIN_NAME)]
        internal static extern void ApplyLinearImpulseToCenter3D(IntPtr bodyHandle, Fix64Vec3 impulse);

        [DllImport(PLUGIN_NAME)]
        internal static extern void ApplyAngularImpulse3D(IntPtr bodyHandle, Fix64Vec3 impulse);

        [DllImport(PLUGIN_NAME)]
        internal static extern void DestroyBody3D(IntPtr worldHandle, IntPtr bodyHandle);

        [DllImport(PLUGIN_NAME)]
        internal static extern void GetTransform3D(IntPtr bodyHandle, ref Fix64Vec3 pos, ref Fix64Quat orientation, ref Fix64Quat orientation0);

        [DllImport(PLUGIN_NAME)]
        internal static extern void GetVelocity3D(IntPtr bodyHandle, ref Fix64Vec3 linearVelocity, ref Fix64Vec3 rz);

        [DllImport(PLUGIN_NAME)]
        internal static extern bool IsAwake3D(IntPtr bodyHandle);

        [DllImport(PLUGIN_NAME)]
        internal static extern void GetBodyMassInfo3D(IntPtr bodyHandle, ref Fix64 mass);

        [DllImport(PLUGIN_NAME)]
        internal static extern void SetAwake3D(IntPtr bodyHandle, bool awake);

        [DllImport(PLUGIN_NAME)]
        internal static extern bool IsEnabled3D(IntPtr bodyHandle);

        [DllImport(PLUGIN_NAME)]
        internal static extern void SetEnabled3D(IntPtr bodyHandle, bool enabled);

        [DllImport(PLUGIN_NAME)]
        internal static extern void GetSleepTime3D(IntPtr bodyHandle, ref Fix64 sleepTime);

        [DllImport(PLUGIN_NAME)]
        internal static extern void SetAwakeForRollback3D(IntPtr bodyHandle, bool awake, Fix64 sleepTime);

        [DllImport(PLUGIN_NAME)]
        internal static extern void GetPointVelocity3D(IntPtr bodyHandle, Fix64Vec3 point, ref Fix64Vec3 v);

        [DllImport(PLUGIN_NAME)]
        internal static extern void UpdateMassData3D(IntPtr bodyHandle, Fix64 mass, Fix64Vec3 centerOfMass);

        [DllImport(PLUGIN_NAME)]
        internal static extern void UpdateMass3D(IntPtr bodyHandle, Fix64 mass);

        //3D fixture
        [DllImport(PLUGIN_NAME)]
        internal static extern IntPtr AddFixtureToBody3D(IntPtr bodyHandle, IntPtr shapeHandle, Fix64 density, ref byte shapeID);

        [DllImport(PLUGIN_NAME)]
        internal static extern void SetLayer3D(IntPtr fixtureHandle, int layer, int layerMask, bool refilter);

        [DllImport(PLUGIN_NAME)]
        internal static extern void SetFixtureProperties3D(IntPtr fixtureHandle, bool isTrigger, Fix64 friction, Fix64 bounciness);

        //3D shapes
        [DllImport(PLUGIN_NAME)]
        internal static extern IntPtr CreateCube3D(Fix64 x, Fix64 y, Fix64 z, Fix64Vec3 center, Fix64Quat rotation);

        [DllImport(PLUGIN_NAME)]
        internal static extern void UpdateCube3D(IntPtr shapeHandle, IntPtr fixtureHandle, Fix64 x, Fix64 y, Fix64 z, Fix64Vec3 center, Fix64Quat rotation);

        [DllImport(PLUGIN_NAME)]
        internal static extern IntPtr CreateSphere3D(Fix64 radius, Fix64Vec3 center);

        [DllImport(PLUGIN_NAME)]
        internal static extern void UpdateSphere3D(IntPtr shapeHandle, IntPtr fixtureHandle, Fix64 radius, Fix64Vec3 center);

        [DllImport(PLUGIN_NAME)]
        internal static extern IntPtr CreateCapsule3D(Fix64Vec3 v1, Fix64Vec3 v2, Fix64 radius, Fix64Vec3 center, Fix64Quat rotation);

        [DllImport(PLUGIN_NAME)]
        internal static extern void UpdateCapsule3D(IntPtr shapeHandle, IntPtr fixtureHandle, Fix64Vec3 v1, Fix64Vec3 v2, Fix64 radius, Fix64Vec3 center, Fix64Quat rotation);

        [DllImport(PLUGIN_NAME)]
        internal static extern IntPtr CreateConvex3D(Fix64Vec3[] verts, UInt32 vertsCount, ParallelEdge[] edges, UInt32 edgesCount, ParallelFace[] faces, UInt32 faceCount, ParallelPlane[] planes, Fix64Vec3 scale, Fix64Vec3 center, Fix64Quat rotation);

        [DllImport(PLUGIN_NAME)]
        internal static extern void UpdateConvex3D(IntPtr shapeHandle, IntPtr fixtureHandle, Fix64Vec3 scale);

        [DllImport(PLUGIN_NAME)]
        internal static extern IntPtr CreateMesh3D(Fix64Vec3[] verts, UInt32 vertsCount, ParallelTriangle[] triangles, UInt32 triangleCount, Fix64Vec3 scale);

        [DllImport(PLUGIN_NAME)]
        internal static extern void UpdateMesh3D(IntPtr shapeHandle, IntPtr fixtureHandle, Fix64Vec3 scale);

        //cast
        [DllImport(PLUGIN_NAME)]
        internal static extern bool RayCast3D(
                            Fix64Vec3 point1, 
                            Fix64Vec3 point2, 
                            int mask,
                            ref Fix64Vec3 point, 
                            ref Fix64Vec3 normal, 
                            ref Fix64 fraction, 
                            ref UInt16 bodyID, 
                            IntPtr worldHandle);

        [DllImport(PLUGIN_NAME)]
        internal static extern bool SphereCast3D(
                            IntPtr worldHandle,
                            int mask,
                            Fix64Vec3 center,
                            Fix64 radius,
                            Fix64Vec3 t,
                            ref Fix64Vec3 point,
                            ref Fix64Vec3 normal,
                            ref Fix64 fraction,
                            ref UInt16 bodyID);

        //overlap
        [DllImport(PLUGIN_NAME)]
        internal static extern bool SphereOverlap3D(IntPtr worldHandle, int mask, Fix64Vec3 center, Fix64 radius, UInt16[] bodyIDs, ref int count);

        [DllImport(PLUGIN_NAME)]
        internal static extern bool CubeOverlap3D(IntPtr worldHandle, int mask, Fix64Vec3 center, Fix64Quat rot, Fix64 x, Fix64 y, Fix64 z, UInt16[] bodyIDs, ref int count);

        [DllImport(PLUGIN_NAME)]
        internal static extern IntPtr GetContactList3D(IntPtr worldHandle);

        [DllImport(PLUGIN_NAME)]
        internal static extern IntPtr ExportAndReturnNextContact3D(
                            IntPtr contactHandle, 
                            ref PContactExport3D export);

        //convex
        [DllImport(PLUGIN_NAME)]
        internal static extern void ConvexHull3D(Fix64Vec3[] verts, UInt32 vertsCount, Fix64Vec3[] vertsOut, ref UInt32 vertsOutCount, ParallelEdge[] edgesOut, ref UInt32 edgesOutCount, ParallelFace[] faceOut, ref UInt32 facesOutCount, ParallelPlane[] planesOut, bool simplify, Fix64 angle);

        [DllImport(PLUGIN_NAME)]
        internal static extern void ConvexHull3D1(Vector3[] verts, UInt32 vertsCount);

        [DllImport(PLUGIN_NAME)]
        internal static extern void ConvexHull3D2(Vector3[] verts, UInt32 vertsCount, ParallelIntTriangle[] outTri, ref UInt32 outTriCount, Vector3[] outVerts, int limit);

        //transform
        [DllImport(PLUGIN_NAME)]
        internal static extern Fix64Vec3 Mul(Fix64Vec3 pos, Fix64Quat rot, Fix64Vec3 point, ref Fix64Vec3 output);

        [DllImport(PLUGIN_NAME)]
        internal static extern Fix64Vec3 MulT(Fix64Vec3 pos, Fix64Quat rot, Fix64Vec3 point, ref Fix64Vec3 output);

        //vector
        [DllImport(PLUGIN_NAME)]
        internal static extern void Vec3Normalize64(Fix64Vec3 a, ref Fix64Vec3 result);

        [DllImport(PLUGIN_NAME)]
        internal static extern void Vec3Length64(Fix64Vec3 a, ref Fix64 result);

        //triangulation
        [DllImport(PLUGIN_NAME)]
        internal static extern IntPtr CreatePolyIsland(
                                        Fix64Vec2[] verts, 
                                        int[] indexes, 
                                        int vertsCount);

        [DllImport(PLUGIN_NAME)]
        internal static extern void DestroyPolyIsland(IntPtr PolyIslandHandle);

        [DllImport(PLUGIN_NAME)]
        internal static extern IntPtr AddHolePolyIsland(
                                        Fix64Vec2[] verts, 
                                        int[] indexes, 
                                        int vertsCount, 
                                        IntPtr PolyIslandHandle);

        [DllImport(PLUGIN_NAME)]
        internal static extern bool TriangulatePolyIsland(
                                        int[] indices, 
                                        int[] indiceCounts, 
                                        ref int triangleCount, 
                                        ref int totalIndicesCount, 
                                        int level, 
                                        IntPtr PolyIslandHandle);

        //joint
        [DllImport(PLUGIN_NAME)]
        internal static extern void DestroyJoint3D(IntPtr worldHandle, IntPtr jointHandle);

        //mouse joint
        [DllImport(PLUGIN_NAME)]
        internal static extern IntPtr CreateMouseJoint3D(IntPtr worldHandle, IntPtr bodyHandleA, IntPtr bodyHandleB, Fix64Vec3 position, Fix64 maxForce);

        [DllImport(PLUGIN_NAME)]
        internal static extern void MoveMouseJoint3D(IntPtr MoveMouseJoint, Fix64Vec3 position);

        //spring joint
        [DllImport(PLUGIN_NAME)]
        internal static extern IntPtr CreateDistanceJoint3D(IntPtr worldHandle,
                                                            IntPtr bodyHandleA,
                                                            IntPtr bodyHandleB,
                                                            Fix64Vec3 anchorA,
                                                            Fix64Vec3 anchorB,
                                                            bool collide,
                                                            Fix64 frequency,
                                                            Fix64 damp);

        //hinge joint
        [DllImport(PLUGIN_NAME)]
        internal static extern IntPtr CreateHingeJoint3D(IntPtr worldHandle,
                                                         IntPtr bodyHandleA,
                                                         IntPtr bodyHandleB,
                                                         Fix64Vec3 anchor,
                                                         Fix64Vec3 axis,
                                                         bool collide,
                                                         bool limit, Fix64 lowerAngle, Fix64 upperAngle,
                                                         bool motor, Fix64 motorSpeed, Fix64 motorTorque);

        //cone joint
        [DllImport(PLUGIN_NAME)]
        internal static extern IntPtr CreateConeJoint3D(IntPtr worldHandle,
                                                         IntPtr bodyHandleA,
                                                         IntPtr bodyHandleB,
                                                         Fix64Vec3 anchor,
                                                         Fix64Vec3 axis,
                                                         bool collide,
                                                         bool limit, Fix64 angle,
                                                         bool twist, Fix64 lowerAngle, Fix64 upperAngle);
    }
}