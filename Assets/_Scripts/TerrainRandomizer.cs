using UnityEngine;
using System.Linq;
using NaughtyAttributes;

[ExecuteInEditMode]
public class TerrainRandomizer : MonoBehaviour {
    public int width = 256;
    public int length = 256;
    public int height = 256;
    [Space()]
    public int seed = 1;
    [Space()]
    public int octaves = 4;
    [Range(0.05f, 0.75f)]
    public float octavesIncrementalVariance = 0.3f;
    [Range(1f, 100f)]
    public float scale = 20f;

    float curOctaveVariance = 1f;
    private Terrain cachedTerrain;

    public bool circularFade;
    [Range(4f, 0.1f)]
    public float circleRadius = 1f;

    [Range(0f, 1f)]
    public float circularFadeExtraOffset = 0.3f;

    [Range(0f, 1f)]
    public float circularFadePower = 0.3f;

    public float waterHeightOffset = 25f;


    [Button("Randomize Terrain")]
    public void GenerateRandomTerrain() {
        if (!cachedTerrain) {
            cachedTerrain = GetComponent<Terrain>();
        }
        cachedTerrain.terrainData = RandomizeTerrain(cachedTerrain.terrainData);

        if (circularFade) {
            CircularFadeTerrain();
        }
        ResetDefaults();
        GenerateSplatMap();
    }

    private void Awake() {
        cachedTerrain = GetComponent<Terrain>();
        cachedTerrain.terrainData = RandomizeTerrain(cachedTerrain.terrainData);

        if (circularFade) {
            CircularFadeTerrain();
        }
        ResetDefaults();
        GenerateSplatMap();
    }
    private void Update() {

    }
    TerrainData RandomizeTerrain(TerrainData tData) {
        seed = Random.Range(1, 9999);
        tData.heightmapResolution = width + 1;
        tData.size = new Vector3(width, height, length);

        if (octaves > 1) {
            tData.SetHeights(0, 0, RandomizeHeightsAdditive());
        }
        else {
            tData.SetHeights(0, 0, RandomizeHeights());
        }
        return tData;
    }

    float[,] RandomizeHeights() {
        float[,] tempHeights = new float[width, length];

        for (int x = 0; x < width; x++) {
            for (int z = 0; z < length; z++) {
                tempHeights[x, z] += CalculateHeight(x, z);
            }
        }
        return tempHeights;
    }
    float[,] RandomizeHeightsAdditive() {
        float[,] tempHeights = new float[width, length];

        for (int i = 0; i < octaves; i++) {
            for (int x = 0; x < width; x++) {
                for (int z = 0; z < length; z++) {

                    if (i == 0) {
                        tempHeights[x, z] += CalculateHeight(x, z);
                    }
                    else {
                        tempHeights[x, z] *= CalculateHeight(x, z, curOctaveVariance);
                    }
                }
            }
            curOctaveVariance += octavesIncrementalVariance;
        }
        return tempHeights;
    }
    float CalculateHeight(int x, int z, float scaleMod = 1f) {
        float xCoord = (float)x / width * scale * scaleMod;
        float zCoord = (float)z / length * scale * scaleMod;

        return Mathf.PerlinNoise(xCoord + seed, zCoord + seed);
    }

