using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LatigoBehaviour : MonoBehaviour
{
    public Vector3 playerPos;
    // Start is called before the first frame update
    void Start()
    {
        playerPos = PlayerController.instance.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Resize(1, playerPos);
    }

    public void Resize(float amount, Vector3 direction)
    {
        transform.position += direction * amount / 2; // Move the object in the direction of scaling, so that the corner on ther side stays in place
        transform.localScale += direction * amount; // Scale object in the specified direction
    }
}
