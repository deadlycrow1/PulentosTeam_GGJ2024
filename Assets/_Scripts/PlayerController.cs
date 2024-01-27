using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    private Rigidbody playerRb;
    public float speed;
    Transform cachedCam;
    Transform t;
    Vector2 moveInput;

    void Awake() {
        t = this.transform;
        playerRb = GetComponent<Rigidbody>();
        cachedCam = Camera.main.transform;
    }
    void Update() {
        // Get cursor position
        Ray rayFromCameraToCursor = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane playerPlane = new Plane(Vector3.up, transform.position);
        playerPlane.Raycast(rayFromCameraToCursor, out float distanceFromCamera);
        Vector3 cursorPosition = rayFromCameraToCursor.GetPoint(distanceFromCamera);

        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        Vector3 inputVector = (cachedCam.right.normalized *
            moveInput.x) + (cachedCam.forward.normalized * moveInput.y);
        inputVector.y = 0;
        // VER ESTO PARA QUE MIRE MOUSE,
        // PERO PIERNAS CALCULEN VECTOR DE DIRECCION PA LA ANIMACION MAS ADELANTE
        if (moveInput != Vector2.zero) {
            t.rotation = Quaternion.LookRotation(inputVector);
        }
        t.LookAt(cursorPosition);
        playerRb.MovePosition(t.position + inputVector * speed * Time.deltaTime); // Actually move there
    }
}

