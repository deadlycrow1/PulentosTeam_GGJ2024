using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackHelper : MonoBehaviour
{
    //    public EnemyBehaviour enemy;
    public float attackDmg = 10f;
    public float attackAngleRange = 0.3f;
    Transform t;
    float attackCD;
    private void Awake()
    {
        t = this.transform;
    }
    public void Attack()
    {
        if (Time.time > attackCD)
        {
            Debug.Log("Jefe atacando!");
            attackCD = Time.time + 0.5f;
            Vector3 fwd = t.forward;
            Vector3 targetDir = PlayerController.instance.transform.position - t.position;
            float dot = Vector3.Dot(fwd, targetDir);
            if (dot > attackAngleRange)
            {
                //en rango
                PlayerController.instance.GetDamage(attackDmg);
                print("Jefe atacando!");
            }
        }
    }
}
