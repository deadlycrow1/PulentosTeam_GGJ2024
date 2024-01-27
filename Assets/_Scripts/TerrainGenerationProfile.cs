using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(menuName = "SO/Terrain Profile")]
public class TerrainGenerationProfile : ScriptableObject {
    public bool randomizeSeed;
    public int seed = 1;
    public int octaves = 4;
    [Range(0.05f, 0.75f)]
    public float octavesIncrementalVariance = 0.3f;
    [Range(1f, 100f)]
    public float scale = 20f;
    public bool circularFade;
    [Range(4f, 0.1f)]
    public float circleRadius = 1f;
    [Range(0f, 1f)]
    public float circularFadeExtraOffset = 0.3f;
    [Range(0f, 1f)]
    public float circularFadePower = 0.3f;
    public float waterHeightOffset = 25f;
    [Range(0.001f, 1f)]
    public float grassNoiseScale = 0.5f;
    [Range(0.001f, 1f)]
    public float grassRate = 0.5f;
    public float flattenHeight = 62f;
    public float flattenReach = 1f;
    [Range(0f, 1f)]
    public float flattenIntensity = .5f;
    [Range(0f, 1f)]
    public float blurAverageOfPreviousPixel = 0.3f;
    [Range(0f, 1f)]
    public float blurIntensity = 0.3f;

    [Button("Override Profile To Terrain")]
    private void OverrideProfile() {
        TerrainRandomizer.GenerateTerrainRandomExternal(this);
    }
}
