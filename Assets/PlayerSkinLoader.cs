using UnityEngine;
using System.Collections.Generic;

public class PlayerSkinLoader : MonoBehaviour
{
    [Header("Skins")]
    public List<CharacterSkin> skins; // Тот же список скинов, что и в магазине

    [Header("Player Settings")]
    public Transform skinContainer; // Объект, внутри которого будет создана модель скина

    private const string SelectedSkinKey = "SelectedSkinIndex";

    void Awake()
    {
        LoadPlayerSkin();
    }

    private void LoadPlayerSkin()
    {
        int selectedSkinIndex = PlayerPrefs.GetInt(SelectedSkinKey, GetDefaultSkinIndex());

        if (selectedSkinIndex >= 0 && selectedSkinIndex < skins.Count)
        {
            CharacterSkin selectedSkin = skins[selectedSkinIndex];
            
            // Удаляем старую модель, если она есть
            foreach (Transform child in skinContainer)
            {
                Destroy(child.gameObject);
            }

            // Создаем и размещаем новую модель
            Instantiate(selectedSkin.skinPrefab, skinContainer.position, skinContainer.rotation, skinContainer);
        }
        else
        {
            Debug.LogError("Selected skin index is out of range!");
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
