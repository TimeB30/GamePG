using UnityEngine;
using UnityEngine.SceneManagement; // 1. Добавляем для работы со сценами
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // Теперь это поле будет находиться автоматически
    private TextMeshProUGUI coinText; 
    private int totalCoins;

    private const string CoinsSaveKey = "TotalCoins";

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        LoadCoins();
    }

    // 2. Подписываемся на событие загрузки сцены
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // 3. Отписываемся, чтобы избежать утечек памяти
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // 4. Этот метод будет вызываться КАЖДЫЙ РАЗ, когда загружается новая сцена
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Ищем объект с тегом "CoinCounter" в новой сцене
        GameObject coinTextObject = GameObject.FindWithTag("CoinCounter");
        if (coinTextObject != null)
        {
            coinText = coinTextObject.GetComponent<TextMeshProUGUI>();
            Debug.Log("Coin counter found in the new scene!");
        }
        else
        {
            coinText = null; // Если не нашли, сбрасываем ссылку
            Debug.LogWarning("Could not find a GameObject with the 'CoinCounter' tag in the new scene.");
        }
        
        // Сразу обновляем текст
        UpdateCoinText();
    }

    public void AddCoin(int amount)
    {
        totalCoins += amount;
        UpdateCoinText();
    }

    public bool CanSpendCoins(int amount)
    {
        return totalCoins >= amount;
    }

    public void SpendCoins(int amount)
    {
        if (CanSpendCoins(amount))
        {
            totalCoins -= amount;
            UpdateCoinText();
        }
    }

    public int GetCurrentCoins()
    {
        return totalCoins;
    }

    private void UpdateCoinText()
    {
        if (coinText != null)
        {
            coinText.text = "Coins: " + totalCoins;
        }
    }

    private void LoadCoins()
    {
        totalCoins = PlayerPrefs.GetInt(CoinsSaveKey, 0);
    }

    private void SaveCoins()
    {
        PlayerPrefs.SetInt(CoinsSaveKey, totalCoins);
        PlayerPrefs.Save();
        Debug.Log("Coins saved!");
    }

    private void OnApplicationQuit()
    {
        SaveCoins();
    }
}
