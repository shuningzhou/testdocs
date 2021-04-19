using System.Collections.Generic;
using UnityEngine;

namespace Parallel
{
    public struct InterpolationPostionTimeData
    {
        public float time;
        public Vector3 pos;
        public Quaternion rot;

        public static InterpolationPostionTimeData empty
        {
            get
            {
                return new InterpolationPostionTimeData(0.0f, Vector3.zero, Quaternion.identity);
            }
        }

        public InterpolationPostionTimeData(float time, Vector3 pos, Quaternion rot)
        {
            this.time = time;
            this.pos = pos;
            this.rot = rot;
        }
    }

    public delegate void TransformScaleUpdated();

    /// <summary>
    /// Position, rotation, and scale of an object.
    /// </summary>
    [ExecuteInEditMode]
    public class ParallelTransform : MonoBehaviour
    {
        // used for interpolation
        static List<ParallelTransform> _sParallelTransforms = new List<ParallelTransform>();
        public static void Commit(float deltaTime)
        {
            foreach(ParallelTransform parallelTransform in _sParallelTransforms)
            {
                parallelTransform.CommitChanges(deltaTime);
            }
        }

        public static void Sync()
        {
            foreach (ParallelTransform parallelTransform in _sParallelTransforms)
            {
                parallelTransform.transform.localPosition = (Vector3)parallelTransform.localPosition;
                parallelTransform.transform.localRotation = (Quaternion)parallelTransform.localRotation;
            }
        }

        [SerializeField]
        bool Interpolate = false;
        public bool debug = false;

        [SerializeField]
        FVector3 _localPosition = FVector3.zero;
        [SerializeField]
        FQuaternion _localRotation = FQuaternion.identity;
        [SerializeField]
        FVector3 _localEularAngles = FVector3.zero;
        [SerializeField]
        FVector3 _localScale = FVector3.one;

        //
        ParallelRigidbody2D _rigidbody2D;
        ParallelRigidbody3D _rigidbody3D;

        bool _quatReady = false;
        bool _eularReady = true;

        void Log(string message)
        {
            if(debug)
            {
                Debug.Log(message);
            }
        }

        FVector3 _internalLocalEularAngles
        {
            get
            {
                if(eularReady)
                {
                    //Log($"read _localEularAngles=>{_localEularAngles}");
                    return _localEularAngles;
                }
                else
                {
                    FVector3 newEuler = _localRotation.eulerAngles;//_localRotation.EulerAngles();
                    //Log($"read newEuler=>{newEuler}");
                    _localEularAngles = newEuler;
                    //Log($"read _localEularAngles=>{_localEularAngles}");
                    eularReady = true;
                    return _localEularAngles;
                }
            }
            set
            {
                //Log($"set _localEularAngles=>{value}");
                _localEularAngles = value;
                eularReady = true;
                quatReady = false;
            }
        }

        FQuaternion _internalLocalRotation
        {
            get
            {
                if(quatReady)
                {
                    //Log($"read _localRotation=>{_localEularAngles}");
                    return _localRotation;
                }
                else
                {
                    //Log($"read _localRotation=>{_localEularAngles}");
                    //_localRotation = FQuaternion.FromEulerAngles(_localEularAngles);
                    _localRotation.eulerAngles = _localEularAngles;
                    quatReady = true;
                    return _localRotation;
                }
            }
            set
            {
                //Log($"set _localRotation=>{value}");
                _localRotation = value;
                quatReady = true;
                eularReady = false;
            }
        }

        bool quatReady
        {
            get
            {
                //Log($"{name} read quat=>{_quatReady}");

                return _quatReady;
            }
            set
            {

                //Log($"{name} set quat=>{value}");

                _quatReady = value;
            }
        }

        bool eularReady
        {
            get
            {

                //Log($"{name} read eular=>{_eularReady}");

                return _eularReady;
            }
            set
            {
                //Log($"{name} set eular=>{value}");
                _eularReady = value;
            }
        }

        public ParallelTransform parent
        {
            get
            {
                Transform uParent = unityParent;
                if(uParent == null)
                {
                    return null;
                }
                ParallelTransform parentParallelTransform = uParent.gameObject.GetComponent<ParallelTransform>();
                return parentParallelTransform;
            }
        }

        public Transform unityParent
        {
            get
            {
                Transform p = transform.parent;
                return p;
            }
        }

        FMatrix4x4 matrix
        {
            get
            {
                FMatrix4x4 m = FMatrix4x4.TRS(_localPosition, _internalLocalRotation, _localScale);
                return m;
            }
        }

