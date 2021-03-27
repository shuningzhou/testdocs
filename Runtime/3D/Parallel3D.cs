using AOT;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Parallel
{
    [Serializable]
    public struct ParallelEdge
    {
        public UInt32 origin;
        public UInt32 twin;
        public UInt32 face;
        public UInt32 prev;
        public UInt32 next;
    }

    [Serializable]
    public struct ParallelFace
    {
        public UInt32 edge;
    }

    [Serializable]
    public struct ParallelPlane
    {
        public FVector3 normal;
        public FFloat offset;
    }

    [Serializable]
    public struct ParallelQHullData
    {
        public UInt32 vertexCount;
        public FVector3[] vertices;
        public UInt32 edgeCount;
        public ParallelEdge[] edges;
        public UInt32 faceCount;
        public ParallelFace[] faces;
        public ParallelPlane[] planes;
    }

    [Serializable]
    public struct ParallelQHullData2
    {
        public UInt32 triCount;
        public ParallelIntTriangle[] tris;
        public Vector3[] vertices;
    }

    [Serializable]
    public struct ParallelTriangle
    {
        public UInt32 v1;
        public UInt32 v2;
        public UInt32 v3;
    }

    [Serializable]
    public struct ParallelIntTriangle
    {
        public Int32 v1;
        public Int32 v2;
        public Int32 v3;
    }

    [Serializable]
    public struct ParallelMeshData
    {
        public UInt32 vertexCount;
        public FVector3[] vertices;
        public UInt32 triangleCount;
        public ParallelTriangle[] triangles;
    }

    [Serializable]
    public struct ParallelTerrianData
    {
        public UInt32 vertexCount;
        public FFloat[] vertices;
        public UInt32 xCount;
        public UInt32 zCount;
        public FFloat resolution;
    }

    public struct PRaycastHit3D
    {
        public IParallelRigidbody3D rigidbody;
        public FVector3 point;
        public FVector3 normal;
        public FFloat fraction;
    }

    public struct PShapecastHit3D
    {
        public IParallelRigidbody3D rigidbody;
        public FVector3 point;
        public FVector3 normal;
        public FFloat fraction;
    }

    public class PShapeOverlapResult3D
    {
        public IParallelRigidbody3D[] rigidbodies;
        public int count;

        public PShapeOverlapResult3D()
        {
            count = 0;
            rigidbodies = new IParallelRigidbody3D[ParallelConstants.SHAPE_OVERLAP_BODY_COUNT_3D];
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PContactExport3D
    {
        public UInt64 id;
        public FVector3 relativeVelocity;
        public bool isTrigger;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PConvexCacheExport3D
    {
        public bool initialized;

        public FFloat metric; // lenght or area or volume
        public UInt16 count; // number of support vertices

        // support vertices on proxy 1
        public byte index10;
        public byte index11;
        public byte index12;
        public byte index13;

        // support vertices on proxy 2
        public byte index20;
        public byte index21;
        public byte index22;
        public byte index23;

        public byte state; // sat result
        public byte type; // feature pair type
        public UInt32 findex1; // feature index on hull 1
        public UInt32 findex2; // feature index on hull 2
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PManifoldExport3D
    {
        public UInt32 pointCount;
        public FVector2 tangentImpulse;
        public FFloat motorImpulse;

        public UInt32 p1triangleKey;
        public UInt64 p1key1;
        public UInt64 p1key2;
        public FFloat p1normalImpulse;

        public UInt32 p2triangleKey;
        public UInt64 p2key1;
        public UInt64 p2key2;
        public FFloat p2normalImpulse;

        public UInt32 p3triangleKey;
        public UInt64 p3key1;
        public UInt64 p3key2;
        public FFloat p3normalImpulse;

        public UInt32 p4triangleKey;
        public UInt64 p4key1;
        public UInt64 p4key2;
        public FFloat p4normalImpulse;
    }

    public class PContact3DWrapper
    {
        public PContact3D contact;
        public PContact3DWrapper next;
    }

    public class PInternalState3D
    {
        public PContactExport3D[] contactExports;
        public PConvexCacheExport3D[] convexExports;
        public PManifoldExport3D[] manifoldExports;
        public int contactCount;

        public PInternalState3D()
        {
            contactCount = 0;
            contactExports = new PContactExport3D[ParallelConstants.MAX_CONTACT_COUNT_3D];
            convexExports = new PConvexCacheExport3D[ParallelConstants.MAX_CONTACT_COUNT_3D];
            manifoldExports = new PManifoldExport3D[ParallelConstants.MAX_CONTACT_COUNT_3D * 3];
        }
    }

    internal delegate void ContactEnterCallBack3D(IntPtr contactPtr, UInt64 contactID);
    internal delegate void ContactExitCallBack3D(IntPtr contactPtr, UInt64 contactID);
    internal delegate void RollbackAddRigidbodyCallback3D(UInt16 externalID, UInt16 bodyID, IntPtr previousBody, IntPtr world);
    internal delegate void RollbackRemoveRigidbodyCallback3D(UInt16 externalID, UInt16 bodyID, IntPtr body, IntPtr world);


    public class Parallel3D
    {
        internal static SortedList<UInt32, PBody3D> bodySortedList = new SortedList<UInt32, PBody3D>();

        //CONTACT
        static int _contactCount;
        static IntPtr[] contactPtrs = new IntPtr[ParallelConstants.MAX_CONTACT_COUNT_3D];
        static PContactExport3D[] contactExports = new PContactExport3D[ParallelConstants.MAX_CONTACT_COUNT_3D];
        static PConvexCacheExport3D[] convexExports = new PConvexCacheExport3D[ParallelConstants.MAX_CONTACT_COUNT_3D];
        static PManifoldExport3D[] manifoldExports = new PManifoldExport3D[ParallelConstants.MAX_CONTACT_COUNT_3D * 3];

        static Dictionary<UInt64, PContact3D> contactDictionary = new Dictionary<UInt64, PContact3D>();
        static PCollision3D _tempCollision = new PCollision3D();

        //enter contacts
        static int _enterContactCount;
        static PContact3DWrapper _enterContactWrapperHead = new PContact3DWrapper();
        static PContact3DWrapper _enterContactWrapperEnd = _enterContactWrapperHead;
        //exit contacts
        static int _exitContactCount;
        static PContact3DWrapper _exitContactWrapperHead = new PContact3DWrapper();
        static PContact3DWrapper _exitContactWrapperEnd = _exitContactWrapperHead;
        //enter+stay contacts
        //we always export all contacts after each step update
        //we should loop through all the contacts
        //send OnCollisionEnter for the entering contacts
        //send OnCollisionStay to the rest of the contacts
        static int _allContactCount;
        static PContact3DWrapper _allContactWrapperHead = new PContact3DWrapper();
        static PContact3DWrapper _allContactWrapperEnd = _allContactWrapperHead;

        //
        static bool initialized = false;
        static PWorld3D internalWorld;
        static IntPtr referenceBody;
        public static FVector3 gravity = new FVector3(FFloat.zero, FFloat.FromDivision(-98, 10), FFloat.zero);
        public static SimulationMode simulationMode = SimulationMode.Stateful;

        //used for cast and overlap queruies
        static UInt16[] _queryBodyIDs = new UInt16[ParallelConstants.SHAPE_OVERLAP_BODY_COUNT_3D];

        //layer
        static Dictionary<int, int> masksByLayer = new Dictionary<int, int>();

        //rollback
        internal static Action<UInt16, UInt16, IntPtr> _rollbackAddRigidbodyCallback3D;
        internal static Action<UInt16, UInt16> _rollbackRemoveRigidbodyCallback3D;

        //common
        public static void Initialize()
        {
            ReadCollisionLayerMatrix();
            NativeParallel3D.Initialize();
            internalWorld = CreateWorld(gravity, simulationMode);

            for (int i = 0; i < ParallelConstants.MAX_CONTACT_COUNT_3D; i++)
            {
                _enterContactWrapperEnd.next = new PContact3DWrapper();
                _enterContactWrapperEnd = _enterContactWrapperEnd.next;

                _exitContactWrapperEnd.next = new PContact3DWrapper();
                _exitContactWrapperEnd = _exitContactWrapperEnd.next;

                _allContactWrapperEnd.next = new PContact3DWrapper();
                _allContactWrapperEnd = _allContactWrapperEnd.next;
            }

            UInt16 bodyID = 0;
            referenceBody = NativeParallel3D.CreateBody3D(
                internalWorld.IntPointer,
                (int)BodyType.Static,
                FVector3.zero,
                FQuaternion.identity,
                FVector3.zero,
                FVector3.zero,
                FVector3.zero,
                true,
                true,
                true,
                true,
                true,
                true,
                0,
                ref bodyID);

            initialized = true;
        }

        public static void CleanUp()
        {
            if (initialized)
            {
                masksByLayer.Clear();
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
                    if (!Physics.GetIgnoreLayerCollision(i, j))
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
            //using (new SProfiler($"ExportContacts"))
            {
                ExportContacts();
                PrepareContacts();
            }

            //using (new SProfiler($"rigidBody"))
            {
                foreach (var pair in bodySortedList)
                {
                    PBody3D body = pair.Value;
                    body.ReadNative();
                }
            }
        }

        public static void ExcuteUserCallbacks(FFloat timeStep)
        {
            //call contact exit callback

            PContact3DWrapper currentWrapper = _exitContactWrapperHead;

            for (int i = 0; i < _exitContactCount; i++)
            {
                PContact3D contact = currentWrapper.contact;

                PBody3D body1 = bodySortedList[contact.Body1ID];
                PBody3D body2 = bodySortedList[contact.Body2ID];

                if (contact.IsTrigger)
                {
                    body1.RigidBody.OnParallelTriggerExit(body2.RigidBody, contact.Shape1ID, contact.Shape2ID);
                    body2.RigidBody.OnParallelTriggerExit(body1.RigidBody, contact.Shape2ID, contact.Shape1ID);
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
                PContact3D contact = currentWrapper.contact;

                if (contact.state == ContactState.Active)
                {
                    PBody3D body1 = bodySortedList[contact.Body1ID];
                    PBody3D body2 = bodySortedList[contact.Body2ID];

                    if (contact.IsTrigger)
                    {
                        body1.RigidBody.OnParallelTriggerStay(body2.RigidBody, contact.Shape1ID, contact.Shape2ID);
                        body2.RigidBody.OnParallelTriggerStay(body1.RigidBody, contact.Shape2ID, contact.Shape1ID);
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
                PContact3D contact = currentWrapper.contact;
                PBody3D body1 = bodySortedList[contact.Body1ID];
                PBody3D body2 = bodySortedList[contact.Body2ID];

                if (contact.IsTrigger)
                {
                    body1.RigidBody.OnParallelTriggerEnter(body2.RigidBody, contact.Shape1ID, contact.Shape2ID);
                    body2.RigidBody.OnParallelTriggerEnter(body1.RigidBody, contact.Shape2ID, contact.Shape1ID);
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
                PBody3D body = pair.Value;
                body.Step(time);
            }
        }

        //3D
        static PWorld3D CreateWorld(FVector3 gravity, SimulationMode simulationMode)
        {
            IntPtr m_NativeObject = NativeParallel3D.CreateWorld3D(
                gravity,
                (int)simulationMode,
                OnContactEnterCallback,
                OnContactExitCallBack,
                OnRollbackAddRigidbodyCallback3D,
                OnRollbackRemoveRigidbodyCallback3D);
            return new PWorld3D(m_NativeObject);
        }

        public static void GetWorldSize(ref FVector3 lower, ref FVector3 uppder)
        {
            if (!initialized)
            {
                Initialize();
            }

            NativeParallel3D.GetWorldSize3D(internalWorld.IntPointer, ref lower, ref uppder);
        }

        static void DestroyWorld(PWorld3D world)
        {
            NativeParallel3D.DestroyWorld3D(world.IntPointer);
        }

        public static PSnapshot3D Snapshot()
        {
            IntPtr m_NativeObject = NativeParallel3D.Snapshot3D(internalWorld.IntPointer);
            return new PSnapshot3D(m_NativeObject);
        }

        public static void Restore(PSnapshot3D snapshot)
        {
            NativeParallel3D.Restore3D(internalWorld.IntPointer, snapshot.IntPointer);

            foreach (var pair in bodySortedList)
            {
                PBody3D body = pair.Value;
                body.ReadNative();
            }
        }

        public static int ExportSnapshot(PSnapshot3D snapshot, byte[] bytes, int size)
        {
            int snapshotSize = NativeParallel3D.ExportSnapshot3D(internalWorld.IntPointer, snapshot.IntPointer, bytes, size);
            return snapshotSize;
        }

        public static PSnapshot3D ImportSnapshot(byte[] bytes, int size)
        {
            IntPtr m_NativeObject = NativeParallel3D.ImportSnapshot3D(internalWorld.IntPointer, bytes, size);
            return new PSnapshot3D(m_NativeObject);
        }

        [MonoPInvokeCallback(typeof(RollbackAddRigidbodyCallback3D))]
        public static void OnRollbackAddRigidbodyCallback3D(UInt16 externalID, UInt16 bodyID, IntPtr previousBody, IntPtr world)
        {
            _rollbackAddRigidbodyCallback3D(externalID, bodyID, previousBody);
        }

        [MonoPInvokeCallback(typeof(RollbackRemoveRigidbodyCallback3D))]
        public static void OnRollbackRemoveRigidbodyCallback3D(UInt16 externalID, UInt16 bodyID, IntPtr body, IntPtr world)
        {
            _rollbackRemoveRigidbodyCallback3D(externalID, bodyID);
        }

        public static void DestroySnapshot(PSnapshot3D snapshot)
        {
            NativeParallel3D.DestroySnapshot3D(snapshot.IntPointer);
        }

        public static void Step(FFloat time, int velocityIterations, int positionIterations)
        {
            if (!initialized)
            {
                Initialize();
            }

            //using (new SProfiler($"==========STEP========"))
            {
                ResetEnterContacts();
                ResetExitContacts();
                ResetAllContacts();

                //using (new SProfiler($"Physics"))
                {
                    NativeParallel3D.Step3D(internalWorld.IntPointer, time, velocityIterations, positionIterations);
                }

                //using (new SProfiler($"Export"))
                {
                    ExportFromEngine();
                }
            }

        }

        //3D fixture
        public static PFixture3D AddFixture(PBody3D body, PShape3D shape, FFloat density, FFloat mass)
        {
            if (!initialized)
            {
                Initialize();
            }

            byte shapeID = 0;
            IntPtr m_NativeObject = NativeParallel3D.AddFixtureToBody3D(body.IntPointer, shape.IntPointer, density, mass, ref shapeID);
            return new PFixture3D(shapeID, m_NativeObject);
        }

        public static PShape3D GetShapeOfFixture(PFixture3D fixture)
        {
            if (!initialized)
            {
                Initialize();
            }

            return new PShape3D(fixture.IntPointer);
        }

        public static void SetLayer(PFixture3D fixture, int layer, bool refilter)
        {
            if (!initialized)
            {
                Initialize();
            }

            int mask = masksByLayer[layer];
            //shift layer
            int shiftedLayer = 1 << layer;

            NativeParallel3D.SetLayer3D(fixture.IntPointer, shiftedLayer, mask, refilter);
        }

        public static void SetFixtureProperties(PFixture3D fixture, bool isTrigger, FFloat friction, FFloat bounciness)
        {
            if (!initialized)
            {
                Initialize();
            }

            NativeParallel3D.SetFixtureProperties3D(fixture.IntPointer, isTrigger, friction, bounciness);
        }

        //3D shapes
        public static void DestroyShape(PShape3D shape)
        {
            if (!initialized)
            {
                Initialize();
            }

            NativeParallel3D.DestroyShape3D(shape.IntPointer);
        }

        public static PShape3D CreateCube(FFloat x, FFloat y, FFloat z, FVector3 center, FQuaternion rotation)
        {
            if (!initialized)
            {
                Initialize();
            }

            IntPtr m_NativeObject = NativeParallel3D.CreateCube3D(x, y, z, center, rotation);
            return new PShape3D(m_NativeObject);
        }

        public static void UpdateCube(PShape3D shape, PFixture3D fixture, FFloat x, FFloat y, FFloat z, FVector3 center, FQuaternion rotation)
        {
            if (!initialized)
            {
                Initialize();
            }

            NativeParallel3D.UpdateCube3D(shape.IntPointer, fixture.IntPointer, x, y, z, center, rotation);
        }

        public static PShape3D CreateSphere(FFloat radius, FVector3 center)
        {
            if (!initialized)
            {
                Initialize();
            }

            IntPtr m_NativeObject = NativeParallel3D.CreateSphere3D(radius, center);
            return new PShape3D(m_NativeObject);
        }

        public static void UpdateSphere(PShape3D shape, PFixture3D fixture, FFloat radius, FVector3 center)
        {
            if (!initialized)
            {
                Initialize();
            }

            NativeParallel3D.UpdateSphere3D(shape.IntPointer, fixture.IntPointer, radius, center);
        }

        public static PShape3D CreateCapsule(FVector3 v1, FVector3 v2, FFloat radius, FVector3 center, FQuaternion rotation)
        {
            if (!initialized)
            {
                Initialize();
            }

            IntPtr m_NativeObject = NativeParallel3D.CreateCapsule3D(v1, v2, radius, center, rotation);
            return new PShape3D(m_NativeObject);
        }

        public static void UpdateCapsule(PShape3D shape, PFixture3D fixture, FVector3 v1, FVector3 v2, FFloat radius, FVector3 center, FQuaternion rotation)
        {
            if (!initialized)
            {
                Initialize();
            }

            NativeParallel3D.UpdateCapsule3D(shape.IntPointer, fixture.IntPointer, v1, v2, radius, center, rotation);
        }

        public static PShape3D CreatePolyhedron(ParallelQHullData parallelQHullData, FVector3 scale, FVector3 center, FQuaternion rotation)
        {
            if (!initialized)
            {
                Initialize();
            }

            string output = "";
            output += $"b3Vec3 verts[{parallelQHullData.vertexCount}] = {{}};\n";
            //Debug.Log($"b3Vec3 verts[{convexData.vertexCount}] = {{}};");
            for (int i = 0; i < parallelQHullData.vertexCount; i++)
            {
                Vector3 vec3 = (Vector3)parallelQHullData.vertices[i];
                output += $"verts[{i}] = b3Vec3({vec3.x}, {vec3.y}, {vec3.z});\n";
                //Debug.Log($"verts[{i}] = b3Vec3({vec3.x}, {vec3.y}, {vec3.z});");
            }
            Debug.Log(output);

            IntPtr m_NativeObject = NativeParallel3D.CreateConvexPolyhedron3D(parallelQHullData.vertices, parallelQHullData.vertexCount, scale, center, rotation);
            //IntPtr m_NativeObject = NativeParallel3D.CreateConvex3D(parallelQHullData.vertices, parallelQHullData.vertexCount, parallelQHullData.edges, parallelQHullData.edgeCount, parallelQHullData.faces, parallelQHullData.faceCount, parallelQHullData.planes, scale, center, rotation);
            return new PShape3D(m_NativeObject);
        }

        public static void UpdatePolyhedron(PShape3D shape, PFixture3D fixture, FVector3 scale)
        {
            if (!initialized)
            {
                Initialize();
            }

            NativeParallel3D.UpdateConvex3D(shape.IntPointer, fixture.IntPointer, scale);
        }

        public static PShape3D CreateMesh(ParallelMeshData parallelMeshData, FVector3 scale)
        {
            if (!initialized)
            {
                Initialize();
            }

            IntPtr m_NativeObject = NativeParallel3D.CreateMesh3D(parallelMeshData.vertices, parallelMeshData.vertexCount, parallelMeshData.triangles, parallelMeshData.triangleCount, scale);

            return new PShape3D(m_NativeObject);
        }

        public static void UpdateMesh(PShape3D shape, PFixture3D fixture, FVector3 scale)
        {
            if (!initialized)
            {
                Initialize();
            }

            NativeParallel3D.UpdateMesh3D(shape.IntPointer, fixture.IntPointer, scale);
        }

        public static PShape3D CreateTerrian(ParallelTerrianData parallelTerrainData)
        {
            if (!initialized)
            {
                Initialize();
            }

            IntPtr m_NativeObject = NativeParallel3D.CreateTerrian3D(parallelTerrainData.vertices, parallelTerrainData.vertexCount, parallelTerrainData.xCount, parallelTerrainData.zCount, parallelTerrainData.resolution);

            return new PShape3D(m_NativeObject);
        }

        //3D body
        public static PBody3D AddBody(
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
            IParallelRigidbody3D rigidBody,
            UInt32 externalID)
        {
            if (!initialized)
            {
                Initialize();
            }

            UInt16 bodyID = 0;

            IntPtr m_NativeObject = NativeParallel3D.CreateBody3D(
                internalWorld.IntPointer,
                bodyType,
                position,
                orientation,
                linearDamping,
                angularDamping,
                gravityScale,
                fixedRotationX,
                fixedRotationY,
                fixedRotationZ,
                fixedPositionX,
                fixedPositionY,
                fixedPositionZ,
                externalID,
                ref bodyID);

            PBody3D body = new PBody3D(m_NativeObject, bodyID, externalID, rigidBody as ParallelRigidbody3D);
            bodySortedList[bodyID] = body;

            ReadNativeBody(body);

            return body;
        }

        public static PBody3D InsertBody(
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
            IParallelRigidbody3D rigidBody,
            UInt32 externalID,
            UInt16 bodyID,
            IntPtr previousBody)
        {
            if (!initialized)
            {
                Initialize();
            }

            IntPtr m_NativeObject = NativeParallel3D.InsertBody3D(
                internalWorld.IntPointer,
                bodyType,
                position,
                orientation,
                linearDamping,
                angularDamping,
                gravityScale,
                fixedRotationX,
                fixedRotationY,
                fixedRotationZ,
                fixedPositionX,
                fixedPositionY,
                fixedPositionZ,
                externalID,
                bodyID,
                previousBody);

            PBody3D body = new PBody3D(m_NativeObject, bodyID, externalID, rigidBody as ParallelRigidbody3D);
            bodySortedList[bodyID] = body;

            ReadNativeBody(body);

            return body;
        }

        internal static PBody3D FindBodyByID(UInt16 bodyID)
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

        public static void UpdateBodyTransForm(PBody3D body, FVector3 pos, FQuaternion rot)
        {
            if (!initialized)
            {
                Initialize();
            }

            NativeParallel3D.UpdateBodyTransform3D(body.IntPointer, pos, rot);
        }

        public static void UpdateBodyTransformForRollback(PBody3D body, FVector3 pos, FQuaternion rot, FQuaternion rot0)
        {
            if (!initialized)
            {
                Initialize();
            }

            NativeParallel3D.UpdateBodyTransformForRollback3D(body.IntPointer, pos, rot, rot0);
        }

        public static void UpdateBodyVelocity(PBody3D body, FVector3 linearVelocity, FVector3 angularVelocity)
        {
            if (!initialized)
            {
                Initialize();
            }

            NativeParallel3D.UpdateBodyVelocity3D(body.IntPointer, linearVelocity, angularVelocity);
        }

        public static void UpdateBodyProperties(PBody3D body,
            int bodyType,
            FVector3 linearDamping,
            FVector3 angularDamping,
            FVector3 gravityScale,
            bool fixedRotationX,
            bool fixedRotationY,
            bool fixedRotationZ)
        {
            if (!initialized)
            {
                Initialize();
            }

            NativeParallel3D.UpdateBodyProperties3D(
                body.IntPointer,
                bodyType,
                linearDamping,
                angularDamping,
                gravityScale,
                fixedRotationX,
                fixedRotationY,
                fixedRotationZ);
        }

        public static FVector3 GetPointVelocity(PBody3D body, FVector3 point)
        {
            if (!initialized)
            {
                Initialize();
            }

            FVector3 result = FVector3.zero;

            NativeParallel3D.GetPointVelocity3D(body.IntPointer, point, ref result);

            return result;
        }


        public static void UpdateCOM(PBody3D body, FVector3 centerOfMass)
        {
            if (!initialized)
            {
                Initialize();
            }

            NativeParallel3D.UpdateCOM3D(body.IntPointer, centerOfMass);
        }

        public static void UpdateMass(PBody3D body, FFloat mass)
        {
            if (!initialized)
            {
                Initialize();
            }

            NativeParallel3D.UpdateMass3D(body.IntPointer, mass);
        }

        public static void ApplyForce(PBody3D body, FVector3 point, FVector3 force)
        {
            if (!initialized)
            {
                Initialize();
            }

            NativeParallel3D.ApplyForce3D(body.IntPointer, point, force);
        }

        public static void ApplyForceToCenter(PBody3D body, FVector3 force)
        {
            if (!initialized)
            {
                Initialize();
            }

            NativeParallel3D.ApplyForceToCenter3D(body.IntPointer, force);
        }

        public static void ApplyTorque(PBody3D body, FVector3 torque)
        {
            if (!initialized)
            {
                Initialize();
            }

            NativeParallel3D.ApplyTorque3D(body.IntPointer, torque);
        }

        public static void ApplyLinearImpulse(PBody3D body, FVector3 point, FVector3 impulse)
        {
            if (!initialized)
            {
                Initialize();
            }

            NativeParallel3D.ApplyLinearImpulse3D(body.IntPointer, point, impulse);
        }

        public static void ApplyLinearImpulseToCenter(PBody3D body, FVector3 impulse)
        {
            if (!initialized)
            {
                Initialize();
            }

            NativeParallel3D.ApplyLinearImpulseToCenter3D(body.IntPointer, impulse);
        }

        public static void ApplyAngularImpulse(PBody3D body, FVector3 impulse)
        {
            if (!initialized)
            {
                Initialize();
            }

            NativeParallel3D.ApplyAngularImpulse3D(body.IntPointer, impulse);
        }

        public static void DestoryBody(PBody3D body, IParallelRigidbody3D rigidBody3D)
        {
            if (!initialized)
            {
                return;
            }

            if (bodySortedList.ContainsKey(body.BodyID))
            {
                bodySortedList.Remove(body.BodyID);
            }

            NativeParallel3D.DestroyBody3D(internalWorld.IntPointer, body.IntPointer);
        }

        public static void ReadNativeBody(PBody3D body)
        {
            if (!initialized)
            {
                Initialize();
            }

            NativeParallel3D.GetTransform3D(body.IntPointer, ref body.position, ref body.orientation, ref body.orientation0);
            NativeParallel3D.GetVelocity3D(body.IntPointer, ref body.linearVelocity, ref body.angularVelocity);


            body.awake = NativeParallel3D.IsAwake3D(body.IntPointer);
            NativeParallel3D.GetSleepTime3D(body.IntPointer, ref body.sleepTime);
        }

        public static void ReadBodyMassInfo(PBody3D body)
        {
            if (!initialized)
            {
                Initialize();
            }

            NativeParallel3D.GetBodyMassInfo3D(body.IntPointer, ref body.mass);
        }

        public static void SetAwakeForRollback(PBody3D body, bool awake, FFloat sleepTime)
        {
            if (!initialized)
            {
                Initialize();
            }

            NativeParallel3D.SetAwakeForRollback3D(body.IntPointer, awake, sleepTime);
        }

        public static void SetAwake(PBody3D body, bool awake)
        {
            if (!initialized)
            {
                Initialize();
            }

            NativeParallel3D.SetAwake3D(body.IntPointer, awake);
        }

        public static bool IsAwake(PBody3D body)
        {
            if (!initialized)
            {
                Initialize();
            }

            return NativeParallel3D.IsAwake3D(body.IntPointer);
        }

        public static void SetEnabled(PBody3D body, bool enabled)
        {
            if (!initialized)
            {
                return;
            }

            NativeParallel3D.SetEnabled3D(body.IntPointer, enabled);
        }

        public static bool IsEnabled(PBody3D body)
        {
            if (!initialized)
            {
                return false;
            }

            return NativeParallel3D.IsEnabled3D(body.IntPointer);
        }

        public static ParallelQHullData ConvextHull3D(FVector3[] verts, UInt32 count, bool simplify, FFloat rad)
        {
            UInt32 outCount = 1024 * 10;
            FVector3[] vertsOut = new FVector3[outCount];
            ParallelEdge[] edgesOut = new ParallelEdge[outCount];
            ParallelFace[] facesOut = new ParallelFace[outCount];
            ParallelPlane[] planesOut = new ParallelPlane[outCount];

            UInt32 vertsOutCount = outCount;
            UInt32 edgesOutCount = outCount;
            UInt32 facesOutCount = outCount;

            NativeParallel3D.ConvexHull3D(verts, count, vertsOut, ref vertsOutCount, edgesOut, ref edgesOutCount, facesOut, ref facesOutCount, planesOut, simplify, rad);

            ParallelQHullData parallelQHullData = new ParallelQHullData();
            parallelQHullData.vertexCount = vertsOutCount;
            parallelQHullData.edgeCount = edgesOutCount;
            parallelQHullData.faceCount = facesOutCount;
            parallelQHullData.vertices = vertsOut;
            parallelQHullData.edges = edgesOut;
            parallelQHullData.faces = facesOut;
            parallelQHullData.planes = planesOut;
            return parallelQHullData;
        }


        public static void ConvextHull3D1(Vector3[] verts, UInt32 count)
        {
            NativeParallel3D.ConvexHull3D1(verts, count);
        }

        public static ParallelQHullData2 ConvextHull3D2(Vector3[] verts, UInt32 count, int limit)
        {
            UInt32 outCount = 1024;
            ParallelIntTriangle[] trisOut = new ParallelIntTriangle[outCount];

            Vector3[] vertsOut = new Vector3[count];

            UInt32 trisOutCount = outCount;

            NativeParallel3D.ConvexHull3D2(verts, count, trisOut, ref trisOutCount, vertsOut, limit);

            ParallelQHullData2 parallelQHullData = new ParallelQHullData2();
            parallelQHullData.triCount = trisOutCount;
            parallelQHullData.tris = trisOut;
            parallelQHullData.vertices = vertsOut;
            return parallelQHullData;
        }

        //raycast
        public static bool RayCast(FVector3 start, FVector3 direction, FFloat range, out PRaycastHit3D raycastHit3D)
        {
            return RayCast(start, start + range * direction, out raycastHit3D);
        }

        public static bool RayCast(FVector3 start, FVector3 end, out PRaycastHit3D raycastHit3D)
        {
            return RayCast(start, end, -1, out raycastHit3D);
        }

        public static bool RayCast(FVector3 start, FVector3 direction, FFloat range, int mask, out PRaycastHit3D raycastHit3D)
        {
            return RayCast(start, start + range * direction, mask, out raycastHit3D);
        }

        public static bool RayCast(FVector3 start, FVector3 end, int mask, out PRaycastHit3D raycastHit3D)
        {
            if (!initialized)
            {
                Initialize();
            }

            raycastHit3D = new PRaycastHit3D();

            if (FVector3.Distance(start, end) < ParallelConstants.SMALLEST_RAYCAST_RANGE)
            {
                Debug.Log("RayCast range too short");
                return false;
            }

            FVector3 point = FVector3.zero;
            FVector3 normal = FVector3.zero;
            FFloat fraction = FFloat.one;

            raycastHit3D = new PRaycastHit3D();
            UInt16 bodyID = 0;

            bool hit = NativeParallel3D.RayCast3D(start, end, mask, ref point, ref normal, ref fraction, ref bodyID, internalWorld.IntPointer);

            if (hit)
            {
                raycastHit3D.point = point;
                raycastHit3D.normal = normal;
                raycastHit3D.fraction = fraction;
                if (bodySortedList.ContainsKey(bodyID))
                {
                    raycastHit3D.rigidbody = bodySortedList[bodyID].RigidBody;
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

        public static bool ShapeCast(PShape3D shape, FVector3 pos, FQuaternion rot, FVector3 movement, int mask, ref PShapecastHit3D shapeCastHit3D, ParallelRigidbody3D ignoreBody)
        {
            if (!initialized)
            {
                Initialize();
            }

            if (movement.magnitude < ParallelConstants.SMALLEST_RAYCAST_RANGE)
            {
                //Debug.Log("ShapeCast range too short");
                return false;
            }

            FVector3 point = FVector3.zero;
            FVector3 normal = FVector3.zero;
            FFloat fraction = FFloat.one;
            UInt16 bodyID = 0;

            UInt16 ignoreBodyID = 0;

            if (ignoreBody != null)
            {
                ignoreBodyID = ignoreBody._body3D.BodyID;
            }

            bool hit = NativeParallel3D.ShapeCast3D(internalWorld.IntPointer, mask, shape.IntPointer, pos, rot, movement, ref point, ref normal, ref fraction, ref bodyID, ignoreBodyID);

            if (hit)
            {
                shapeCastHit3D.point = point;
                shapeCastHit3D.normal = normal;
                shapeCastHit3D.fraction = fraction;

                if (bodySortedList.ContainsKey(bodyID))
                {
                    shapeCastHit3D.rigidbody = bodySortedList[bodyID].RigidBody;
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

        public static bool SphereCast(FVector3 start, FFloat radius, FVector3 movement, ref PShapecastHit3D shapeCastHit3D, ParallelRigidbody3D ignoreBody)
        {
            return SphereCast(start, radius, movement, -1, ref shapeCastHit3D, ignoreBody);
        }

        public static bool SphereCast(FVector3 start, FFloat radius, FVector3 movement, int mask, ref PShapecastHit3D shapeCastHit3D, ParallelRigidbody3D ignoreBody)
        {
            if (!initialized)
            {
                Initialize();
            }

            if (movement.magnitude < ParallelConstants.SMALLEST_RAYCAST_RANGE)
            {
                //Debug.Log("ShapeCast range too short");
                return false;
            }

            FVector3 point = FVector3.zero;
            FVector3 normal = FVector3.zero;
            FFloat fraction = FFloat.one;
            UInt16 bodyID = 0;

            UInt16 ignoreBodyID = 0;

            if (ignoreBody != null)
            {
                ignoreBodyID = ignoreBody._body3D.BodyID;
            }

            bool hit = NativeParallel3D.SphereCast3D(internalWorld.IntPointer, mask, start, radius, movement, ref point, ref normal, ref fraction, ref bodyID, ignoreBodyID);

            if (hit)
            {
                shapeCastHit3D.point = point;
                shapeCastHit3D.normal = normal;
                shapeCastHit3D.fraction = fraction;

                if (bodySortedList.ContainsKey(bodyID))
                {
                    shapeCastHit3D.rigidbody = bodySortedList[bodyID].RigidBody;
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
        public static bool OverlapSphere(FVector3 center, FFloat radius, PShapeOverlapResult3D shapeOverlapResult)
        {
            return OverlapSphere(center, radius, -1, shapeOverlapResult);
        }

        public static bool OverlapSphere(FVector3 center, FFloat radius, int mask, PShapeOverlapResult3D shapeOverlapResult)
        {
            if (!initialized)
            {
                Initialize();
            }

            int count = 0;
            bool hit = NativeParallel3D.SphereOverlap3D(internalWorld.IntPointer, mask, center, radius, _queryBodyIDs, ref count);

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


        public static bool OverlapCube(FVector3 center, FQuaternion rotation, FFloat x, FFloat y, FFloat z, int mask, PShapeOverlapResult3D shapeOverlapResult)
        {
            if (!initialized)
            {
                Initialize();
            }

            int count = 0;
            bool hit = NativeParallel3D.CubeOverlap3D(internalWorld.IntPointer, mask, center, rotation, x, y, z, _queryBodyIDs, ref count);

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
        [MonoPInvokeCallback(typeof(ContactEnterCallBack3D))]
        public static void OnContactEnterCallback(IntPtr contactPtr, UInt64 contactID)
        {
            PContact3D c;

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
                c = new PContact3D(contactID);
                c.state = ContactState.Enter;
                contactDictionary[contactID] = c;
            }

            AddEnterContactWrapper(c);
            //Debug.Log("Enter contact");
        }

        static void AddEnterContactWrapper(PContact3D contact)
        {
            _enterContactWrapperEnd.contact = contact;

            _enterContactWrapperEnd = _enterContactWrapperEnd.next;

            _enterContactCount++;
        }

        static void ResetEnterContacts()
        {
            _enterContactCount = 0;
            _enterContactWrapperEnd = _enterContactWrapperHead;
        }

        [MonoPInvokeCallback(typeof(ContactExitCallBack3D))]
        public static void OnContactExitCallBack(IntPtr contactPtr, UInt64 contactID)
        {
            PContact3D c;

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
                c = new PContact3D(contactID);
                c.state = ContactState.Enter;
                contactDictionary[contactID] = c;
            }

            AddExitContactWrapper(c);
            //Debug.Log("Exit contact");
        }

        static void AddExitContactWrapper(PContact3D contact)
        {
            _exitContactWrapperEnd.contact = contact;

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
                PContactExport3D export = contactExports[i];

                if (export.id == 0)
                {
                    continue;
                }

                PContact3D c;

                if (contactDictionary.ContainsKey(export.id))
                {
                    c = contactDictionary[export.id];
                }
                else
                {
                    c = new PContact3D(export.id);
                    contactDictionary[export.id] = c;
                }

                c.Update(
                    contactPtrs[i],
                    export.relativeVelocity,
                    export.isTrigger
                    );

                AddAllContactWrapper(c);
            }
        }

        static void AddAllContactWrapper(PContact3D contact)
        {
            _allContactWrapperEnd.contact = contact;

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

            IntPtr contactPtr = NativeParallel3D.GetContactList3D(internalWorld.IntPointer);

            while (contactPtr != IntPtr.Zero)
            {
                contactPtrs[index] = contactPtr;
                PContactExport3D export = contactExports[index];

                contactPtr = NativeParallel3D.ExportAndReturnNextContact3D(contactPtr, ref export);
                contactExports[index] = export;
                index++;
            }

            _contactCount = index;
        }

        //public static void GetContactDetail(IntPtr contactHandler, ref PContactPoints2D contactPoints2D)
        //{
        //    if (!initialized)
        //    {
        //        Initialize();
        //    }

        //    contactPoints2D.contactPointCount = 0;


        //    contactPoints2D.contactPointCount = NativeParallel.GetContactDetail(
        //                                                            contactHandler,
        //                                                            ref contactPoints2D.point1,
        //                                                            ref contactPoints2D.point2,
        //                                                            ref contactPoints2D.penetration1,
        //                                                            ref contactPoints2D.penetration2,
        //                                                            ref contactPoints2D.contactNormal);
        //}

        //triangulation
        public static PolyIsland CreatePolyIsland(FVector2[] verts, int[] indexes, int count)
        {
            IntPtr m_NativeObject = NativeParallel3D.CreatePolyIsland(verts, indexes, count);
            return new PolyIsland(m_NativeObject);
        }

        public static void DestroyPolyIsland(PolyIsland polyIsland)
        {
            NativeParallel3D.DestroyPolyIsland(polyIsland.IntPointer);
        }

        public static void AddHolePolyIsland(FVector2[] verts, int[] indexes, int count, PolyIsland polyIsland)
        {
            NativeParallel3D.AddHolePolyIsland(verts, indexes, count, polyIsland.IntPointer);
        }

        public static bool TriangulatePolyIsland(int[] indices, int[] indiceCounts, ref int triangleCount, ref int totalIndicesCount, int level, PolyIsland polyIsland)
        {
            return NativeParallel3D.TriangulatePolyIsland(indices, indiceCounts, ref triangleCount, ref totalIndicesCount, level, polyIsland.IntPointer);
        }

        //joints
        public static void DestroyJoint(PJoint3D joint)
        {
            if (!initialized)
            {
                return;
            }

            NativeParallel3D.DestroyJoint3D(internalWorld.IntPointer, joint.IntPointer);
        }

        public static PJoint3D CreateMouseJoint(ParallelRigidbody3D rb, FVector3 p, FFloat maxForce)
        {
            if (!initialized)
            {
                Initialize();
            }

            IntPtr m_NativeObject = NativeParallel3D.CreateMouseJoint3D(internalWorld.IntPointer, referenceBody, rb._body3D.IntPointer, p, maxForce);

            PJoint3D j = new PJoint3D(m_NativeObject);

            return j;
        }

        public static void MoveMouseJoint(PJoint3D joint, FVector3 position)
        {
            if (!initialized)
            {
                Initialize();
            }

            NativeParallel3D.MoveMouseJoint3D(joint.IntPointer, position);
        }

        public static PJoint3D CreateSprintJoint(ParallelRigidbody3D rbA,
                                                 ParallelRigidbody3D rbB,
                                                 FVector3 anchorA,
                                                 FVector3 anchorB,
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
                other = rbB._body3D.IntPointer;
            }

            IntPtr m_NativeObject = NativeParallel3D.CreateDistanceJoint3D(internalWorld.IntPointer,
                                                                            rbA._body3D.IntPointer,
                                                                            other,
                                                                            anchorA, anchorB,
                                                                            collide,
                                                                            frequency, damping);

            PJoint3D j = new PJoint3D(m_NativeObject);

            return j;
        }

        public static PJoint3D CreateHingeJoint(ParallelRigidbody3D rbA,
                                                ParallelRigidbody3D rbB,
                                                FVector3 anchor,
                                                FVector3 axis,
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
                other = rbB._body3D.IntPointer;
            }

            IntPtr m_NativeObject = NativeParallel3D.CreateHingeJoint3D(internalWorld.IntPointer,
                                                                            rbA._body3D.IntPointer,
                                                                            other,
                                                                            anchor, axis,
                                                                            collide,
                                                                            limit, lowerAngle, upperAngle,
                                                                            motor, motorSpeed, motorTorque);

            PJoint3D j = new PJoint3D(m_NativeObject);

            return j;
        }

        public static PJoint3D CreateConeJoint(ParallelRigidbody3D rbA,
                                                ParallelRigidbody3D rbB,
                                                FVector3 anchor,
                                                FVector3 axis,
                                                bool collide,
                                                bool limit, FFloat angle,
                                                bool twist, FFloat lowerAngle, FFloat upperAngle)
        {
            if (!initialized)
            {
                Initialize();
            }

            IntPtr other = referenceBody;

            if (rbB != null)
            {
                other = rbB._body3D.IntPointer;
            }

            IntPtr m_NativeObject = NativeParallel3D.CreateConeJoint3D(internalWorld.IntPointer,
                                                                            rbA._body3D.IntPointer,
                                                                            other,
                                                                            anchor, axis,
                                                                            collide,
                                                                            limit, angle,
                                                                            twist, lowerAngle, upperAngle);

            PJoint3D j = new PJoint3D(m_NativeObject);

            return j;
        }
    }
}