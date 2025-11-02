using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game rules")]
    public int score = 0;
    public float totalTime = 60f; // seconds
    public bool gameOver = false;

    [Header("UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI targetText;
    public GameObject gameOverPanel;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI feedbackText; // message for wrong pick / hazard

    [Header("Collectible target")]
    public string targetColor = "Gold";

    float timer;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        timer = totalTime;
        UpdateUI();
        HideGameOver();
        if (feedbackText) feedbackText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (gameOver) return;

        // Count down using unscaled delta to avoid freezing issues if you use timescale
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            timer = 0f;
            EndGame(false);
        }
        UpdateUI();
    }

    public void AddScore(int amount)
    {
        score += amount;
        if (score < 0) score = 0;
        UpdateUI();
    }

    public void SetTarget(string newTarget)
    {
        targetColor = newTarget;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (scoreText) scoreText.text = $"Score: {score}";
        if (timerText) timerText.text = Mathf.CeilToInt(timer).ToString();
        if (targetText) targetText.text = $"Target: {targetColor}";
    }

    // displays and pauses play on game over
    public void EndGame(bool won)
    {
        gameOver = true;
        // show panel and freeze time for gameplay
        if (gameOverPanel) gameOverPanel.SetActive(true);

        if (finalScoreText) finalScoreText.text = $"Final Score: {score}\nTime Left: {Mathf.CeilToInt(timer)}";

        // Stop gameplay but keep UI coroutines working by using unscaled time for UI routines.
        Time.timeScale = 0f;
    }

    // Hides game over and ensures timeScale back to 1
    public void HideGameOver()
    {
        if (gameOverPanel) gameOverPanel.SetActive(false);
        gameOver = false;
        Time.timeScale = 1f;
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Called when player hits a hazard like bush/barrier
    public void ApplyHazardPenalty(int penalty, string reason = "Hit Hazard")
    {
        AddScore(-penalty);
        ShowFeedback($"-{penalty} : {reason}");
    }

    // Display short message on screen (uses realtime so it still hides if timescale=0)
    public void ShowFeedback(string msg, float duration = 1.2f)
    {
        if (feedbackText == null) return;
        StopAllCoroutines();
        StartCoroutine(ShowFeedbackRoutine(msg, duration));
    }

    IEnumerator ShowFeedbackRoutine(string msg, float dur)
    {
        feedbackText.text = msg;
        feedbackText.gameObject.SetActive(true);
        // use realtime so it still hides correctly even when Time.timeScale == 0
        yield return new WaitForSecondsRealtime(dur);
        feedbackText.gameObject.SetActive(false);
    }
}
