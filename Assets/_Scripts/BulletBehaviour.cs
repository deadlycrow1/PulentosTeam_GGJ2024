using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    public float bulletSpeed;
    public float secondsUntilDestroyed;
    private Rigidbody bulletRb;

    public float bulletDamage;

    // Start is called before the first frame update
    void Start()
    {
        bulletRb = GetComponent<Rigidbody>();
        bulletRb.velocity = transform.forward * bulletSpeed;

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
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.GetComponent<EnemyBehaviour>() != null)
        {
            HealthSystem theirHealthSystem = other.gameObject.GetComponent<HealthSystem>();
            if (theirHealthSystem != null)
            {
              //  theirHealthSystem.TakeDamage(bulletDamage);
                Destroy(gameObject);
            }
        }
        else if (other.gameObject.GetComponent<Terrain>() != null)
        {
            Destroy(gameObject);
        }

    }
}
