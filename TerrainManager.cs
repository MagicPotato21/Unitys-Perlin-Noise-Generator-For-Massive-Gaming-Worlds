using UnityEngine;

public class TerrainManager : MonoBehaviour
{
    public Terrain terrainPrefab; // The prefab for the terrain
    public Transform player; // The player object
    public float terrainSize = 100f; // Size of each terrain
    public int terrainResolution = 256; // Resolution of each terrain
    public int terrainDistance = 2; // Number of terrains to generate in each direction

    private Vector2Int currentTerrainIndex;
    private Vector2Int lastTerrainIndex;

    void Start()
    {
        currentTerrainIndex = GetTerrainIndexFromPosition(player.position);
        GenerateInitialTerrains();
    }

    void Update()
    {
        currentTerrainIndex = GetTerrainIndexFromPosition(player.position);

        // Check if the player has moved to a new terrain
        if (currentTerrainIndex != lastTerrainIndex)
        {
            GenerateNewTerrains();
        }

        lastTerrainIndex = currentTerrainIndex;
    }

    Vector2Int GetTerrainIndexFromPosition(Vector3 position)
    {
        int x = Mathf.FloorToInt(position.x / terrainSize);
        int z = Mathf.FloorToInt(position.z / terrainSize);
        return new Vector2Int(x, z);
    }

    void GenerateInitialTerrains()
    {
        for (int x = -terrainDistance; x <= terrainDistance; x++)
        {
            for (int z = -terrainDistance; z <= terrainDistance; z++)
            {
                Vector3 terrainPosition = new Vector3(
                    currentTerrainIndex.x + x * terrainSize,
                    0f,
                    currentTerrainIndex.y + z * terrainSize
                );

                Terrain terrain = Instantiate(terrainPrefab, terrainPosition, Quaternion.identity);
                terrain.terrainData = GenerateTerrainData();
            }
        }
    }

    void GenerateNewTerrains()
    {
        for (int x = -terrainDistance; x <= terrainDistance; x++)
        {
            for (int z = -terrainDistance; z <= terrainDistance; z++)
            {
                Vector2Int newTerrainIndex = new Vector2Int(
                    currentTerrainIndex.x + x,
                    currentTerrainIndex.y + z
                );

                if (Mathf.Abs(newTerrainIndex.x - currentTerrainIndex.x) <= terrainDistance &&
                    Mathf.Abs(newTerrainIndex.y - currentTerrainIndex.y) <= terrainDistance)
                {
                    // Check if the terrain at this index already exists
                    if (!TerrainExistsAt(newTerrainIndex))
                    {
                        Vector3 terrainPosition = new Vector3(
                            newTerrainIndex.x * terrainSize,
                            0f,
                            newTerrainIndex.y * terrainSize
                        );

                        Terrain terrain = Instantiate(terrainPrefab, terrainPosition, Quaternion.identity);
                        terrain.terrainData = GenerateTerrainData();
                    }
                }
            }
        }
    }

    TerrainData GenerateTerrainData()
    {
        TerrainData terrainData = new TerrainData
        {
            heightmapResolution = terrainResolution,
            size = new Vector3(terrainSize, 50f, terrainSize)
            // You can add more terrain data settings here
        };

        return terrainData;
    }

    bool TerrainExistsAt(Vector2Int index)
    {
        Terrain[] terrains = Terrain.activeTerrains;
        foreach (Terrain terrain in terrains)
        {
            Vector2Int terrainIndex = GetTerrainIndexFromPosition(terrain.transform.position);
            if (terrainIndex == index)
            {
                return true;
            }
        }
        return false;
    }
}
