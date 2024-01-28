using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    public Slider hpBAR;
    public GameObject deadBtn;
    public Animator anim;
    public float health = 100f;
    public bool isAlive = true;
    public bool canBeDamaged = false;
    public bool canDrive = false;
    public bool isMelee = true;
    public PlayerWeaponHelper swordHelper, bookHelper;
    public bool isAttacking = false;
    public bool isGrounded;
    public bool isDashing;
    private Rigidbody playerRb;
    public float speed;
    //public float jumpForce;
    public float dashSpeed;
    public float dashDuration;
    public GameObject sporeSpherePrefab;
    private float dashCdTimer, dashTime;
    Transform cachedCam;
    Transform t;
    Vector2 moveInput;

    public float basicAttackCooldown = 0.5f;
    public float sphereAttackCooldown = 3f;
    public float attackSphereRadius = 1f;
    public float attackSphereForwardOffset = 0.5f;
    public float basicAttackDamage = 20f;
    public float sphereAttackDamage = 10f;
    public float rangeAttackRadius = 2f;
    public float rangeAttackDuration = 2f;

    public float rangeAttackCooldown = 0.25f;
    public float rangeAttackDamage = 2f;

    Vector3 dashDirection, cursorPosition;
    CapsuleCollider capsuleCollider;
    float NextBasicAttack;
    float NextSphereAttack;
    float FinishAttack;
    float rollDelay = 0.1f;
    float isAttackingHoldTime;
    public VisualEffect rangeTalkVFX;

    void Awake()
    {
        instance = this;
        t = this.transform;
        playerRb = GetComponent<Rigidbody>();
        cachedCam = Camera.main.transform;
        capsuleCollider = GetComponent<CapsuleCollider>();

        sporeSphereBehaviour sphereBheaviour = sporeSpherePrefab.GetComponent<sporeSphereBehaviour>();
        sphereBheaviour.sphereAttackRadius = attackSphereRadius;
        swordHelper.isSocketA = true;
        bookHelper.isSocketA = false;
    }
    void Update()
    {
        if (!isAlive) return;
        if (!canDrive) return;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            isMelee = !isMelee;
            CheckCurrentWeapon();
        }

        Ray rayFromCameraToCursor = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane playerPlane = new Plane(Vector3.up, transform.position);
        playerPlane.Raycast(rayFromCameraToCursor, out float distanceFromCamera);
        cursorPosition = rayFromCameraToCursor.GetPoint(distanceFromCamera);

        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        if (!isDashing && Input.GetKeyDown(KeyCode.LeftShift) && Time.time > dashCdTimer)
        {
            dashCdTimer = Time.time + 1.5f;
            dashTime = Time.time + dashDuration;
            isDashing = true;
            dashDirection = (cachedCam.right *
            moveInput.x) + (cachedCam.forward * moveInput.y);
            dashDirection.y = 0;
            anim.SetTrigger("Roll");
            rollDelay = 0.1f;
        }
        if (isDashing)
        {
            t.rotation = Quaternion.LookRotation(dashDirection);
            bool dashObstructed = false;
            int lm = 1 << 10;
            if (Physics.Raycast(t.position + (Vector3.up * 0.4f), dashDirection.normalized, capsuleCollider.radius + 0.15f, lm))
            {
                dashObstructed = true;
            }
            if (!dashObstructed)
            {
                if (rollDelay > 0) {
                    rollDelay -= Time.deltaTime;
                    return;
                }
                if(dashDirection == Vector3.zero) {
                    dashDirection = t.forward.normalized;
                    dashDirection.y = 0;
                }
                t.position += (dashDirection.normalized * dashSpeed * Time.deltaTime);
            }
            if (Time.time > dashTime) {
                isDashing = false;
            }
            return;
        }
        //t.LookAt(cursorPosition);
        AttackHandler();
        AnimationHandler();
        if (isAttacking) {
            if(Time.time > isAttackingHoldTime) {
                isAttacking = false;
            }
        }
    }
    private void CheckCurrentWeapon()
    {
        //aca decidir animaciones y objeto a mostrar dependiendo si es melee o rango.
        swordHelper.SwitchSocket();
        bookHelper.SwitchSocket();
        anim.SetBool("IsBook", !isMelee);
    }
    private void AnimationHandler() {
        float tMove = 0;
        if(moveInput != Vector2.zero) {
            tMove = 1f;
        }
        anim.SetFloat("Speed", tMove, 0.15f, Time.deltaTime * 2f);
    }
    private void FixedUpdate()
    {
        if (!isAlive) return;
        if (isDashing) return;
        if (!canDrive) return;
        Vector3 inputVector = (cachedCam.right *
            moveInput.x) + (cachedCam.forward * moveInput.y);
        inputVector.y = 0;
        if (moveInput != Vector2.zero)
        {
            t.rotation = Quaternion.Slerp(t.rotation, Quaternion.LookRotation(inputVector), Time.deltaTime * 9f) ;
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
    private void AttackHandler() {
        if (isMelee) {
            rangeTalkVFX.Stop();
            if (Input.GetMouseButtonDown(0) && Time.time > NextBasicAttack) {
                StartCoroutine(FixRotationSeq());
                NextBasicAttack = Time.time + basicAttackCooldown;
                isAttacking = true;
                isAttackingHoldTime = Time.time + 0.5f;
                anim.SetTrigger("Attack");
                //print("Atacando!");
                Vector3 attackSphereCenter = t.position + (Vector3.up * 0.5f) + (t.forward * attackSphereForwardOffset);
                //int lm = 1 << 11;
                Collider[] hitCollider = Physics.OverlapSphere(attackSphereCenter, attackSphereRadius);

                if (hitCollider != null && hitCollider.Length > 0) {
                    for (int i = 0; i < hitCollider.Length; i++) {
                        if (hitCollider[i].gameObject.TryGetComponent(out EnemyBehaviour enemy)) {
                            enemy.GetDamage(basicAttackDamage);
                        }
                    }
                }
            }
        }
        else {
            if (Input.GetMouseButton(0) && Time.time > NextBasicAttack) {
                StartCoroutine(FixRotationSeq());
                NextBasicAttack = Time.time + rangeAttackCooldown;
                isAttacking = true;
                isAttackingHoldTime = Time.time + 0.66f;
                FinishAttack = Time.time + rangeAttackDuration;
                rangeTalkVFX.Play();
                //print("Atacando!");
                Vector3 attackSphereCenter = t.position + (Vector3.up * 0.5f);
                //int lm = 1 << 11;
                Collider[] hitCollider = Physics.OverlapSphere(attackSphereCenter, rangeAttackRadius);

                if (hitCollider != null && hitCollider.Length > 0 && Time.time < FinishAttack) {
                    for (int i = 0; i < hitCollider.Length; i++) {
                        if (hitCollider[i].gameObject.TryGetComponent(out EnemyBehaviour enemy)) {
                            enemy.GetDamage(basicAttackDamage);
                        }
                    }
                }

            }
            else {
                rangeTalkVFX.Stop();
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > NextSphereAttack) {
            NextSphereAttack = Time.time + sphereAttackCooldown;
            print("Esfera!");
            anim.SetTrigger("Spores");
            Invoke(nameof(SporesDelayed), 1.33f);
        }
    }
    private void SporesDelayed() {
        Instantiate(sporeSpherePrefab, t.position, Quaternion.identity);
    }
    public void GetDamage(float dmgPoints)
    {
        if (!canBeDamaged) return;

        if (health > 0)
        {
            health -= dmgPoints;
            anim.SetTrigger("GetHit");
            hpBAR.value = health;
        }
        else
        {
            //se muere el pj
            health = 0;
            hpBAR.value = health;
            isAlive = false;
            anim.SetTrigger("Death");
            deadBtn.SetActive(true);
        }
    }
    IEnumerator FixRotationSeq() {
        float lerp = 0f;
        Vector3 dirLook = cursorPosition - t.position;
        dirLook.y = 0;
        Quaternion initRot = t.rotation;
        Quaternion lookRot = Quaternion.LookRotation(dirLook);

        while (lerp < 1f) {
            t.rotation = Quaternion.Slerp(initRot, lookRot, lerp);
            lerp += Time.deltaTime / 0.1f;
            yield return new WaitForEndOfFrame();
        }
        t.rotation = lookRot;
    }
}

