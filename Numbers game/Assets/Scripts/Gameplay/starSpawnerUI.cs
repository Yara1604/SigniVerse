using UnityEngine;

public class starSpawnerUI : MonoBehaviour
{
    public GameObject starPrefab;
    public float spawnRate = 1f;
    public float spawnXRange = 300f;
    public float spawnY = 500f;

    public RectTransform canvasRect;

    void Start()
    {
        InvokeRepeating("SpawnStar", 0f, spawnRate);
    }

    void SpawnStar()
    {
        float randomX = Random.Range(-spawnXRange, spawnXRange);

        GameObject star = Instantiate(starPrefab, canvasRect);
        RectTransform rect = star.GetComponent<RectTransform>();

        rect.anchoredPosition = new Vector2(randomX, spawnY);
    }
}