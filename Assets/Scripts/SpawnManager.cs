using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public Transform[] spawnPoints;
    public GameObject[] objectsToSpawn;

    void Start()
    {
        List<int> used = new List<int>();

        foreach (GameObject obj in objectsToSpawn)
        {
            int index;

            do
            {
                index = Random.Range(0, spawnPoints.Length);
            }
            while (used.Contains(index));

            used.Add(index);

            Instantiate(obj,
                spawnPoints[index].position,
                spawnPoints[index].rotation);
        }
    }
}