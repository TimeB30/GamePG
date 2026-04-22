using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class SkinShopManager : MonoBehaviour
{
    [Header("Skins")]
    public List<CharacterSkin> skins;

    [Header("UI Elements")]
    public TextMeshProUGUI skinNameText;
    public TextMeshProUGUI coinText;
    public GameObject buyButton;
    public TextMeshProUGUI buyButtonText;
    public GameObject selectButton;
    public GameObject selectedButton;

    [Header("Shop Settings")]
    public Transform modelSpawnPoint;
    public float rotationSpeed = 50f;

    private int currentSkinIndex = 0;
    private GameObject currentModelInstance;

    private const string SelectedSkinKey = "SelectedSkinIndex";
    private const string UnlockedSkinKeyPrefix = "SkinUnlocked_";

    void Start()
    {
        currentSkinIndex = PlayerPrefs.GetInt(SelectedSkinKey, GetDefaultSkinIndex());
        
        SpawnModel();
        UpdateUI();
        UpdateCoinText();
    }

    void Update()
    {
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
            currentSkinIndex = 0;
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
        }
    }

    public void SelectSkin()
    {
        PlayerPrefs.SetInt(SelectedSkinKey, currentSkinIndex);
        PlayerPrefs.Save(); 
        
        // --- ОТЛАДОЧНОЕ СООБЩЕНИЕ ---
        Debug.Log($"[SkinShopManager] Сохранен скин с индексом: {currentSkinIndex}");

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
        PlayerPrefs.Save();
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
