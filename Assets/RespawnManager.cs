using UnityEngine;
using System;

public class RespawnManager : MonoBehaviour
{
    [Header("Respawn Settings")]
    public Transform playerTransform;
    public float respawnYThreshold = -10f;

    private Vector3 respawnPoint;
    private Rigidbody playerRigidbody;
    private FallingPlatform[] allFallingPlatforms; // Массив для хранения всех платформ

    // Событие, которое будет вызываться при респавне игрока
    public static event Action OnPlayerRespawn;

    void Awake()
    {
        if (playerTransform == null)
        {
            Debug.LogError("Player Transform is not assigned in the RespawnManager. Disabling script.");
            enabled = false;
            return;
        }

        respawnPoint = playerTransform.position;
        playerRigidbody = playerTransform.GetComponent<Rigidbody>();
        if (playerRigidbody == null)
        {
            Debug.LogWarning("RespawnManager could not find a Rigidbody on the player. Velocity will not be reset on respawn.");
        }
    }

    void Start()
    {
        // Находим все платформы один раз при старте и сохраняем их
        allFallingPlatforms = FindObjectsByType<FallingPlatform>(FindObjectsSortMode.None);
    }

    void FixedUpdate()
    {
        if (playerTransform.position.y < respawnYThreshold)
        {
            Respawn();
        }
    }

    public void SetRespawnPoint(Vector3 newPoint)
    {
        respawnPoint = newPoint;
    }

    void Respawn()
    {
        // 1. Сначала восстанавливаем все платформы из нашего массива
        foreach (FallingPlatform platform in allFallingPlatforms)
        {
            if (platform != null)
            {
                platform.ResetPlatform();
            }
        }

        // 2. Теперь возрождаем игрока
        playerTransform.position = respawnPoint;
        if (playerRigidbody != null)
        {
            playerRigidbody.linearVelocity = Vector3.zero;
            playerRigidbody.angularVelocity = Vector3.zero;
        }

        // 3. Вызываем событие, сообщая всем, что игрок респавнился
        OnPlayerRespawn?.Invoke();
    }
}
