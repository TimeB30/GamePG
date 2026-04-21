using UnityEngine;

public class Coin : MonoBehaviour
{
    public int coinValue = 1; // Количество очков за монету

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Collect();
        }
    }

    private void Collect()
    {
        // Проверяем, существует ли GameManager
        if (GameManager.Instance != null)
        {
            // Сообщаем GameManager'у, что нужно добавить монету
            GameManager.Instance.AddCoin(coinValue);
        }
        else
        {
            Debug.LogError("GameManager not found in the scene!");
        }

        // Уничтожаем игровой объект монеты
        Destroy(gameObject);
    }
}
