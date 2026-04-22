using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager Instance { get; private set; }

    public DifficultySettings currentSettings;

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
        }
    }

    public void SetDifficulty(DifficultySettings newSettings)
    {
        currentSettings = newSettings;
        Debug.Log("Difficulty set to: " + newSettings.difficultyName);
    }
}
