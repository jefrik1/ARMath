using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// Manajer skor kuis, high score, dan penilaian bintang (★ 1–3 Stars) per kategori/scene.
/// </summary>
public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    private TMP_Text scoreText;
    private TMP_Text highScoreText;

    private int score = 0;
    private int highScore = 0;

    public int Score { get { return score; } }
    public int HighScore { get { return highScore; } }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject scoreTextObj = null;
        try { scoreTextObj = GameObject.FindWithTag("ScoreText"); } catch { }
        if (scoreTextObj == null) scoreTextObj = GameObject.Find("ScoreText");
        if (scoreTextObj != null)
            scoreText = scoreTextObj.GetComponent<TMP_Text>();

        GameObject highScoreTextObj = null;
        try { highScoreTextObj = GameObject.FindWithTag("HighScoreText"); } catch { }
        if (highScoreTextObj == null) highScoreTextObj = GameObject.Find("HighScoreText");
        if (highScoreTextObj != null)
            highScoreText = highScoreTextObj.GetComponent<TMP_Text>();

        // Ambil high score spesifik scene ini
        highScore = GetHighScore(scene.name);
        UpdateUI();
    }

    private void Start()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        highScore = GetHighScore(currentScene);
        UpdateUI();
    }

    public void AddScore(int value)
    {
        score += value;
        string currentScene = SceneManager.GetActiveScene().name;

        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore_" + currentScene, highScore);
            PlayerPrefs.SetInt("HighScore", highScore); // Fallback global
            PlayerPrefs.Save();
            MathSoundFX.PlayHighScore();
        }

        // Simpan perolehan bintang penguasaan (Mastery Stars)
        UpdateMasteryStars(currentScene, score);
        UpdateUI();
    }

    /// <summary>
    /// Perhitungan bintang penguasaan materi SD:
    /// >= 30 poin = 1 Bintang (★)
    /// >= 60 poin = 2 Bintang (★★)
    /// >= 100 poin = 3 Bintang (★★★)
    /// </summary>
    public static void UpdateMasteryStars(string sceneName, int currentScore)
    {
        int earnedStars = 0;
        if (currentScore >= 100) earnedStars = 3;
        else if (currentScore >= 60) earnedStars = 2;
        else if (currentScore >= 30) earnedStars = 1;

        int existingStars = PlayerPrefs.GetInt("Stars_" + sceneName, 0);
        if (earnedStars > existingStars)
        {
            PlayerPrefs.SetInt("Stars_" + sceneName, earnedStars);
            PlayerPrefs.Save();
        }
    }

    public static int GetStars(string sceneName)
    {
        return PlayerPrefs.GetInt("Stars_" + sceneName, 0);
    }

    public static string GetStarsString(string sceneName)
    {
        int count = GetStars(sceneName);
        switch (count)
        {
            case 3: return "★★★";
            case 2: return "★★☆";
            case 1: return "★☆☆";
            default: return "☆☆☆";
        }
    }

    public static int GetHighScore(string sceneName)
    {
        int sceneHs = PlayerPrefs.GetInt("HighScore_" + sceneName, 0);
        if (sceneHs > 0) return sceneHs;
        return PlayerPrefs.GetInt("HighScore", 0);
    }

    private void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;

        if (highScoreText != null)
            highScoreText.text = "High Score: " + highScore;
    }

    public void ResetScore()
    {
        score = 0;
        string currentScene = SceneManager.GetActiveScene().name;
        highScore = GetHighScore(currentScene);
        UpdateUI();
    }
}
