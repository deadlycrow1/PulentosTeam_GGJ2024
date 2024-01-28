using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class sporeSphereBehaviour : MonoBehaviour
{
    public VisualEffect sporesVFX;
    private Transform t;
    public float sphereRadius;
    public float secondsUntilDestroyed;

    public float sphereAttackRadius;

    public float sphereAttackDamage = 10f;
    bool setToDestroy;

    // Start is called before the first frame update
    void Awake()
    {
        t = gameObject.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (secondsUntilDestroyed < 0)
        {
            if (!setToDestroy) {
                setToDestroy = true;
                sporesVFX.Stop();
                Invoke(nameof(DestroyDelayed), 5f);
            }
            return;
        }
        else {
            secondsUntilDestroyed -= Time.deltaTime;
        }

        Collider[] hitCollider = Physics.OverlapSphere(transform.position, sphereAttackRadius);
        if (hitCollider != null && hitCollider.Length > 0)
        {

            for (int i = 0; i < hitCollider.Length; i++)
            {
                if (hitCollider[i].gameObject.TryGetComponent(out EnemyBehaviour enemy))
                {
                    enemy.GetDamage(sphereAttackDamage * Time.deltaTime);
                }
            }
        }
    }

    private void DestroyDelayed() {
        Destroy(gameObject);
    }
}
