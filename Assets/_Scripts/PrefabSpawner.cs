using UnityEngine;

[CreateAssetMenu(fileName = "New Prefab Spawner", menuName = "SO/Create Prefab Spawner")]
public class PrefabSpawner : ScriptableObject
{
    public int groundTypeIndex = -1;
    public GameObject[] prefabs;
    public int targetAmount;
    public float minimumDistance = 0.1f;
    public Vector2 scaleRange = new Vector2(0.8f,1.2f);
    public Vector3 spawnOffset;
    public bool alignToSurface;
    [Range(0f,1f)]
    public float alignmentRate = 1f;
    public bool discardXYAxis;
    public bool randomizeRotation;
    public Vector3 randomRotationAxis;
    public string parentName;
}
