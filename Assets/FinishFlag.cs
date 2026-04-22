using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishFlag : MonoBehaviour
{
    // Этот метод вызывается, когда другой коллайдер входит в триггер-зону флажка.
    private void OnTriggerEnter(Collider other)
    {
        // Проверяем, что объект, который вошел в триггер, имеет тег "Player".
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player reached the finish line! Reloading level...");
            RestartLevel();
        }
    }

    private void RestartLevel()
    {
        // Получаем индекс текущей активной сцены и перезагружаем ее.
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }
}
