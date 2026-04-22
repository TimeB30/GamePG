using UnityEngine;

[CreateAssetMenu(fileName = "New Skin", menuName = "Shop/Character Skin")]
public class CharacterSkin : ScriptableObject
{
    [Header("Skin Info")]
    public string skinName;
    public int price;

    [Header("Model")]
    public GameObject skinPrefab; // Префаб модели персонажа для этого скина

    [Header("Shop")]
    public bool isDefault = false; // Является ли этот скин стандартным (бесплатным и доступным сразу)
}
