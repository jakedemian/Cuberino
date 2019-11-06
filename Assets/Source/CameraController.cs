using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Vector3 currentFocusPoint = Vector3.zero;
    float currentFOV = 60f;
    Vector3 cameraOffset;
    private bool cameraIsRotating = true;

    public float rotationSpeed = 5f;
    public float smoothFactor = 0.5f;

    void Start(){
        transform.LookAt(currentFocusPoint);
        GetComponent<Camera>().fieldOfView = currentFOV;
        cameraOffset = transform.position - currentFocusPoint;
    }

    private void LateUpdate() {
        if (cameraIsRotating) {
            Quaternion camTurnAngle =
                Quaternion.AngleAxis(Input.GetAxis("Mouse X") * rotationSpeed, Vector3.up);

            cameraOffset = camTurnAngle * cameraOffset;

            Vector3 newPos = currentFocusPoint + cameraOffset;
            transform.position = Vector3.Slerp(transform.position, newPos, smoothFactor);
            transform.LookAt(currentFocusPoint);
        }
    }

    public void IsCameraActive(bool isCamActive) {
        cameraIsRotating = isCamActive;
    }
}
