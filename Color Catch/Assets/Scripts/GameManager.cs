using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public enum ItemType { Coin, Bomb }

    [Header("Game rules")]
    public int score = 0;
    public float totalTime = 60f;
    public bool gameOver = false;

    [Header("Win Condition")]
    public int winScore = 200;

    [Header("Collectible target")]
    public ItemType targetItem = ItemType.Coin;
    [Tooltip("If true the target will rotate randomly each interval")]
    public bool rotateTargetOverTime = false;
    public float rotateInterval = 10f;

    [Header("UI (assign TextMeshProUGUI objects)")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI targetText;
    public TextMeshProUGUI feedbackText;
    public GameObject gameOverPanel;
    public TextMeshProUGUI finalScoreText;
    public UnityEngine.UI.Button replayButton;

    [Header("Game over settings")]
    public bool loseWhenTimerZero = true;

    float timer;
    Coroutine feedbackCoroutine;
    Coroutine rotateCoroutine;

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    void Start()
    {
        timer = totalTime;
        gameOver = false;
        Time.timeScale = 1f;

        if (gameOverPanel) gameOverPanel.SetActive(false);

        // Setup replay button
        if (replayButton != null)
        {
            replayButton.onClick.AddListener(Restart);
        }

        TryArrangeUI();
        UpdateUI();

        if (rotateTargetOverTime)
        {
            if (rotateCoroutine != null) StopCoroutine(rotateCoroutine);
            rotateCoroutine = StartCoroutine(RotateTargetRoutine());
        }
    }

    void Update()
    {
        if (gameOver) return;

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            timer = 0f;
            if (loseWhenTimerZero) EndGame(false);
        }

        UpdateUI();
    }

    void TryArrangeUI()
    {
        try
        {
            if (scoreText != null)
            {
                RectTransform rt = scoreText.rectTransform;
                rt.anchorMin = new Vector2(0f, 1f);
                rt.anchorMax = new Vector2(0f, 1f);
                rt.pivot = new Vector2(0f, 1f);
                rt.anchoredPosition = new Vector2(12f, -12f);
            }

            if (timerText != null)
            {
                RectTransform rt = timerText.rectTransform;
                rt.anchorMin = new Vector2(0.5f, 1f);
                rt.anchorMax = new Vector2(0.5f, 1f);
                rt.pivot = new Vector2(0.5f, 1f);
                rt.anchoredPosition = new Vector2(0f, -12f);
            }

            if (targetText != null)
            {
                RectTransform rt = targetText.rectTransform;
                rt.anchorMin = new Vector2(1f, 1f);
                rt.anchorMax = new Vector2(1f, 1f);
                rt.pivot = new Vector2(1f, 1f);
                rt.anchoredPosition = new Vector2(-12f, -12f);
            }

            if (feedbackText != null)
            {
                RectTransform rt = feedbackText.rectTransform;
                rt.anchorMin = new Vector2(0.5f, 1f);
                rt.anchorMax = new Vector2(0.5f, 1f);
                rt.pivot = new Vector2(0.5f, 1f);
                rt.anchoredPosition = new Vector2(0f, -48f);
                feedbackText.gameObject.SetActive(false);
            }

            if (gameOverPanel != null)
            {
                RectTransform rtPanel = gameOverPanel.GetComponent<RectTransform>();
                if (rtPanel != null)
                {
                    rtPanel.anchorMin = new Vector2(0.5f, 0.5f);
                    rtPanel.anchorMax = new Vector2(0.5f, 0.5f);
                    rtPanel.pivot = new Vector2(0.5f, 0.5f);
                    rtPanel.anchoredPosition = Vector2.zero;
                }
            }
        }
        catch
        {
            // ignore errors
        }
    }

    void UpdateUI()
    {
        if (scoreText) scoreText.text = $"Score: {score}";
        if (timerText) timerText.text = $"Time: {Mathf.CeilToInt(timer)}";
        if (targetText) targetText.text = $"Target: {targetItem}";
    }

    public void HandleCollectiblePicked(ItemType type, int points)
    {
        if (gameOver) return;

        if (type == targetItem)
        {
            AddScore(points);
            ShowFeedback($"+{points} ({type})");
        }
        else
        {
            AddScore(-points);
            ShowFeedback($"-{points} Wrong ({type})");
        }
    }

    public void AddScore(int amount)
    {
        score += amount;
        if (score < 0) score = 0;
        UpdateUI();

        // Check win condition
        if (score >= winScore && !gameOver)
        {
            EndGame(true);
        }
    }

    public void OnPoliceCatch(int penaltyPoints = 15)
    {
        if (gameOver) return;
        AddScore(-penaltyPoints);
        ShowFeedback($"-{penaltyPoints} Caught by Police");
    }

    public void OnHazardHit(int penaltyPoints = 5, string hazardName = "Hazard")
    {
        if (gameOver) return;
        AddScore(-penaltyPoints);
        ShowFeedback($"-{penaltyPoints} ({hazardName})");
    }

    public void ShowFeedback(string text, float duration = 1.2f)
    {
        if (feedbackText == null) return;
        if (feedbackCoroutine != null) StopCoroutine(feedbackCoroutine);
        feedbackCoroutine = StartCoroutine(FeedbackRoutine(text, duration));
    }

    IEnumerator FeedbackRoutine(string text, float dur)
    {
        feedbackText.text = text;
        feedbackText.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(dur);
        feedbackText.gameObject.SetActive(false);
    }

    IEnumerator RotateTargetRoutine()
    {
        while (!gameOver)
        {
            yield return new WaitForSeconds(rotateInterval);
            targetItem = (targetItem == ItemType.Coin) ? ItemType.Bomb : ItemType.Coin;
            UpdateUI();
            ShowFeedback($"Target is now {targetItem}", 1.0f);
        }
    }

    public void EndGame(bool playerWon)
    {
        gameOver = true;
        if (gameOverPanel) gameOverPanel.SetActive(true);

        string resultText = playerWon ? "YOU WIN!" : "GAME OVER";
        if (finalScoreText)
            finalScoreText.text = $"{resultText}\nFinal Score: {score}";

        Time.timeScale = 0f;
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}