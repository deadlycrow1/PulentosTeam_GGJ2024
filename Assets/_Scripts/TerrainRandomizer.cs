using UnityEngine;
using System.Linq;
using NaughtyAttributes;

[ExecuteInEditMode]
public class TerrainRandomizer : MonoBehaviour {
    public static TerrainRandomizer instance;
    [BoxGroup("NO MOVER ESTOS")]
    public int width = 256;
    [BoxGroup("NO MOVER ESTOS")]
    public int length = 256;
    [BoxGroup("NO MOVER ESTOS")]
    public int height = 256;
    [BoxGroup("Parametros para editar")]
    public bool randomizeSeed = true;
    [BoxGroup("Parametros para editar")]
    public int seed = 1;
    [BoxGroup("Parametros para editar")]
    public int octaves = 4;

    [BoxGroup("Parametros para editar")]
    [Range(0.05f, 0.75f)]
    public float octavesIncrementalVariance = 0.3f;
    [BoxGroup("Parametros para editar")]
    [Range(1f, 100f)]
    public float scale = 20f;

    float curOctaveVariance = 1f;
    private Terrain cachedTerrain;

    [BoxGroup("Parametros para editar")]
    public bool circularFade;

    [BoxGroup("Parametros para editar")]
    [Range(4f, 0.1f)]
    public float circleRadius = 1f;

    [BoxGroup("Parametros para editar")]
    [Range(0f, 1f)]
    public float circularFadeExtraOffset = 0.3f;

    [BoxGroup("Parametros para editar")]
    [Range(0f, 1f)]
    public float circularFadePower = 0.3f;

    [BoxGroup("Parametros para editar")]
    public float waterHeightOffset = 25f;
    [BoxGroup("Parametros para editar")]
    [Range(0.001f, 1f)]
    public float grassNoiseScale = 0.5f;
    [BoxGroup("Parametros para editar")]
    [Range(0.001f, 1f)]
    public float grassRate = 0.5f;
    [BoxGroup("Parametros para editar")]
    public float flattenHeight = 62f;
    [BoxGroup("Parametros para editar")]
    public float flattenReach = 1f;
    [BoxGroup("Parametros para editar")]
    [Range(0f, 1f)]
    public float flattenIntensity = .5f;
    [BoxGroup("Parametros para editar")]
    [Range(0f, 1f)]
    public float blurAverageOfPreviousPixel = 0.3f;
    [BoxGroup("Parametros para editar")]
    [Range(0f, 1f)]
    public float blurIntensity = 0.3f;

    [BoxGroup("Parametros para editar")]
    public TerrainGenerationProfile profileOverride;

