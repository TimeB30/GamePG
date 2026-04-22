using UnityEngine;
using System.Collections.Generic; // Добавляем для использования List

[CreateAssetMenu(fileName = "New Difficulty", menuName = "Game/Difficulty Setting")]
public class DifficultySettings : ScriptableObject
{
    [Header("Difficulty Name")]
    public string difficultyName;

    [Header("Level Generator Settings")]
    public int numberOfPlatforms = 50;
    public Vector3 platformOffset = new Vector3(0, 0, 2);

    [Header("Platform Types for this Difficulty")]
    public List<PlatformType> platformTypes; // <--- ПЕРЕНЕСЛИ СПИСОК СЮДА

    [Header("Hazard Multipliers")]
    [Tooltip("Multiplies the base chance of a platform being a falling one. 1 = normal, 0.5 = half chance, 2 = double chance.")]
    [Range(0, 5)] public float fallingPlatformMultiplier = 1f;
    
    [Tooltip("Multiplies the base chance of a platform being a moving one.")]
    [Range(0, 5)] public float movingPlatformMultiplier = 1f;

    [Header("Collectible Settings")]
    [Range(0, 1)] public float coinSpawnChance = 0.5f;
}
