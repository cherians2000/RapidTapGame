using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class PrefabSpawner : MonoBehaviour
{
    public List<GameObject> prefabList;
    public int initialPoolSize = 5;
    public float minLifetime = 2f;
    public float maxLifetime = 10f;
    public RectTransform spawnAreaReference;
    public Canvas parentCanvas;
    public float spawnCooldown = 1f;
    public int spawnCount = 1;
    public int maxActivePrefabs = 5;

    private Queue<GameObject> availablePrefabs = new Queue<GameObject>();
    private List<GameObject> spawnedPrefabs = new List<GameObject>();
    private bool canSpawn = true;

    AudioSource audioSource;
    public AudioClip smasheffect;
    void Start()
    {
        InitializePool();
        SpawnRandomPrefabs(initialPoolSize);

        CountdownTimer timer = FindObjectOfType<CountdownTimer>();
        if (timer != null)
        {
            timer.onTimeUp.AddListener(StopSpawning);
        }
        audioSource = GetComponent<AudioSource>();

    }

    void InitializePool()
    {
        foreach (var prefab in prefabList)
        {
            for (int i = 0; i < initialPoolSize; i++)
            {
                GameObject instance = Instantiate(prefab, parentCanvas.transform);
                instance.SetActive(false);
                availablePrefabs.Enqueue(instance);
            }
        }
    }

    void SpawnRandomPrefabs(int count)
    {
        int maxSpawnCount = Mathf.Min(count, availablePrefabs.Count, maxActivePrefabs - spawnedPrefabs.Count);

        if (maxSpawnCount <= 0)
        {
            return;
        }

        for (int i = 0; i < maxSpawnCount; i++)
        {
            GameObject prefabToSpawn = availablePrefabs.Dequeue();
            ActivatePrefab(prefabToSpawn);
        }
    }

    private void ActivatePrefab(GameObject prefab)
    {
        prefab.SetActive(true);
        Vector2 randomPosition = GetRandomPositionWithinRectTransform();
        RectTransform prefabRectTransform = prefab.GetComponent<RectTransform>();

        if (prefabRectTransform != null)
        {
            prefabRectTransform.anchoredPosition = randomPosition;
        }

        prefab.transform.localScale = Vector3.zero;
        prefab.transform.DOScale(Vector3.one, 1f).SetEase(Ease.OutBack);

        float lifetime = Random.Range(minLifetime, maxLifetime);
        StartCoroutine(HandlePrefabLifetime(prefab, lifetime));

        Button prefabButton = prefab.GetComponentInChildren<Button>();
        if (prefabButton != null)
        {
            prefabButton.onClick.AddListener(() => OnPrefabClicked(prefab));
        }

        spawnedPrefabs.Add(prefab);
    }

    IEnumerator HandlePrefabLifetime(GameObject prefab, float lifetime)
    {
        yield return new WaitForSeconds(lifetime);
        DeactivatePrefab(prefab);
    }

    private void DeactivatePrefab(GameObject prefab)
    {
        prefab.SetActive(false);
        spawnedPrefabs.Remove(prefab);
        availablePrefabs.Enqueue(prefab);

        if (canSpawn)
        {
            StartCoroutine(SpawnCooldown());
        }
    }

    IEnumerator SpawnCooldown()
    {
        canSpawn = false;
        yield return new WaitForSeconds(spawnCooldown);
        canSpawn = true;
        SpawnRandomPrefabs(spawnCount);
    }

    Vector2 GetRandomPositionWithinRectTransform()
    {
        float width = spawnAreaReference.rect.width;
        float height = spawnAreaReference.rect.height;
        Vector2 pivot = spawnAreaReference.pivot;
        Vector2 anchoredPosition = spawnAreaReference.anchoredPosition;

        float randomX = Random.Range(-width * pivot.x, width * (1 - pivot.x));
        float randomY = Random.Range(-height * pivot.y, height * (1 - pivot.y));

        return new Vector2(randomX, randomY) + anchoredPosition;
    }

    public void OnPrefabClicked(GameObject clickedPrefab)
    {
        DeactivatePrefab(clickedPrefab);
        UIManager.Instance.UpdateScore();
        audioSource.PlayOneShot(smasheffect);
    }

    public void StopSpawning()
    {
        canSpawn = false;
        StopAllCoroutines();

        foreach (var prefab in spawnedPrefabs)
        {
            Button prefabButton = prefab.GetComponentInChildren<Button>();
            if (prefabButton != null)
            {
                prefabButton.interactable = false;
                prefab.SetActive(false);
            }
        }
    }
}
