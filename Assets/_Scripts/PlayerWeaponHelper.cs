using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponHelper : MonoBehaviour
{
    public bool isSocketA;
    public PlayerController pc;
    public Animator anim;
    public Transform socket_A, socket_B;
    Transform t;
    private void Awake() {
        t = this.transform;
    }
    public void SwitchSocket() {
        isSocketA = !isSocketA;
    }
    private void Update() {
        float tSpeed = 0f;
        if (isSocketA) {
            tSpeed = 1f;
            if (pc.isAttacking) {
                tSpeed = 2f;
            }
        }
        anim.SetFloat("Speed", tSpeed,0.15f, Time.deltaTime*2f);
    }
    private void LateUpdate() {
        if (socket_A == null || socket_B == null || t == null) return;
        t.localScale = isSocketA ? socket_A.localScale : socket_B.localScale;
        t.SetPositionAndRotation(
            isSocketA ? socket_A.position : socket_B.position, 
            isSocketA ? socket_A.rotation : socket_B.rotation);
    }
}
