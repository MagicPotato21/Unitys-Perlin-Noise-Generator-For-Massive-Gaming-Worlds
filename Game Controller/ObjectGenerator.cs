// Made By Chris Hodges
using UnityEngine;
using System.Collections.Generic;

public class ObjectGenerator : MonoBehaviour
{
    public GameObject[] treePrefabs;
    public GameObject rockPrefab;
    public GameObject mobPrefab;

    public int treeCount = 100; // Number of trees to spawn.
    public int rockCount = 50;  // Number of rocks to spawn.
    public int mobCount = 10;   // Number of mobs to spawn.

    public Vector2 treeDensity = new Vector2(0.1f, 0.5f); // Tree spawn density range (min, max).
    public Vector2 rockDensity = new Vector2(0.05f, 0.2f); // Rock spawn density range (min, max).
    public Vector2 mobDensity = new Vector2(0.02f, 0.1f);  // Mob spawn density range (min, max).

    private List<Terrain> terrainList = new List<Terrain>();
    private List<Terrain> NewterrainList = new List<Terrain>(); // List for newly spawned terrains.
    private List<Vector3> spawnedPositions = new List<Vector3>();
    private List<bool> terrainSpawned = new List<bool>(); // Tracks if objects have been spawned on each terrain.
    private bool hasSpawnedObjects = false;

    void Start()
    {
        // Call FindTerrainsWithTag once at the start to detect existing terrains.
        FindTerrainsWithTag();

        // Start checking for new terrains every 5 seconds.
        InvokeRepeating("CheckForNewTerrains", 5f, 5f);
    }

    void CheckForNewTerrains()
    {
        // Check for new terrains periodically (every 5 seconds).
        FindTerrainsWithTag();

        // If there are terrains in NewterrainList, move them to terrainList.
        if (NewterrainList.Count > 0)
        {
            terrainList.AddRange(NewterrainList);
            NewterrainList.Clear();
            // Initialize the terrainSpawned list for the new terrains.
            terrainSpawned.AddRange(new bool[terrainList.Count]);
            SpawnObjectsOnTerrains();
        }
    }

    void FindTerrainsWithTag()
    {
        // Find all GameObjects with the "Map" tag (case-sensitive) and add their terrains to the list.
        GameObject[] mapObjects = GameObject.FindGameObjectsWithTag("Map");
        terrainList.Clear(); // Clear the previous list to avoid duplicates.

        foreach (GameObject mapObject in mapObjects)
        {
            Terrain terrain = mapObject.GetComponent<Terrain>();
            if (terrain != null)
            {
                // Check if the terrain is already in the NewterrainList or terrainList.
                if (!NewterrainList.Contains(terrain) && !terrainList.Contains(terrain))
                {
                    NewterrainList.Add(terrain); // Add to NewterrainList if not already present.
                }
            }
        }

        if (terrainList.Count == 0 && NewterrainList.Count > 0)
        {
            // Move terrains from NewterrainList to terrainList for object spawning.
            terrainList.AddRange(NewterrainList);
            NewterrainList.Clear();
            // Initialize the terrainSpawned list for the new terrains.
            terrainSpawned.AddRange(new bool[terrainList.Count]);
            SpawnObjectsOnTerrains();
        }
        else if (terrainList.Count == 0)
        {
            Debug.LogWarning("No terrains found with the 'Map' tag.");
        }
    }

    void SpawnObjectsOnTerrains()
    {
        // Spawn objects on each terrain in the list.
        for (int i = 0; i < terrainList.Count; i++)
        {
            // Check if objects have already been spawned on this terrain.
            if (!terrainSpawned[i])
            {
                Terrain terrain = terrainList[i];
                // Spawn trees, rocks, and mobs on this terrain.
                SpawnObjects(treePrefabs, treeCount, treeDensity, terrain);
                SpawnObjects(new GameObject[] { rockPrefab }, rockCount, rockDensity, terrain);
                SpawnObjects(new GameObject[] { mobPrefab }, mobCount, mobDensity, terrain);
                terrainSpawned[i] = true; // Mark this terrain as having objects spawned.
            }
        }

        hasSpawnedObjects = true; // Set the flag to true after objects are spawned.
    }

    void SpawnObjects(GameObject[] prefabs, int count, Vector2 densityRange, Terrain terrain)
    {
        TerrainData terrainData = terrain.terrainData;
        Vector3 terrainPosition = terrain.transform.position;

        for (int i = 0; i < count; i++)
        {
            Vector2 spawnPoint = new Vector2(Random.Range(0f, 1f), Random.Range(0f, 1f));
            Vector3 worldSpawnPoint = new Vector3(
                terrainPosition.x + spawnPoint.x * terrainData.size.x,
                0,
                terrainPosition.z + spawnPoint.y * terrainData.size.z
            );

            worldSpawnPoint.y = terrain.SampleHeight(worldSpawnPoint) + 1f; // Add 1f to lift objects above terrain.

            // Check if the position has already been used, and if not, spawn the object.
            if (!spawnedPositions.Contains(worldSpawnPoint))
            {
                GameObject prefab = prefabs[Random.Range(0, prefabs.Length)];
                Instantiate(prefab, worldSpawnPoint, Quaternion.identity);
                spawnedPositions.Add(worldSpawnPoint); // Add the position to the list of spawned positions.
            }
        }
    }
}
