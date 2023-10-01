//Made By Chris Hodges
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class AutoAssignTerrainCollider : MonoBehaviour
{
    private float checkInterval = 5f; // Check for Terrains every 5 seconds
    private List<Terrain> processedTerrains = new List<Terrain>(); // List to store processed Terrains

    private void Start()
    {
        // Start the coroutine to check for Terrains
        StartCoroutine(CheckForTerrains());
    }

    private IEnumerator CheckForTerrains()
    {
        while (true)
        {
            // Find all GameObjects with the "Map" tag
            GameObject[] mapObjects = GameObject.FindGameObjectsWithTag("Map");

            // Loop through each GameObject and check if it has a Terrain component
            foreach (GameObject mapObject in mapObjects)
            {
                Terrain terrain = mapObject.GetComponent<Terrain>();

                if (terrain != null && !processedTerrains.Contains(terrain))
                {
                    TerrainCollider terrainCollider = mapObject.GetComponent<TerrainCollider>();

                    if (terrainCollider == null)
                    {
                        // If TerrainCollider doesn't exist, add it
                        terrainCollider = mapObject.AddComponent<TerrainCollider>();
                        Debug.LogWarning("TerrainCollider added to " + terrain.name + " GameObject.");
                    }

                    terrainCollider.terrainData = terrain.terrainData;
                    Debug.Log("TerrainCollider for " + terrain.name + " has been auto-assigned.");

                    // Add the processed Terrain to the list
                    processedTerrains.Add(terrain);
                }
            }

            // Wait for the specified interval before checking again
            yield return new WaitForSeconds(checkInterval);
        }
    }
}