        FMatrix4x4 matrixUnscaled
        {
            get
            {
                FMatrix4x4 m = FMatrix4x4.TRS(_localPosition, _internalLocalRotation, FVector3.one);
                return m;
            }
        }

        FMatrix4x4 inverseMatrix
        {
            get
            {
                //FVector3 inverseTranslation = -_localPosition;
                //FQuaternion inverseRotation = FQuaternion.Inverse(_internalLocalRotation);
                //FVector3 inverseScale = FFloat.one / _localScale;

                //FMatrix4x4 m = FMatrix4x4.TRS(inverseTranslation, inverseRotation, inverseScale);
                //return m;

                return FMatrix4x4.Inverse(matrix);
            }
        }

        FQuaternion inverseRotation
        {
            get
            {
                FQuaternion inverseRotation = FQuaternion.Inverse(_internalLocalRotation);
                return inverseRotation;
            }
        }


        public FMatrix4x4 localToWorldMatrix
        {
            get
            {
                if (parent == null)
                {
                    return matrix;
                }

                FMatrix4x4 parentLocalToWorld = parent.localToWorldMatrix;
                FMatrix4x4 r = parentLocalToWorld * matrix;
                return r;
            }
        }

        public FMatrix4x4 localToWorldMatrixUnscaled
        {
            get
            {
                if (parent == null)
                {
                    return matrixUnscaled;
                }

                FMatrix4x4 parentLocalToWorld = parent.localToWorldMatrixUnscaled;
                FMatrix4x4 r = parentLocalToWorld * matrixUnscaled;
                return r;
            }
        }

        public FMatrix4x4 worldToLocalMatrix
        {
            get
            {
                FMatrix4x4 r = FMatrix4x4.Inverse(localToWorldMatrix);
                return r;
            }
        }

        /// <summary>
        /// Exports the fixed point values to the Unity Transform Component
        /// </summary>
        private void ExportToUnity()
        {
            transform.localPosition = (Vector3)_localPosition;

            if (eularReady)
            {
                transform.localEulerAngles = (Vector3)_localEularAngles;
            }
            else
            {
                transform.localRotation = (Quaternion)_internalLocalRotation;
            }

            transform.localScale = (Vector3)_localScale;
        }

        /// <summary>
        /// Imports the floating point values from the Unity Transform Component.
        /// Only works in the editing mode.
        /// NOT deterministic
        /// </summary>
        public bool ImportFromUnity()
        {
            bool changed = false;

            if(!Application.isPlaying)
            {
                FVector3 newPosition = (FVector3)transform.localPosition;
                FQuaternion newRotation = (FQuaternion)transform.localRotation;
                FVector3 newEularAngles = (FVector3)transform.localEulerAngles;
                FVector3 newScale = (FVector3)transform.localScale;

                if(_localPosition != newPosition)
                {
                    changed = true;
                    _localPosition = newPosition;
                }
                
                if(_localRotation != newRotation)
                {
                    changed = true;
                    _localRotation = newRotation;
                }
                
                if(_localEularAngles != newEularAngles)
                {
                    changed = true;
                    _localEularAngles = newEularAngles;
                }

                if(_localScale != newScale)
                {
                    changed = true;
                    _localScale = newScale;
                }  
            }

            return changed;
        }

        public FVector3 position
        {
            get
            {
                if(parent == null)
                {
                    return _localPosition;
                }

                FMatrix4x4 m = parent.localToWorldMatrix;
                FVector3 p = m.MultiplyPoint3x4(_localPosition);
                return p;
            }
            set
            {
                if (parent == null)
                {
                    localPosition = value;
                    return;
                }

                FMatrix4x4 m = parent.worldToLocalMatrix;

                FVector3 p = m.MultiplyPoint3x4(value);

                localPosition = p;
            }
        }

        public FVector3 localPosition
        {
            get
            {
                return _localPosition;
            }
            set
            {
                _localPosition = value;

                UpdateRigidbodyTransform();

                if(!Interpolate)
                {
                    ExportToUnity();
                }
            }
        }

        void UpdateRigidbodyTransform()
        {
            if(_rigidbody2D != null)
            {
                Parallel2D.UpdateBodyTransForm(_rigidbody2D._body2D, (FVector2)_localPosition, FFloat.DegToRad(_internalLocalEularAngles.z));
            }

            if(_rigidbody3D != null && _rigidbody3D.enabled)
            {
                Parallel3D.UpdateBodyTransForm(_rigidbody3D._body3D, _localPosition, _internalLocalRotation);
            }
        }


