using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelCounter : MonoBehaviour
{
    public static LevelCounter instance;
    public int currentEnemyLevels;

    private void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }
    }
    public void LoadNextLevel() {
        currentEnemyLevels++;
        bool isBoss = false;
        if (currentEnemyLevels >= 2) {
            currentEnemyLevels = 0;
            isBoss = true;
        }
        SceneManager.LoadScene(isBoss ? "GAMEPLAY_BOSS" : "GAMEPLAY");
    }
}
