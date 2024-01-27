using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    public float depression = 100f;
    float maxDepression;

    private void Awake() {
        maxDepression = depression;
    }
}
