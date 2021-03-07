using UnityEngine;

namespace Parallel
{
    [RequireComponent(typeof(Camera))]
    [RequireComponent(typeof(ParallelTransform))]
    [ExecuteInEditMode]
    public class ParallelCamera : MonoBehaviour
    {
        Camera _camera;
        ParallelTransform _parallelTransform;
        [SerializeField]
        Fix64 _aspect = Fix64.one;

        [SerializeField]
        Fix64 _fieldOfView = Fix64.FromDivision(160, 1);

        [SerializeField]
        Fix64 _nearClipPlane = Fix64.FromDivision(3, 100);

        [SerializeField]
        Fix64 _farClipPlane = Fix64.FromDivision(1000, 1);

        [SerializeField]
        Fix64 _unityAspect = Fix64.FromDivision(16, 9);

        Fix64Matrix4X4 _projectionMatrix;

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
            Gizmos.DrawFrustum(Vector3.zero, (float)_fieldOfView, (float)_farClipPlane, (float)_nearClipPlane, (float)_aspect);
            Gizmos.matrix = Matrix4x4.identity;
        }

        private void OnDrawGizmos()
        {

        }

        // Start is called before the first frame update
        void Start()
        {
            _camera = GetComponent<Camera>();
            _parallelTransform = GetComponent<ParallelTransform>();
            //_camera.fieldOfView = (float)_fieldOfView;
            _camera.nearClipPlane = (float)_nearClipPlane;
            _camera.farClipPlane = (float)_farClipPlane;

            _projectionMatrix = CalculateFixedProjectionMatrix();
        }

        public Fix64 aspect
        {
            get
            {
                return _aspect;
            }
            set
            {
                _aspect = value;
                _projectionMatrix = CalculateFixedProjectionMatrix();
            }
        }

        public Fix64 fieldOfView
        {
            get
            {
                return _fieldOfView;
            }
            set
            {
                _fieldOfView = value;
                //_camera.fieldOfView = (float)_fieldOfView;
                _projectionMatrix = CalculateFixedProjectionMatrix();
            }
        }

        public Fix64 nearClipPlane
        {
            get
            {
                return _nearClipPlane;
            }
            set
            {
                _nearClipPlane = value;
                _camera.nearClipPlane = (float)_nearClipPlane;
                _projectionMatrix = CalculateFixedProjectionMatrix();
            }
        }

        public Fix64 farClipPlane
        {
            get
            {
                return _farClipPlane;
            }
            set
            {
                _farClipPlane = value;
                _camera.farClipPlane = (float)_farClipPlane;
                _projectionMatrix = CalculateFixedProjectionMatrix();
            }
        }

        public Fix64Matrix4X4 projectionMatrix
        {
            get
            {
                return _projectionMatrix;
            }
        }

        // converts a view port point of the Unity Camera to a view port point of the parallel camera
        public Fix64Vec3 GetParallelViewPortPointFromUnityViewPortPoint(Vector3 unityViewPortPoint)
        {
            float unityAspect = _camera.aspect;
            float unityFOV = _camera.fieldOfView;
            float fov = (float)_fieldOfView;

            float unityX = unityViewPortPoint.x;
            float unityY = unityViewPortPoint.y;
            float unityZ = unityViewPortPoint.z;

            //distances from camera to the near clip plane are the same 
            float halfFOVRad = fov / 2 * Mathf.Deg2Rad;
            float halfUnityFOVYRad = unityFOV / 2 * Mathf.Deg2Rad;
            float halfUnityFOVXRad = Mathf.Atan(unityAspect * Mathf.Tan(halfUnityFOVYRad));

            float ratioY = Mathf.Tan(halfFOVRad) / Mathf.Tan(halfUnityFOVYRad);
            float ratioX = Mathf.Tan(halfFOVRad) / Mathf.Tan(halfUnityFOVXRad);

            // parallel length = A
            // Unity length = a
            // view port unity = Va
            // view port parallel = VA
            // VA = ( (A - a) / 2 + Va * a ) / A 
            // if A : a = R
            // VA = (( R * a - a) / 2 + Va * a ) / R * a
            // VA = (( R - ) / 2 + Va ) / R 
            float y = ((ratioY - 1) / 2 + unityY) / ratioY;
            float x = ((ratioX - 1) / 2 + unityX) / ratioX;

            return new Fix64Vec3((Fix64)x, (Fix64)y, (Fix64)unityZ);
        }

