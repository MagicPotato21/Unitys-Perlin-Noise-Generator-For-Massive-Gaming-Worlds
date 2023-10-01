//Made By Chris Hodges
using UnityEngine;

public class RandomMountainGenerator : MonoBehaviour
{
    public int width = 512;            // Width of the terrain
    public int length = 512;           // Length of the terrain
    public float maxHeight = 20.0f;    // Maximum height of mountains
    public float minHeight = 0.0f;     // Minimum height (set to zero)
    public int borderSize = 10;        // Width of the flat border

    private float scale;               // Randomly generated scale

    void Start()
    {
        // Generate a random scale value between 1 and 5
        scale = Random.Range(1.0f, 5.0f);

        Terrain terrain = GetComponent<Terrain>();
        terrain.terrainData = GenerateTerrain(terrain.terrainData);
    }

    TerrainData GenerateTerrain(TerrainData terrainData)
    {
        terrainData.heightmapResolution = width + 1;
        terrainData.size = new Vector3(width, maxHeight, length);
        terrainData.SetHeights(0, 0, GenerateHeights());
        return terrainData;
    }

    float[,] GenerateHeights()
    {
        float[,] heights = new float[width, length];

        // Generate terrain heights
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < length; y++)
            {
                // Check if we're in the border area
                if (x < borderSize || x > width - borderSize || y < borderSize || y > length - borderSize)
                {
                    heights[x, y] = minHeight;  // Set to the minimum height for the border
                }
                else
                {
                    float xCoord = (float)(x - borderSize) / (width - 2 * borderSize) * scale;
                    float yCoord = (float)(y - borderSize) / (length - 2 * borderSize) * scale;
                    float perlinValue = Mathf.PerlinNoise(xCoord, yCoord);
                    float height = perlinValue * maxHeight + minHeight;

                    // Apply the height to the terrain
                    heights[x, y] = height;
                }
            }
        }

        return heights;
    }
}