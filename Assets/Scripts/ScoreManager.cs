using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    private TMP_Text scoreText;
    private TMP_Text highScoreText;

    private int score = 0;
    private int highScore = 0;

    // Properti untuk membaca nilai skor dari skrip lain
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
        // Cari UI berdasarkan Tag setiap kali scene dimuat
        GameObject scoreTextObj = GameObject.FindWithTag("ScoreText");
        if (scoreTextObj != null)
            scoreText = scoreTextObj.GetComponent<TMP_Text>();

        GameObject highScoreTextObj = GameObject.FindWithTag("HighScoreText");
        if (highScoreTextObj != null)
            highScoreText = highScoreTextObj.GetComponent<TMP_Text>();

        UpdateUI();
    }

    private void Start()
    {
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        UpdateUI();
    }

    public void AddScore(int value)
    {
        score += value;
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
        }
        UpdateUI();
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
        UpdateUI();
    }
}