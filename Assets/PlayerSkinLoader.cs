using UnityEngine;
using System.Collections.Generic;

public class PlayerSkinLoader : MonoBehaviour
{
    [Header("Skins")]
    public List<CharacterSkin> skins;

    [Header("Player Settings")]
    public Transform skinContainer;

    private const string SelectedSkinKey = "SelectedSkinIndex";

    void Awake()
    {
        LoadPlayerSkin();
    }

    private void LoadPlayerSkin()
    {
        int defaultSkinIndex = GetDefaultSkinIndex();
        int selectedSkinIndex = PlayerPrefs.GetInt(SelectedSkinKey, defaultSkinIndex);

        // --- ОТЛАДОЧНОЕ СООБЩЕНИЕ ---
        Debug.Log($"[PlayerSkinLoader] Загружен индекс скина: {selectedSkinIndex}. (Дефолтный: {defaultSkinIndex})");

        if (selectedSkinIndex >= 0 && selectedSkinIndex < skins.Count)
        {
            CharacterSkin selectedSkin = skins[selectedSkinIndex];
            
            foreach (Transform child in skinContainer)
            {
                Destroy(child.gameObject);
            }

            Instantiate(selectedSkin.skinPrefab, skinContainer.position, skinContainer.rotation, skinContainer);
        }
        else
        {
            Debug.LogError($"[PlayerSkinLoader] Ошибка! Индекс {selectedSkinIndex} вне диапазона. Загружаем дефолтный скин.");
            // Загружаем дефолтный скин в случае ошибки
            if (defaultSkinIndex >= 0 && defaultSkinIndex < skins.Count)
            {
                Instantiate(skins[defaultSkinIndex].skinPrefab, skinContainer.position, skinContainer.rotation, skinContainer);
            }
        }
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
