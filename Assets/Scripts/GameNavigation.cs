using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Scene navigator: memuat scene kuis berdasarkan nama.
/// Scene yang tersedia harus didaftarkan di Build Settings.
/// </summary>
public class GameNavigation : MonoBehaviour
{
    /// <summary>
    /// Load scene kuis. Scene name harus sesuai dengan nama di Build Settings.
    /// </summary>
    public void LoadGameScene(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning("GameNavigation: sceneName kosong, tidak memuat scene.");
            return;
        }

        Scene scene = SceneManager.GetSceneByName(sceneName);
        if (!scene.IsValid())
        {
            Debug.LogError("GameNavigation: scene '" + sceneName + "' tidak ditemukan di Build Settings!");
            return;
        }

        Debug.Log("Memuat scene: " + sceneName);
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// Kembali ke Main Menu.
    /// </summary>
    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// Restart scene saat ini.
    /// </summary>
    public void RestartCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Load scene oleh index (alternatif untuk nama).
    /// </summary>
    public void LoadSceneByIndex(int index)
    {
        int count = SceneManager.sceneCountInBuildSettings;
        if (index >= 0 && index < count)
        {
            SceneManager.LoadScene(index);
        }
        else
        {
            Debug.LogWarning("GameNavigation: Index scene tidak valid: " + index);
        }
    }
}