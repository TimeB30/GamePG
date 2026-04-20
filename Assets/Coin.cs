using UnityEngine;

public class Coin : MonoBehaviour
{
    // Этот метод вызывается, когда другой коллайдер входит в триггер-зону монеты.
    // Чтобы он сработал, у монеты должен быть коллайдер с галочкой "Is Trigger".
    private void OnTriggerEnter(Collider other)
    {
        // Проверяем, что объект, который вошел в триггер, имеет тег "Player".
        if (other.CompareTag("Player"))
        {
            // Если это игрок, "собираем" монету.
            Collect();
        }
    }

    private void Collect()
    {
        // В будущем здесь можно добавить звук, эффекты или увеличение счета.
        // Сейчас мы просто выводим сообщение в консоль для проверки.
        Debug.Log("Coin collected!");

        // Уничтожаем игровой объект монеты.
        Destroy(gameObject);
    }
}
