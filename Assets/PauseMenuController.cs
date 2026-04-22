using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; // 1. Добавляем пространство имен для новой системы ввода

public class PauseMenuController : MonoBehaviour
{
    public static bool isGamePaused = false;

    [Header("UI")]
    [Tooltip("Панель, которая является самим меню паузы")]
    public GameObject pauseMenuUI;

    [Header("Settings")]
    [Tooltip("Индекс сцены главного меню в Build Settings")]
    public int mainMenuSceneIndex = 0;

    void Start()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isGamePaused = false;
    }

    void Update()
    {
        // 2. Заменяем старый вызов на новый
        // Проверяем, что клавиатура подключена и клавиша была нажата в этом кадре
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isGamePaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isGamePaused = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isGamePaused = true;
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneIndex);
    }
}
