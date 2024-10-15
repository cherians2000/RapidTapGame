using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;
using System.Collections.Generic;

public class Bomb : MonoBehaviour
{
    public GameObject prefab;
    public RectTransform uiContainer;
    public RectTransform BountaryReference;
    public float minSpawnTime = 1f;
    public float maxSpawnTime = 5f;
    public int initialPoolSize = 10;
    public int minSpawnCount = 1;
    public int maxSpawnCount = 5;
    public float bombLifetime = 3f;
    private Queue<GameObject> pool = new Queue<GameObject>();
    private Coroutine spawnCoroutine;
   [SerializeField] private CountdownTimer countdownTimer;

    AudioSource audioSource;
    public AudioClip bombeffect;

    private void Start()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            obj.transform.SetParent(uiContainer, false);
            pool.Enqueue(obj);
        }

        countdownTimer = FindObjectOfType<CountdownTimer>();
        if (countdownTimer != null)
        {
            spawnCoroutine = StartCoroutine(SpawnPrefabs());
        }

         audioSource = GetComponent<AudioSource>();
    }

    private IEnumerator SpawnPrefabs()
    {
        while (true)
        {
            int spawnCount = Random.Range(minSpawnCount, maxSpawnCount + 1);
            for (int i = 0; i < spawnCount; i++)
            {
                SpawnPrefab();
            }

            yield return new WaitForSeconds(Random.Range(minSpawnTime, maxSpawnTime));
        }
    }

    private void SpawnPrefab()
    {
        Vector2 containerSize = BountaryReference.rect.size;
        Vector3 spawnPosition = new Vector3(
            Random.Range(-containerSize.x / 2, containerSize.x / 2),
            Random.Range(-containerSize.y / 2, containerSize.y / 2),
            0);

        if (pool.Count > 0)
        {
            GameObject spawnedObject = pool.Dequeue();
            spawnedObject.transform.localPosition = spawnPosition;
            spawnedObject.SetActive(true);

            spawnedObject.transform.localScale = Vector3.zero;
            spawnedObject.transform.DOScale(Vector3.one, 1f).SetEase(Ease.OutBack);

            Button button = spawnedObject.GetComponent<Button>();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => OnButtonClick(spawnedObject));

            StartCoroutine(DestroyBombAfterLifetime(spawnedObject));
        }
    }

    private IEnumerator DestroyBombAfterLifetime(GameObject bomb)
    {
        yield return new WaitForSeconds(bombLifetime);
        ReturnToPool(bomb);
    }

    private void ReturnToPool(GameObject bomb)
    {
        bomb.SetActive(false);
        pool.Enqueue(bomb);
    }

    private void OnButtonClick(GameObject button)
    {
        Debug.Log("Bomb clicked, stopping the game.");

        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;

            if (countdownTimer != null)
            {
                countdownTimer.StopCountdown();
                countdownTimer.onTimeUp.Invoke();
                OnGameOverScript.isClicked = true;
                audioSource.PlayOneShot(bombeffect);
            }
        }

        ReturnToPool(button);
    }

    public void StopSpawning()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }

        foreach (var bomb in pool)
        {
            if (bomb.activeInHierarchy)
            {
                ReturnToPool(bomb);
            }
        }
    }
}