        /// <summary>
        ///   <para>The rotation as Euler angles in degrees.</para>
        /// </summary>
        public FVector3 localEulerAngles
        {
            get
            {
                return _internalLocalEularAngles;
            }
            set
            {
                if(_internalLocalEularAngles == value)
                {
                    return;
                }

                _internalLocalEularAngles = value;
                UpdateRigidbodyTransform();
                if (!Interpolate)
                {
                    ExportToUnity();
                }
            } 
        }

        public FVector3 eulerAngles
        {
            get
            {
                if (parent == null)
                {
                    return localEulerAngles;
                }

                FVector3 parentAngles = parent.eulerAngles;
                FVector3 worldAngles = parentAngles + _internalLocalEularAngles;
                return worldAngles;
            }
            set
            {
                if(parent == null)
                {
                    localEulerAngles = value;
                    return;
                }

                //expensive
                FQuaternion invRotP = parent.inverseRotation;
                FQuaternion localRot = invRotP * FQuaternion.FromEulerRad(value * FMath.Deg2Rad);
                localRotation = localRot;
            }
        }

        public FQuaternion localRotation
        {
            get
            {
                return _internalLocalRotation;
            }
            set
            {
                if (_internalLocalRotation == value)
                {
                    return;
                }

                _internalLocalRotation = value;
                UpdateRigidbodyTransform();
                if (!Interpolate)
                {
                    ExportToUnity();
                }
            }
        }

        public FQuaternion rotation
        {
            get
            {
                if (parent == null)
                {
                    return _internalLocalRotation;
                }

                FQuaternion rotP = parent.rotation;
                FQuaternion worldRot = rotP * _internalLocalRotation;
                return worldRot;
            }
            set
            {
                if(parent == null)
                {
                    localRotation = value;
                    return;
                }

                FQuaternion invRotP = parent.inverseRotation;
                FQuaternion localRot = invRotP * value;
                localRotation = localRot;
            }
        }


        /// <summary>
        ///   <para>The scale of the transform relative to the parent. won't change collider dimensions in children GameObjects</para>
        /// </summary>
        public FVector3 localScale
        {
            get
            {
                return _localScale;
            }
            set
            {
                if (_localScale == value)
                {
                    return;
                }

                _localScale = value;
                //Todo: update collider dimensions
                if (!Interpolate)
                {
                    ExportToUnity();
                }
            }
        }

        public FVector3 right
        {
            get
            {
                return rotation * FVector3.right;
            }
        }

        public FVector3 up
        {
            get
            {
                return rotation * FVector3.up;
            }
        }

        public FVector3 forward
        {
            get
            {
                return rotation * FVector3.forward;
            }
        }

        // translate
        public void Translate(FVector3 translation)
        {
            ParallelSpace relativeTo = ParallelSpace.Self;
            Translate(translation, relativeTo);
        }

        public void Translate(FVector3 translation, ParallelSpace relativeTo)
        {
            if(relativeTo == ParallelSpace.World)
            {
                position += translation;
            }
            else
            {
                position += TransformDirection(translation);
            }
        }

        public void Translate(FFloat x, FFloat y, FFloat z)
        {
            ParallelSpace relativeTo = ParallelSpace.Self;
            Translate(x, y, z, relativeTo);
        }

        public void Translate(FFloat x, FFloat y, FFloat z, ParallelSpace relativeTo)
        {
            Translate(new FVector3(x, y, z), relativeTo);
        }

        //rotate
        //rotate in degree
        public void Rotate(FVector3 eulerAngles)
        {
            ParallelSpace relativeTo = ParallelSpace.Self;
            Rotate(eulerAngles, relativeTo);
        }

        public void Rotate(FVector3 eulerAngles, ParallelSpace relativeTo)
        {
            FQuaternion quat = FQuaternion.FromEulerRad(eulerAngles * FMath.Deg2Rad);

            if (relativeTo == ParallelSpace.Self)
            {
                localRotation *= quat; 
            }
            else
            {
                rotation *= quat;
            }
        }

        public void Rotate(FFloat xAngle, FFloat yAngle, FFloat zAngle)
        {
            ParallelSpace relativeTo = ParallelSpace.Self;
            Rotate(xAngle, yAngle, zAngle, relativeTo);
        }

        public void Rotate(FFloat xAngle, FFloat yAngle, FFloat zAngle, ParallelSpace relativeTo)
        {
            Rotate(new FVector3(xAngle, yAngle, zAngle), relativeTo);
        }

