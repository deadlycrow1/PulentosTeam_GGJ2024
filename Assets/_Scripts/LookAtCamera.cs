using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LookAtCamera : MonoBehaviour
{
    Transform t, cachedCam;
    private void Awake() {
        t = this.transform;
        cachedCam = Camera.main.transform;
    }
    private void LateUpdate() {
        t.LookAt(cachedCam);
    }
}
