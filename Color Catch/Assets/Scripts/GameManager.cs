using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game rules")]
    public int score = 0;
    public float totalTime = 60f; // change as needed
    public bool gameOver = false;

    [Header("UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI targetText;
    public GameObject gameOverPanel;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI feedbackText; // small message for wrong pick / hazard

    [Header("Collectible target")]
    public string targetColor = "Gold"; // example: "Gold", "Silver", "Bronze"

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
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            timer = 0f;
            EndGame(false); // player loses when time finishes
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

    public void EndGame(bool won)
    {
        gameOver = true;
        Time.timeScale = 0f;
        if (gameOverPanel) gameOverPanel.SetActive(true);
        if (finalScoreText) finalScoreText.text = $"Final Score: {score}\nTime: {Mathf.CeilToInt(timer)}";
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

    // Display short message on screen
    public void ShowFeedback(string msg, float duration = 1.2f)
    {
        if (feedbackText == null) return;
        StopAllCoroutines();
        StartCoroutine(ShowFeedbackRoutine(msg, duration));
    }

    System.Collections.IEnumerator ShowFeedbackRoutine(string msg, float dur)
    {
        feedbackText.text = msg;
        feedbackText.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(dur); // realtime so it's independent if you pause/timeScale=0
        feedbackText.gameObject.SetActive(false);
    }
}