        public void RotateInWorldSpace(FVector3 eulers)
        {
            //Rotation = FQuaternion.FromEulerAngles(eulers) * Rotation;
            rotation = rotation * FQuaternion.FromEulerRad(eulers * FMath.Deg2Rad);
        }

        public void RotateInLocalSpace(FVector3 eulers)
        {
            //Rotation = FQuaternion.FromEulerAngles(eulers) * Rotation;
            localRotation = localRotation * FQuaternion.FromEulerRad(eulers * FMath.Deg2Rad);
        }

        public void Rotate(FVector3 axis, FFloat angle)
        {
            ParallelSpace relativeTo = ParallelSpace.Self;
            Rotate(axis, angle, relativeTo);
        }

        public void Rotate(FVector3 axis, FFloat angle, ParallelSpace relativeTo)
        {
            if (relativeTo == ParallelSpace.Self)
            {
                FVector3 selfAxis = TransformDirection(axis);
                FQuaternion q = FQuaternion.AngleAxis(angle, selfAxis);
                rotation *= q;
            }
            else
            {
                FQuaternion q = FQuaternion.AngleAxis(angle, axis);
                rotation *= q;
            }
        }

        public void RotateAround(FVector3 point, FVector3 axis, FFloat angle)
        {
            FVector3 worldPos = position;
            FQuaternion q = FQuaternion.AngleAxis(angle, axis);
            FVector3 dif = worldPos - point;
            dif = q * dif;
            worldPos = point + dif;
            position = worldPos;
            rotation *= q;
        }

        // lookat
        public void LookAt(ParallelTransform target)
        {
            FVector3 up = FVector3.up;
            LookAt(target, up);
        }

        public void LookAt(ParallelTransform target, FVector3 worldUp)
        {
            if (target)
            {
                LookAt(target.position, worldUp);
            }
        }

        public void LookAt(FVector3 worldPosition, FVector3 worldUp)
        {
            FVector3 dir = (worldPosition - position).normalized;
            rotation = FQuaternion.LookRotation(dir, worldUp);
        }

        public void LookAt(FVector3 worldPosition)
        {
            FVector3 up = FVector3.up;
            FVector3 dir = (worldPosition - position).normalized;
            rotation = FQuaternion.LookRotation(dir, up);
        }

        //Transforms position from local space to world space.
        public FVector3 TransformPoint(FVector3 position)
        {
            FMatrix4x4 m = localToWorldMatrix;
            FVector3 p = m.MultiplyPoint3x4(position);
            return p;
        }

        public FVector3 TransformPointUnscaled(FVector3 position)
        {
            FMatrix4x4 m = localToWorldMatrixUnscaled;
            FVector3 p = m.MultiplyPoint3x4(position);
            return p;
        }

        //Transforms position from world space to local space.
        public FVector3 InverseTransformPoint(FVector3 position)
        {
            FMatrix4x4 m = worldToLocalMatrix;
            FVector3 p = m.MultiplyPoint3x4(position);
            return p;
        }

        //Transforms direction from local space to world space.
        public FVector3 TransformDirection(FVector3 direction)
        {
            FMatrix4x4 m = localToWorldMatrix;
            FVector3 p = m.MultiplyVector(direction);
            return p;
        }

        public FVector3 TransformDirectionUnscaled(FVector3 direction)
        {
            FMatrix4x4 m = localToWorldMatrixUnscaled;
            FVector3 p = m.MultiplyVector(direction);
            return p;
        }

        //Transforms direction from world space to local space.
        public FVector3 InverseTransfromDirction(FVector3 direction)
        {
            FMatrix4x4 m = worldToLocalMatrix;
            FVector3 p = m.MultiplyVector(direction);
            return p;
        }

        //interpolation
        public void UpdateUnityTransform()
        {
            if (!Interpolate)
            {
                return;
            }

            transform.localPosition = (Vector3)localPosition;
            transform.localRotation = (Quaternion)localRotation;
        }


        //should only be used by the attached rigidbody to update the transform
        //IMPORTATNT: all the internal transform data are in world space and should only be called on the root GameObject
        internal void _internal_WriteTranform2D(FVector2 position, FVector3 eulerAngles, bool xzPlane)
        {
            if(xzPlane)
            {
                _localPosition = new FVector3(position.x, _localPosition.y, position.y);
            }
            else
            {
                _localPosition = new FVector3(position.x, position.y, _localPosition.z);
            }

            _internalLocalEularAngles = eulerAngles;

            if (!Interpolate)
            {
                ExportToUnity();
            }
        }

