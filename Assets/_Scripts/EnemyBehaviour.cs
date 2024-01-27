using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBehaviour : MonoBehaviour
{
    public enum EnemyState { Idle, Walk, Attacking};
    public Animator anim;
    public bool isAggresive=true;
    public EnemyState enemyState;
    public Vector2 decisionTimeRange;
    public Transform currentMoveTarget;
    public float moveSpeed, rotationSpeed, playerDetectionDistance, playerAttackRange, attackCooldownTime;
    public bool playerDetected, playerInRange;
    public float depression = 100f;
    public SkinnedMeshRenderer enemyMesh;

    float maxDepression;
    GameObject[] patrolPoints;
    float NextDecision, NextDistanceCheck, playerDistCheck, curPlayerDistance, attackCD;
    Transform t;
    Rigidbody rb;
    private void Awake() {
        maxDepression = depression;
        t = this.transform;
        rb = GetComponent<Rigidbody>();
    }
    private void Start() {
        patrolPoints = GameObject.FindGameObjectsWithTag("PatrolPoint");
    }
    private void ProcessBrain() {
        if (enemyState == EnemyState.Attacking) return;
        if(Random.value < 0.36f) {
            enemyState = EnemyState.Idle;
        }
        else {
            enemyState = EnemyState.Walk;
            if (patrolPoints.Length == 0) return;
            currentMoveTarget = GetRandomPatrolPoint();
        }
    }
    private Transform GetRandomPatrolPoint() {
        return patrolPoints[Random.Range(0, patrolPoints.Length - 1)].transform;
    }
    private void Update() {
        if (Time.time > NextDecision) {
            NextDecision = Time.time + Random.Range(decisionTimeRange.x, decisionTimeRange.y);
            ProcessBrain();
        }
        PlayerCheck();
        AttackPlayerHandler();
        AnimationHandler();
    }

    private void AttackPlayerHandler() {
        if (!isAggresive) return;
        if (playerInRange) {
            if (Time.time > attackCD) {
                attackCD = Time.time + attackCooldownTime;
                anim.SetTrigger("Attack");
                enemyState = EnemyState.Attacking;
            }
        }
        else {
            if (playerDetected) {
                if (Time.time > attackCD && enemyState == EnemyState.Attacking) {
                    enemyState = EnemyState.Walk;
                }
            }
        }
    }
    private void PlayerCheck() {
        if (PlayerController.instance == null || !isAggresive) return;
        if(Time.time > playerDistCheck) {
            playerDistCheck = Time.time + Random.Range(0.1f,0.3f);
            curPlayerDistance = Vector3.Distance(t.position, PlayerController.instance.transform.position);
        }
        if(curPlayerDistance < playerDetectionDistance) {
            playerDetected = true;
            currentMoveTarget = PlayerController.instance.transform;
        }
        else {
            playerDetected = false;
        }
        if(curPlayerDistance < playerAttackRange) {
            playerInRange = true;
        }
        else {
            playerInRange = false;
        }
    }
    private void AnimationHandler() {
        float tSpeed = 0;
        switch (enemyState) {
            case EnemyState.Idle:
            case EnemyState.Attacking:
                tSpeed = 0;
                break;
            case EnemyState.Walk:
                tSpeed = 1;
                break;
        }
        anim.SetFloat("Speed", tSpeed, 0.15f, Time.deltaTime);
        anim.SetFloat("Stance", playerDetected && isAggresive ? 1f:0f, 0.15f, Time.deltaTime);
    }
    private void FixedUpdate() {
        if (enemyState != EnemyState.Walk) return;
        if(Time.time > NextDistanceCheck) {
            NextDistanceCheck = Time.time + Random.Range(0.2f,0.33f);
            float curDist = Vector3.Distance(t.position, currentMoveTarget.position);
            if(curDist <= 1f) {
                //llega al punto patrol, hacer algo.
                ProcessBrain();
            }
        }
        Vector3 dirLook = currentMoveTarget.position- t.position;
        dirLook.y = 0;
        t.rotation = Quaternion.Slerp(t.rotation, Quaternion.LookRotation(dirLook), Time.deltaTime * rotationSpeed);

        rb.MovePosition(t.position + (t.forward.normalized * moveSpeed * Time.deltaTime));
    }
}
