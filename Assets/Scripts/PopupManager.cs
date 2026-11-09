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
        HideAllPanels();
    }

    // Fungsi ini sekarang hanya menangani "Game Over" dan "Pilih Jawaban Dulu"
    public void ShowPopup(string message, bool isCorrect, bool isGameOver = false)
    {
        HideAllPanels();

        if (isGameOver)
        {
            if (panelGameOver != null)
            {
                if (textGameOver != null) textGameOver.text = message;
                panelGameOver.SetActive(true);
                Time.timeScale = 0f;
            }
            if (restartButton != null)
            {
                restartButton.gameObject.SetActive(true);
                restartButton.onClick.RemoveAllListeners();
                restartButton.onClick.AddListener(RestartGame);
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

    // --- UBAH FUNGSI INI ---
    /// <summary>
    /// Menampilkan panel "Benar" dengan pesan konfirmasi.
    /// Panel ini akan auto-hide.
    /// </summary>
    public void ShowPopupBenar(string pesanKonfirmasi)
    {

        HideAllPanels();
        if (panelBenar != null)
        {
            if (textKonfirmasiBenar != null) textKonfirmasiBenar.text = pesanKonfirmasi;
            panelBenar.SetActive(true);
        }
        Invoke("HideAllPanels", 2f);
    }
    // ---------------------------------

    public void ShowPembahasan(string message)
    {

        HideAllPanels();
        if (panelSalah != null)
        {
            panelSalah.SetActive(true);
            if (groupInfoSalah != null) groupInfoSalah.SetActive(false);
            if (groupInfoPembahasan != null) groupInfoPembahasan.SetActive(true);
            if (textPembahasan != null) textPembahasan.text = message;
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