        internal void _internal_WriteTranform(FVector3 position, FQuaternion rotation)
        {
            _localPosition = position;
            _internalLocalRotation = rotation;

            if (!Interpolate)
            {
                ExportToUnity();
            }
        }

        // used for interpolation
        float _updateTime;
        float _internalTime;
        //bool _oldIs1 = true;
        InterpolationPostionTimeData _interpolationPostionTimeData = InterpolationPostionTimeData.empty;
        //InterpolationPostionTimeData _interpolationPostionTimeData2 = InterpolationPostionTimeData.empty;

        void CommitChanges(float deltaTime)
        {
            if(_internalTime < 0.001f)
            {
                _internalTime += deltaTime;
            }

            _internalTime += deltaTime;

            //Log(name + "CommitChanges: add=" + deltaTime + " internal=" + _internalTime);

            _interpolationPostionTimeData.time = _internalTime;
            _interpolationPostionTimeData.pos = (Vector3)_localPosition;
            _interpolationPostionTimeData.rot = (Quaternion)localRotation;

            transform.localScale = (Vector3)_localScale;
        }

        void IncreateInternalTime(float deltaTime)
        {
            _internalTime += deltaTime;
            _interpolationPostionTimeData.time = _internalTime;
            //Log(name + "IncreateInternalTime: add=" + deltaTime + " internal=" + _internalTime);
        }

        void UpdateUnityTransformWithInterpolatedPositionAndRotation(float deltaTime)
        {
            float prevTime = _updateTime;
            float newUpdateTime = prevTime + deltaTime;

            if(newUpdateTime >= _interpolationPostionTimeData.time)
            {
                newUpdateTime = prevTime;
            }

            _updateTime = newUpdateTime;

            float a = deltaTime;
            float b = _interpolationPostionTimeData.time - prevTime;
            float ratio = a / b;

            Vector3 prevPos = transform.localPosition;
            transform.localPosition = Vector3.Lerp(transform.localPosition, _interpolationPostionTimeData.pos, ratio);
            float displacement = (transform.localPosition - prevPos).magnitude;
            float speed = displacement / a;
            //Log(name + "Interpolation: updateTime=" + _updateTime + " prevTime=" + prevTime + " newTime=" + _interpolationPostionTimeData.time + " a=" + a + " b=" + b + " ratio=" + ratio + " displacement=" + displacement + " speed=" + speed);

            transform.localRotation = Quaternion.Slerp(transform.localRotation, _interpolationPostionTimeData.rot, ratio);
            return;
        }

        void InitializeInterpolation()
        {
            _updateTime = 0;
            _internalTime = 0;

            _interpolationPostionTimeData.time = 0;

            _interpolationPostionTimeData.pos = (Vector3)_localPosition;

            _interpolationPostionTimeData.rot = (Quaternion)localRotation;
        }

        // Unity events

        private void Awake()
        {
            if (_rigidbody2D == null)
            {
                _rigidbody2D = GetComponent<ParallelRigidbody2D>();
            }

            if (_rigidbody3D == null)
            {
                _rigidbody3D = GetComponent<ParallelRigidbody3D>();
            }
        }

        private void OnEnable()
        {
            if(Interpolate && Application.isPlaying)
            {
                InitializeInterpolation();
                //Log($"ParallelTransform interpolation [ADD]={this.name}");
                _sParallelTransforms.Add(this);
            }
        }

        private void OnDisable()
        {
            if (Interpolate && Application.isPlaying)
            {
                //Log($"ParallelTransform interpolation [REMOVE]={this.name}");
                _sParallelTransforms.Remove(this);
            }
        }

        void Reset()
        {
#if UNITY_EDITOR
            ImportFromUnity();
            ExportToUnity();
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }

        void Update()
        {
            if (!Application.isPlaying)
            {
#if UNITY_EDITOR
                if (transform.hasChanged)
                {
                    bool changed = ImportFromUnity();

                    if(changed)
                    {
                        UnityEditor.EditorUtility.SetDirty(this);
                        transform.hasChanged = false;
                    }
                }
#endif
            }
            else
            {
                if(Interpolate)
                {
                    if(_internalTime < 0.001f)
                    {
                        //wait for the first ParallelTransform commit
                        return;
                    }
                    UpdateUnityTransformWithInterpolatedPositionAndRotation(Time.deltaTime);
                }
            }
        }
    }
}
