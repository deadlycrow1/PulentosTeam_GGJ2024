using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRig : MonoBehaviour
{
    public Transform playerTarget;
    public float lerpSpeed = 5f;
    Transform t;
    private void Awake() {
        t = this.transform;
    }

    private void LateUpdate() {
        if (playerTarget == null) return;
        t.position = Vector3.Lerp(t.position, playerTarget.position, Time.deltaTime * lerpSpeed);
    }
}
