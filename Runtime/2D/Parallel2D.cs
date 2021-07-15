using AOT;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;


//TODO: need to export sweep start orientation for convex cache
//TODO: better mesh contact points reduce algo
//TODO: broadphase sort for duplicate
//TODO: multi-threading

//Custom script icon
//  icon: {fileID: 2800000, guid: f8955ad4909f2ae41baeb5ca6ce71b44, type: 1}

//Deterministic notes:
/*
 * We are not managing the AABBs used in the broad-phase.
 * this seems okay as long as the collision detection is correct.
 * 
 * sometimes contacts with no contact points are created in broard-phase
 * because the AABBs are different when the engine enters a step.
 * this seems okay and does not affect the determinism of the engine
 * because the empty contacts are not used in the solver.
 * 
 * we need to check contact points count for collision detection
 * We should only pass contacts that have contact points to the user.
 * 
 * We need to sort the contacts of bodies
 * because the contacts are added to island by their sequence.
 * because the sequence of contacts in the solver affects the simulation results.
 * 
 * Empty Contacts are not touching so they are not added to the island and they will break the contact graph
 * 
 * We cannot rely on body.transform.angle
 * becasue it is not used in the solver
 * We must use body.GetAngle which returns the body sweep angle which is used in the solver
 * 
 * body.transform.angle and body.GetAngle can give different results!
 * 
 * body order is the key for determinism of the engine.
 * it determines how the contacts are added to the island solver.
 */

