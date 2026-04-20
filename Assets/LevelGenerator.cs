using UnityEngine;
using System.Collections.Generic;

// Класс для удобной настройки префабов в инспекторе
[System.Serializable]
public class PlatformType
{
    public GameObject prefab;
    [Range(0, 1)] public float spawnChance = 1.0f; // Шанс появления этой платформы
}

public class LevelGenerator : MonoBehaviour
{
    [Header("Level Settings")]
    public int numberOfPlatforms = 50;
    public Vector3 startPosition = new Vector3(0, 0, 20f);
    public Vector3 platformOffset = new Vector3(0, 0, 10);

    [Header("Platform Types")]
    public List<PlatformType> platformTypes;

    [Header("Platform Behavior Chances")]
    [Range(0, 1)] public float fallingPlatformChance = 0.1f;
    [Range(0, 1)] public float movingPlatformChance = 0.15f;

    [Header("Collectibles")]
    public GameObject coinPrefab;
    public Vector3 coinOffset = new Vector3(0, 2, 0);
    [Range(0, 1)] public float coinSpawnChance = 0.5f;

    void Awake()
    {
        if (platformTypes == null || platformTypes.Count == 0)
        {
            Debug.LogError("Platform Types list is not set up in the LevelGenerator. Disabling script.");
            enabled = false;
            return;
        }

        GenerateLevel();
    }

    void GenerateLevel()
    {
        Vector3 currentPosition = startPosition;
        
        for (int i = 0; i < numberOfPlatforms; i++)
        {
            PlatformType selectedType = GetRandomPlatformType();
            if (selectedType == null) continue;

            GameObject platformInstance = Instantiate(selectedType.prefab, currentPosition, Quaternion.identity, transform);
            
            Rigidbody rb = platformInstance.GetComponent<Rigidbody>();
            float randomValue = Random.value;

            if (randomValue < fallingPlatformChance)
            {
                if (rb == null) { rb = platformInstance.AddComponent<Rigidbody>(); }
                platformInstance.AddComponent<FallingPlatform>();
            }
            else if (randomValue < fallingPlatformChance + movingPlatformChance)
            {
                if (rb == null) { rb = platformInstance.AddComponent<Rigidbody>(); }
                rb.isKinematic = true;
                platformInstance.AddComponent<MovingPlatform>();
            }
            else
            {
                if (rb != null) { rb.isKinematic = true; }
            }

            if (coinPrefab != null && Random.value < coinSpawnChance)
            {
                Instantiate(coinPrefab, currentPosition + coinOffset, Quaternion.identity, transform);
            }

            currentPosition += platformOffset;
        }
    }

    PlatformType GetRandomPlatformType()
    {
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
}