    [Button("Randomize Terrain")]
    public void GenerateRandomTerrainWithProfile(TerrainGenerationProfile newProfile) {
        Debug.Log("Generando terreno desde perfil: "+ newProfile.name);
        randomizeSeed = false;
        profileOverride = newProfile;
        ProcessProfileOverride();
        Random.InitState(seed);
        if (!cachedTerrain) {
            cachedTerrain = GetComponent<Terrain>();
        }
        cachedTerrain.terrainData = RandomizeTerrain(cachedTerrain.terrainData);

        if (circularFade) {
            CircularFadeTerrain();
        }
        if (flattenIntensity > 0f) {
            FlattenTerrain();
        }
        if (blurIntensity > 0f) {
            BlurTerrain();
        }
        ResetDefaults();
        GenerateSplatMap();
    }
    public void GenerateRandomTerrain() {
        if (randomizeSeed) {
            seed = Random.Range(1, 1000000);
        }
        ProcessProfileOverride();
        Random.InitState(seed);
        if (!cachedTerrain) {
            cachedTerrain = GetComponent<Terrain>();
        }
        cachedTerrain.terrainData = RandomizeTerrain(cachedTerrain.terrainData);

        if (circularFade) {
            CircularFadeTerrain();
        }
        if (flattenIntensity > 0f) {
            FlattenTerrain();
        }
        if(blurIntensity > 0f) {
            BlurTerrain();
        }
        ResetDefaults();
        GenerateSplatMap();
    }
    public static void GenerateTerrainRandomExternal(TerrainGenerationProfile newProfile) {
        if (instance == null) {
            //Debug.LogError("No existe instancia de randomizador de terrain!!");
            instance = FindObjectOfType<TerrainRandomizer>();
        }
        instance.profileOverride = newProfile;
        instance.GenerateRandomTerrain();
    }
    private void Awake() {
        /*
        if (randomizeSeed) {
            seed = Random.Range(1, 1000000);
        }
        ProcessProfileOverride();
        Random.InitState(seed);
        cachedTerrain = GetComponent<Terrain>();
        cachedTerrain.terrainData = RandomizeTerrain(cachedTerrain.terrainData);

        if (circularFade) {
            CircularFadeTerrain();
        }
        if (flattenIntensity > 0f) {
            FlattenTerrain();
        }
        if (blurIntensity > 0f) {
            BlurTerrain();
        }
        ResetDefaults();
        GenerateSplatMap();
        */
    }
    private void ProcessProfileOverride() {
        if (profileOverride == null) return;
        randomizeSeed = profileOverride.randomizeSeed;
        seed = profileOverride.seed;
        /*
        if (randomizeSeed) {
            seed = Random.Range(1, 1000000);
            profileOverride.seed = seed;
        }
        */
        octaves = profileOverride.octaves;
        octavesIncrementalVariance = profileOverride.octavesIncrementalVariance;
        scale = profileOverride.scale;
        circularFade = profileOverride.circularFade;
        circleRadius = profileOverride.circleRadius;
        circularFadeExtraOffset = profileOverride.circularFadeExtraOffset;
        circularFadePower = profileOverride.circularFadePower;
        waterHeightOffset = profileOverride.waterHeightOffset;
        grassNoiseScale = profileOverride.grassNoiseScale;
        grassRate = profileOverride.grassRate;
        flattenHeight = profileOverride.flattenHeight;
        flattenReach = profileOverride.flattenReach;
        flattenIntensity = profileOverride.flattenIntensity;
        blurAverageOfPreviousPixel = profileOverride.blurAverageOfPreviousPixel;
        blurIntensity = profileOverride.blurIntensity;
    }
    TerrainData RandomizeTerrain(TerrainData tData) {
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

                heights[x, y] = Mathf.Lerp(
                    heights[x, y] *= Mathf.Lerp(1f,
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
    private void FlattenTerrain() {
        float[,] heights = cachedTerrain.terrainData.GetHeights(0, 0, width, length);
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < length; y++) {
                float curHeight = cachedTerrain.terrainData.GetHeight(x, y);
                float normalizedHeight = Mathf.InverseLerp(0, height, curHeight);
                float distanceWeightLoss =Mathf.Clamp(1f-Mathf.Abs((flattenHeight- curHeight)/(flattenHeight + curHeight)/2f), 0f, 1f);

                if (curHeight > (flattenHeight-flattenReach) && 
                    curHeight < (flattenHeight + flattenReach)) {
                    float targetHeightAfterFlatten = Mathf.Lerp(curHeight, flattenHeight, flattenIntensity * distanceWeightLoss);
                    normalizedHeight = Mathf.InverseLerp(0, height, targetHeightAfterFlatten);

                }
                heights[x, y] = normalizedHeight;
            }
        }
        cachedTerrain.terrainData.SetHeights(0, 0, heights);
    }
    private void BlurTerrain() {
        float[,] heights = cachedTerrain.terrainData.GetHeights(0, 0, width, length);
        float prevPixelHeight = 0;
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < length; y++) {
                float curHeight = cachedTerrain.terrainData.GetHeight(x, y)/height;

                if (x == 0 && y == 0) {
                    prevPixelHeight = curHeight;
                }
                float BlurredPixelHeight = Mathf.Lerp(curHeight, prevPixelHeight, blurAverageOfPreviousPixel);
                
                heights[x, y] = Mathf.Lerp(curHeight, BlurredPixelHeight, blurIntensity);
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

                splatWeights[2] = (Mathf.PerlinNoise(x * grassNoiseScale, y * grassNoiseScale) < grassRate ? 1f : 0f) * (height > waterHeightOffset + 1f ? 1f -
                    Mathf.Clamp01(
                        steepness * steepness * 0.1f / (cachedTerrain.terrainData.heightmapResolution / 2.0f)) : 0f) * 20f;

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
