using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class FallingPlatform : MonoBehaviour
{
    public float fallDelay = 0.5f;

    private Rigidbody rb;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private bool isTriggered = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        rb.isKinematic = true; // Платформа изначально "заморожена"
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isTriggered && collision.gameObject.CompareTag("Player"))
        {
            isTriggered = true;
            StartCoroutine(FallAfterDelay());
        }
    }

    private IEnumerator FallAfterDelay()
    {
        yield return new WaitForSeconds(fallDelay);

        rb.isKinematic = false; // "Отпускаем" платформу

        // Вместо уничтожения, деактивируем платформу через 5 секунд
        yield return new WaitForSeconds(5f);
        gameObject.SetActive(false);
    }

    public void ResetPlatform()
    {
        // 1. Останавливаем все запущенные корутины.
        StopAllCoroutines();

        // 2. Активируем объект.
        gameObject.SetActive(true);

        // 3. Возвращаем на исходную позицию.
        transform.position = originalPosition;
        transform.rotation = originalRotation;

        // 4. Сбрасываем физическое состояние в ПРАВИЛЬНОМ ПОРЯДКЕ.
        // Сначала убеждаемся, что тело НЕ кинематическое, чтобы можно было сбросить скорости.
        rb.isKinematic = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        // Теперь, когда скорости сброшены, делаем тело кинематическим, чтобы оно замерло на месте.
        rb.isKinematic = true;

        // 5. Сбрасываем триггер.
        isTriggered = false;
    }
}
