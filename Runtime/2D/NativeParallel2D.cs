using UnityEngine;
using System;
using System.Runtime.InteropServices;
using AOT;

namespace Parallel
{
    internal class NativeParallel2D
    {
        // Name of the plugin when using [DllImport]
#if !UNITY_EDITOR && UNITY_IOS
		const string PLUGIN_NAME = "__Internal";
#else
        const string PLUGIN_NAME = "parallel";
#endif
        internal static void Initialize()
        {
            RegisterDebugCallback(NativeParallelEventHandler.OnDebugCallback);
        }

        [DllImport(PLUGIN_NAME)]
        internal static extern void RegisterDebugCallback(debugCallback cb);

        [DllImport(PLUGIN_NAME)]
        internal static extern void HelloWorld();

        //2D world
        [DllImport(PLUGIN_NAME)]
        internal static extern IntPtr CreateWorld(
            FVector2 gravity, 
            bool allowSleep, 
            bool warmStart, 
            ContactEnterCallBack enterCallback, 
            ContactExitCallBack exitCallback, 
            RollbackAddRigidbodyCallback rollbackAddRigidbodyCallback,
            RollbackRemoveRigidbodyCallback rollbackRemoveRigidbodyCallback);

        [DllImport(PLUGIN_NAME)]
        internal static extern IntPtr SetGravity(IntPtr worldHandle, FVector2 gravity);

        [DllImport(PLUGIN_NAME)]
        internal static extern void DestroyWorld(IntPtr worldHandle);

        [DllImport(PLUGIN_NAME)]
        internal static extern void Step(IntPtr worldHandle, FFloat time, int velocityIterations, int positionIterations);

        [DllImport(PLUGIN_NAME)]
        internal static extern IntPtr Snapshot(IntPtr worldHandle);

        [DllImport(PLUGIN_NAME)]
        internal static extern void Restore(IntPtr worldHandle, IntPtr snapshotHandle);

        [DllImport(PLUGIN_NAME)]
        internal static extern void DestroySnapshot(IntPtr snapshotHandle);

        //2D body
        [DllImport(PLUGIN_NAME)]
        internal static extern IntPtr CreateBody(IntPtr worldHandle, 
            int bodyType, 
            FVector2 position, 
            FFloat angle,
            FFloat linearDamping,
            FFloat angularDamping,
            bool fixedRotation,
            FFloat gravityScale,
            UInt32 externalID,
            ref UInt16 bodyID);

        [DllImport(PLUGIN_NAME)]
        internal static extern IntPtr InsertBody(IntPtr worldHandle,
            int bodyType,
            FVector2 position,
            FFloat angle,
            FFloat linearDamping,
            FFloat angularDamping,
            bool fixedRotation,
            FFloat gravityScale,
            UInt32 externalID,
            UInt16 bodyID,
            IntPtr previousBody);

        [DllImport(PLUGIN_NAME)]
        internal static extern void UpdateBodyTransform(IntPtr bodyHandle, FVector2 position, FFloat angle);

        [DllImport(PLUGIN_NAME)]
        internal static extern void UpdateBodyVelocity(IntPtr bodyHandle, FVector2 linearVelocity, FFloat angularVelocity);

        [DllImport(PLUGIN_NAME)]
        internal static extern void UpdateBodyProperties(IntPtr bodyHandle,
            int bodyType,
            FFloat linearDamping,
            FFloat angularDamping,
            bool fixedRotation,
            FFloat gravityScale);

        [DllImport(PLUGIN_NAME)]
        internal static extern void ApplyForce(IntPtr bodyHandle, FVector2 point, FVector2 force);

        [DllImport(PLUGIN_NAME)]
        internal static extern void ApplyForceToCenter(IntPtr bodyHandle, FVector2 force);

        [DllImport(PLUGIN_NAME)]
        internal static extern void ApplyTorque(IntPtr bodyHandle, FFloat torque);

        [DllImport(PLUGIN_NAME)]
        internal static extern void ApplyLinearImpulse(IntPtr bodyHandle, FVector2 point, FVector2 impulse);

        [DllImport(PLUGIN_NAME)]
        internal static extern void ApplyLinearImpulseToCenter(IntPtr bodyHandle, FVector2 impulse);

        [DllImport(PLUGIN_NAME)]
        internal static extern void ApplyAngularImpulse(IntPtr bodyHandle, FFloat impulse);

        [DllImport(PLUGIN_NAME)]
        internal static extern void DestroyBody(IntPtr worldHandle, IntPtr bodyHandle);

        [DllImport(PLUGIN_NAME)]
        internal static extern void GetTransform(IntPtr bodyHandle, ref FVector2 pos, ref FFloat angle);

        [DllImport(PLUGIN_NAME)]
        internal static extern void GetVelocity(IntPtr bodyHandle, ref FVector2 linearVelocity, ref FFloat rz);

        [DllImport(PLUGIN_NAME)]
        internal static extern void GetBodyMassInfo(IntPtr bodyHandle, ref FFloat mass);

        //2D fixture
        [DllImport(PLUGIN_NAME)]
        internal static extern IntPtr AddFixtureToBody(IntPtr bodyHandle, IntPtr shapeHandle, FFloat density, FFloat mass);

        [DllImport(PLUGIN_NAME)]
        internal static extern IntPtr GetShapeOfFixture(IntPtr fixtureHandle);

        [DllImport(PLUGIN_NAME)]
        internal static extern void SetLayer(IntPtr fixtureHandle, int layer, int layerMask, bool refilter);

