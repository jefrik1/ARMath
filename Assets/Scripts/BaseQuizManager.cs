using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Base class untuk seluruh quiz. Semua logika UI state, health, score,
/// popup, dan destroy pilihan sudah di sini.
/// Subclass hanya override: GenerateRandomQuestion() dan IsAnswerCorrect().
/// </summary>
public abstract class BaseQuizManager<TAnswer> : MonoBehaviour, IQuizManager
{
    [Header("Referensi UI")]
    public TMP_Text questionText;
    public TMP_Text answerText;
    public Button checkButton;
    public Button undoButton;

    [Header("Manajer Lain")]
    public PopupManager popupManager;
    public HealthManager healthManager;
    public ItemSpawner itemSpawner;

    // huruf label jawaban benar (A, B, ...)
    public string correctAnswerLabel;
    public string playerAnswerLabel;
    public string playerAnswerValue;
    public string soalSaatIni;
    public string langkahPenyelesaian;

    // nilai jawaban benar sebagai string (untuk ditampilkan di pembahasan)
    public string correctAnswerValue;

    protected List<GameObject> objekPilihanAktif = new List<GameObject>();
    protected GameObject objekTerpilih;

    private string[] huruf = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K" };

    protected virtual void Start()
    {
        checkButton.onClick.AddListener(CekJawaban);
        undoButton.onClick.AddListener(UndoJawaban);
        if (ScoreManager.instance != null) ScoreManager.instance.ResetScore();
        GenerateRandomQuestion();
    }

    /// <summary>Subclasses: generate soal baru + spawn pilihan.</summary>
    public abstract void GenerateRandomQuestion();

    /// <summary>Subclasses: bandingkan jawaban pemain vs jawaban benar.
    /// Jawaban benar diambil dari correctAnswerValue (sudah di-set saat generate soal).
    /// Override dan return true jika jawaban pemain sesuai.</summary>
    public abstract bool IsAnswerCorrect(string playerAnswer);

    /// <summary>
    /// Convert jawaban benar ke string untuk ditampilkan di popup.
    /// Override jika perlu format khusus (mis. float F1).
    /// </summary>
    protected virtual string ConvertAnswerToString(TAnswer answer) => answer.ToString();

    /// <summary>
    /// Coba load soal dari JSON (SoalJsonLoader). Return true jika berhasil load.
    /// Dipanggil subclass di GenerateRandomQuestion() sebelum generasi acak.
    /// </summary>
    public bool TryLoadFromJson(string kategori)
    {
        SoalEntry entry = SoalJsonLoader.GetRandomSoal(kategori);
        if (entry == null) return false;

        soalSaatIni = entry.soal;
        correctAnswerValue = entry.jawaban;
        langkahPenyelesaian = entry.langkah ?? "";

        // Load pilihan dari JSON
        string[] pilihanArray = entry.pengecof != null && entry.pengecof.Length > 0
            ? entry.pengecof
            : new string[0];

        // Bangun daftar pilihan dan tentukan huruf jawaban benar
        List<string> semuaPilihan = new List<string>(pilihanArray);
        semuaPilihan.Add(entry.jawaban);

        // Shuffle dan tentukan huruf benar
        string[] h = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K" };

        // Shuffle
        for (int i = semuaPilihan.Count - 1; i > 0; i--)
        {
            int r = Random.Range(0, i + 1);
            string tmp = semuaPilihan[i];
            semuaPilihan[i] = semuaPilihan[r];
            semuaPilihan[r] = tmp;
        }

        correctAnswerLabel = null;
        for (int i = 0; i < semuaPilihan.Count; i++)
        {
            if (semuaPilihan[i] == entry.jawaban)
            {
                correctAnswerLabel = (i < h.Length) ? h[i] : null;
                break;
            }
        }

        string[] strs = semuaPilihan.ToArray();
        objekPilihanAktif = itemSpawner.SpawnPilihan(strs, h, this);

        questionText.text = soalSaatIni;

        return true;
    }

    // ---- SHARED UI / FLOW ----

    public void SetJawabanPilihan(string pilihan, int nilai, GameObject objekYangDipilih)
    {
        playerAnswerLabel = pilihan;
        playerAnswerValue = nilai.ToString();
        objekTerpilih = objekYangDipilih;
        answerText.text = "Jawaban Anda: " + nilai;
        checkButton.interactable = true;
        undoButton.gameObject.SetActive(true);
    }

    public void SetJawabanPilihan(string pilihan, string nilai, GameObject objekYangDipilih)
    {
        playerAnswerLabel = pilihan;
        playerAnswerValue = nilai != null ? nilai : "";
        objekTerpilih = objekYangDipilih;
        answerText.text = "Jawaban Anda: " + nilai;
        checkButton.interactable = true;
        undoButton.gameObject.SetActive(true);
    }

    public void SetJawabanPilihan(string pilihan, float nilai, GameObject objekYangDipilih)
    {
        playerAnswerLabel = pilihan;
        playerAnswerValue = nilai.ToString("F1");
        objekTerpilih = objekYangDipilih;
        answerText.text = "Jawaban Anda: " + nilai.ToString("F1");
        checkButton.interactable = true;
        undoButton.gameObject.SetActive(true);
    }

    public void UndoJawaban()
    {
        if (objekTerpilih == null) return;
        ResetAnswerState();
    }

    public void ResetAnswerState()
    {
        playerAnswerLabel = "";
        playerAnswerValue = "";
        objekTerpilih = null;
        if (answerText != null) answerText.text = "Jawaban Anda: -";
        if (checkButton != null) checkButton.interactable = false;
        if (undoButton != null) undoButton.gameObject.SetActive(false);
    }

