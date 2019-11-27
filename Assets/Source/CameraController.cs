using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Vector3 currentFocusPoint = Vector3.zero;
    private float currentFOV = 60f;
    private Vector3 cameraOffset;
    private bool cameraIsActive = false;
    private bool camIsRotating = false;
    private Coroutine rotateCoroutine;

    public float smoothFactor = 0.5f;

    void Start(){
        transform.LookAt(currentFocusPoint);
        GetComponent<Camera>().fieldOfView = currentFOV;
        cameraOffset = transform.position - currentFocusPoint;
    }

    private void LateUpdate() {
        if (cameraIsActive && !camIsRotating) {
            currentFocusPoint = new Vector3(currentFocusPoint.x, Mathf.Max(currentFocusPoint.y + Input.GetAxis("Mouse Y"),0f), currentFocusPoint.z);
            Vector3 newPos = currentFocusPoint + cameraOffset;
            transform.position = Vector3.Slerp(transform.position, newPos, smoothFactor);
            transform.LookAt(currentFocusPoint);
        }
    }

    private IEnumerator RotateCam(float rotateDuration) {
        camIsRotating = true;
        float timeLeft = rotateDuration;


        while (timeLeft > 0) {
            yield return null;

            float elapsedRotationTime = Mathf.Min(timeLeft, Time.deltaTime);
            float angleThisFrame = 180f * elapsedRotationTime / rotateDuration;

            Quaternion camTurnAngle = Quaternion.AngleAxis(angleThisFrame, Vector3.up);

            this.cameraOffset = camTurnAngle * this.cameraOffset;
            transform.position = this.currentFocusPoint + this.cameraOffset;
            transform.LookAt(this.currentFocusPoint);

            timeLeft -= Time.deltaTime;
        }

        camIsRotating = false;
    }

    public void RotateCamera() {
        if (!camIsRotating) {
            this.rotateCoroutine = StartCoroutine(RotateCam(0.2f));
        }
    }

    public void SetCameraActive(bool isCamActive) {
        cameraIsActive = isCamActive;
    }
}
