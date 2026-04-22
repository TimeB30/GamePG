using UnityEngine;
using UnityEngine.SceneManagement;

public class DifficultyController : MonoBehaviour
{
    public static DifficultyController Instance { get; private set; }

    // Статическая переменная, чтобы легко переносить выбор между сценами.
    // Устанавливаем значение по умолчанию на случай прямого запуска игровой сцены.
    public static string SelectedDifficultyName { get; private set; } = "Easy"; 

    void Awake()
    {
        // Классический синглтон, который не уничтожается при загрузке
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

    public void SetDifficultyAndLoadScene(string difficultyName, int sceneIndex)
    {
        SelectedDifficultyName = difficultyName;
        Debug.Log($"Difficulty set to: {SelectedDifficultyName}");
        SceneManager.LoadScene(sceneIndex);
    }
}
