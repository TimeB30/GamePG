using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float moveDistance = 5f;

    private Vector3 startPosition;
    private Vector3 lastFixedUpdatePosition; // Позиция платформы в начале FixedUpdate
    private Rigidbody platformRb; // Rigidbody самой платформы

    void Awake()
    {
        startPosition = transform.position;
        platformRb = GetComponent<Rigidbody>();
        if (platformRb == null)
        {
            Debug.LogError("MovingPlatform requires a Rigidbody component on itself.");
            enabled = false;
            return;
        }
        platformRb.isKinematic = true; // Убеждаемся, что платформа кинематическая
        lastFixedUpdatePosition = transform.position; // Инициализируем
    }

    void FixedUpdate()
    {
        // Сохраняем текущую позицию перед перемещением для расчета дельты
        lastFixedUpdatePosition = transform.position;

        // Вычисляем целевую позицию
        float xOffset = Mathf.Sin(Time.time * moveSpeed) * moveDistance;
        Vector3 targetPosition = new Vector3(startPosition.x + xOffset, startPosition.y, startPosition.z);

        // Перемещаем платформу, используя Rigidbody.MovePosition
        platformRb.MovePosition(targetPosition);
    }

    
}
