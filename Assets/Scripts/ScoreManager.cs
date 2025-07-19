using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public int Score { get; private set; }
    public int HighScore { get; private set; }

    public TextMeshProUGUI scoreText;
    [HideInInspector]public TextMeshProUGUI highScoreText; // we dont need highscore for now

    private const string ScoreKey = "Score";
    private const string HighScoreKey = "HighScore";

    private void Awake()
    {
        // Singleton pattern 
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadScores();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        UpdateUI();
    }

    public void AddScore(int amount)
    {
        Score += amount;
        if (Score > HighScore)
        {
            HighScore = Score;
        }
        SaveScores();
        UpdateUI();
    }

    public void SaveScores()
    {
        PlayerPrefs.SetInt(ScoreKey, Score);
        PlayerPrefs.SetInt(HighScoreKey, HighScore);
        PlayerPrefs.Save();
    }

    public void LoadScores()
    {
        Score = PlayerPrefs.GetInt(ScoreKey, 0);
        HighScore = PlayerPrefs.GetInt(HighScoreKey, 0);
    }

    public void ResetScores()
    {
        Score = 0;
        HighScore = 0;
        PlayerPrefs.DeleteKey(ScoreKey);
        PlayerPrefs.DeleteKey(HighScoreKey);
        PlayerPrefs.Save();
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text =  Score.ToString();

        if (highScoreText != null)
            highScoreText.text =  HighScore.ToString();
    }
}
