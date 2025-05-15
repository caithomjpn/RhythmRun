#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class obstaclespawner : MonoBehaviour
{
    public GameObject[] obstaclePrefabs;
    public int numberOfObstacles = 10;
    public float spacing = 8f;
    public float startZ = 0f;

    [ContextMenu("Spawn Obstacles")]
    void SpawnObstacles()
    {
        string lastType = "";
        int sameTypeCount = 0;
        int lastDodgeLaneIndex = -1;

        for (int i = 0; i < numberOfObstacles; i++)
        {
            GameObject prefabToSpawn;
            string prefabName;

            // Prevent repeating the same obstacle type more than twice
            int attempts = 0;
            do
            {
                prefabToSpawn = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];
                prefabName = prefabToSpawn.name.ToLower();
                attempts++;
            }
            while (attempts < 5 && prefabName == lastType && sameTypeCount >= 2);

            if (prefabName == lastType)
                sameTypeCount++;
            else
            {
                lastType = prefabName;
                sameTypeCount = 1;
            }

            GameObject clone = (GameObject)PrefabUtility.InstantiatePrefab(prefabToSpawn);
            Vector3 pos;

            float[] laneX = { -1.2f, 0f, 1.2f };

            if (prefabName.Contains("dodge"))
            {
                // Pick a lane different from lastDodgeLaneIndex
                int laneIndex;
                int loopCount = 0;
                do
                {
                    laneIndex = Random.Range(0, laneX.Length);
                    loopCount++;
                }
                while (laneIndex == lastDodgeLaneIndex && loopCount < 10);

                lastDodgeLaneIndex = laneIndex;
                float x = laneX[laneIndex];
                pos = new Vector3(x, 1.26f, startZ + i * spacing);
            }
            else if (prefabName.Contains("slide"))
            {
                // Keep original size and place in center
                pos = new Vector3(0f, 1f, startZ + i * spacing);
            }
            else if (prefabName.Contains("jump"))
            {
                // Place in center, optional size tweak
                pos = new Vector3(0f, 1f, startZ + i * spacing);
                clone.transform.localScale = new Vector3(1f, 1f, 2f); // adjust if needed
            }
            else
            {
                // Default center lane for any unknown type
                pos = new Vector3(0f, 1f, startZ + i * spacing);
            }

            clone.transform.position = pos;
            Undo.RegisterCreatedObjectUndo(clone, "Spawn Obstacle");
        }
    }
}
#endif