    private void ResetDefaults() {
        curOctaveVariance = 1f;
    }
    private void CircularFadeTerrain() {

        int offset = width / 2;
        //raise a  circle. sort of.
        float[,] heights = cachedTerrain.terrainData.GetHeights(0, 0, width, length);
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < length; y++) {
                float currentRadiusSqr = (new Vector2(width / 2, length / 2) - new Vector2(x, y)).sqrMagnitude * circleRadius;

                /*
                if (currentRadiusSqr < offset * offset) {

                    heights[y, x] *= height * (1 - currentRadiusSqr / (offset * offset)) / (height * sphereHeightOffset);
                }
                */

                heights[y, x] = Mathf.Lerp(
                    heights[y, x] *= Mathf.Lerp(1f,
                    height * Mathf.Clamp((1 - currentRadiusSqr / (offset * offset)) / height, 0.0001f, 3f * height),
                    circularFadePower),

                    Mathf.Lerp(1f,
                    height * Mathf.Clamp((1 - currentRadiusSqr / (offset * offset)) / height, 0.0001f, 3f * height),
                    circularFadePower)
                    ,
                    circularFadeExtraOffset);
            }
        }
        cachedTerrain.terrainData.SetHeights(0, 0, heights);
    }
    private void GenerateSplatMap() {
        float[,,] splatmapData = new float[cachedTerrain.terrainData.alphamapWidth,
            cachedTerrain.terrainData.alphamapHeight, cachedTerrain.terrainData.alphamapLayers];

        for (int y = 0; y < cachedTerrain.terrainData.alphamapHeight; y++) {
            for (int x = 0; x < cachedTerrain.terrainData.alphamapWidth; x++) {
                // Normalise x/y coordinates to range 0-1 
                float y_01 = (float)y / (float)cachedTerrain.terrainData.alphamapHeight;
                float x_01 = (float)x / (float)cachedTerrain.terrainData.alphamapWidth;

                // Sample the height at this location (note GetHeight expects int coordinates corresponding to locations in the heightmap array)
                float height = cachedTerrain.terrainData.GetHeight(Mathf.RoundToInt(y_01 * cachedTerrain.terrainData.heightmapResolution), Mathf.RoundToInt(x_01 * cachedTerrain.terrainData.heightmapResolution));

                // Calculate the normal of the terrain (note this is in normalised coordinates relative to the overall terrain dimensions)
                Vector3 normal = cachedTerrain.terrainData.GetInterpolatedNormal(y_01, x_01);

                // Calculate the steepness of the terrain
                float steepness = cachedTerrain.terrainData.GetSteepness(y_01, x_01);

                // Setup an array to record the mix of texture weights at this point
                float[] splatWeights = new float[cachedTerrain.terrainData.alphamapLayers];

                // CHANGE THE RULES BELOW TO SET THE WEIGHTS OF EACH TEXTURE ON WHATEVER RULES YOU WANT

                // ARENA MOJADA
                splatWeights[0] = height < waterHeightOffset + 0.75f ? 1f : 0f;

                // ARENA SECA
                splatWeights[1] = height > waterHeightOffset + 0.7f ?
                    Mathf.Clamp01((cachedTerrain.terrainData.heightmapResolution - height + waterHeightOffset)) : 0f;

                // PASTO

                // Texture[2] stronger on flatter terrain
                // Note "steepness" is unbounded, so we "normalise" it by dividing by the extent of heightmap height and scale factor
                // Subtract result from 1.0 to give greater weighting to flat surfaces

                splatWeights[2] = (height > waterHeightOffset + 1f ? 1f -
                    Mathf.Clamp01(
                        steepness * steepness * 1.6f / (cachedTerrain.terrainData.heightmapResolution / 5.0f)) : 0f) * 40f;

                // ROCA

                // Texture[3] increases with height but only on surfaces facing positive Z axis 
                //splatWeights[3] = height * Mathf.Clamp01(normal.z);
                bool c1 = steepness > 47f && steepness < 59f && Random.value > 0.25f;
                bool c2 = steepness > 65f;

                splatWeights[3] = c1 || c2 ? 20f : 0f;

                // Sum of all textures weights must add to 1, so calculate normalization factor from sum of weights
                float z = splatWeights.Sum();

                // Loop through each terrain texture
                for (int i = 0; i < cachedTerrain.terrainData.alphamapLayers; i++) {

                    // Normalize so that sum of all texture weights = 1
                    splatWeights[i] /= z;

                    // Assign this point to the splatmap array
                    splatmapData[x, y, i] = splatWeights[i];
                }
            }
        }

        // Finally assign the new splatmap to the terrainData:
        cachedTerrain.terrainData.SetAlphamaps(0, 0, splatmapData);
    }
}
