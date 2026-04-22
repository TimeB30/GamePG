using UnityEngine;
using System.Collections.Generic;

// 'Рецепт' для создания одного типа платформы.
// Мы можем создать много таких 'рецептов' в инспекторе.
[System.Serializable]
public class PlatformType
{
    public GameObject prefab;
    [Range(0, 1)] public float spawnChance = 1.0f;

    [Header("Каким будет поведение у этой платформы?")]
    [Range(0, 1)] public float fallingChance = 0.0f; // Шанс, что она станет падающей
    [Range(0, 1)] public float movingChance = 0.0f;  // Шанс, что она станет движущейся
}

// Этот скрипт строит весь уровень из кусочков при старте.
public class LevelGenerator : MonoBehaviour
{
    [Header("Общие настройки уровня")]
    public int numberOfPlatforms = 50; // Сколько всего платформ будет в уровне
    public Vector3 startPosition = Vector3.zero; // Где начнется генерация
    public Vector3 platformOffset = new Vector3(0, 0, 2); // Минимальный зазор между платформами

    [Header("Особые платформы")]
    public GameObject startPlatformPrefab; // С чего начинаем
    public GameObject finishPlatformPrefab; // Чем заканчиваем
    public GameObject finishFlagPrefab; // Что стоит на финише

    [Header("Список 'рецептов' платформ")]
    public List<PlatformType> platformTypes;

    [Header("Монетки")]
    public GameObject coinPrefab;
    public Vector3 coinOffset = new Vector3(0, 1, 0); // Насколько высоко над платформой появится монетка
    [Range(0, 1)] public float coinSpawnChance = 0.5f;

    void Awake()
    {
        // Если забыли указать, какие платформы использовать, лучше не начинать.
        if (platformTypes == null || platformTypes.Count == 0 && numberOfPlatforms > 0)
        {
            Debug.LogError("Не указаны типы платформ в LevelGenerator! Генерация отменена.");
            enabled = false;
            return;
        }

        GenerateLevel();
    }

    void GenerateLevel()
    {
        // Эта точка хранит, где должен начаться край следующей платформы.
        Vector3 nextPlatformStartPosition = startPosition;
        
        // Сначала ставим стартовую платформу, если она есть.
        if (startPlatformPrefab != null)
        {
            GameObject startPlatform = Instantiate(startPlatformPrefab, nextPlatformStartPosition, Quaternion.identity, transform);
            Collider collider = GetCollider(startPlatform);
            if (collider != null)
            {
                // Сдвигаем точку старта для следующей платформы за край этой.
                nextPlatformStartPosition.z = collider.bounds.max.z + platformOffset.z;
                nextPlatformStartPosition.x += platformOffset.x;
                nextPlatformStartPosition.y += platformOffset.y;
            }
        }

        // Теперь в цикле создаем все промежуточные платформы.
        for (int i = 0; i < numberOfPlatforms; i++)
        {
            PlatformType selectedType = GetRandomPlatformType();
            if (selectedType == null) continue;

            Collider prefabCollider = GetCollider(selectedType.prefab);
            if (prefabCollider == null)
            {
                Debug.LogError($"У префаба '{selectedType.prefab.name}' нет коллайдера! Пропускаем.");
                continue;
            }

            // Чтобы правильно разместить платформу, нужно знать ее размеры.
            float halfZ = prefabCollider.bounds.extents.z;
            Vector3 newPlatformCenter = nextPlatformStartPosition;
            newPlatformCenter.z += halfZ; // Сдвигаем pivot в центр.

            GameObject platformInstance = Instantiate(selectedType.prefab, newPlatformCenter, Quaternion.identity, transform);

            // Решаем, будет ли эта платформа двигаться или падать.
            AddPlatformBehavior(platformInstance, selectedType);

            // Теперь, когда платформа создана, можно поставить на нее монетку.
            Collider instanceCollider = GetCollider(platformInstance);
            if (instanceCollider != null)
            {
                SpawnCoin(platformInstance, instanceCollider);

                // И обновляем точку старта для следующей платформы.
                nextPlatformStartPosition.z = instanceCollider.bounds.max.z + platformOffset.z;
                nextPlatformStartPosition.x += platformOffset.x;
                nextPlatformStartPosition.y += platformOffset.y;
            }
        }

        // В самом конце ставим финишную платформу.
        if (finishPlatformPrefab != null)
        {
            Collider prefabCollider = GetCollider(finishPlatformPrefab);
            if (prefabCollider != null)
            {
                float halfZ = prefabCollider.bounds.extents.z;
                Vector3 finishPlatformCenter = nextPlatformStartPosition;
                finishPlatformCenter.z += halfZ;

                GameObject finishPlatform = Instantiate(finishPlatformPrefab, finishPlatformCenter, Quaternion.identity, transform);

                // И на нее ставим флажок.
                if (finishFlagPrefab != null)
                {
                    Collider finishCollider = GetCollider(finishPlatform);
                    Vector3 flagPosition = new Vector3(
                        finishPlatform.transform.position.x,
                        finishCollider.bounds.max.y, // Точно на верхнюю грань.
                        finishPlatform.transform.position.z
                    );
                    Instantiate(finishFlagPrefab, flagPosition, Quaternion.identity, transform); 
                }
            }
        }
    }

    // Добавляет платформам "характер" - делает их падающими или движущимися.
    void AddPlatformBehavior(GameObject platformInstance, PlatformType selectedType)
    {
        Rigidbody rb = platformInstance.GetComponent<Rigidbody>();
        float randomValue = Random.value; // Бросаем кубик от 0 до 1.

        if (randomValue < selectedType.fallingChance)
        {
            // Делаем платформу падающей.
            if (rb == null) { rb = platformInstance.AddComponent<Rigidbody>(); }
            rb.isKinematic = false; // Чтобы гравитация работала.
            platformInstance.AddComponent<FallingPlatform>();
        }
        else if (randomValue < selectedType.fallingChance + selectedType.movingChance)
        {
            // Делаем платформу движущейся.
            if (rb == null) { rb = platformInstance.AddComponent<Rigidbody>(); }
            rb.isKinematic = true; // Движением будет управлять скрипт, а не физика.
            platformInstance.AddComponent<MovingPlatform>();
        }
        else
        {
            // В противном случае платформа остается статичной.
            if (rb != null) { rb.isKinematic = true; }
        }
    }

    // Спавнит монетку на платформе, если повезет.
    void SpawnCoin(GameObject platformInstance, Collider platformCollider)
    {
        if (coinPrefab != null && Random.value < coinSpawnChance)
        {
            Vector3 coinPosition = new Vector3(
                platformInstance.transform.position.x, // По центру X
                platformCollider.bounds.max.y,         // На верхней грани
                platformInstance.transform.position.z  // По центру Z
            );
            Instantiate(coinPrefab, coinPosition + coinOffset, Quaternion.identity, transform);
        }
    }

    // Выбирает случайный тип платформы из списка, учитывая шансы спавна.
    // Работает как рулетка.
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

    // Утилита, чтобы найти коллайдер на объекте или его детях.
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
