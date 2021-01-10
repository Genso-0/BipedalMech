
using UnityEngine;
namespace Mech.Utilities
{
    public class CameraRotator : MonoBehaviour
    {
        public Vector3 autoRotateSpeed;
        public Transform target;
        public float targetTrackingSpeed = 1;
        [HideInInspector] public Camera cam;
        [HideInInspector] public bool adjustingToNewZ;
        [HideInInspector] public float newZ;
        public float rotationSpeed_Yaw = 2.0f;
        public float rotationSpeed_Pitch = 2.0f;
        [HideInInspector] private float yaw = 0.0f;
        [HideInInspector] private float pitch = 0.0f;
        void Awake()
        {
            cam = GetComponentInChildren<Camera>();
        }
        void Update()
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0f) // forward
            {
                adjustingToNewZ = true;
                newZ = cam.transform.localPosition.z + 3;
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f) // backwards 
            {
                adjustingToNewZ = true;
                newZ = cam.transform.localPosition.z - 3;
            }
            if (Input.GetMouseButton(1))
            {
                RotateCamera();
            }
            transform.Rotate(autoRotateSpeed);
            if (adjustingToNewZ)
                AdjustCamera();
            transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * targetTrackingSpeed);
        }

        private void AdjustCamera()
        {
            var oldPos = cam.transform.localPosition;
            var newPos = new Vector3(oldPos.x, oldPos.y, newZ);
            cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, newPos, Time.deltaTime);
            if (Mathf.Abs(cam.transform.localPosition.z - newZ) < 1)
                adjustingToNewZ = false;
        }
        public void RotateCamera()
        {
            yaw += rotationSpeed_Yaw * Input.GetAxis("Mouse X");
            pitch -= rotationSpeed_Pitch * Input.GetAxis("Mouse Y");
            transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
        }
    }
}