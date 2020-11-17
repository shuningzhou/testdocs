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
        Fix64 _aspect = Fix64.FromDivision(16, 9);

        [SerializeField]
        Fix64 _fieldOfView = Fix64.FromDivision(60, 1);

        [SerializeField]
        Fix64 _nearClipPlane = Fix64.FromDivision(3, 100);

        [SerializeField]
        Fix64 _farClipPlane = Fix64.FromDivision(1000, 1);

        Fix64Matrix4X4 _projectionMatrix;

        // Start is called before the first frame update
        void Start()
        {
            _camera = GetComponent<Camera>();
            _parallelTransform = GetComponent<ParallelTransform>();
            _camera.aspect = (float)_aspect;
            _camera.fieldOfView = (float)_fieldOfView;
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
                _camera.aspect = (float)_aspect;
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
                _camera.fieldOfView = (float)_fieldOfView;
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

        Fix64Matrix4X4 CalculateFixedProjectionMatrix()
        {
            //Fix64 rad = fieldOfView / Fix64.two * Fix64Math.DegreeToRad;
            //Fix64 e = Fix64.one / Fix64Math.Tan(rad);
            //Fix64 a = Fix64.one / aspectRatio;
            //Fix64 n = nearClipPlane;
            //Fix64 f = farClipPlane;

            //Fix64Vec4 column0 = new Fix64Vec4(e * a, Fix64.zero, Fix64.zero, Fix64.zero);
            //Fix64Vec4 column1 = new Fix64Vec4(Fix64.zero, e, Fix64.zero, Fix64.zero);
            //Fix64Vec4 column2 = new Fix64Vec4(Fix64.zero, Fix64.zero, -(f + n) / (f - n), Fix64.NegOne);
            //Fix64Vec4 column3 = new Fix64Vec4(Fix64.zero, Fix64.zero, -(Fix64.two * f * n) / (f - n), Fix64.zero);

            //return new Fix64Mat4x4(column0, column1, column2, column3);

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

            Fix64Matrix4X4 P = _projectionMatrix;
            Fix64Matrix4X4 V = _parallelTransform.worldToLocalMatrix;
            Fix64Matrix4X4 VP = P * V;
            // get projection W by Z
            Fix64Vec4 projW = P * new Fix64Vec4(Fix64.zero, Fix64.zero, point3D.z, Fix64.one);
            // restore point4
            Fix64Vec4 point4 = new Fix64Vec4(Fix64.one - (point3D.x * Fix64.two), Fix64.one - (point3D.y * Fix64.two), projW.z / projW.w, Fix64.one);
            Fix64Vec4 result4 = Fix64Matrix4X4.Inverse(VP) * point4;  // multiply 4 components
            Fix64Vec3 resultInv = new Fix64Vec3(result4.x / result4.w, result4.y / result4.w, result4.z / result4.w);  // store 3 components of the resulting 4 components

            return resultInv;
        }

        public ParallelRay ViewportToRay(Fix64Vec3 viewportPoint)
        {
            if(viewportPoint.z == Fix64.zero)
            {
                viewportPoint = new Fix64Vec3(viewportPoint.x, viewportPoint.y, nearClipPlane);
            }
            Fix64Vec3 worldPoint = ViewportToWorldPoint(viewportPoint);
            ParallelRay ray = new ParallelRay(worldPoint, worldPoint - _parallelTransform.position);
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

            _fieldOfView = (Fix64)_camera.fieldOfView;
            _nearClipPlane = (Fix64)_camera.nearClipPlane;
            _farClipPlane = (Fix64)_camera.farClipPlane;
        }
    }
}

