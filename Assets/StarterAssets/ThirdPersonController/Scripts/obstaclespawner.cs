using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class obstaclespawner : MonoBehaviour
{
    public GameObject[] obstaclePrefabs;
    public int numberOfObstacles = 10;
    public float spacing = 8f;
    public float startZ = 0f;

#if UNITY_EDITOR
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
                pos = new Vector3(x, clone.transform.position.y, startZ + i * spacing);
            }
            else if (prefabName.Contains("slide"))
            {
                pos = new Vector3(0f, clone.transform.position.y, startZ + i * spacing);
            }
            else if (prefabName.Contains("jump"))
            {
                pos = new Vector3(0f, 0.7f, startZ + i * spacing);
            }
            else
            {
                DestroyImmediate(clone);
                continue;
            }

            clone.transform.position = pos;
            Undo.RegisterCreatedObjectUndo(clone, "Spawn Obstacle");
        }
    }
#endif

    void Start()
    {
        if (SceneManager.GetActiveScene().name == "RandomStage")
        {
            GenerateRandomCourse(); 
        }
    }

    public void GenerateRandomCourse()
    {
        Debug.Log("GenerateRandomCourse() called!");
        Debug.Log($"Generating {numberOfObstacles} obstacles starting at Z={startZ}, spacing={spacing}");

        string lastType = "";
        int sameTypeCount = 0;
        int lastDodgeLaneIndex = -1;

        for (int i = 0; i < numberOfObstacles; i++)
        {
            GameObject prefabToSpawn = null;
            string prefabName = "";

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

            GameObject clone = Instantiate(prefabToSpawn);
            Vector3 pos;

            float[] laneX = { -1.2f, 0f, 1.2f };

            if (prefabName.Contains("dodge"))
            {
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
                pos = new Vector3(x, clone.transform.position.y, startZ + i * spacing);
            }
            else if (prefabName.Contains("slide"))
            {
                pos = new Vector3(0f, clone.transform.position.y, startZ + i * spacing);
            }
            else if (prefabName.Contains("jump"))
            {
                pos = new Vector3(0f, 0.7f, startZ + i * spacing);
            }
            else
            {
                Destroy(clone); 
                continue;
            }

            clone.transform.position = pos;
            Debug.Log($" Obstacle {i + 1}: {prefabName}");
        }
    }


}
