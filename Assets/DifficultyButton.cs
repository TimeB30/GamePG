using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Button))]
public class DifficultyButton : MonoBehaviour
{
    [Header("Settings")]
    public DifficultySettings difficultyToSet;
    public int gameSceneIndex = 1; // Индекс вашей основной игровой сцены в Build Settings

    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
        if (DifficultyManager.Instance != null && difficultyToSet != null)
        {
            // Устанавливаем сложность
            DifficultyManager.Instance.SetDifficulty(difficultyToSet);
            
            // Загружаем игровую сцену
            SceneManager.LoadScene(gameSceneIndex);
        }
        else
        {
            Debug.LogError("DifficultyManager not found or difficultyToSet is not assigned!");
        }
    }
}
