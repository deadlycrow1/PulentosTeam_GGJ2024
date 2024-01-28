using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public PlayerController playerTarget;
    public TerrainRandomizer terrainRandomizer;
    public TerrainGenerationProfile[] terrainProfiles;
    public EnemyBehaviour[] enemyPrefabs;
    public int enemyAmountToSpawn = 20;
    GameObject[] patrolPoints;
     void Awake() {
        instance = this;
    }
    IEnumerator Start() {
        terrainRandomizer.profileOverride = terrainProfiles[Random.Range(0, terrainProfiles.Length)];
        terrainRandomizer.GenerateRandomTerrain();
        yield return new WaitForSeconds(1f);
        RandomSpawner.instance.RandomSpawn();
        yield return new WaitForSeconds(0.2f);
        patrolPoints = GameObject.FindGameObjectsWithTag("PatrolPoint");
        yield return new WaitForSeconds(0.2f);
        for (int i = 0; i < enemyAmountToSpawn; i++) {
            Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Length)], GetRandomPatrolPoint().position+Vector3.up, Quaternion.identity);
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(0.2f);
        playerTarget.transform.position = GetRandomPatrolPoint().position + Vector3.up;
        playerTarget.canDrive = true;
        yield return new WaitForSeconds(0.1f);
        CameraRig.instance.SnapCamera();
        yield return new WaitForSeconds(0.3f);
        Fader.instance.FadeIn();
    }
    public static Transform GetRandomPatrolPoint() {
        if (instance == null) return null;
        return instance.patrolPoints[Random.Range(0, instance.patrolPoints.Length)].transform;
    }
}
