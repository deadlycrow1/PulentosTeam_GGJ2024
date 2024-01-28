using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;

public class EnemyBehaviour : MonoBehaviour
{
    public enum EnemyState { Idle, Walk, Attacking, Laughing };
    public bool isBoss;
    public Animator anim;
    public bool isAggresive = true;
    public EnemyState enemyState;
    public Vector2 decisionTimeRange;
    public Transform currentMoveTarget;
    public float moveSpeed, rotationSpeed, playerDetectionDistance, playerAttackRange, attackCooldownTime;
    public bool playerDetected, playerInRange;
    public float depression = 100f;
    public SkinnedMeshRenderer enemyMesh;
    public EnemyBar enemyBar;
    public VisualEffect vfxHahas;
    public GameObject latigoPrefab;

    float maxDepression;
    float NextDecision, NextDistanceCheck, playerDistCheck, curPlayerDistance, attackCD;
    Transform t;
    Rigidbody rb;
    public float LaughHoldTime = 2f;
    float laughingTime;
    private void Awake()
    {
        maxDepression = depression;
        t = this.transform;
        rb = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        if (enemyBar)
        {
            enemyBar.SetupBar(maxDepression, depression, false);
        }
    }
    private void ProcessBrain()
    {
        if (enemyState == EnemyState.Attacking) return;
        if (Random.value < 0.36f)
        {
            enemyState = EnemyState.Idle;
        }
        else
        {
            enemyState = EnemyState.Walk;
            currentMoveTarget = GameManager.GetRandomPatrolPoint();
        }
    }
    private void Update()
    {
        if (Time.time > NextDecision)
        {
            NextDecision = Time.time + Random.Range(decisionTimeRange.x, decisionTimeRange.y);
            ProcessBrain();
        }
        PlayerCheck();
        AttackPlayerHandler();
        if (laughingTime > 0)
        {
            laughingTime -= Time.deltaTime;
            enemyState = EnemyState.Laughing;
        }
        else
        {
            if (enemyState == EnemyState.Laughing)
            {
                enemyState = EnemyState.Idle;
                vfxHahas.Stop();
            }
        }
        AnimationHandler();
    }
    IEnumerator FixRotationSeq()
    {
        float lerp = 0f;
        Vector3 dirLook = currentMoveTarget.position - t.position;
        dirLook.y = 0;
        Quaternion initRot = t.rotation;
        Quaternion lookRot = Quaternion.LookRotation(dirLook);

        while (lerp < 1f)
        {
            t.rotation = Quaternion.Slerp(initRot, lookRot, lerp);
            lerp += Time.deltaTime / 0.25f;
            yield return new WaitForEndOfFrame();
        }

        t.rotation = lookRot;
    }
    private void AttackPlayerHandler()
    {
        if (!isAggresive) return;
        if (playerInRange)
        {
            if (Time.time > attackCD)
            {
                attackCD = Time.time + attackCooldownTime;
                anim.SetTrigger("Attack");
                enemyState = EnemyState.Attacking;

                StartCoroutine(FixRotationSeq());
            }
        }
        else
        {
            if (playerDetected)
            {
                if (Time.time > attackCD && enemyState == EnemyState.Attacking)
                {
                    enemyState = EnemyState.Walk;
                }
            }
        }
    }
    private void PlayerCheck()
    {
        if (PlayerController.instance == null) return;
        if (!isAggresive) return;
        if (Time.time > playerDistCheck)
        {
            playerDistCheck = Time.time + Random.Range(0.1f, 0.3f);
            curPlayerDistance = Vector3.Distance(t.position, PlayerController.instance.transform.position);
        }
        if (curPlayerDistance < playerDetectionDistance)
        {
            playerDetected = true;
            currentMoveTarget = PlayerController.instance.transform;
            if (enemyBar)
            {
                enemyBar.RefreshValue(depression, true);
            }
        }
        else
        {
            playerDetected = false;
            if (enemyBar && !isBoss)
            {
                enemyBar.RefreshValue(depression, false);
            }
        }
        if (curPlayerDistance < playerAttackRange)
        {
            playerInRange = true;
        }
        else
        {
            playerInRange = false;
        }
    }
    private void AnimationHandler()
    {
        float tSpeed = 0f;
        if (enemyState == EnemyState.Walk)
        {
            tSpeed = 1f;
        }
        anim.SetBool("Laugh", enemyState == EnemyState.Laughing);
        anim.SetFloat("Speed", tSpeed, 0.15f, Time.deltaTime);
        anim.SetFloat("Stance", playerDetected && isAggresive ? 1f : 0f, 0.15f, Time.deltaTime);
    }
    private void FixedUpdate()
    {
        if (enemyState != EnemyState.Walk) return;
        if (Time.time > NextDistanceCheck)
        {
            NextDistanceCheck = Time.time + Random.Range(0.2f, 0.33f);
            float curDist = Vector3.Distance(t.position, currentMoveTarget.position);
            if (curDist <= 1f)
            {
                //llega al punto patrol, hacer algo.
                ProcessBrain();
            }
        }
        Vector3 dirLook = currentMoveTarget.position - t.position;
        dirLook.y = 0;
        t.rotation = Quaternion.Slerp(t.rotation, Quaternion.LookRotation(dirLook), Time.deltaTime * rotationSpeed);

        rb.MovePosition(t.position + (t.forward.normalized * moveSpeed * Time.deltaTime));
    }
    public void GetDamage(float dmgPoints)
    {
        if (!isAggresive) return;
        if (depression > 0)
        {
            depression -= dmgPoints;
            if (enemyBar != null)
            {
                enemyBar.RefreshValue(depression, true);
            }
            vfxHahas.Play();
            laughingTime = LaughHoldTime;
            anim.ResetTrigger("Attack");
        }
        else
        {
            depression = 0;
            isAggresive = false;
            laughingTime = 0;
            playerDetected = false;
            playerInRange = false;
            vfxHahas.Stop();
            if (enemyState == EnemyState.Laughing)
            {
                enemyState = EnemyState.Idle;
            }
            StartCoroutine(WipeDepressionSeq());
            currentMoveTarget = GameManager.GetRandomPatrolPoint();
            ProcessBrain();
        }
    }
    IEnumerator WipeDepressionSeq()
    {
        if (enemyBar != null)
        {
            enemyBar.RefreshValue(depression, false);
        }
        float lerp = 1f;
        while (lerp > 0f)
        {
            if (enemyMesh)
            {
                enemyMesh.material.SetFloat("_CorruptionLevel", lerp);
            }
            lerp -= Time.deltaTime / 1f;
            yield return new WaitForEndOfFrame();
        }
        if (enemyMesh)
        {
            enemyMesh.material.SetFloat("_CorruptionLevel", 0f);
        }
    }
}
