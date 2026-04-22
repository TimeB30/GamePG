using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    void Awake()
    {
        // Получаем имя выбранной сложности
        string difficulty = DifficultyController.SelectedDifficultyName;
        Debug.Log($"Loading level with difficulty: {difficulty}");

        // Проходим по всем дочерним объектам этого LevelLoader'а
        foreach (Transform childGenerator in transform)
        {
            // Сравниваем имя дочернего объекта с выбранной сложностью
            if (childGenerator.name == difficulty)
            {
                // Если имена совпали - включаем этот генератор
                childGenerator.gameObject.SetActive(true);
                Debug.Log($"Activating generator: {childGenerator.name}");
            }
            else
            {
                // Все остальные - выключаем на всякий случай
                childGenerator.gameObject.SetActive(false);
            }
        }
    }
}
