using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_ThornsHelper : MonoBehaviour
{
    public float damageDistanceThresold;
    public float speed;
    public AnimationCurve scaleCurve;
    public float scaleDuration;
    public float destroyAfterTime = 2f;
    float scaleLerp;
    Vector3 playerDir;
    Transform t;
    bool playerDamaged;
    public float thornDamage = 33f;

    private void Awake() {
        t = this.transform;
    }
    private void Start() {
        playerDir = PlayerController.instance.transform.position- t.position;
        Invoke(nameof(DestroyDelayed), destroyAfterTime);
    }
    private void DestroyDelayed() {
        Destroy(gameObject);
    }
    private void Update() {
        t.localScale = Vector3.one * scaleCurve.Evaluate(scaleLerp);
        if (scaleLerp < 1f) {
            scaleLerp += Time.deltaTime / scaleDuration;
        }
        else {
            scaleLerp = 1f;
        }
        t.rotation = Quaternion.LookRotation(playerDir);
        t.position += t.forward * speed * Time.deltaTime;
        if (!playerDamaged) {
            float curDist = Vector3.Distance(t.position, PlayerController.instance.transform.position);
            if(curDist <= damageDistanceThresold * t.localScale.x) {
                playerDamaged = true;
                PlayerController.instance.GetDamage(thornDamage);
            }
        }
    }
}
