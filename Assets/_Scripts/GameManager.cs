using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public PlayerController playerTarget;
    public TerrainRandomizer terrainRandomizer;
    public TerrainGenerationProfile[] terrainProfiles;
    public EnemyBehaviour[] enemyPrefabs;
    public int enemyAmountToSpawn = 20;
    public bool isBossLevel;
    public EnemyBehaviour[] bossPrefabs;
    GameObject[] patrolPoints;
     void Awake() {
        instance = this;
    }
    IEnumerator Start() {
        
        terrainRandomizer.GenerateRandomTerrainWithProfile(terrainProfiles[Random.Range(0, terrainProfiles.Length)]);
        yield return new WaitForSeconds(0.5f);
        RandomSpawner.instance.RandomSpawn();
        yield return new WaitForSeconds(0.2f);
        patrolPoints = GameObject.FindGameObjectsWithTag("PatrolPoint");
        yield return new WaitForSeconds(0.2f);
        if (enemyAmountToSpawn > 0) {
            for (int i = 0; i < enemyAmountToSpawn; i++) {
                Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Length)], GetRandomPatrolPoint().position + Vector3.up, Quaternion.identity);
                yield return new WaitForSeconds(0.1f);
            }
        }

        yield return new WaitForSeconds(0.2f);
        playerTarget.transform.position = GetRandomPatrolPoint().position + Vector3.up;
        playerTarget.canDrive = true;
        yield return new WaitForSeconds(0.1f);
        CameraRig.instance.SnapCamera();
        if (isBossLevel && bossPrefabs != null) {
            SpawnBoss();
        }
        yield return new WaitForSeconds(0.3f);
        Fader.instance.FadeIn();
        yield return new WaitForSeconds(0.3f);
        playerTarget.canBeDamaged = true;
    }
    private void SpawnBoss() {
        Vector3 spawnPos = new Vector3(64f, 300f, 64f);
        int lm = 1 << 10;
        if(Physics.Raycast(new Vector3(64f,300f,64f), Vector3.down, out RaycastHit hit, 500f, lm)) {
            spawnPos = hit.point;
        }
        Instantiate(bossPrefabs[Random.Range(0, bossPrefabs.Length)], spawnPos + Vector3.up, Quaternion.identity);

    }
    public static Transform GetRandomPatrolPoint() {
        if (instance == null) return null;
        return instance.patrolPoints[Random.Range(0, instance.patrolPoints.Length)].transform;
    }
    public void RestartLevel() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
