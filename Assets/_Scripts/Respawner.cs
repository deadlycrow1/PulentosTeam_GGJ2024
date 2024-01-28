using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawner : MonoBehaviour {
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player") || other.CompareTag("Enemy")) {
            other.transform.position = GameManager.GetRandomPatrolPoint().position + Vector3.up;
        }
    }
}
