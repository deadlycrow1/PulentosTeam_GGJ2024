using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRig : MonoBehaviour
{
    public static CameraRig instance;
    public float curZoom;
    public Transform vcamTransform;
    public Transform playerTarget;
    public float lerpSpeed = 5f;
    public Vector2 zoomRange;
    public float zoomScale = 1f;
    float curRot;
    Transform t;
    private void Awake() {
        instance = this;
        t = this.transform;
    }
    private void LateUpdate() {
        if (playerTarget == null) return;
        t.position = Vector3.Lerp(t.position, playerTarget.position, Time.deltaTime * lerpSpeed);
        if (Input.GetMouseButton(1)) {
            curRot += Input.GetAxis("Mouse X") * Time.deltaTime * 50f;
        }
        t.rotation = Quaternion.Slerp(t.rotation, Quaternion.Euler(Vector3.up * curRot), Time.deltaTime * 3f);
        if(Input.mouseScrollDelta.y != 0) {
            curZoom += Input.mouseScrollDelta.y * Time.deltaTime * zoomScale;
            curZoom = Mathf.Clamp(curZoom, zoomRange.x, zoomRange.y);
        }
        vcamTransform.localPosition = Vector3.Lerp(vcamTransform.localPosition, Vector3.forward * curZoom, Time.deltaTime * 3f);
    }
    public void SnapCamera() {
        if (playerTarget == null) return;
        t.position = playerTarget.position;

    }
}
