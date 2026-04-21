using UnityEngine;
using TMPro; // Убедитесь, что у вас импортирован TextMesh Pro

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public TextMeshProUGUI coinText; // Перетащите сюда ваш UI-текст для монет из инспектора
    private int totalCoins;

    private const string CoinsSaveKey = "TotalCoins";

    void Awake()
    {
        // Реализация синглтона
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Делаем GameManager постоянным между сценами
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Загружаем монеты при старте
        LoadCoins();
    }

    void Start()
    {
        UpdateCoinText();
    }

    public void AddCoin(int amount)
    {
        totalCoins += amount;
        UpdateCoinText();
        // Не сохраняем здесь, чтобы не обращаться к диску слишком часто.
        // Сохранение будет при выходе.
    }

    private void UpdateCoinText()
    {
        if (coinText != null)
        {
            coinText.text = "Coins: " + totalCoins;
        }
        else
        {
            Debug.LogWarning("Coin Text UI is not assigned in GameManager!");
        }
    }

    private void LoadCoins()
    {
        totalCoins = PlayerPrefs.GetInt(CoinsSaveKey, 0); // Загружаем, по умолчанию 0
    }

    private void SaveCoins()
    {
        PlayerPrefs.SetInt(CoinsSaveKey, totalCoins);
        PlayerPrefs.Save(); // Физически сохраняем данные на диск
        Debug.Log("Coins saved!");
    }

    // Этот метод вызывается автоматически, когда игра закрывается
    private void OnApplicationQuit()
    {
        SaveCoins();
    }
}
