using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class RandomSpawner : MonoBehaviour {
    public static RandomSpawner instance;
    [Header("World Spawn Settings")]
    public Terrain cachedTerrain;
    public PrefabSpawner[] spawners;
    public Transform spawnAnchor_A, spawnAnchor_B;
    Transform cachedParent;
    Vector3 tempRayPos;
    int tempSpawnCount = 0;

    private void Awake() {
        instance = this;
    }
    private void Start() {
        //RandomSpawn();
    }
    public void RandomSpawn() {
        cachedParent = null;
        tempSpawnCount = 0;
        int lm = 1 << 10;
        Vector3 lastPos = Vector3.zero;

        if (spawners != null && spawners.Length > 0) {
            for (int i = 0; i < spawners.Length; i++) {
                if (spawners[i].parentName != null) {
                    cachedParent = GameObject.Find(spawners[i].parentName).transform;
                }
                for (int k = 0; tempSpawnCount < spawners[i].targetAmount; k++) {
                    RandomizeRayPos();
                    if (Physics.Raycast(tempRayPos, Vector3.down, out RaycastHit hit, 700f, lm)) {
                        if (hit.collider.tag == "Terrain") {
                            int tempGroundIndex = GroundType(hit.point);
                            if (tempGroundIndex == spawners[i].groundTypeIndex) {
                                if ((hit.point - lastPos).sqrMagnitude > spawners[i].minimumDistance * spawners[i].minimumDistance) {
                                    GameObject go = Instantiate(
                                        spawners[i].prefabs[Random.Range(0, spawners[i].prefabs.Length)],
                                        hit.point + spawners[i].spawnOffset, Quaternion.identity);

                                    if (cachedParent != null) {
                                        go.transform.parent = cachedParent;
                                    }

                                    go.transform.localScale =
                                        Vector3.one * Random.Range(spawners[i].scaleRange.x, spawners[i].scaleRange.y);

                                    float modY = Random.Range(spawners[i].verticalScaleRange.x, spawners[i].verticalScaleRange.y);
                                    Vector3 modifiedHeightScale = go.transform.localScale;

                                    modifiedHeightScale.y *= modY;

                                    go.transform.localScale = modifiedHeightScale;

                                    if (spawners[i].alignToSurface) {
                                        Quaternion tRot = Quaternion.FromToRotation(go.transform.up, hit.normal);
                                        go.transform.rotation = Quaternion.Slerp(go.transform.rotation,
                                            go.transform.rotation * tRot, spawners[i].alignmentRate);

                                        if (spawners[i].discardXYAxis) {
                                            Vector3 tempRot = go.transform.localEulerAngles;
                                            tempRot.x = 0;
                                            tempRot.y = 0;
                                            go.transform.localEulerAngles = tempRot;
                                        }
                                    }
                                    if (spawners[i].randomizeRotation) {
                                        Vector3 randomRot = Vector3.zero;
                                        randomRot.x = spawners[i].randomRotationAxis.x * Random.Range(0f, 359f);
                                        randomRot.y = spawners[i].randomRotationAxis.y * Random.Range(0f, 359f);
                                        randomRot.z = spawners[i].randomRotationAxis.z * Random.Range(0f, 359f);

                                        go.transform.Rotate(randomRot);
                                    }
                                    tempSpawnCount++;
                                    lastPos = hit.point;
                                }
                            }
                        }
                        else {
                            Debug.LogWarning("Terrain no detectado!");
                        }
                    }
                    //yield return new WaitForSeconds(0.15f);
                }
                lastPos = Vector3.zero;
                tempSpawnCount = 0;
                cachedParent = null;
            }
        }
    }
    private void RandomizeRayPos() {
        tempRayPos.x = Random.Range(spawnAnchor_A.position.x, spawnAnchor_B.position.x);
        tempRayPos.y = 400f;
        tempRayPos.z = Random.Range(spawnAnchor_A.position.z, spawnAnchor_B.position.z);
    }
    public int GroundType(Vector3 checkPos) {
        return TerrainSurface.GetMainTexture(checkPos);
    }
}
