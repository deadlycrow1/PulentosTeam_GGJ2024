using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sporeSphereBehaviour : MonoBehaviour
{
    private Transform t;
    public float sphereRadius;
    public float secondsUntilDestroyed;

    public float sphereAttackDamage = 10f;

    // Start is called before the first frame update
    void Awake()
    {
        t = gameObject.transform;
        t.localScale *= sphereRadius;
    }

    // Update is called once per frame
    void Update()
    {
        secondsUntilDestroyed -= Time.deltaTime;
        if (secondsUntilDestroyed < 1)
        {
            transform.localScale *= secondsUntilDestroyed;
        }
        if (secondsUntilDestroyed < 0)
        {
            Destroy(gameObject);
        }

        Collider[] hitCollider = Physics.OverlapSphere(transform.position, sphereRadius);
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
}
