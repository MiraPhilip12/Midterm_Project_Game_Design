// Assets/Scripts/GameManager.cs
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

    [Header("Collectible target")]
    public ItemType targetItem = ItemType.Coin;   // current required item type to earn points
    [Tooltip("If true the target will rotate randomly each interval")]
    public bool rotateTargetOverTime = false;
    public float rotateInterval = 10f; // seconds

    [Header("UI (assign TextMeshProUGUI objects)")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI targetText;
    public TextMeshProUGUI feedbackText;
    public GameObject gameOverPanel;
    public TextMeshProUGUI finalScoreText;

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

        // Ensure UI panel visibility
        if (gameOverPanel) gameOverPanel.SetActive(false);

        // Setup layout to minimize overlapping (best-effort)
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

    #region UI & layout helpers

    void TryArrangeUI()
    {
        // This tries to place UI elements in sensible screen corners if they were mis-positioned.
        // If you prefer editor control, you can disable this behavior.
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
                rt.anchoredPosition = new Vector2(0f, -48f); // just below timer
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
            // ignore if any element is missing - this is a safe best-effort arrangement
        }
    }

    void UpdateUI()
    {
        if (scoreText) scoreText.text = $"Score: {score}";
        if (timerText) timerText.text = Mathf.CeilToInt(timer).ToString();
        if (targetText) targetText.text = $"Target: {targetItem}";
    }

    #endregion

    #region Score & game events

    // Call this from your collectible script when player picks an item.
    // Example: GameManager.Instance.HandleCollectiblePicked(GameManager.ItemType.Coin, 10);
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
    }

    // Police calls this when player is caught
    public void OnPoliceCatch(int penaltyPoints = 15)
    {
        if (gameOver) return;
        AddScore(-penaltyPoints);
        ShowFeedback($"-{penaltyPoints} Caught by Police");
        // optionally add stun or respawn logic here
    }

    // Hazards (bushes/barriers) call this
    public void OnHazardHit(int penaltyPoints = 5, string hazardName = "Hazard")
    {
        if (gameOver) return;
        AddScore(-penaltyPoints);
        ShowFeedback($"-{penaltyPoints} ({hazardName})");
    }

    #endregion

    #region Feedback & target rotation

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
            // flip target
            targetItem = (targetItem == ItemType.Coin) ? ItemType.Bomb : ItemType.Coin;
            UpdateUI();
            ShowFeedback($"Target is now {targetItem}", 1.0f);
        }
    }

    #endregion

    #region Game flow

    public void EndGame(bool playerWon)
    {
        gameOver = true;
        if (gameOverPanel) gameOverPanel.SetActive(true);
        if (finalScoreText) finalScoreText.text = $"Final Score: {score}\nTime left: {Mathf.CeilToInt(timer)}";
        Time.timeScale = 0f;
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    #endregion
}
