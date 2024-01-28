using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fader : MonoBehaviour
{
    public static Fader instance;
    public Image img;
    public float fadeDuration = 1f;
    public AnimationCurve fadeCurve;
    private void Awake() {
        instance = this;
        img.color = Color.black;
    }
    public void FadeIn() {
        StopAllCoroutines();
        StartCoroutine(FadeSeq(false));
    }
    public void FadeOut() {
        StopAllCoroutines();
        StartCoroutine(FadeSeq(false));
    }
    IEnumerator FadeSeq(bool isOut) {
        if (!img.gameObject.activeInHierarchy) {
            img.gameObject.SetActive(true);
        }
        float lerp = 0;
        while (lerp < 1f) {
            img.color = Color.Lerp(Color.clear, Color.black,fadeCurve.Evaluate( isOut ? lerp : 1f - lerp));
            lerp += Time.deltaTime / fadeDuration;
            yield return new WaitForEndOfFrame();
        }
        img.color = isOut ? Color.black : Color.clear;
        if (!isOut) {
            img.gameObject.SetActive(false);
        }
    }
}
