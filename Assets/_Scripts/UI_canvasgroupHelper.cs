using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class UI_canvasgroupHelper : MonoBehaviour
{
    public CanvasGroup cg;
    public float duration = 0.6f;
    public AnimationCurve curve;

    private void OnEnable() {
        StopAllCoroutines();
        StartCoroutine(Seq(false));
    }
    public void FadeOut() {
        StopAllCoroutines();
        StartCoroutine(Seq(true));
    }
    IEnumerator Seq(bool isOut) {
        float lerp = 0f;
        while (lerp < 1f) {
            cg.alpha = curve.Evaluate(isOut ? 1f - lerp : lerp);
            lerp += Time.deltaTime / duration;
            yield return new WaitForEndOfFrame();
        }
        cg.alpha = curve.Evaluate(isOut ? 0f : 1f);
    }
}
