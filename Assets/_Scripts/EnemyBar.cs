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
        barObj.SetActive(isVisible);
    }
}