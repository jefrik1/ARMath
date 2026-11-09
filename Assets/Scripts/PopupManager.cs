using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PopupManager : MonoBehaviour
{
    [Header("Panel Referensi (3 Panel Utama)")]
    public GameObject panelBenar;
    public GameObject panelSalah;
    public GameObject panelGameOver;

    [Header("Grup Internal Panel Salah")]
    public GameObject groupInfoSalah;
    public GameObject groupInfoPembahasan;

    [Header("Komponen Kustom")]
    public TMP_Text textGameOver;
    public TMP_Text textPembahasan; // Ini untuk di panelSalah

    // --- GANTI BARIS INI ---
    [Tooltip("Teks di dalam PanelBenar untuk konfirmasi singkat.")]
    public TMP_Text textKonfirmasiBenar; // Ganti dari textPembahasanBenar
    // -------------------------

    [Header("Tombol Utama")]
    public Button restartButton;

    private void Start()
    {
        StitchUIThemeApplier.EnhanceModals(this);
        HideAllPanels();
    }

    // Fungsi ini menangani "Game Over" dan "Pilih Jawaban Dulu"
    public void ShowPopup(string message, bool isCorrect, bool isGameOver = false)
    {
        HideAllPanels();

        if (isGameOver)
        {
            if (panelGameOver != null)
            {
                if (textGameOver != null)
                    textGameOver.text = $"<color=#EF4444><b>GAME OVER</b></color>\n<size=18>Nyawa Habis! Jangan Menyerah, Coba Lagi! 💪</size>\n\n{message}";
                panelGameOver.SetActive(true);
                Time.timeScale = 0f;
            }
            if (restartButton != null)
            {
                restartButton.gameObject.SetActive(true);
                restartButton.onClick.RemoveAllListeners();
                restartButton.onClick.AddListener(RestartGame);
                StitchUIThemeApplier.ApplyTactileButtonStyle(restartButton, StitchUIThemeApplier.SecondaryEmerald, StitchUIThemeApplier.SecondaryEmeraldDark, Color.white);
            }
        }
        else if (isCorrect)
        {
        }
        else
        {
            if (panelSalah != null)
            {
                panelSalah.SetActive(true);
                if (groupInfoSalah != null) groupInfoSalah.SetActive(true);
                if (groupInfoPembahasan != null) groupInfoPembahasan.SetActive(false);
            }
            Invoke("HideAllPanels", 2f);
        }
    }

    /// <summary>
    /// Menampilkan panel "Benar" dengan format Stitch gamified celebratory modal.
    /// </summary>
    public void ShowPopupBenar(string pesanKonfirmasi)
    {
        HideAllPanels();
        if (panelBenar != null)
        {
            if (textKonfirmasiBenar != null)
            {
                textKonfirmasiBenar.text = $"<color=#22C55E><b>🎉 JAWABAN TEPAT!</b></color>\n" +
                                           $"<color=#15803D><size=18>Hebat, Matematikawan Cilik! +50 XP</size></color>\n\n" +
                                           $"<color=#111C2D>{pesanKonfirmasi}</color>";
            }
            panelBenar.SetActive(true);
        }
        Invoke("HideAllPanels", 2.5f);
    }

    public void ShowPembahasan(string message)
    {
        HideAllPanels();
        if (panelSalah != null)
        {
            panelSalah.SetActive(true);
            if (groupInfoSalah != null) groupInfoSalah.SetActive(false);
            if (groupInfoPembahasan != null) groupInfoPembahasan.SetActive(true);
            if (textPembahasan != null)
            {
                textPembahasan.text = $"<color=#246EA6><b>📘 PEMBAHASAN LANGKAH DEMI LANGKAH</b></color>\n\n{message}";
            }
        }
    }

    public void HideAllPanels()
    {
        // Fungsi ini tetap sama
        CancelInvoke("HideAllPanels");
        if (panelBenar != null) panelBenar.SetActive(false);
        if (panelSalah != null) panelSalah.SetActive(false);
        if (panelGameOver != null) panelGameOver.SetActive(false);
    }

    private void RestartGame()
    {
        // Fungsi ini tetap sama
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}