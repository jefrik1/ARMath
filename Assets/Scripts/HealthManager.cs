using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Mengelola nyawa (hearts) pemain. Setiap jawaban salah mengurangi 1 nyawa.
/// </summary>
public class HealthManager : MonoBehaviour
{
    public int maxHealth = 3;
    public int currentHealth = 3;

    [Header("UI Referensi")]
    public Image[] healthIcons;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    private void Start()
    {
        Time.timeScale = 1f;
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    /// <summary>
    /// Kurangi nyawa 1 point. Return true jika game over (nyawa habis).
    /// </summary>
    public bool ReduceHealth()
    {
        if (currentHealth > 0)
        {
            currentHealth--;
            UpdateHealthUI();
        }
        return currentHealth <= 0;
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    public void UpdateHealthUI()
    {
        if (healthIcons != null)
        {
            for (int i = 0; i < healthIcons.Length; i++)
            {
                if (healthIcons[i] == null) continue;
                if (i < currentHealth)
                {
                    if (fullHeart != null) healthIcons[i].sprite = fullHeart;
                    healthIcons[i].color = Color.white;
                }
                else
                {
                    if (emptyHeart != null) healthIcons[i].sprite = emptyHeart;
                    healthIcons[i].color = new Color(1, 1, 1, 0.5f);
                }
            }
        }
    }
}