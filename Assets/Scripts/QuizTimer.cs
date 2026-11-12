using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Countdown timer per soal. Saat waktu habis, callback OnTimeUp dipanggil.
/// Bisa dikombinasikan dengan health manager untuk kurangi nyawa saat waktu habis.
/// </summary>
public class QuizTimer : MonoBehaviour
{
    [Header("Pengaturan Timer")]
    [Tooltip("Durasi dalam detik per soal")]
    public float duration = 15f;

    [Header("UI")]
    public TMP_Text timerText;
    public Image timerBar;        // progress bar visual (opsional)
    public Color normalColor = new Color(0.133f, 0.773f, 0.369f, 1f);  // Stitch SecondaryEmerald #22C55E
    public Color warningColor = new Color(0.961f, 0.620f, 0.043f, 1f); // Stitch TertiaryAmber #F59E0B
    public Color dangerColor = new Color(0.937f, 0.267f, 0.267f, 1f);  // Stitch CoralRed #EF4444

    [Header("Callback")]
    public GameObject targetObject;  // objek yang mengandung method yang dipanggil
    public string methodName = "OnTimeUp";

    private float remainingTime;
    private bool isRunning = false;
    private bool wasTimeUp = false;

    void Start()
    {
        ResetTimer();
    }

    public void ResetTimer()
    {
        remainingTime = duration;
        wasTimeUp = false;
        isRunning = true;
        UpdateTimerUI();
    }

    public void PauseTimer()
    {
        isRunning = false;
    }

    public void ResumeTimer()
    {
        isRunning = true;
    }

    void Update()
    {
        if (!isRunning || remainingTime <= 0) return;

        remainingTime -= Time.deltaTime;
        if (remainingTime <= 0)
        {
            remainingTime = 0;
            isRunning = false;
            wasTimeUp = true;
            UpdateTimerUI();
            InvokeTimeUp();
        }
        else
        {
            UpdateTimerUI();
        }
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int seconds = Mathf.CeilToInt(remainingTime);
            timerText.text = seconds.ToString();
        }

        if (timerBar != null)
        {
            float fill = Mathf.Clamp01(remainingTime / duration);
            timerBar.fillAmount = fill;
            if (fill > 0.5f) timerBar.color = normalColor;
            else if (fill > 0.25f) timerBar.color = warningColor;
            else timerBar.color = dangerColor;
        }
    }

    private void InvokeTimeUp()
    {
        if (targetObject != null && !string.IsNullOrEmpty(methodName))
        {
            targetObject.SendMessage(methodName, SendMessageOptions.RequireReceiver);
        }
    }

    public float maxTime { get => duration; set => duration = value; }
    public bool useTimer { get => isRunning; set => isRunning = value; }
    public float timeLeft => remainingTime;

    public void AddTime(float amount)
    {
        remainingTime += amount;
    }

    public void SetTimerDisplay()
    {
        UpdateTimerUI();
    }
}