    // flag: jawaban terakhir benar?
    private bool lastWasCorrect;

    public void CekJawaban()
    {
        if (string.IsNullOrEmpty(playerAnswerLabel))
        {
            popupManager.ShowPopup("Pilih jawaban dulu!", false);
            return;
        }

        foreach (var obj in objekPilihanAktif)
            if (obj != null) Destroy(obj);
        objekPilihanAktif.Clear();

        TAnswer correctAnswer = default;

        lastWasCorrect = (!string.IsNullOrEmpty(correctAnswerLabel) && playerAnswerLabel == correctAnswerLabel)
                         || IsAnswerCorrect(playerAnswerValue)
                         || IsAnswerCorrect(playerAnswerLabel);

        if (lastWasCorrect)
        {
            if (ScoreManager.instance != null) ScoreManager.instance.AddScore(10);

            string pesan = $"Jawaban kamu '{playerAnswerLabel}' ({correctAnswerLabel}) adalah benar! " +
                           $"Jawaban: {correctAnswerValue}. Poin bertambah.";
            popupManager.ShowPopupBenar(pesan);
            Invoke(nameof(LanjutKeSoalBerikutnya), 2f);
        }
        else
        {
            bool isGameOver = healthManager.ReduceHealth();
            string pembahasan = $"PEMBAHASAN\n\nSoal: {soalSaatIni}\n" +
                                $"Jawaban Anda: {playerAnswerValue} ({playerAnswerLabel})\n" +
                                $"Jawaban Benar: {correctAnswerLabel} ({correctAnswerValue})\n\n" +
                                langkahPenyelesaian;

            if (isGameOver)
            {
                string msg = $"Game Over! \uD83D\uDE22\nSkor Anda: {ScoreManager.instance?.Score ?? 0}";
                popupManager.ShowPopup(msg, false, true);
            }
            else
            {
                popupManager.ShowPembahasan(pembahasan);
            }
        }
    }

    /// <summary>
    /// Dipanggil oleh QuizTimer saat waktu habis.
    /// Default: tanda jawaban salah, kemudian proses seperti CekJawaban.
    /// </summary>
    public virtual void OnTimeUp()
    {
        lastWasCorrect = false; // waktu habis = otomatis salah

        if (string.IsNullOrEmpty(playerAnswerLabel))
        {
            foreach (var obj in objekPilihanAktif)
                if (obj != null) Destroy(obj);
            objekPilihanAktif.Clear();

            bool isGameOver = healthManager.ReduceHealth();
            string pembahasan = $"PEMBAHASAN\n\nSoal: {soalSaatIni}\n" +
                                $"Jawaban Anda: (tidak dijawab)\n" +
                                $"Jawaban Benar: {correctAnswerLabel} ({correctAnswerValue})\n\n" +
                                langkahPenyelesaian;

            if (isGameOver)
            {
                string msg = $"Game Over! \uD83D\uDE22\nSkor Anda: {ScoreManager.instance?.Score ?? 0}";
                popupManager.ShowPopup(msg, false, true);
            }
            else
            {
                popupManager.ShowPembahasan(pembahasan);
                Invoke(nameof(LanjutKeSoalBerikutnya), 2f);
            }
        }
        else
        {
            CekJawaban();
        }
    }

    /// <summary>
    /// Lanjut ke soal berikutnya. Dipanggil via Invoke setelah popup ditampilkan.
    /// </summary>
    public void LanjutKeSoalBerikutnya()
    {
        ResetAnswerState();
        popupManager.HideAllPanels();
        GenerateRandomQuestion();
    }

    /// <summary>
    /// Versi dengan parameter (bisa dipanggil langsung tanpa delay).
    /// </summary>
    public void LanjutKeSoalBerikutnya(bool isCorrect)
    {
        ResetAnswerState();
        popupManager.HideAllPanels();
        GenerateRandomQuestion();
    }

    // ---- helpers ----

    /// <summary>
    /// Dapatkan nama kategori saat ini (override di subclass jika perlu).
    /// </summary>
    protected virtual string GetKategoriName()
    {
        // Gunakan nama class kecuali di-override
        return GetType().Name.Replace("Manager", "").Replace("controller", "");
    }

    /// <summary>
    /// Dapatkan ID unik untuk soal ini (digunakan spaced repetition).
    /// </summary>
    protected virtual string GetSoalId()
    {
        return soalSaatIni.GetHashCode().ToString() + "_" + GetKategoriName();
    }

    protected void SpawnPilihan(List<TAnswer> daftarJawaban, TAnswer jawabanBenar)
    {
        string[] h = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K" };

        string[] strs = new string[daftarJawaban.Count];
        for (int i = 0; i < daftarJawaban.Count; i++)
            strs[i] = ConvertAnswerToString(daftarJawaban[i]);

        string jawabanStr = ConvertAnswerToString(jawabanBenar);
        correctAnswerValue = jawabanStr;

        for (int i = strs.Length - 1; i > 0; i--)
        {
            int r = Random.Range(0, i + 1);
            string tmp = strs[i];
            strs[i] = strs[r];
            strs[r] = tmp;
        }

        correctAnswerLabel = null;
        for (int i = 0; i < strs.Length; i++)
        {
            if (strs[i] == jawabanStr)
            {
                correctAnswerLabel = (i < h.Length) ? h[i] : null;
                break;
            }
        }

        objekPilihanAktif = itemSpawner.SpawnPilihan(strs, h, this);
    }
}