        [DllImport(PLUGIN_NAME)]
        internal static extern void SetFixtureProperties(IntPtr fixtureHandle, bool isTrigger, FFloat friction, FFloat bounciness);

        //2D shapes
        [DllImport(PLUGIN_NAME)]
        internal static extern IntPtr CreateCircle(FFloat radius, FVector2 center);

        [DllImport(PLUGIN_NAME)]
        internal static extern void UpdateCircle(IntPtr shapeHandle, IntPtr fixtureHandle, FFloat radius, FVector2 center);

        [DllImport(PLUGIN_NAME)]
        internal static extern IntPtr CreateBox(FFloat width, FFloat height, FVector2 center, FFloat angle);

        [DllImport(PLUGIN_NAME)]
        internal static extern void UpdateBox(IntPtr shapeHandle, IntPtr fixtureHandle, FFloat width, FFloat height, FVector2 center, FFloat angle);

        [DllImport(PLUGIN_NAME)]
        internal static extern IntPtr CreateCapsule(FVector2 v1, FVector2 v2, FFloat radius, FVector2 center, FFloat angle);

        [DllImport(PLUGIN_NAME)]
        internal static extern void UpdateCapsule(IntPtr shapeHandle, IntPtr fixtureHandle, FVector2 v1, FVector2 v2, FFloat radius, FVector2 center, FFloat angle);

        [DllImport(PLUGIN_NAME)]
        internal static extern IntPtr CreatePolygon(ref ParallelVec2List parallelVec2List, FVector2 center, FFloat angle);

        [DllImport(PLUGIN_NAME)]
        internal static extern void UpdatePolygon(IntPtr shapeHandle, IntPtr fixtureHandle, ref ParallelVec2List parallelVec2List, FVector2 center, FFloat angle);

        //cast
        [DllImport(PLUGIN_NAME)]
        internal static extern bool RayCast(FVector2 point1, FVector2 point2, int mask, ref FVector2 point, ref FVector2 normal, out UInt16 bodyID, IntPtr worldHandle);

        [DllImport(PLUGIN_NAME)]
        internal static extern bool CircleCast(FVector2 center, FFloat radius, int mask, FVector2 translation, ref FVector2 point, ref FVector2 normal, ref FFloat fraction, out UInt16 bodyID, IntPtr worldHandle);

        [DllImport(PLUGIN_NAME)]
        internal static extern bool ShapeCast(IntPtr shapeHandle, FVector3 pos, FFloat rot, int mask, FVector2 translation, ref FVector2 point, ref FVector2 normal, ref FFloat fraction, out UInt16 bodyID, IntPtr worldHandle);

        //overlap
        [DllImport(PLUGIN_NAME)]
        internal static extern bool CircleOverlap(IntPtr worldHandle, FVector2 center, FFloat radius, int mask, UInt16[] bodyIDs, ref int count);

        [DllImport(PLUGIN_NAME)]
        internal static extern bool BoxOverlap(IntPtr worldHandle, FVector2 center, FFloat width, FFloat height, FFloat angle, int mask, UInt16[] bodyIDs, ref int count);

        //contact
        [DllImport(PLUGIN_NAME)]
        internal static extern IntPtr GetContactList(IntPtr worldHandle);

        [DllImport(PLUGIN_NAME)]
        internal static extern IntPtr ExportAndReturnNextContact(IntPtr contactHandle, ref PContactExport2D export);

        [DllImport(PLUGIN_NAME)]
        internal static extern int GetContactDetail(IntPtr contactHandle, ref FVector2 point1, ref FVector2 point2, ref FFloat penetration1, ref FFloat penetration2, ref FVector2 normal);

        //convex
        [DllImport(PLUGIN_NAME)]
        internal static extern void ConvexHull2D(ref ParallelVec2List parallelVec2List, ref ParallelVec2List parallelVec2ListOut, int limit);

        //vector
        [DllImport(PLUGIN_NAME)]
        internal static extern void Vec2Normalize64(FVector2 a, ref FVector2 result);

        [DllImport(PLUGIN_NAME)]
        internal static extern void Vec2Length64(FVector2 a, ref FFloat result);

        //joint
        [DllImport(PLUGIN_NAME)]
        internal static extern void DestroyJoint(IntPtr worldHandle, IntPtr jointHandle);

        //mouse joint
        [DllImport(PLUGIN_NAME)]
        internal static extern IntPtr CreateMouseJoint(IntPtr worldHandle, IntPtr bodyHandleA, IntPtr bodyHandleB, FVector2 position, FFloat maxForce);

        [DllImport(PLUGIN_NAME)]
        internal static extern void MoveMouseJoint(IntPtr MoveMouseJoint, FVector2 position);

        //spring joint
        [DllImport(PLUGIN_NAME)]
        internal static extern IntPtr CreateDistanceJoint(IntPtr worldHandle,
                                                          IntPtr bodyHandleA,
                                                          IntPtr bodyHandleB,
                                                          FVector2 anchorA,
                                                          FVector2 anchorB,
                                                          bool collide,
                                                          FFloat frequency,
                                                          FFloat damp);

        //hinge joint
        [DllImport(PLUGIN_NAME)]
        internal static extern IntPtr CreateHingeJoint(IntPtr worldHandle,
                                                          IntPtr bodyHandleA,
                                                          IntPtr bodyHandleB,
                                                          FVector2 anchor,
                                                          bool collide,
                                                          bool limit, FFloat lowerAngle, FFloat upperAngle,
                                                          bool motor, FFloat motorSpeed, FFloat motorTorque);
    }
}