        Fix64Matrix4X4 CalculateFixedProjectionMatrix()
        {
            Fix64 rad = fieldOfView / Fix64.two * Fix64Math.DegreeToRad;
            Fix64 e = Fix64.one / Fix64Math.Tan(rad);
            Fix64 a = Fix64.one / _aspect;
            Fix64 n = nearClipPlane;
            Fix64 f = farClipPlane;

            Fix64Vec4 column0 = new Fix64Vec4(e * a, Fix64.zero, Fix64.zero, Fix64.zero);
            Fix64Vec4 column1 = new Fix64Vec4(Fix64.zero, e, Fix64.zero, Fix64.zero);
            Fix64Vec4 column2 = new Fix64Vec4(Fix64.zero, Fix64.zero, -(f + n) / (f - n), Fix64.negOne);
            Fix64Vec4 column3 = new Fix64Vec4(Fix64.zero, Fix64.zero, -(Fix64.two * f * n) / (f - n), Fix64.zero);

            Fix64Matrix4X4 result = Fix64Matrix4X4.zero;
            result.SetColumn(0, column0);
            result.SetColumn(1, column1);
            result.SetColumn(2, column2);
            result.SetColumn(3, column3);

            return result;

            return Fix64Matrix4X4.Perspective(fieldOfView, aspect, nearClipPlane, farClipPlane);
        }

        public Fix64Vec3 WorldToViewportPoint(Fix64Vec3 point3D)
        {
            Fix64Matrix4X4 P = _projectionMatrix;
            Fix64Matrix4X4 V = _parallelTransform.worldToLocalMatrix;
            Fix64Matrix4X4 VP = P * V;

            Fix64Vec4 point4 = new Fix64Vec4(point3D.x, point3D.y, point3D.z, Fix64.one);  // turn into (x,y,z,1)

            Fix64Vec4 result4 = VP * point4;  // multiply 4 components

            Fix64Vec3 result = new Fix64Vec3(result4.x, result4.y, result4.z);  // store 3 components of the resulting 4 components

            // normalize by "-w"
            result /= -result4.w;

            // clip space => view space
            result.x = result.x / Fix64.two + Fix64.half;
            result.y = result.y / Fix64.two + Fix64.half;

            // "The z position is in world units from the camera."
            result.z = -result4.w;

            return result;
        }

        public Fix64Vec3 ViewportToWorldPoint(Fix64Vec3 point3D)
        {
            if(point3D.z == Fix64.zero)
            {
                return _parallelTransform.position; 
            }

            //Vector3 upoint3D = (Vector3)point3D;

            Fix64Matrix4X4 P = _projectionMatrix;
            //Matrix4x4 up = _camera.projectionMatrix;
            //Fix64Matrix4X4 iv = _parallelTransform.localToWorldMatrix;
            Fix64Matrix4X4 V = _parallelTransform.worldToLocalMatrix;
            //Matrix4x4 uiv = transform.localToWorldMatrix;
            //Matrix4x4 uv = transform.worldToLocalMatrix;
            Fix64Matrix4X4 VP = P * V;
            //Matrix4x4 uvp = up * uv;

            // get projection W by Z
            Fix64Vec4 projW = P * new Fix64Vec4(Fix64.zero, Fix64.zero, point3D.z, Fix64.one);
            //Vector4 uprojW = up * new Vector4(0, 0, upoint3D.z, 1);


            // restore point4
            Fix64Vec4 point4 = new Fix64Vec4(Fix64.one - (point3D.x * Fix64.two), Fix64.one - (point3D.y * Fix64.two), projW.z / projW.w, Fix64.one);
            //Vector4 upoint4 = new Vector4(1.0f - (upoint3D.x * 2.0f), 1.0f - (upoint3D.y * 2.0f), uprojW.z / uprojW.w, 1);

            Fix64Vec4 result4 = Fix64Matrix4X4.Inverse(VP) * point4;  // multiply 4 components
            //Vector4 uresult4 = uvp.inverse * upoint4;  // multiply 4 components

            Fix64Vec3 resultInv = new Fix64Vec3(result4.x / result4.w, result4.y / result4.w, result4.z / result4.w);  // store 3 components of the resulting 4 components
            //Vector3 uresultInv = uresult4 / uresult4.w;  // store 3 components of the resulting 4 components

            return resultInv;
        }

        public ParallelRay ViewportPointToRay(Fix64Vec3 viewportPoint)
        {
            if(viewportPoint.z == Fix64.zero)
            {
                viewportPoint = new Fix64Vec3(viewportPoint.x, viewportPoint.y, nearClipPlane);
            }
            //Vector3 uWorldPoint = _camera.ViewportToWorldPoint((Vector3)viewportPoint);
            Fix64Vec3 worldPoint = ViewportToWorldPoint(viewportPoint);
            ParallelRay ray = new ParallelRay(worldPoint, worldPoint - _parallelTransform.position);
            //Ray uRay = _camera.ViewportPointToRay((Vector3)viewportPoint);
            //Debug.Log(uRay);
            return ray;
        }

        void Update()
        {
            //only import from unity in editing mode
            if (!Application.isPlaying)
            {
                ImportFromUnity();
            }
        }

        public void ImportFromUnity()
        {
            if(_camera == null)
            {
                _camera = GetComponent<Camera>();
            }

            //_fieldOfView = (Fix64)_camera.fieldOfView;
            //_nearClipPlane = (Fix64)_camera.nearClipPlane;
            _farClipPlane = (Fix64)_camera.farClipPlane;
            _unityAspect = (Fix64)_camera.aspect;
        }
    }
}

