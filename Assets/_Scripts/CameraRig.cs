using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRig : MonoBehaviour
{
    public static CameraRig instance;
    public float fov;
    public Transform playerTarget;
    public float lerpSpeed = 5f;
    Transform t;
    private void Awake() {
        instance = this;
        t = this.transform;
    }
    private void LateUpdate() {
        if (playerTarget == null) return;
        t.position = Vector3.Lerp(t.position, playerTarget.position + Vector3.up * fov - Vector3.back * fov, Time.deltaTime * lerpSpeed);
    }
    public void SnapCamera() {
        if (playerTarget == null) return;
        t.position = playerTarget.position + Vector3.up * fov - Vector3.back * fov;

    }
}