namespace Parallel
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ParallelVec2List
    {
        public int count;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
        public FVector2[] points;
    }

    public struct PRaycastHit2D
    {
        public IParallelRigidbody2D rigidbody;
        public FVector2 point;
        public FVector2 normal;
    }

    public struct PShapecastHit2D
    {
        public IParallelRigidbody2D rigidbody;
        public FVector2 point;
        public FVector2 normal;
        public FFloat fraction;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PContactExport2D
    {
        public UInt32 id;
        public int count;

        public UInt32 k1;
        public FFloat n1;
        public FFloat t1;

        public UInt32 k2;
        public FFloat n2;
        public FFloat t2;

        public FVector2 relativeVelocity;
        public bool isTrigger;
    }

    public class PContact2DWrapper
    {
        public PContact2D contact;
        public PContact2DWrapper next;
    }

    public struct PContactPoints2D
    {
        public int contactPointCount;
        public FVector2 contactNormal;

        public FVector2 point1;
        public FFloat penetration1;

        public FVector2 point2;
        public FFloat penetration2;

        public override string ToString()
        {
            if (contactPointCount == 0)
            {
                return $"PContactPoints2D(contactPointCount={contactPointCount})";
            }
            else if (contactPointCount == 1)
            {
                return $"PContactPoints2D(contactPointCount={contactPointCount}, " +
                        $"contactNormal={contactNormal}, " +
                        $"point1={point1}" +
                        $"penetration1={penetration1})";
            }
            else
            {
                return $"PContactPoints2D(contactPointCount={contactPointCount}, " +
                        $"contactNormal={contactNormal}, " +
                        $"point1={point1}" +
                        $"penetration1={penetration1}" +
                        $"point2={point2}" +
                        $"penetration2={penetration2})";
            }

        }
    }

    public class PShapeOverlapResult2D
    {
        public IParallelRigidbody2D[] rigidbodies;
        public int count;

        public PShapeOverlapResult2D()
        {
            count = 0;
            rigidbodies = new IParallelRigidbody2D[ParallelConstants.SHAPE_OVERLAP_BODY_COUNT_2D];
        }
    }

    public class PInternalState2D
    {
        public PContactExport2D[] contactExports;
        public int contactCount;

        public PInternalState2D()
        {
            contactCount = 0;
            contactExports = new PContactExport2D[ParallelConstants.MAX_CONTACT_COUNT_2D];
        }
    }

    internal delegate void ContactEnterCallBack(IntPtr contactPtr, UInt32 contactID);
    internal delegate void ContactExitCallBack(IntPtr contactPtr, UInt32 contactID);
    internal delegate void RollbackAddRigidbodyCallback(UInt32 externalID, UInt16 bodyID, IntPtr previousBody, IntPtr world);
    internal delegate void RollbackRemoveRigidbodyCallback(UInt32 externalID, UInt16 bodyID, IntPtr body, IntPtr world);

    public class Parallel2D
    {
        //static List<IParallelRigidbody2D> rigidBodies = new List<IParallelRigidbody2D>();
        //static Dictionary<UInt32, PBody2D> bodyDictionary = new Dictionary<UInt32, PBody2D>();
        internal static SortedList<UInt32, PBody2D> bodySortedList = new SortedList<UInt32, PBody2D>();
        //CONTACT
        static int _contactCount;
        static IntPtr[] contactPtrs = new IntPtr[ParallelConstants.MAX_CONTACT_COUNT_2D];
        static PContactExport2D[] contactExports = new PContactExport2D[ParallelConstants.MAX_CONTACT_COUNT_2D];
        static Dictionary<UInt32, PContact2D> contactDictionary = new Dictionary<uint, PContact2D>();
        static PCollision2D _tempCollision = new PCollision2D();

        //enter contacts
        static int _enterContactCount;
        static PContact2DWrapper _enterContactWrapperHead = new PContact2DWrapper();
        static PContact2DWrapper _enterContactWrapperEnd = _enterContactWrapperHead;

        //exit contacts
        static int _exitContactCount;
        static PContact2DWrapper _exitContactWrapperHead = new PContact2DWrapper();
        static PContact2DWrapper _exitContactWrapperEnd = _exitContactWrapperHead;

        //enter+stay contacts
        //we always export all contacts after each step update
        //we should loop through all the contacts
        //send OnCollisionEnter for enter contacts
        //send OnCollisionStay to the rest of contacts
        static int _allContactCount;
        static PContact2DWrapper _allContactWrapperHead = new PContact2DWrapper();
        static PContact2DWrapper _allContactWrapperEnd = _allContactWrapperHead;

        static bool initialized = false;
        static PWorld2D internalWorld;
        static IntPtr referenceBody;
        public static FVector2 gravity = new FVector2(FFloat.zero, FFloat.FromDivision(-98, 10));
        public static bool allowSleep = true;
        public static bool warmStart = true;

        //used for cast and overlap queries
        static UInt16[] _queryBodyIDs = new UInt16[ParallelConstants.SHAPE_OVERLAP_BODY_COUNT_2D];

        //layer
        static Dictionary<int, int> masksByLayer = new Dictionary<int, int>();

        //rollback
        internal static Action<UInt32, UInt16, IntPtr> _rollbackAddRigidbodyCallback;
        internal static Action<UInt32, UInt16> _rollbackRemoveRigidbodyCallback;

        //common
        public static void Initialize()
        {
            ReadCollisionLayerMatrix();
            NativeParallel2D.Initialize();
            internalWorld = CreateWorld(gravity, allowSleep, warmStart);
            UInt16 bodyID = 0;
            referenceBody = NativeParallel2D.CreateBody(
                                        internalWorld.IntPointer,
                                        (int)Parallel.BodyType.Static,
                                        FVector2.zero,
                                        FFloat.zero,
                                        FFloat.zero,
                                        FFloat.zero,
                                        false,
                                        FFloat.zero, 0, ref bodyID);
            initialized = true;
        }

        public static void HelloWorld()
        {
            NativeParallel2D.HelloWorld();
        }

        public static void CleanUp()
        {
            if (initialized)
            {
                //rigidBodies.Clear();
                masksByLayer.Clear();
                //bodyDictionary.Clear();
                bodySortedList.Clear();
                DestroyWorld(internalWorld);
                initialized = false;
            }
        }

        public static void ReadCollisionLayerMatrix()
        {
            for (int i = 0; i < 32; i++)
            {
                int mask = 0;
                for (int j = 0; j < 32; j++)
                {
                    if (!Physics2D.GetIgnoreLayerCollision(i, j))
                    {
                        mask |= 1 << j;
                    }
                }
                masksByLayer.Add(i, mask);
            }
        }

        public static void SetLoggingLevel(LogLevel level)
        {
            NativeParallelEventHandler.logLevel = level;
            if (!initialized)
            {
                Initialize();
            }
        }

        static void ExportFromEngine()
        {
            using (new SProfiler($"ExportContacts"))
            {
                ExportContacts();
            }

            using (new SProfiler($"PrepareContacts"))
            {
                PrepareContacts();
            }

            using (new SProfiler($"body.ReadNative"))
            {
                foreach (var pair in bodySortedList)
                {
                    PBody2D body = pair.Value;
                    body.ReadNative();
                }
            }
        }

        public static void ExcuteUserCallbacks(FFloat time)
        {
            //call contact exit callback

            PContact2DWrapper currentWrapper = _exitContactWrapperHead;

            for (int i = 0; i < _exitContactCount; i++)
            {
                PContact2D contact = currentWrapper.contact;

                PBody2D body1 = bodySortedList[contact.Body1ID];
                PBody2D body2 = bodySortedList[contact.Body2ID];

                if (contact.IsTrigger)
                {
                    body1.RigidBody.OnParallelTriggerExit(body2.RigidBody);
                    body2.RigidBody.OnParallelTriggerExit(body1.RigidBody);
                }
                else
                {
                    _tempCollision.SetContact(contact, body2.RigidBody);
                    body1.RigidBody.OnParallelCollisionExit(_tempCollision);

                    _tempCollision.SetContact(contact, body1.RigidBody);
                    body2.RigidBody.OnParallelCollisionExit(_tempCollision);
                }

                contact.state = ContactState.Inactive;
                currentWrapper = currentWrapper.next;
            }

            //call contact stay callback
            currentWrapper = _allContactWrapperHead;

            for (int i = 0; i < _allContactCount; i++)
            {
                PContact2D contact = currentWrapper.contact;

                if (contact.state == ContactState.Active)
                {
                    PBody2D body1 = bodySortedList[contact.Body1ID];
                    PBody2D body2 = bodySortedList[contact.Body2ID];

                    if (contact.IsTrigger)
                    {
                        body1.RigidBody.OnParallelTriggerStay(body2.RigidBody);
                        body2.RigidBody.OnParallelTriggerStay(body1.RigidBody);
                    }
                    else
                    {
                        _tempCollision.SetContact(contact, body2.RigidBody);
                        body1.RigidBody.OnParallelCollisionStay(_tempCollision);

                        _tempCollision.SetContact(contact, body1.RigidBody);
                        body2.RigidBody.OnParallelCollisionStay(_tempCollision);
                    }
                }

                currentWrapper = currentWrapper.next;
            }

            //call contact enter callback
            currentWrapper = _enterContactWrapperHead;

            for (int i = 0; i < _enterContactCount; i++)
            {
                PContact2D contact = currentWrapper.contact;
                PBody2D body1 = bodySortedList[contact.Body1ID];
                PBody2D body2 = bodySortedList[contact.Body2ID];

                if (contact.IsTrigger)
                {
                    body1.RigidBody.OnParallelTriggerEnter(body2.RigidBody);
                    body2.RigidBody.OnParallelTriggerEnter(body1.RigidBody);
                }
                else
                {
                    _tempCollision.SetContact(contact, body2.RigidBody);
                    body1.RigidBody.OnParallelCollisionEnter(_tempCollision);

                    _tempCollision.SetContact(contact, body1.RigidBody);
                    body2.RigidBody.OnParallelCollisionEnter(_tempCollision);
                }

                contact.state = ContactState.Active;
                currentWrapper = currentWrapper.next;
            }
        }

        public static void ExcuteUserFixedUpdate(FFloat time)
        {
            foreach (var pair in bodySortedList)
            {
                PBody2D body = pair.Value;
                body.Step(time);
            }
        }

        //2D
        static PWorld2D CreateWorld(FVector2 gravity, bool allowSleep, bool warmStart)
        {
            IntPtr m_NativeObject = NativeParallel2D.CreateWorld(
                gravity, 
                allowSleep, 
                warmStart, 
                OnContactEnterCallback, 
                OnContactExitCallBack, 
                OnRollbackAddRigidbodyCallback,
                OnRollbackRemoveRigidbodyCallback);

            return new PWorld2D(m_NativeObject);
        }

        static void DestroyWorld(PWorld2D world)
        {
            NativeParallel2D.DestroyWorld(world.IntPointer);
        }

        public static PSnapshot2D Snapshot()
        {
            IntPtr m_NativeObject = NativeParallel2D.Snapshot(internalWorld.IntPointer);
            return new PSnapshot2D(m_NativeObject);
        }

        public static void Restore(PSnapshot2D snapshot)
        {
            NativeParallel2D.Restore(internalWorld.IntPointer, snapshot.IntPointer);

            foreach (var pair in bodySortedList)
            {
                PBody2D body = pair.Value;
                body.ReadNative();
            }
        }

        [MonoPInvokeCallback(typeof(RollbackAddRigidbodyCallback))]
        public static void OnRollbackAddRigidbodyCallback(UInt32 externalID, UInt16 bodyID, IntPtr previousBody, IntPtr world)
        {
            _rollbackAddRigidbodyCallback(externalID, bodyID, previousBody);
        }

        [MonoPInvokeCallback(typeof(RollbackRemoveRigidbodyCallback))]
        public static void OnRollbackRemoveRigidbodyCallback(UInt32 externalID, UInt16 bodyID, IntPtr body, IntPtr world)
        {
            _rollbackRemoveRigidbodyCallback(externalID, bodyID);
        }

        public static void DestroySnapshot(PSnapshot2D snapshot)
        {
            NativeParallel2D.DestroySnapshot(snapshot.IntPointer);
        }

        public static void Step(FFloat time, int velocityIterations, int positionIterations)
        {
            if (!initialized)
            {
                Initialize();
            }

            //using (new SProfiler($"Reset contacts"))
            {
                ResetEnterContacts();
                ResetExitContacts();
                ResetAllContacts();
            }

            using (new SProfiler($"Engine Step"))
            {
                NativeParallel2D.Step(internalWorld.IntPointer, time, velocityIterations, positionIterations);
            }

            ExportFromEngine();
        }

        //2D fixture
        public static PFixture2D AddFixture(PBody2D body2D, PShape2D shape2D, FFloat density, FFloat mass)
        {
            if (!initialized)
            {
                Initialize();
            }

            IntPtr m_NativeObject = NativeParallel2D.AddFixtureToBody(body2D.IntPointer, shape2D.IntPointer, density, mass);
            return new PFixture2D(m_NativeObject);
        }

        public static PShape2D GetShapeOfFixture(PFixture2D fixture2D)
        {
            if (!initialized)
            {
                Initialize();
            }

            IntPtr m_NativeObject = NativeParallel2D.GetShapeOfFixture(fixture2D.IntPointer);
            return new PShape2D(m_NativeObject);
        }

        public static void SetLayer(PFixture2D fixture, int layer, bool refilter)
        {
            if (!initialized)
            {
                Initialize();
            }

            int mask = masksByLayer[layer];
            //shift layer
            int shiftedLayer = 1 << layer;

            NativeParallel2D.SetLayer(fixture.IntPointer, shiftedLayer, mask, refilter);
        }

        public static void SetFixtureProperties(PFixture2D fixture, bool isTrigger, FFloat friction, FFloat bounciness)
        {
            if (!initialized)
            {
                Initialize();
            }

            NativeParallel2D.SetFixtureProperties(fixture.IntPointer, isTrigger, friction, bounciness);
        }

        //2D shapes
        public static PShape2D CreateCircle(FFloat radius, FVector2 center)
        {
            if (!initialized)
            {
                Initialize();
            }

            IntPtr m_NativeObject = NativeParallel2D.CreateCircle(radius, center);
            return new PShape2D(m_NativeObject);
        }

        public static void UpdateCircle(PShape2D shape, PFixture2D fixture, FFloat radius, FVector2 center)
        {
            if (!initialized)
            {
                Initialize();
            }

            NativeParallel2D.UpdateCircle(shape.IntPointer, fixture.IntPointer, radius, center);
        }

        public static PShape2D CreateBox(FFloat width, FFloat height, FVector2 center, FFloat angle)
        {
            if (!initialized)
            {
                Initialize();
            }

            IntPtr m_NativeObject = NativeParallel2D.CreateBox(width, height, center, angle);
            return new PShape2D(m_NativeObject);
        }

        public static void UpdateBox(PShape2D shape, PFixture2D fixture, FFloat width, FFloat height, FVector2 center, FFloat angle)
        {
            if (!initialized)
            {
                Initialize();
            }

            NativeParallel2D.UpdateBox(shape.IntPointer, fixture.IntPointer, width, height, center, angle);
        }

        public static PShape2D CreateCapsule(FVector2 v1, FVector2 v2, FFloat radius, FVector2 center, FFloat angle)
        {
            if (!initialized)
            {
                Initialize();
            }

            IntPtr m_NativeObject = NativeParallel2D.CreateCapsule(v1, v2, radius, center, angle);
            return new PShape2D(m_NativeObject);
        }

        public static void UpdateCapsule(PShape2D shape, PFixture2D fixture, FVector2 v1, FVector2 v2, FFloat radius, FVector2 center, FFloat angle)
        {
            if (!initialized)
            {
                Initialize();
            }

            NativeParallel2D.UpdateCapsule(shape.IntPointer, fixture.IntPointer, v1, v2, radius, center, angle);
        }

        //polygon
        public static PShape2D CreatePolygon(FVector2[] verts, int count, FVector2 center, FFloat angle)
        {
            if (!initialized)
            {
                Initialize();
            }

            ParallelVec2List parallelVec2List = new ParallelVec2List();
            parallelVec2List.count = count;
            parallelVec2List.points = verts;

            IntPtr m_NativeObject = NativeParallel2D.CreatePolygon(ref parallelVec2List, center, angle);
            return new PShape2D(m_NativeObject);
        }

        public static void UpdatePolygon(PShape2D shape, PFixture2D fixture, FVector2[] verts, int count, FVector2 center, FFloat angle)
        {
            if (!initialized)
            {
                Initialize();
            }

            ParallelVec2List parallelVec2List = new ParallelVec2List();
            parallelVec2List.count = count;
            parallelVec2List.points = verts;
            NativeParallel2D.UpdatePolygon(shape.IntPointer, fixture.IntPointer, ref parallelVec2List, center, angle);
        }

        //2D body
        public static PBody2D AddBody
            (int bodyType,
            FVector2 position,
            FFloat angle,
            FFloat linearDamping,
            FFloat angularDamping,
            bool fixedRotation,
            FFloat gravityScale,
            IParallelRigidbody2D rigidBody2D,
            UInt32 externalID)
        {
            if (!initialized)
            {
                Initialize();
            }

            UInt16 bodyID = 0;

            IntPtr m_NativeObject = NativeParallel2D.CreateBody(
                internalWorld.IntPointer,
                bodyType,
                position,
                angle,
                linearDamping,
                angularDamping,
                fixedRotation,
                gravityScale,
                externalID,
                ref bodyID);

            PBody2D body2D = new PBody2D(m_NativeObject, bodyID, externalID, rigidBody2D as ParallelRigidbody2D);
            bodySortedList[bodyID] = body2D;

            ReadNativeBody(body2D);

            return body2D;
        }

        public static PBody2D InsertBody
            (int bodyType,
            FVector2 position,
            FFloat angle,
            FFloat linearDamping,
            FFloat angularDamping,
            bool fixedRotation,
            FFloat gravityScale,
            IParallelRigidbody2D rigidBody2D,
            UInt16 bodyID,
            UInt32 externalID,
            IntPtr previousBody)
        {
            if (!initialized)
            {
                Initialize();
            }

            IntPtr m_NativeObject = NativeParallel2D.InsertBody(
                internalWorld.IntPointer,
                bodyType,
                position,
                angle,
                linearDamping,
                angularDamping,
                fixedRotation,
                gravityScale,
                externalID,
                bodyID,
                previousBody);

            PBody2D body2D = new PBody2D(m_NativeObject, bodyID, externalID, rigidBody2D as ParallelRigidbody2D);
            bodySortedList[bodyID] = body2D;

            ReadNativeBody(body2D);

            return body2D;
        }

        internal static PBody2D FindBodyByID(UInt16 bodyID)
        {
            if (bodySortedList.ContainsKey(bodyID))
            {
                return bodySortedList[bodyID];
            }
            else
            {
                return null;
            }
        }

        public static void UpdateBodyTransForm(PBody2D body, FVector2 pos, FFloat angle)
        {
            if (!initialized)
            {
                Initialize();
            }

            NativeParallel2D.UpdateBodyTransform(body.IntPointer, pos, angle);
        }

        public static void UpdateBodyVelocity(PBody2D body, FVector2 linearVelocity, FFloat angularVelocity)
        {
            if (!initialized)
            {
                Initialize();
            }

            NativeParallel2D.UpdateBodyVelocity(body.IntPointer, linearVelocity, angularVelocity);
        }

        public static void SkipPositionContraintSolverForNextUpdate(PBody2D body, FVector2 direction)
        {
            if (!initialized)
            {
                Initialize();
            }

            NativeParallel2D.SkipPositionContraintSolverForNextUpdate(body.IntPointer, direction);
        }

        public static void UpdateBodyProperties(PBody2D body,
            int bodyType,
            FFloat linearDamping,
            FFloat angularDamping,
            bool fixedRotation,
            FFloat gravityScale)
        {
            if (!initialized)
            {
                Initialize();
            }

            NativeParallel2D.UpdateBodyProperties(body.IntPointer, bodyType, linearDamping, angularDamping, fixedRotation, gravityScale);
        }

        public static void ApplyForce(PBody2D body, FVector2 point, FVector2 force)
        {
            if (!initialized)
            {
                Initialize();
            }

            NativeParallel2D.ApplyForce(body.IntPointer, point, force);
        }

        public static void ApplyForceToCenter(PBody2D body, FVector2 force)
        {
            if (!initialized)
            {
                Initialize();
            }

            NativeParallel2D.ApplyForceToCenter(body.IntPointer, force);
        }

        public static void ApplyTorque(PBody2D body, FFloat torque)
        {
            if (!initialized)
            {
                Initialize();
            }

            NativeParallel2D.ApplyTorque(body.IntPointer, torque);
        }

        public static void ApplyLinearImpulse(PBody2D body, FVector2 point, FVector2 impulse)
        {
            if (!initialized)
            {
                Initialize();
            }

            NativeParallel2D.ApplyLinearImpulse(body.IntPointer, point, impulse);
        }

        public static void ApplyLinearImpulseToCenter(PBody2D body, FVector2 impulse)
        {
            if (!initialized)
            {
                Initialize();
            }

            NativeParallel2D.ApplyLinearImpulseToCenter(body.IntPointer, impulse);
        }

        public static void ApplyAngularImpulse(PBody2D body, FFloat impulse)
        {
            if (!initialized)
            {
                Initialize();
            }

            NativeParallel2D.ApplyAngularImpulse(body.IntPointer, impulse);
        }

        public static void DestoryBody(PBody2D body2D, IParallelRigidbody2D rigidBody2D)
        {
            if (!initialized)
            {
                Initialize();
            }

            if (bodySortedList.ContainsKey(body2D.BodyID))
            {
                bodySortedList.Remove(body2D.BodyID);
            }

            NativeParallel2D.DestroyBody(internalWorld.IntPointer, body2D.IntPointer);
        }

        public static void ReadNativeBody(PBody2D body2D)
        {
            if (!initialized)
            {
                Initialize();
            }
            NativeParallel2D.GetTransform(body2D.IntPointer, ref body2D.position, ref body2D.angle);
            NativeParallel2D.GetVelocity(body2D.IntPointer, ref body2D.linearVelocity, ref body2D.angularVelocity);
        }

        public static void ReadBodyMassInfo(PBody2D body2D)
        {
            if (!initialized)
            {
                Initialize();
            }

            NativeParallel2D.GetBodyMassInfo(body2D.IntPointer, ref body2D.mass);
        }

        //raycast
        public static bool RayCast(FVector2 p1, FVector2 p2, out PRaycastHit2D raycastHit2D)
        {
            return RayCast(p1, p2, -1, out raycastHit2D);
        }

        public static bool RayCast(FVector2 p1, FVector2 p2, int mask, out PRaycastHit2D raycastHit2D)
        {
            if (!initialized)
            {
                Initialize();
            }

            raycastHit2D = new PRaycastHit2D();
            UInt16 bodyID = 0;
            FVector2 point = FVector2.zero;
            FVector2 normal = FVector2.zero;

            bool hit = NativeParallel2D.RayCast(p1, p2, mask, ref point, ref normal, out bodyID, internalWorld.IntPointer);

            if (hit)
            {
                raycastHit2D.point = point;
                raycastHit2D.normal = normal;

                if (bodySortedList.ContainsKey(bodyID))
                {
                    raycastHit2D.rigidbody = bodySortedList[bodyID].RigidBody;
                }
                else
                {
                    Debug.LogError($"Rigibody not found: {bodyID}");
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool CircleCast(FVector2 center, FFloat radius, FVector2 translation, ref PShapecastHit2D shapecastHit2D)
        {
            return CircleCast(center, radius, translation, -1, ref shapecastHit2D);
        }

        public static bool CircleCast(FVector2 center, FFloat radius, FVector2 translation, int mask, ref PShapecastHit2D shapecastHit2D)
        {
            if (!initialized)
            {
                Initialize();
            }

            UInt16 bodyID = 0;
            FVector2 point = FVector2.zero;
            FVector2 normal = FVector2.zero;
            FFloat fraction = FFloat.zero;
            bool hit = NativeParallel2D.CircleCast(center, radius, mask, translation, ref point, ref normal, ref fraction, out bodyID, internalWorld.IntPointer);

            if (hit)
            {
                shapecastHit2D.point = point;
                shapecastHit2D.normal = normal;
                shapecastHit2D.fraction = fraction;

                if (bodySortedList.ContainsKey(bodyID))
                {
                    shapecastHit2D.rigidbody = bodySortedList[bodyID].RigidBody;
                }
                else
                {
                    Debug.LogError($"Rigibody not found: {bodyID}");
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        //overlap
        public static bool OverlapCircle(FVector2 center, FFloat radius, PShapeOverlapResult2D shapeOverlapResult)
        {
            return OverlapCircle(center, radius, -1, shapeOverlapResult);
        }

        public static bool OverlapCircle(FVector2 center, FFloat radius, int mask, PShapeOverlapResult2D shapeOverlapResult)
        {
            if (!initialized)
            {
                Initialize();
            }

            int count = 0;
            bool hit = NativeParallel2D.CircleOverlap(internalWorld.IntPointer, center, radius, mask, _queryBodyIDs, ref count);

            shapeOverlapResult.count = count;

            for (int i = 0; i < count; i++)
            {
                UInt16 bodyID = _queryBodyIDs[i];
                if (bodySortedList.ContainsKey(bodyID))
                {
                    shapeOverlapResult.rigidbodies[i] = bodySortedList[bodyID].RigidBody;
                }
                else
                {
                    Debug.LogError($"Rigibody not found: {bodyID}");
                }
            }

            return hit;
        }

        public static bool OverlapBox(FVector2 center, FFloat width, FFloat height, FFloat angle, PShapeOverlapResult2D shapeOverlapResult)
        {
            return OverlapBox(center, width, height, angle, -1, shapeOverlapResult);
        }

        public static bool OverlapBox(FVector2 center, FFloat width, FFloat height, FFloat angle, int mask, PShapeOverlapResult2D shapeOverlapResult)
        {
            if (!initialized)
            {
                Initialize();
            }

            int count = 0;
            bool hit = NativeParallel2D.BoxOverlap(internalWorld.IntPointer, center, width, height, angle, mask, _queryBodyIDs, ref count);

            shapeOverlapResult.count = count;

            for (int i = 0; i < count; i++)
            {
                UInt16 bodyID = _queryBodyIDs[i];
                if (bodySortedList.ContainsKey(bodyID))
                {
                    shapeOverlapResult.rigidbodies[i] = bodySortedList[bodyID].RigidBody;
                }
                else
                {
                    Debug.LogError($"Rigibody not found: {bodyID}");
                }
            }

            return hit;
        }

        //contact
        [MonoPInvokeCallback(typeof(ContactEnterCallBack))]
        public static void OnContactEnterCallback(IntPtr contactPtr, UInt32 contactID)
        {
            PContact2D c;

            if (contactDictionary.ContainsKey(contactID))
            {
                //already has this contact
                //the native contact for this body pair was created and destroyed before
                c = contactDictionary[contactID];
                c.state = ContactState.Enter;

            }
            else
            {
                //first time
                c = new PContact2D(contactID);
                c.state = ContactState.Enter;
                contactDictionary[contactID] = c;
            }

            AddEnterContactWrapper(c);
            //Debug.Log("Enter contact");
        }

        static void AddEnterContactWrapper(PContact2D contact)
        {
            _enterContactWrapperEnd.contact = contact;

            if (_enterContactWrapperEnd.next == null)
            {
                _enterContactWrapperEnd.next = new PContact2DWrapper();
            }
            _enterContactWrapperEnd = _enterContactWrapperEnd.next;

            _enterContactCount++;
        }

        static void ResetEnterContacts()
        {
            _enterContactCount = 0;
            _enterContactWrapperEnd = _enterContactWrapperHead;
        }

        [MonoPInvokeCallback(typeof(ContactExitCallBack))]
        public static void OnContactExitCallBack(IntPtr contactPtr, UInt32 contactID)
        {
            PContact2D c;

            if (contactDictionary.ContainsKey(contactID))
            {
                //already has this contact
                //the native contact for this body pair was created and destroyed before
                c = contactDictionary[contactID];
                c.state = ContactState.Exit;

            }
            else
            {
                //first time
                c = new PContact2D(contactID);
                c.state = ContactState.Enter;
                contactDictionary[contactID] = c;
            }

            AddExitContactWrapper(c);
            //Debug.Log("Exit contact");
        }

        static void AddExitContactWrapper(PContact2D contact)
        {
            _exitContactWrapperEnd.contact = contact;

            if (_exitContactWrapperEnd.next == null)
            {
                _exitContactWrapperEnd.next = new PContact2DWrapper();
            }
            _exitContactWrapperEnd = _exitContactWrapperEnd.next;

            _exitContactCount++;
        }

        static void ResetExitContacts()
        {
            _exitContactCount = 0;
            _exitContactWrapperEnd = _exitContactWrapperHead;
        }

        public static void PrepareContacts()
        {
            for (int i = 0; i < _contactCount; i++)
            {
                PContactExport2D export = contactExports[i];

                //some
                if (export.id == 0)
                {
                    continue;
                }

                PContact2D c;

                if (contactDictionary.ContainsKey(export.id))
                {
                    c = contactDictionary[export.id];
                }
                else
                {
                    c = new PContact2D(export.id);
                    contactDictionary[export.id] = c;
                }

                c.Update(
                    contactPtrs[i],
                    export.count,
                    export.relativeVelocity,
                    export.isTrigger
                    );

                AddAllContactWrapper(c);
            }
        }

        static void AddAllContactWrapper(PContact2D contact)
        {
            _allContactWrapperEnd.contact = contact;

            if (_allContactWrapperEnd.next == null)
            {
                _allContactWrapperEnd.next = new PContact2DWrapper();
            }
            _allContactWrapperEnd = _allContactWrapperEnd.next;

            _allContactCount++;
        }

        static void ResetAllContacts()
        {
            _allContactCount = 0;
            _allContactWrapperEnd = _allContactWrapperHead;
        }

        public static void ExportContacts()
        {
            if (!initialized)
            {
                Initialize();
            }

            _contactCount = 0;
            int index = 0;

            IntPtr contactPtr = NativeParallel2D.GetContactList(internalWorld.IntPointer);

            while (contactPtr != IntPtr.Zero)
            {
                contactPtrs[index] = contactPtr;
                PContactExport2D export = contactExports[index];

                contactPtr = NativeParallel2D.ExportAndReturnNextContact(contactPtr, ref export);
                contactExports[index] = export;
                index++;
            }

            _contactCount = index;
        }

        public static void GetContactDetail(IntPtr contactHandler, ref PContactPoints2D contactPoints2D)
        {
            if (!initialized)
            {
                Initialize();
            }

            contactPoints2D.contactPointCount = 0;


            contactPoints2D.contactPointCount = NativeParallel2D.GetContactDetail(
                                                                    contactHandler,
                                                                    ref contactPoints2D.point1,
                                                                    ref contactPoints2D.point2,
                                                                    ref contactPoints2D.penetration1,
                                                                    ref contactPoints2D.penetration2,
                                                                    ref contactPoints2D.contactNormal);
        }


        // convex hull
        public static ParallelVec2List ConvexHull2D(FVector2[] verts, int count, int limit)
        {
            if (!initialized)
            {
                Initialize();
            }

            ParallelVec2List parallelVec2List = new ParallelVec2List();
            parallelVec2List.count = count;
            parallelVec2List.points = verts;

            ParallelVec2List parallelVec2ListOutput = new ParallelVec2List();
            parallelVec2ListOutput.count = count;
            parallelVec2ListOutput.points = new FVector2[count];

            NativeParallel2D.ConvexHull2D(ref parallelVec2List, ref parallelVec2ListOutput, limit);

            return parallelVec2ListOutput;
        }

        //joints
        public static void DestroyJoint(PJoint2D joint)
        {
            if (!initialized)
            {
                return;
            }

            NativeParallel2D.DestroyJoint(internalWorld.IntPointer, joint.IntPointer);
        }

        public static PJoint2D CreateMouseJoint(ParallelRigidbody2D rb, FVector2 p, FFloat maxForce)
        {
            if (!initialized)
            {
                Initialize();
            }

            IntPtr m_NativeObject = NativeParallel2D.CreateMouseJoint(internalWorld.IntPointer, referenceBody, rb._body2D.IntPointer, p, maxForce);

            PJoint2D j = new PJoint2D(m_NativeObject);

            return j;
        }

        public static void MoveMouseJoint(PJoint2D joint, FVector2 position)
        {
            if (!initialized)
            {
                Initialize();
            }

            NativeParallel2D.MoveMouseJoint(joint.IntPointer, position);
        }

        public static PJoint2D CreateSprintJoint(ParallelRigidbody2D rbA,
                                                 ParallelRigidbody2D rbB,
                                                 FVector2 anchorA,
                                                 FVector2 anchorB,
                                                 bool collide,
                                                 FFloat frequency,
                                                 FFloat damping)
        {
            if (!initialized)
            {
                Initialize();
            }

            IntPtr other = referenceBody;

            if (rbB != null)
            {
                other = rbB._body2D.IntPointer;
            }

            IntPtr m_NativeObject = NativeParallel2D.CreateDistanceJoint(internalWorld.IntPointer,
                rbA._body2D.IntPointer,
                other,
                anchorA, anchorB,
                collide,
                frequency, damping);

            PJoint2D j = new PJoint2D(m_NativeObject);

            return j;
        }

        public static PJoint2D CreateHingeJoint(ParallelRigidbody2D rbA,
                                                ParallelRigidbody2D rbB,
                                                FVector2 anchor,
                                                bool collide,
                                                bool limit, FFloat lowerAngle, FFloat upperAngle,
                                                bool motor, FFloat motorSpeed, FFloat motorTorque)
        {
            if (!initialized)
            {
                Initialize();
            }

            IntPtr other = referenceBody;

            if (rbB != null)
            {
                other = rbB._body2D.IntPointer;
            }

            IntPtr m_NativeObject = NativeParallel2D.CreateHingeJoint(internalWorld.IntPointer,
                                                                        rbA._body2D.IntPointer,
                                                                        other,
                                                                        anchor,
                                                                        collide,
                                                                        limit, lowerAngle, upperAngle,
                                                                        motor, motorSpeed, motorTorque);

            PJoint2D j = new PJoint2D(m_NativeObject);

            return j;
        }
    }
}