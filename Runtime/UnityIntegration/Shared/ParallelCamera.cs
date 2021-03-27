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
        FFloat _aspect = FFloat.one;

        [SerializeField]
        FFloat _fieldOfView = FFloat.FromDivision(160, 1);

        [SerializeField]
        FFloat _nearClipPlane = FFloat.FromDivision(3, 100);

        [SerializeField]
        FFloat _farClipPlane = FFloat.FromDivision(1000, 1);

        [SerializeField]
        FFloat _unityAspect = FFloat.FromDivision(16, 9);

        FMatrix4x4 _projectionMatrix;

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

        public FFloat aspect
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

        public FFloat fieldOfView
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

        public FFloat nearClipPlane
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

        public FFloat farClipPlane
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

        public FMatrix4x4 projectionMatrix
        {
            get
            {
                return _projectionMatrix;
            }
        }

        // converts a view port point of the Unity Camera to a view port point of the parallel camera
        public FVector3 GetParallelViewPortPointFromUnityViewPortPoint(Vector3 unityViewPortPoint)
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

            return new FVector3((FFloat)x, (FFloat)y, (FFloat)unityZ);
        }

        FMatrix4x4 CalculateFixedProjectionMatrix()
        {
            FFloat rad = fieldOfView / FFloat.two * FMath.Deg2Rad;
            FFloat e = FFloat.one / FMath.Tan(rad);
            FFloat a = FFloat.one / _aspect;
            FFloat n = nearClipPlane;
            FFloat f = farClipPlane;

            FVector4 column0 = new FVector4(e * a, FFloat.zero, FFloat.zero, FFloat.zero);
            FVector4 column1 = new FVector4(FFloat.zero, e, FFloat.zero, FFloat.zero);
            FVector4 column2 = new FVector4(FFloat.zero, FFloat.zero, -(f + n) / (f - n), FFloat.negOne);
            FVector4 column3 = new FVector4(FFloat.zero, FFloat.zero, -(FFloat.two * f * n) / (f - n), FFloat.zero);

            FMatrix4x4 result = FMatrix4x4.zero;
            result.SetColumn(0, column0);
            result.SetColumn(1, column1);
            result.SetColumn(2, column2);
            result.SetColumn(3, column3);

            return result;

            return FMatrix4x4.Perspective(fieldOfView, aspect, nearClipPlane, farClipPlane);
        }

        public FVector3 WorldToViewportPoint(FVector3 point3D)
        {
            FMatrix4x4 P = _projectionMatrix;
            FMatrix4x4 V = _parallelTransform.worldToLocalMatrix;
            FMatrix4x4 VP = P * V;

            FVector4 point4 = new FVector4(point3D.x, point3D.y, point3D.z, FFloat.one);  // turn into (x,y,z,1)

            FVector4 result4 = VP * point4;  // multiply 4 components

            FVector3 result = new FVector3(result4.x, result4.y, result4.z);  // store 3 components of the resulting 4 components

            // normalize by "-w"
            result /= -result4.w;

            // clip space => view space
            result.x = result.x / FFloat.two + FFloat.half;
            result.y = result.y / FFloat.two + FFloat.half;

            // "The z position is in world units from the camera."
            result.z = -result4.w;

            return result;
        }

        public FVector3 ViewportToWorldPoint(FVector3 point3D)
        {
            if(point3D.z == FFloat.zero)
            {
                return _parallelTransform.position; 
            }

            //Vector3 upoint3D = (Vector3)point3D;

            FMatrix4x4 P = _projectionMatrix;
            //Matrix4x4 up = _camera.projectionMatrix;
            //FMatrix4x4 iv = _parallelTransform.localToWorldMatrix;
            FMatrix4x4 V = _parallelTransform.worldToLocalMatrix;
            //Matrix4x4 uiv = transform.localToWorldMatrix;
            //Matrix4x4 uv = transform.worldToLocalMatrix;
            FMatrix4x4 VP = P * V;
            //Matrix4x4 uvp = up * uv;

            // get projection W by Z
            FVector4 projW = P * new FVector4(FFloat.zero, FFloat.zero, point3D.z, FFloat.one);
            //Vector4 uprojW = up * new Vector4(0, 0, upoint3D.z, 1);


            // restore point4
            FVector4 point4 = new FVector4(FFloat.one - (point3D.x * FFloat.two), FFloat.one - (point3D.y * FFloat.two), projW.z / projW.w, FFloat.one);
            //Vector4 upoint4 = new Vector4(1.0f - (upoint3D.x * 2.0f), 1.0f - (upoint3D.y * 2.0f), uprojW.z / uprojW.w, 1);

            FVector4 result4 = FMatrix4x4.Inverse(VP) * point4;  // multiply 4 components
            //Vector4 uresult4 = uvp.inverse * upoint4;  // multiply 4 components

            FVector3 resultInv = new FVector3(result4.x / result4.w, result4.y / result4.w, result4.z / result4.w);  // store 3 components of the resulting 4 components
            //Vector3 uresultInv = uresult4 / uresult4.w;  // store 3 components of the resulting 4 components

            return resultInv;
        }

        public ParallelRay ViewportPointToRay(FVector3 viewportPoint)
        {
            if(viewportPoint.z == FFloat.zero)
            {
                viewportPoint = new FVector3(viewportPoint.x, viewportPoint.y, nearClipPlane);
            }
            //Vector3 uWorldPoint = _camera.ViewportToWorldPoint((Vector3)viewportPoint);
            FVector3 worldPoint = ViewportToWorldPoint(viewportPoint);
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

            //_fieldOfView = (FFloat)_camera.fieldOfView;
            //_nearClipPlane = (FFloat)_camera.nearClipPlane;
            _farClipPlane = (FFloat)_camera.farClipPlane;
            _unityAspect = (FFloat)_camera.aspect;
        }
    }
}

