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
        internal static extern IntPtr CreateWorld3D(
            FVector3 gravity, 
            int simulationMode,
            ContactEnterCallBack3D enterCallback, 
            ContactExitCallBack3D exitCallback,
            RollbackAddRigidbodyCallback3D rollbackAddRigidbodyCallback,
            RollbackRemoveRigidbodyCallback3D rollbackRemoveRigidbodyCallback);

        [DllImport(PLUGIN_NAME)]
        internal static extern void GetWorldSize3D(IntPtr worldHandle, ref FVector3 lower, ref FVector3 upper);

        [DllImport(PLUGIN_NAME)]
        internal static extern IntPtr SetGravity3D(IntPtr worldHandle, FVector3 gravity);

        [DllImport(PLUGIN_NAME)]
        internal static extern void DestroyWorld3D(IntPtr worldHandle);

        [DllImport(PLUGIN_NAME)]
        internal static extern void Step3D(IntPtr worldHandle, FFloat time, int velocityIterations, int positionIterations);

        [DllImport(PLUGIN_NAME)]
        internal static extern IntPtr Snapshot3D(IntPtr worldHandle);

        [DllImport(PLUGIN_NAME)]
        internal static extern void Restore3D(IntPtr worldHandle, IntPtr snapshotHandle);

        [DllImport(PLUGIN_NAME)]
        internal static extern void DestroySnapshot3D(IntPtr snapshotHandle);

        [DllImport(PLUGIN_NAME)]
        internal static extern int ExportSnapshot3D(IntPtr worldHandle, IntPtr snapshotHandle, byte[] buffer, int size);

        [DllImport(PLUGIN_NAME)]
        internal static extern IntPtr ImportSnapshot3D(IntPtr worldHandle, byte[] buffer, int size);

        //3D body
        [DllImport(PLUGIN_NAME)]
        internal static extern IntPtr CreateBody3D(IntPtr worldHandle, 
            int bodyType, 
            FVector3 position, 
            FQuaternion orientation,
            FVector3 linearDamping,
            FVector3 angularDamping,
            FVector3 gravityScale,
            bool fixedRotationX,
            bool fixedRotationY,
            bool fixedRotationZ,
            bool fixedPositionX,
            bool fixedPositionY,
            bool fixedPositionZ,
            UInt32 externalID,
            ref UInt16 bodyID);

        [DllImport(PLUGIN_NAME)]
        internal static extern IntPtr InsertBody3D(
            IntPtr worldHandle,
            int bodyType,
            FVector3 position,
            FQuaternion orientation,
            FVector3 linearDamping,
            FVector3 angularDamping,
            FVector3 gravityScale,
            bool fixedRotationX,
            bool fixedRotationY,
            bool fixedRotationZ,
            bool fixedPositionX,
            bool fixedPositionY,
            bool fixedPositionZ,
            UInt32 externalID,
            UInt16 bodyID,
            IntPtr previousBody);

        [DllImport(PLUGIN_NAME)]
        internal static extern void UpdateBodyTransform3D(IntPtr bodyHandle, FVector3 position, FQuaternion orientation);

        [DllImport(PLUGIN_NAME)]
        internal static extern void UpdateBodyTransformForRollback3D(IntPtr bodyHandle, FVector3 position, FQuaternion orientation, FQuaternion orientation0);

        [DllImport(PLUGIN_NAME)]
        internal static extern void UpdateBodyVelocity3D(IntPtr bodyHandle, FVector3 linearVelocity, FVector3 angularVelocity);

        [DllImport(PLUGIN_NAME)]
        internal static extern void UpdateBodyProperties3D(IntPtr bodyHandle,
            int bodyType,
            FVector3 linearDamping,
            FVector3 angularDamping,
            FVector3 gravityScale,
            bool fixedRotationX,
            bool fixedRotationY,
            bool fixedRotationZ);

        [DllImport(PLUGIN_NAME)]
        internal static extern void ApplyForce3D(IntPtr bodyHandle, FVector3 point, FVector3 force);

        [DllImport(PLUGIN_NAME)]
        internal static extern void ApplyForceToCenter3D(IntPtr bodyHandle, FVector3 force);

        [DllImport(PLUGIN_NAME)]
        internal static extern void ApplyTorque3D(IntPtr bodyHandle, FVector3 torque);

        [DllImport(PLUGIN_NAME)]
        internal static extern void ApplyLinearImpulse3D(IntPtr bodyHandle, FVector3 point, FVector3 impulse);

        [DllImport(PLUGIN_NAME)]
        internal static extern void ApplyLinearImpulseToCenter3D(IntPtr bodyHandle, FVector3 impulse);

        [DllImport(PLUGIN_NAME)]
        internal static extern void ApplyAngularImpulse3D(IntPtr bodyHandle, FVector3 impulse);

        [DllImport(PLUGIN_NAME)]
        internal static extern void DestroyBody3D(IntPtr worldHandle, IntPtr bodyHandle);

        [DllImport(PLUGIN_NAME)]
        internal static extern void GetTransform3D(IntPtr bodyHandle, ref FVector3 pos, ref FQuaternion orientation, ref FQuaternion orientation0);

        [DllImport(PLUGIN_NAME)]
        internal static extern void GetVelocity3D(IntPtr bodyHandle, ref FVector3 linearVelocity, ref FVector3 rz);

        [DllImport(PLUGIN_NAME)]
        internal static extern bool IsAwake3D(IntPtr bodyHandle);

        [DllImport(PLUGIN_NAME)]
        internal static extern void GetBodyMassInfo3D(IntPtr bodyHandle, ref FFloat mass);

        [DllImport(PLUGIN_NAME)]
        internal static extern void SetAwake3D(IntPtr bodyHandle, bool awake);

        [DllImport(PLUGIN_NAME)]
        internal static extern bool IsEnabled3D(IntPtr bodyHandle);

        [DllImport(PLUGIN_NAME)]
        internal static extern void SetEnabled3D(IntPtr bodyHandle, bool enabled);

        [DllImport(PLUGIN_NAME)]
        internal static extern void GetSleepTime3D(IntPtr bodyHandle, ref FFloat sleepTime);

        [DllImport(PLUGIN_NAME)]
        internal static extern void SetAwakeForRollback3D(IntPtr bodyHandle, bool awake, FFloat sleepTime);

        [DllImport(PLUGIN_NAME)]
        internal static extern void GetPointVelocity3D(IntPtr bodyHandle, FVector3 point, ref FVector3 v);

        [DllImport(PLUGIN_NAME)]
        internal static extern void UpdateCOM3D(IntPtr bodyHandle, FVector3 centerOfMass);

        [DllImport(PLUGIN_NAME)]
        internal static extern void UpdateMass3D(IntPtr bodyHandle, FFloat mass);

        //3D fixture
        [DllImport(PLUGIN_NAME)]
        internal static extern IntPtr AddFixtureToBody3D(IntPtr bodyHandle, IntPtr shapeHandle, FFloat density, FFloat mass, ref byte shapeID);

        [DllImport(PLUGIN_NAME)]
        internal static extern void SetLayer3D(IntPtr fixtureHandle, int layer, int layerMask, bool refilter);

        [DllImport(PLUGIN_NAME)]
        internal static extern void SetFixtureProperties3D(IntPtr fixtureHandle, bool isTrigger, FFloat friction, FFloat bounciness);

        //3D shapes
        [DllImport(PLUGIN_NAME)]
        internal static extern void DestroyShape3D(IntPtr shapeHandle);

        [DllImport(PLUGIN_NAME)]
        internal static extern IntPtr CreateCube3D(FFloat x, FFloat y, FFloat z, FVector3 center, FQuaternion rotation);

        [DllImport(PLUGIN_NAME)]
        internal static extern void UpdateCube3D(IntPtr shapeHandle, IntPtr fixtureHandle, FFloat x, FFloat y, FFloat z, FVector3 center, FQuaternion rotation);

        [DllImport(PLUGIN_NAME)]
        internal static extern IntPtr CreateSphere3D(FFloat radius, FVector3 center);

        [DllImport(PLUGIN_NAME)]
        internal static extern void UpdateSphere3D(IntPtr shapeHandle, IntPtr fixtureHandle, FFloat radius, FVector3 center);

        [DllImport(PLUGIN_NAME)]
        internal static extern IntPtr CreateCapsule3D(FVector3 v1, FVector3 v2, FFloat radius, FVector3 center, FQuaternion rotation);

        [DllImport(PLUGIN_NAME)]
        internal static extern void UpdateCapsule3D(IntPtr shapeHandle, IntPtr fixtureHandle, FVector3 v1, FVector3 v2, FFloat radius, FVector3 center, FQuaternion rotation);

        [DllImport(PLUGIN_NAME)]
        internal static extern IntPtr CreateConvexPolyhedron3D(FVector3[] verts, UInt32 vertsCount, FVector3 scale, FVector3 center, FQuaternion rotation);

        [DllImport(PLUGIN_NAME)]
        internal static extern IntPtr CreateConvex3D(FVector3[] verts, UInt32 vertsCount, ParallelEdge[] edges, UInt32 edgesCount, ParallelFace[] faces, UInt32 faceCount, ParallelPlane[] planes, FVector3 scale, FVector3 center, FQuaternion rotation);

        [DllImport(PLUGIN_NAME)]
        internal static extern void UpdateConvex3D(IntPtr shapeHandle, IntPtr fixtureHandle, FVector3 scale);

        [DllImport(PLUGIN_NAME)]
        internal static extern IntPtr CreateMesh3D(FVector3[] verts, UInt32 vertsCount, ParallelTriangle[] triangles, UInt32 triangleCount, FVector3 scale);

        [DllImport(PLUGIN_NAME)]
        internal static extern void UpdateMesh3D(IntPtr shapeHandle, IntPtr fixtureHandle, FVector3 scale);


        [DllImport(PLUGIN_NAME)]
        internal static extern IntPtr CreateTerrian3D(FFloat[] verts, UInt32 vertsCount, UInt32 xCount, UInt32 zCount, FFloat resolution);

        //cast
        [DllImport(PLUGIN_NAME)]
        internal static extern bool RayCast3D(
                            FVector3 point1, 
                            FVector3 point2, 
                            int mask,
                            ref FVector3 point, 
                            ref FVector3 normal, 
                            ref FFloat fraction, 
                            ref UInt16 bodyID, 
                            IntPtr worldHandle);

        [DllImport(PLUGIN_NAME)]
        internal static extern bool SphereCast3D(
                            IntPtr worldHandle,
                            int mask,
                            FVector3 center,
                            FFloat radius,
                            FVector3 t,
                            ref FVector3 point,
                            ref FVector3 normal,
                            ref FFloat fraction,
                            ref UInt16 bodyID,
                            UInt16 ignoreBodyID);


        [DllImport(PLUGIN_NAME)]
        internal static extern bool ShapeCast3D(
                    IntPtr worldHandle,
                    int mask,
                    IntPtr shapeHandle,
                    FVector3 pos,
                    FQuaternion rot,
                    FVector3 t,
                    ref FVector3 point,
                    ref FVector3 normal,
                    ref FFloat fraction,
                    ref UInt16 bodyID,
                    UInt16 ignoreBodyID);


        //overlap
        [DllImport(PLUGIN_NAME)]
        internal static extern bool SphereOverlap3D(IntPtr worldHandle, int mask, FVector3 center, FFloat radius, UInt16[] bodyIDs, ref int count);

        [DllImport(PLUGIN_NAME)]
        internal static extern bool CubeOverlap3D(IntPtr worldHandle, int mask, FVector3 center, FQuaternion rot, FFloat x, FFloat y, FFloat z, UInt16[] bodyIDs, ref int count);

        [DllImport(PLUGIN_NAME)]
        internal static extern IntPtr GetContactList3D(IntPtr worldHandle);

        [DllImport(PLUGIN_NAME)]
        internal static extern IntPtr ExportAndReturnNextContact3D(
                            IntPtr contactHandle, 
                            ref PContactExport3D export);

        //convex
        [DllImport(PLUGIN_NAME)]
        internal static extern void ConvexHull3D(FVector3[] verts, UInt32 vertsCount, FVector3[] vertsOut, ref UInt32 vertsOutCount, ParallelEdge[] edgesOut, ref UInt32 edgesOutCount, ParallelFace[] faceOut, ref UInt32 facesOutCount, ParallelPlane[] planesOut, bool simplify, FFloat angle);

        [DllImport(PLUGIN_NAME)]
        internal static extern void ConvexHull3D1(Vector3[] verts, UInt32 vertsCount);

        [DllImport(PLUGIN_NAME)]
        internal static extern void ConvexHull3D2(Vector3[] verts, UInt32 vertsCount, ParallelIntTriangle[] outTri, ref UInt32 outTriCount, Vector3[] outVerts, int limit);

        //transform
        [DllImport(PLUGIN_NAME)]
        internal static extern FVector3 Mul(FVector3 pos, FQuaternion rot, FVector3 point, ref FVector3 output);

        [DllImport(PLUGIN_NAME)]
        internal static extern FVector3 MulT(FVector3 pos, FQuaternion rot, FVector3 point, ref FVector3 output);

        //vector
        [DllImport(PLUGIN_NAME)]
        internal static extern void Vec3Normalize64(FVector3 a, ref FVector3 result);

        [DllImport(PLUGIN_NAME)]
        internal static extern void Vec3Length64(FVector3 a, ref FFloat result);

        //triangulation
        [DllImport(PLUGIN_NAME)]
        internal static extern IntPtr CreatePolyIsland(
                                        FVector2[] verts, 
                                        int[] indexes, 
                                        int vertsCount);

        [DllImport(PLUGIN_NAME)]
        internal static extern void DestroyPolyIsland(IntPtr PolyIslandHandle);

        [DllImport(PLUGIN_NAME)]
        internal static extern IntPtr AddHolePolyIsland(
                                        FVector2[] verts, 
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
        internal static extern IntPtr CreateMouseJoint3D(IntPtr worldHandle, IntPtr bodyHandleA, IntPtr bodyHandleB, FVector3 position, FFloat maxForce);

        [DllImport(PLUGIN_NAME)]
        internal static extern void MoveMouseJoint3D(IntPtr MoveMouseJoint, FVector3 position);

        //spring joint
        [DllImport(PLUGIN_NAME)]
        internal static extern IntPtr CreateDistanceJoint3D(IntPtr worldHandle,
                                                            IntPtr bodyHandleA,
                                                            IntPtr bodyHandleB,
                                                            FVector3 anchorA,
                                                            FVector3 anchorB,
                                                            bool collide,
                                                            FFloat frequency,
                                                            FFloat damp);

        //hinge joint
        [DllImport(PLUGIN_NAME)]
        internal static extern IntPtr CreateHingeJoint3D(IntPtr worldHandle,
                                                         IntPtr bodyHandleA,
                                                         IntPtr bodyHandleB,
                                                         FVector3 anchor,
                                                         FVector3 axis,
                                                         bool collide,
                                                         bool limit, FFloat lowerAngle, FFloat upperAngle,
                                                         bool motor, FFloat motorSpeed, FFloat motorTorque);

        //cone joint
        [DllImport(PLUGIN_NAME)]
        internal static extern IntPtr CreateConeJoint3D(IntPtr worldHandle,
                                                         IntPtr bodyHandleA,
                                                         IntPtr bodyHandleB,
                                                         FVector3 anchor,
                                                         FVector3 axis,
                                                         bool collide,
                                                         bool limit, FFloat angle,
                                                         bool twist, FFloat lowerAngle, FFloat upperAngle);
    }
}