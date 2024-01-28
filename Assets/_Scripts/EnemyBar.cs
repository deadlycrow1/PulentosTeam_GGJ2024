using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBar : MonoBehaviour {
    public Slider depressionBar;
    public GameObject barObj;
    public void SetupBar(float maxValue, float curValue, bool isVisible) {
        depressionBar.maxValue = maxValue;
        depressionBar.value = curValue;
        if (barObj) {
            barObj.SetActive(isVisible);
        }
    }
    public void RefreshValue(float newVal, bool isVisible = true) {
        depressionBar.value = newVal;
        if (barObj) {
            barObj.SetActive(isVisible);
        }
    }
}
