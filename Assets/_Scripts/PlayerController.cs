using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody playerRb;
    public float speed;
    //public float jumpForce;
    public float dashForce;
    public float dashDuration;
    public KeyCode dashKey = KeyCode.Space;
    public float dashCd;
    private float dashCdTimer;

    public float gravityScale;
    Transform cachedCam;
    Transform t;
    Vector2 moveInput;
    private bool isGrounded;


    void Awake()
    {
        t = this.transform;
        playerRb = GetComponent<Rigidbody>();
        cachedCam = Camera.main.transform;
    }
    void Update()
    {
        // Get cursor position
        Ray rayFromCameraToCursor = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane playerPlane = new Plane(Vector3.up, transform.position);
        playerPlane.Raycast(rayFromCameraToCursor, out float distanceFromCamera);
        Vector3 cursorPosition = rayFromCameraToCursor.GetPoint(distanceFromCamera);

        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        // VER ESTO PARA QUE MIRE MOUSE,
        // PERO PIERNAS CALCULEN VECTOR DE DIRECCION PA LA ANIMACION MAS ADELANTE
        /*
        if (moveInput != Vector2.zero) {
            t.rotation = Quaternion.LookRotation(inputVector);
        }
        */
        t.LookAt(cursorPosition);


    }
    private void FixedUpdate()
    {
        Vector3 inputVector = (cachedCam.right *
            moveInput.x) + (cachedCam.forward * moveInput.y);
        inputVector.y = 0;
        if (moveInput != Vector2.zero)
        {
            playerRb.MovePosition(t.position + inputVector.normalized * speed * Time.deltaTime); // Actually move there
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.GetComponent<Terrain>() != null)
        {
            isGrounded = true;
        }
    }
    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.GetComponent<Terrain>() != null)
        {
            isGrounded = false;
        }
    }
}

