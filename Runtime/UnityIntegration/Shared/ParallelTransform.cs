using System.Collections.Generic;
using UnityEngine;

namespace Parallel
{
    public struct InterpolationPostionTimeData
    {
        public float time;
        public Vector3 pos;

        public static InterpolationPostionTimeData empty
        {
            get
            {
                return new InterpolationPostionTimeData(0.0f, Vector3.zero);
            }
        }

        public InterpolationPostionTimeData(float time, Vector3 pos)
        {
            this.time = time;
            this.pos = pos;
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

        [SerializeField]
        bool Interpolate = false;

        [SerializeField]
        Fix64Vec3 _localPosition = Fix64Vec3.zero;
        [SerializeField]
        Fix64Quat _localRotation = Fix64Quat.identity;
        [SerializeField]
        Fix64Vec3 _localEularAngles = Fix64Vec3.zero;
        [SerializeField]
        Fix64Vec3 _localScale = Fix64Vec3.one;

        //
        ParallelRigidbody2D _rigidbody2D;
        ParallelRigidbody3D _rigidbody3D;

        bool _quatReady = false;
        bool _eularReady = true;

        Fix64Vec3 _internalLocalEularAngles
        {
            get
            {
                if(_eularReady)
                {
                    return _localEularAngles;
                }
                else
                {
                    _localEularAngles = _localRotation.EulerAngles();
                    _eularReady = true;
                    return _localEularAngles;
                }
            }
            set
            {
                _localEularAngles = value;
                _eularReady = true;
                _quatReady = false;
            }
        }

        Fix64Quat _internalLocalRotation
        {
            get
            {
                if(_quatReady)
                {
                    return _localRotation;
                }
                else
                {
                    _localRotation = Fix64Quat.FromEulerAngles(_localEularAngles);
                    _quatReady = true;
                    return _localRotation;
                }
            }
            set
            {
                _localRotation = value;
                _quatReady = true;
                _eularReady = false;
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

        Fix64Matrix4X4 matrix
        {
            get
            {
                Fix64Matrix4X4 m = Fix64Matrix4X4.TRS(_localPosition, _internalLocalRotation, _localScale);
                return m;
            }
        }

        Fix64Matrix4X4 matrixUnscaled
        {
            get
            {
                Fix64Matrix4X4 m = Fix64Matrix4X4.TRS(_localPosition, _internalLocalRotation, Fix64Vec3.one);
                return m;
            }
        }

        Fix64Matrix4X4 inverseMatrix
        {
            get
            {
                //Fix64Vec3 inverseTranslation = -_localPosition;
                //Fix64Quat inverseRotation = Fix64Quat.Inverse(_internalLocalRotation);
                //Fix64Vec3 inverseScale = Fix64.one / _localScale;

                //Fix64Matrix4X4 m = Fix64Matrix4X4.TRS(inverseTranslation, inverseRotation, inverseScale);
                //return m;

                return Fix64Matrix4X4.Inverse(matrix);
            }
        }

        Fix64Quat inverseRotation
        {
            get
            {
                Fix64Quat inverseRotation = Fix64Quat.Inverse(_internalLocalRotation);
                return inverseRotation;
            }
        }


        public Fix64Matrix4X4 localToWorldMatrix
        {
            get
            {
                if (parent == null)
                {
                    return matrix;
                }

                Fix64Matrix4X4 parentLocalToWorld = parent.localToWorldMatrix;
                Fix64Matrix4X4 r = parentLocalToWorld * matrix;
                return r;
            }
        }

        public Fix64Matrix4X4 localToWorldMatrixUnscaled
        {
            get
            {
                if (parent == null)
                {
                    return matrixUnscaled;
                }

                Fix64Matrix4X4 parentLocalToWorld = parent.localToWorldMatrixUnscaled;
                Fix64Matrix4X4 r = parentLocalToWorld * matrixUnscaled;
                return r;
            }
        }

        public Fix64Matrix4X4 worldToLocalMatrix
        {
            get
            {
                if(parent == null)
                {
                    return inverseMatrix;
                }

                Fix64Matrix4X4 parentWorldToLocal = parent.worldToLocalMatrix;
                Fix64Matrix4X4 r = parentWorldToLocal * inverseMatrix;
                return r;
            }
        }

        /// <summary>
        /// Exports the fixed point values to the Unity Transform Component
        /// </summary>
        private void ExportToUnity()
        {
            transform.localPosition = (Vector3)_localPosition;

            if (_eularReady)
            {
                transform.localEulerAngles = (Vector3)_localEularAngles;
            }
            else
            {
                transform.localRotation = (Quaternion)_localRotation;
            }

            transform.localScale = (Vector3)_localScale;
        }

        /// <summary>
        /// Imports the floating point values from the Unity Transform Component.
        /// Only works in the editing mode.
        /// NOT deterministic
        /// </summary>
        public void ImportFromUnity()
        {
            if(!Application.isPlaying)
            {
                _localPosition = (Fix64Vec3)transform.localPosition;
                _localRotation = (Fix64Quat)transform.localRotation;
                _localEularAngles = (Fix64Vec3)transform.localEulerAngles;
                _localScale = (Fix64Vec3)transform.localScale;
            }
        }

        public Fix64Vec3 position
        {
            get
            {
                if(parent == null)
                {
                    return _localPosition;
                }

                Fix64Matrix4X4 m = parent.localToWorldMatrix;
                Fix64Vec3 p = m.MultiplyPoint3x4(_localPosition);
                return p;
            }
            set
            {
                if (parent == null)
                {
                    localPosition = value;
                    return;
                }

                Fix64Matrix4X4 m = parent.worldToLocalMatrix;

                Fix64Vec3 p = m.MultiplyPoint3x4(value);

                localPosition = p;
            }
        }

        public Fix64Vec3 localPosition
        {
            get
            {
                return _localPosition;
            }
            set
            {
                if (_localPosition == value)
                {
                    return;
                }

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
            if(_rigidbody2D == null)
            {
                _rigidbody2D = GetComponent<ParallelRigidbody2D>();
            }

            if(_rigidbody3D == null)
            {
                _rigidbody3D = GetComponent<ParallelRigidbody3D>();
            }

            if(_rigidbody2D != null)
            {
                Parallel2D.UpdateBodyTransForm(_rigidbody2D._body2D, (Fix64Vec2)_localPosition, Fix64.DegToRad(_internalLocalEularAngles.z));
            }

            if(_rigidbody3D != null && _rigidbody3D.enabled)
            {
                Parallel3D.UpdateBodyTransForm(_rigidbody3D._body3D, _localPosition, _internalLocalRotation);
            }
        }


        /// <summary>
        ///   <para>The rotation as Euler angles in degrees.</para>
        /// </summary>
        public Fix64Vec3 localEulerAngles
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

        public Fix64Vec3 eulerAngles
        {
            get
            {
                if (parent == null)
                {
                    return localEulerAngles;
                }

                Fix64Vec3 parentAngles = parent.eulerAngles;
                Fix64Vec3 worldAngles = parentAngles + _internalLocalEularAngles;
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
                Fix64Quat invRotP = parent.inverseRotation;
                Fix64Quat localRot = invRotP * Fix64Quat.FromEulerAngles(value);
                localRotation = localRot;
            }
        }

        public Fix64Quat localRotation
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

        public Fix64Quat rotation
        {
            get
            {
                if (parent == null)
                {
                    return _internalLocalRotation;
                }

                Fix64Quat rotP = parent.rotation;
                Fix64Quat worldRot = rotP * _internalLocalRotation;
                return worldRot;
            }
            set
            {
                if(parent == null)
                {
                    localRotation = value;
                    return;
                }

                Fix64Quat invRotP = parent.inverseRotation;
                Fix64Quat localRot = invRotP * value;
                localRotation = localRot;
            }
        }


        /// <summary>
        ///   <para>The scale of the transform relative to the parent. won't change collider dimensions in children GameObjects</para>
        /// </summary>
        public Fix64Vec3 localScale
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

        public Fix64Vec3 right
        {
            get
            {
                return rotation * Fix64Vec3.right;
            }
        }

        public Fix64Vec3 up
        {
            get
            {
                return rotation * Fix64Vec3.up;
            }
        }

        public Fix64Vec3 forward
        {
            get
            {
                return rotation * Fix64Vec3.forward;
            }
        }

        //transform helper

        //Transforms position from local space to world space.
        public Fix64Vec3 TransformPoint(Fix64Vec3 position)
        {
            Fix64Matrix4X4 m = localToWorldMatrix;
            Fix64Vec3 p = m.MultiplyPoint3x4(position);
            return p;
        }

        public Fix64Vec3 TransformPointUnscaled(Fix64Vec3 position)
        {
            Fix64Matrix4X4 m = localToWorldMatrixUnscaled;
            Fix64Vec3 p = m.MultiplyPoint3x4(position);
            return p;
        }

        //Transforms position from world space to local space.
        public Fix64Vec3 InverseTransformPoint(Fix64Vec3 position)
        {
            Fix64Matrix4X4 m = worldToLocalMatrix;
            Fix64Vec3 p = m.MultiplyPoint3x4(position);
            return p;
        }

        //Transforms direction from local space to world space.
        public Fix64Vec3 TransformDirection(Fix64Vec3 direction)
        {
            Fix64Matrix4X4 m = localToWorldMatrix;
            Fix64Vec3 p = m.MultiplyVector(direction);
            return p;
        }

        public Fix64Vec3 TransformDirectionUnscaled(Fix64Vec3 direction)
        {
            Fix64Matrix4X4 m = localToWorldMatrixUnscaled;
            Fix64Vec3 p = m.MultiplyVector(direction);
            return p;
        }

        //Transforms direction from world space to local space.
        public Fix64Vec3 InverseTransfromDirction(Fix64Vec3 direction)
        {
            Fix64Matrix4X4 m = worldToLocalMatrix;
            Fix64Vec3 p = m.MultiplyVector(direction);
            return p;
        }

        //rotate in degree
        public void RotateInWorldSpace(Fix64Vec3 eulers)
        {
            //Rotation = Fix64Quat.FromEulerAngles(eulers) * Rotation;
            rotation = rotation * Fix64Quat.FromEulerAngles(eulers);
        }

        public void RotateInLocalSpace(Fix64Vec3 eulers)
        {
            //Rotation = Fix64Quat.FromEulerAngles(eulers) * Rotation;
            localRotation = localRotation * Fix64Quat.FromEulerAngles(eulers);
        }

        //should only be used by the attached rigidbody to update the transform
        //IMPORTATNT: all the internal transform data are in world space and should only be called on the root GameObject
        internal void _internal_WriteTranform(Fix64Vec3 position, Fix64Vec3 eulerAngles)
        {
            _localPosition = position;
            _internalLocalEularAngles = eulerAngles;

            if (!Interpolate)
            {
                ExportToUnity();
            }
        }

        internal void _internal_WriteTranform(Fix64Vec3 position, Fix64Quat rotation)
        {
            _localPosition = position;
            _internalLocalRotation = rotation;

            if (!Interpolate)
            {
                ExportToUnity();
            }
        }

        internal void _internal_ExportToUnity()
        {
            transform.localPosition = (Vector3)_localPosition;

            if (_eularReady)
            {
                transform.localEulerAngles = (Vector3)_localEularAngles;
            }
            else
            {
                transform.localRotation = (Quaternion)_localRotation;
            }

            transform.localScale = (Vector3)_localScale;
        }

        // used for interpolation
        float _updateTime;
        float _internalTime;
        bool _oldIs1 = true;
        InterpolationPostionTimeData _interpolationPostionTimeData1 = InterpolationPostionTimeData.empty;
        InterpolationPostionTimeData _interpolationPostionTimeData2 = InterpolationPostionTimeData.empty;

        void CommitChanges(float deltaTime)
        {
            if(_internalTime < 0.001f)
            {
                _internalTime += deltaTime;
            }

            _internalTime += deltaTime;

            //Debug.Log("CommitChanges: add=" + deltaTime + " internal=" + _internalTime);

            if(_oldIs1)
            {
                _interpolationPostionTimeData1.time = _internalTime;
                _interpolationPostionTimeData1.pos = (Vector3)_localPosition;
            }
            else
            {
                _interpolationPostionTimeData2.time = _internalTime;
                _interpolationPostionTimeData2.pos = (Vector3)_localPosition;
            }

            _oldIs1 = !_oldIs1;

            if (_eularReady)
            {
                transform.localEulerAngles = (Vector3)_localEularAngles;
            }
            else
            {
                transform.localRotation = (Quaternion)_localRotation;
            }

            transform.localScale = (Vector3)_localScale;
        }

        public void UpdateUnityTransform()
        {
            if(!Interpolate)
            {
                return;
            }

            //set the current new position to _localPosition
            //in the next commit change call, the new position will become the old position
            //and the old posiiton will be updated and become the new position
            
            if (_oldIs1)
            {
                _interpolationPostionTimeData2.pos = (Vector3)_localPosition;
            }
            else
            {
                _interpolationPostionTimeData1.pos = (Vector3)_localPosition;
            }
        }

        Vector3 CalculateInterpolatedPosition(float time)
        {
            InterpolationPostionTimeData positionTimeDataOld = _interpolationPostionTimeData1;
            InterpolationPostionTimeData positionTimeDataNew = _interpolationPostionTimeData2;

            if (!_oldIs1)
            {
                positionTimeDataOld = _interpolationPostionTimeData2;
                positionTimeDataNew = _interpolationPostionTimeData1;
            }

            //Debug.Log("CalculateInterpolatedPosition: time=" + time + " oldTime=" + positionTimeDataOld.time + " newTime=" + positionTimeDataNew.time);

            float oldTime = positionTimeDataOld.time;

            if(time <= oldTime)
            {
                //Debug.LogError("time is smaller than oldTime");
                return positionTimeDataOld.pos;
            }

            float newTime = positionTimeDataNew.time;

            if (time >= newTime)
            {
                //Debug.LogError("time is larger than newtime");
                return positionTimeDataNew.pos;
            }

            float a = time - oldTime;
            float b = newTime - oldTime;

            return Vector3.Lerp(positionTimeDataOld.pos, positionTimeDataNew.pos, a / b);
        }

        void InitializeInterpolation()
        {
            _updateTime = 0;
            _internalTime = 0;

            _interpolationPostionTimeData1.time = 0;
            _interpolationPostionTimeData2.time = 0;

            _interpolationPostionTimeData1.pos = (Vector3)_localPosition;
            _interpolationPostionTimeData2.pos = (Vector3)_localPosition;
        }

        // Unity events

        private void OnEnable()
        {
            if(Interpolate)
            {
                InitializeInterpolation();
                _sParallelTransforms.Add(this);
            }
        }

        private void OnDisable()
        {
            if (Interpolate)
            {
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
                    ImportFromUnity();
                    UnityEditor.EditorUtility.SetDirty(this);
                    transform.hasChanged = false;
                }
#endif
            }
            else
            {
                if(Interpolate)
                {
                    _updateTime += Time.deltaTime;
                    //Debug.Log("Update: add=" + Time.deltaTime + " _updateTime=" + _updateTime);
                    transform.localPosition = CalculateInterpolatedPosition(_updateTime);
                }
            }
        }
    }
}
