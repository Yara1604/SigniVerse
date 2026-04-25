using UnityEngine;

public class StarSpawner : MonoBehaviour
{
    public GameObject starPrefab;
    public float spawnRate = 1f;
    public float spawnXRange = 8f;
    public float spawnY = 6f;

    void Start()
    {
        InvokeRepeating("SpawnStar", 0f, spawnRate);
    }

    void SpawnStar()
    {
        float randomX = Random.Range(-spawnXRange, spawnXRange);
        Vector3 spawnPos = new Vector3(randomX, spawnY, 0);

        Instantiate(starPrefab, spawnPos, Quaternion.identity);
       
    }
}