using UnityEngine;

public class MeteorSpawnerUI : MonoBehaviour
{
    public RectTransform parentArea;
    public GameObject meteorTemplate;
    public float spawnInterval = 1.5f;

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnMeteor();
            timer = 0f;
        }
    }

    void SpawnMeteor()
    {
        GameObject meteor = Instantiate(meteorTemplate, parentArea);
        meteor.SetActive(true);

        RectTransform rt = meteor.GetComponent<RectTransform>();

        float randomX = Random.Range(-1200f, 850f);
        float randomY = Random.Range(480f, 720f);

        rt.anchoredPosition = new Vector2(randomX, randomY);
        rt.localScale = Vector3.one * 0.35f;
    }
}