using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class SkinShopManager : MonoBehaviour
{
    [Header("Skins")]
    public List<CharacterSkin> skins; // Список всех доступных скинов (перетащите сюда ваши ассеты скинов)

    [Header("UI Elements")]
    public TextMeshProUGUI skinNameText;
    public TextMeshProUGUI coinText; // Для отображения текущего баланса
    public GameObject buyButton;
    public TextMeshProUGUI buyButtonText;
    public GameObject selectButton;
    public GameObject selectedButton;

    [Header("Shop Settings")]
    public Transform modelSpawnPoint; // Точка, где будет появляться моделька персонажа
    public float rotationSpeed = 50f;

    private int currentSkinIndex = 0;
    private GameObject currentModelInstance;

    private const string SelectedSkinKey = "SelectedSkinIndex";
    private const string UnlockedSkinKeyPrefix = "SkinUnlocked_";

    void Start()
    {
        // Загружаем выбранный скин или устанавливаем по умолчанию
        currentSkinIndex = PlayerPrefs.GetInt(SelectedSkinKey, GetDefaultSkinIndex());
        
        SpawnModel();
        UpdateUI();
        UpdateCoinText();
    }

    void Update()
    {
        // Вращаем модельку
        if (currentModelInstance != null)
        {
            currentModelInstance.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }
    }

    public void NextSkin()
    {
        currentSkinIndex++;
        if (currentSkinIndex >= skins.Count)
        {
            currentSkinIndex = 0; // Циклический переход к первому скину
        }
        SpawnModel();
        UpdateUI();
    }

    public void BuySkin()
    {
        CharacterSkin currentSkin = skins[currentSkinIndex];
        if (GameManager.Instance.CanSpendCoins(currentSkin.price))
        {
            GameManager.Instance.SpendCoins(currentSkin.price);
            UnlockSkin(currentSkinIndex);
            UpdateUI();
            UpdateCoinText();
        }
        else
        {
            Debug.Log("Not enough coins!");
            // Здесь можно добавить анимацию дрожания кнопки или звук ошибки
        }
    }

    public void SelectSkin()
    {
        PlayerPrefs.SetInt(SelectedSkinKey, currentSkinIndex);
        UpdateUI();
    }

    private void SpawnModel()
    {
        if (currentModelInstance != null)
        {
            Destroy(currentModelInstance);
        }

        if (modelSpawnPoint != null && skins.Count > 0)
        {
            CharacterSkin skin = skins[currentSkinIndex];
            
            // Создаем нужную ротацию: ротация точки спавна + поворот на 180 градусов по оси Y
            Quaternion initialRotation = modelSpawnPoint.rotation * Quaternion.Euler(0, 180f, 0);
            
            currentModelInstance = Instantiate(skin.skinPrefab, modelSpawnPoint.position, initialRotation, modelSpawnPoint);
        }
    }

    private void UpdateUI()
    {
        CharacterSkin currentSkin = skins[currentSkinIndex];
        skinNameText.text = currentSkin.skinName;

        bool isUnlocked = IsSkinUnlocked(currentSkinIndex);
        bool isSelected = (currentSkinIndex == PlayerPrefs.GetInt(SelectedSkinKey, GetDefaultSkinIndex()));

        if (isUnlocked)
        {
            buyButton.SetActive(false);
            if (isSelected)
            {
                selectButton.SetActive(false);
                selectedButton.SetActive(true);
            }
            else
            {
                selectButton.SetActive(true);
                selectedButton.SetActive(false);
            }
        }
        else
        {
            buyButton.SetActive(true);
            buyButtonText.text = "Buy: " + currentSkin.price;
            selectButton.SetActive(false);
            selectedButton.SetActive(false);
        }
    }

    private void UpdateCoinText()
    {
        if (GameManager.Instance != null && coinText != null)
        {
            coinText.text = "Coins: " + GameManager.Instance.GetCurrentCoins();
        }
    }

    private bool IsSkinUnlocked(int index)
    {
        if (skins[index].isDefault)
        {
            return true;
        }
        return PlayerPrefs.GetInt(UnlockedSkinKeyPrefix + index, 0) == 1;
    }

    private void UnlockSkin(int index)
    {
        PlayerPrefs.SetInt(UnlockedSkinKeyPrefix + index, 1);
    }

    private int GetDefaultSkinIndex()
    {
        for (int i = 0; i < skins.Count; i++)
        {
            if (skins[i].isDefault) return i;
        }
        return 0;
    }
}
