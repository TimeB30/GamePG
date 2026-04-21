using UnityEngine;
using TMPro; // Используем TextMeshPro

public class DifficultyButton : MonoBehaviour
{
    // Меняем тип с Text на TextMeshProUGUI
    public TextMeshProUGUI buttonText; // Ссылка на компонент TextMeshPro на кнопке

    private string[] difficultyLevels = { "Easy", "Normal", "Hard", "F" };
    private int currentDifficultyIndex = 0;

    void Start()
    {
        // Убедимся, что ссылка на TextMeshPro компонент установлена
        if (buttonText == null)
        {
            buttonText = GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText == null)
            {
                Debug.LogError("DifficultyButton: Компонент TextMeshProUGUI не найден на кнопке или в дочерних элементах. Пожалуйста, назначьте его вручную в инспекторе.");
                return;
            }
        }

        // Инициализируем текст кнопки текущим уровнем сложности
        UpdateButtonText();
    }

    // Этот метод будет вызываться при нажатии на кнопку
    public void ChangeDifficulty()
    {
        currentDifficultyIndex = (currentDifficultyIndex + 1) % difficultyLevels.Length;
        UpdateButtonText();

        // Здесь вы можете сохранить выбранную сложность, например, в PlayerPrefs
        // PlayerPrefs.SetInt("Difficulty", currentDifficultyIndex);
        Debug.Log("Выбрана сложность: " + difficultyLevels[currentDifficultyIndex]);
    }

    private void UpdateButtonText()
    {
        if (buttonText != null)
        {
            buttonText.text = difficultyLevels[currentDifficultyIndex];
        }
    }

    // Метод для получения текущего уровня сложности (если нужно из других скриптов)
    public string GetCurrentDifficulty()
    {
        return difficultyLevels[currentDifficultyIndex];
    }

    // Метод для получения индекса текущего уровня сложности
    public int GetCurrentDifficultyIndex()
    {
        return currentDifficultyIndex;
    }
}
