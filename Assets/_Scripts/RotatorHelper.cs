using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RotatorHelper : MonoBehaviour
{
    Transform t;
    public Vector3 axis;
    public float speed;
    private void Awake() {
        t = this.transform;
    }

    private void Update() {
        t.Rotate(axis * Time.deltaTime * speed);
    }
}
