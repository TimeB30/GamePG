using UnityEngine;
using System.Collections.Generic;

// Класс для удобной настройки префабов в инспекторе
[System.Serializable]
public class PlatformType
{
    public GameObject prefab;
    [Range(0, 1)] public float spawnChance = 1.0f; // Шанс появления этой платформы

    [Header("Behavior Chances for this Platform Type")]
    [Range(0, 1)] public float fallingChance = 0.0f; // Шанс, что эта платформа будет падающей
    [Range(0, 1)] public float movingChance = 0.0f;  // Шанс, что эта платформа будет движущейся
}

public class LevelGenerator : MonoBehaviour
{
    [Header("Level Settings")]
    public int numberOfPlatforms = 50;
    public Vector3 startPosition = Vector3.zero;
    [Tooltip("X and Y are direct offsets. Z is the minimum gap between platforms.")]
    public Vector3 platformOffset = new Vector3(0, 0, 2); 

    [Header("Start/Finish Platforms")]
    public GameObject startPlatformPrefab;
    public GameObject finishPlatformPrefab;

    [Header("Platform Types")]
    public List<PlatformType> platformTypes;

    [Header("Collectibles")]
    public GameObject coinPrefab;
    [Tooltip("Offset from the top center of the platform.")]
    public Vector3 coinOffset = new Vector3(0, 1, 0);
    [Range(0, 1)] public float coinSpawnChance = 0.5f;

    void Awake()
    {
        if (platformTypes == null || platformTypes.Count == 0 && numberOfPlatforms > 0)
        {
            Debug.LogError("Platform Types list is not set up in the LevelGenerator. Disabling script.");
            enabled = false;
            return;
        }

        GenerateLevel();
    }

    void GenerateLevel()
    {
        Vector3 nextPlatformStartPosition = startPosition;
        GameObject lastPlatform = null;

        // 1. Создаем стартовую платформу
        if (startPlatformPrefab != null)
        {
            lastPlatform = Instantiate(startPlatformPrefab, nextPlatformStartPosition, Quaternion.identity, transform);
            Collider collider = GetCollider(lastPlatform);
            if (collider != null)
            {
                nextPlatformStartPosition.z = collider.bounds.max.z + platformOffset.z;
                nextPlatformStartPosition.x += platformOffset.x;
                nextPlatformStartPosition.y += platformOffset.y;
            }
        }
        else
        {
            Debug.LogWarning("Start Platform Prefab is not assigned.");
        }

        // 2. Генерируем промежуточные платформы
        for (int i = 0; i < numberOfPlatforms; i++)
        {
            PlatformType selectedType = GetRandomPlatformType();
            if (selectedType == null) continue;

            Collider prefabCollider = GetCollider(selectedType.prefab);
            if (prefabCollider == null)
            {
                Debug.LogError($"Platform prefab '{selectedType.prefab.name}' has no Collider component. Skipping platform.", selectedType.prefab);
                continue;
            }

            float halfZ = prefabCollider.bounds.extents.z;
            Vector3 newPlatformCenter = nextPlatformStartPosition;
            newPlatformCenter.z += halfZ;

            GameObject platformInstance = Instantiate(selectedType.prefab, newPlatformCenter, Quaternion.identity, transform);
            lastPlatform = platformInstance;

            AddPlatformBehavior(platformInstance, selectedType);

            Collider instanceCollider = GetCollider(platformInstance);
            if (instanceCollider != null)
            {
                SpawnCoin(platformInstance, instanceCollider);

                nextPlatformStartPosition.z = instanceCollider.bounds.max.z + platformOffset.z;
                nextPlatformStartPosition.x += platformOffset.x;
                nextPlatformStartPosition.y += platformOffset.y;
            }
        }

        // 3. Создаем финишную платформу
        if (finishPlatformPrefab != null)
        {
            Collider prefabCollider = GetCollider(finishPlatformPrefab);
            if (prefabCollider != null)
            {
                float halfZ = prefabCollider.bounds.extents.z;
                Vector3 finishPlatformCenter = nextPlatformStartPosition;
                finishPlatformCenter.z += halfZ;

                Instantiate(finishPlatformPrefab, finishPlatformCenter, Quaternion.identity, transform);
            }
        }
    }

    void AddPlatformBehavior(GameObject platformInstance, PlatformType selectedType)
    {
        Rigidbody rb = platformInstance.GetComponent<Rigidbody>();
        float randomValue = Random.value;

        if (randomValue < selectedType.fallingChance)
        {
            if (rb == null) { rb = platformInstance.AddComponent<Rigidbody>(); }
            rb.isKinematic = false;
            platformInstance.AddComponent<FallingPlatform>();
        }
        else if (randomValue < selectedType.fallingChance + selectedType.movingChance)
        {
            if (rb == null) { rb = platformInstance.AddComponent<Rigidbody>(); }
            rb.isKinematic = true;
            platformInstance.AddComponent<MovingPlatform>();
        }
        else
        {
            if (rb != null) { rb.isKinematic = true; }
        }
    }

    void SpawnCoin(GameObject platformInstance, Collider platformCollider)
    {
        if (coinPrefab != null && Random.value < coinSpawnChance)
        {
            Vector3 coinBasePosition = new Vector3(
                platformInstance.transform.position.x,
                platformCollider.bounds.max.y,
                platformInstance.transform.position.z
            );
            Instantiate(coinPrefab, coinBasePosition + coinOffset, Quaternion.identity, transform);
        }
    }

    PlatformType GetRandomPlatformType()
    {
        if (platformTypes == null || platformTypes.Count == 0) return null;

        float totalChance = 0;
        foreach (var type in platformTypes)
        {
            totalChance += type.spawnChance;
        }

        float randomValue = Random.Range(0, totalChance);
        float cumulativeChance = 0;

        foreach (var type in platformTypes)
        {
            cumulativeChance += type.spawnChance;
            if (randomValue < cumulativeChance)
            {
                return type;
            }
        }
        return null;
    }

    Collider GetCollider(GameObject obj)
    {
        Collider collider = obj.GetComponent<Collider>();
        if (collider == null)
        {
            collider = obj.GetComponentInChildren<Collider>();
        }
        return collider;
    }
}
