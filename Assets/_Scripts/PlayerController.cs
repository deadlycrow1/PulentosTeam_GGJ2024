using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public static PlayerController instance;
    public float health = 100f;
    public bool isAlive = true;
    public bool canDrive = false;
    public bool isGrounded;
    public bool isDashing;
    private Rigidbody playerRb;
    public float speed;
    //public float jumpForce;
    public float dashSpeed;
    public float dashDuration;
    private float dashCdTimer, dashTime;
    Transform cachedCam;
    Transform t;
    Vector2 moveInput;

    public float basicAttackCooldown = 0.5f;
    public float attackSphereRadius = 1f;
    public float attackSphereForwardOffset = 0.5f;
    public float basicAttackDamage = 20f;

    Vector3 dashDirection;
    CapsuleCollider capsuleCollider;
    float NextBasicAttack;

    void Awake() {
        instance = this;
        t = this.transform;
        playerRb = GetComponent<Rigidbody>();
        cachedCam = Camera.main.transform;
        capsuleCollider = GetComponent<CapsuleCollider>();
    }
    void Update() {
        if (!isAlive) return;
        if (!canDrive) return;
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
        if (!isDashing && Input.GetKeyDown(KeyCode.Space) && Time.time > dashCdTimer) {
            dashCdTimer = Time.time + 2f;
            dashTime = Time.time + dashDuration;
            isDashing = true;
            dashDirection = (cachedCam.right *
            moveInput.x) + (cachedCam.forward * moveInput.y);
            dashDirection.y = 0;
        }
        if (isDashing) {
            if (Time.time > dashTime) {
                isDashing = false;
            }
            t.rotation = Quaternion.LookRotation(dashDirection);
            bool dashObstructed = false;
            int lm = 1 << 10;
            if (Physics.Raycast(t.position + (Vector3.up * 0.2f), dashDirection.normalized, capsuleCollider.radius + 0.15f, lm)) {
                dashObstructed = true;
            }
            if (!dashObstructed) {
                t.position += (dashDirection.normalized * dashSpeed * Time.deltaTime);
            }
            return;
        }
        t.LookAt(cursorPosition);
        /*
        if (secondsSinceLastShot >= secondsBetweenShots && Input.GetButton("Fire1")) {
            Instantiate(bulletPrefab, t.position + t.forward + t.up * bulletHeight, t.rotation);
            secondsSinceLastShot = 0;
        }
        */
        AttackHandler();
    }
    private void FixedUpdate() {
        if (!isAlive) return;
        if (isDashing) return;
        if (!canDrive) return;
        Vector3 inputVector = (cachedCam.right *
            moveInput.x) + (cachedCam.forward * moveInput.y);
        inputVector.y = 0;
        if (moveInput != Vector2.zero) {
            playerRb.MovePosition(t.position + inputVector.normalized * speed * Time.deltaTime); // Actually move there
        }
    }
    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.GetComponent<Terrain>() != null) {
            isGrounded = true;
        }
    }
    private void OnCollisionExit(Collision other) {
        if (other.gameObject.GetComponent<Terrain>() != null) {
            isGrounded = false;
        }
    }
    private void AttackHandler() {
        if(Input.GetMouseButtonDown(0) && Time.time > NextBasicAttack) {
            NextBasicAttack = Time.time + basicAttackCooldown;
            print("Atacando!");
            Vector3 attackSphereCenter = t.position + (Vector3.up * 0.5f) + (t.forward * attackSphereForwardOffset);
            //int lm = 1 << 11;
            Collider[] hitCollider = Physics.OverlapSphere(attackSphereCenter, attackSphereRadius);

            if(hitCollider != null && hitCollider.Length > 0) {
                for (int i = 0; i < hitCollider.Length; i++) {
                    if (hitCollider[i].gameObject.TryGetComponent(out EnemyBehaviour enemy)) {
                        enemy.GetDamage(basicAttackDamage);
                    }
                }
            }
        }
    }
    public void GetDamage(float dmgPoints) {
        if(health > 0) {
            health -= dmgPoints;
        }
        else {
            //se muere el pj
            health = 0;
            isAlive = false;
        }
    }
}

