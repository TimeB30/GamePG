using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class MainMenuController : MonoBehaviour
{
    [Header("Difficulty Settings")]
    [Tooltip("UI Слайдер для выбора сложности.")]
    public Slider difficultySlider;

    [Tooltip("Текстовое поле для отображения текущей выбранной сложности.")]
    public TextMeshProUGUI difficultyText;

    [Tooltip("Список имен сложностей. Порядок важен! 'Easy', 'Normal', 'Hard', 'F'")]
    public List<string> difficultyNames;

    [Header("Scene Settings")]
    [Tooltip("Индекс игровой сцены в Build Settings (обычно 1).")]
    public int gameSceneIndex = 1;

    private string selectedDifficulty;

    void Start()
    {
        if (difficultySlider != null)
        {
            // Настраиваем слайдер при старте
            difficultySlider.minValue = 0;
            difficultySlider.maxValue = difficultyNames.Count - 1;
            difficultySlider.wholeNumbers = true;

            // Подписываемся на событие изменения значения
            difficultySlider.onValueChanged.AddListener(OnSliderValueChanged);
            
            // Устанавливаем начальное значение
            OnSliderValueChanged(difficultySlider.value);
        }
    }

    // Этот метод вызывается каждый раз, когда пользователь двигает слайдер
    private void OnSliderValueChanged(float value)
    {
        int index = Mathf.RoundToInt(value);
        if (index >= 0 && index < difficultyNames.Count)
        {
            selectedDifficulty = difficultyNames[index];
            if (difficultyText != null)
            {
                difficultyText.text = selectedDifficulty;
            }
        }
    }

    // --- Публичные методы для кнопок ---

    // Этот метод вызывается кнопкой "Играть"
    public void StartGame()
    {
        if (DifficultyController.Instance != null && !string.IsNullOrEmpty(selectedDifficulty))
        {
            DifficultyController.Instance.SetDifficultyAndLoadScene(selectedDifficulty, gameSceneIndex);
        }
        else
        {
            Debug.LogError("DifficultyController not found or no difficulty selected!");
        }
    }

    // Этот метод вызывается кнопкой "Выход"
    public void QuitGame()
    {
        Debug.Log("Quitting game..."); // Для проверки в редакторе
        Application.Quit();
    }
}
