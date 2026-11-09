using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Base class untuk seluruh quiz. Semua logika UI state, health, score,
/// popup, hint/petunjuk, dan destroy pilihan sudah di sini.
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

    // Huruf label jawaban benar (A, B, ...)
    public string correctAnswerLabel;
    public string playerAnswerLabel;
    public string playerAnswerValue;
    public string soalSaatIni;
    public string langkahPenyelesaian;
    public string petunjukSoal; // Scaffolding petunjuk langkah awal

    // Nilai jawaban benar sebagai string (untuk ditampilkan di pembahasan)
    public string correctAnswerValue;

    protected List<GameObject> objekPilihanAktif = new List<GameObject>();
    protected GameObject objekTerpilih;

    private string[] huruf = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K" };
    private Button hintButton;

    protected virtual void Start()
    {
        if (checkButton != null) checkButton.onClick.AddListener(CekJawaban);
        if (undoButton != null) undoButton.onClick.AddListener(UndoJawaban);
        if (ScoreManager.instance != null) ScoreManager.instance.ResetScore();

        SetupHintButton();
        GenerateRandomQuestion();
    }

    /// <summary>
    /// Buat tombol "💡 Petunjuk" secara dinamis di samping checkButton jika belum ada.
    /// </summary>
    protected void SetupHintButton()
    {
        if (checkButton == null) return;
        Transform parent = checkButton.transform.parent;
        if (parent == null) return;

        Transform existing = parent.Find("Btn_Petunjuk");
        if (existing != null)
        {
            hintButton = existing.GetComponent<Button>();
            if (hintButton != null)
            {
                hintButton.onClick.RemoveAllListeners();
                hintButton.onClick.AddListener(TampilkanPetunjuk);
            }
            return;
        }

        GameObject hintGo = new GameObject("Btn_Petunjuk", typeof(RectTransform), typeof(Image), typeof(Button));
        hintGo.transform.SetParent(parent, false);

        RectTransform checkRt = checkButton.GetComponent<RectTransform>();
        RectTransform hintRt = hintGo.GetComponent<RectTransform>();

        hintRt.anchorMin = checkRt.anchorMin;
        hintRt.anchorMax = checkRt.anchorMax;
        hintRt.pivot = checkRt.pivot;
        hintRt.sizeDelta = new Vector2(checkRt.sizeDelta.x * 0.9f, checkRt.sizeDelta.y);
        hintRt.anchoredPosition = checkRt.anchoredPosition + new Vector2(checkRt.sizeDelta.x + 20f, 0f);

        Image img = hintGo.GetComponent<Image>();
        img.color = new Color(0.95f, 0.72f, 0.12f); // Warna Emas / Kuning terang

        GameObject textGo = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
        textGo.transform.SetParent(hintGo.transform, false);
        TextMeshProUGUI tmp = textGo.GetComponent<TextMeshProUGUI>();
        tmp.text = "💡 Petunjuk";
        tmp.fontSize = 20;
        tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;

        RectTransform textRt = textGo.GetComponent<RectTransform>();
        textRt.anchorMin = Vector2.zero;
        textRt.anchorMax = Vector2.one;
        textRt.offsetMin = Vector2.zero;
        textRt.offsetMax = Vector2.zero;

        hintButton = hintGo.GetComponent<Button>();
        hintButton.onClick.AddListener(TampilkanPetunjuk);
    }

    /// <summary>
    /// Tampilkan petunjuk langkah awal tanpa mengurangi nyawa (scaffolding).
    /// </summary>
    public void TampilkanPetunjuk()
    {
        MathSoundFX.PlayClick();
        string isiPetunjuk = !string.IsNullOrEmpty(petunjukSoal)
            ? petunjukSoal
            : (!string.IsNullOrEmpty(langkahPenyelesaian) ? langkahPenyelesaian : "Perhatikan rumus atau konsep dasar materi ini.");

        if (popupManager != null)
        {
            popupManager.ShowPembahasan("💡 PETUNJUK PENYELESAIAN:\n\n" + isiPetunjuk);
        }
    }

    public abstract void GenerateRandomQuestion();

    public abstract bool IsAnswerCorrect(string playerAnswer);

    protected virtual string ConvertAnswerToString(TAnswer answer) => answer.ToString();

    public bool TryLoadFromJson(string kategori)
    {
        SoalEntry entry = SoalJsonLoader.GetRandomSoal(kategori);
        if (entry == null) return false;

        soalSaatIni = entry.soal;
        correctAnswerValue = entry.jawaban;
        langkahPenyelesaian = entry.langkah ?? "";
        petunjukSoal = "Ingat konsep dasar kategori " + kategori;

        string[] pilihanArray = entry.pengecof != null && entry.pengecof.Length > 0
            ? entry.pengecof
            : new string[0];

        List<string> semuaPilihan = new List<string>(pilihanArray);
        semuaPilihan.Add(entry.jawaban);

        string[] h = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K" };

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

    public void SetJawabanPilihan(string pilihan, int nilai, GameObject objekYangDipilih)
    {
        MathSoundFX.PlayClick();
        playerAnswerLabel = pilihan;
        playerAnswerValue = nilai.ToString();
        objekTerpilih = objekYangDipilih;
        answerText.text = "Jawaban Anda: " + nilai;
        checkButton.interactable = true;
        undoButton.gameObject.SetActive(true);
    }

    public void SetJawabanPilihan(string pilihan, string nilai, GameObject objekYangDipilih)
    {
        MathSoundFX.PlayClick();
        playerAnswerLabel = pilihan;
        playerAnswerValue = nilai != null ? nilai : "";
        objekTerpilih = objekYangDipilih;
        answerText.text = "Jawaban Anda: " + nilai;
        checkButton.interactable = true;
        undoButton.gameObject.SetActive(true);
    }

    public void SetJawabanPilihan(string pilihan, float nilai, GameObject objekYangDipilih)
    {
        MathSoundFX.PlayClick();
        playerAnswerLabel = pilihan;
        playerAnswerValue = nilai.ToString("F1");
        objekTerpilih = objekYangDipilih;
        answerText.text = "Jawaban Anda: " + nilai.ToString("F1");
        checkButton.interactable = true;
        undoButton.gameObject.SetActive(true);
    }

    public void UndoJawaban()
    {
        MathSoundFX.PlayClick();
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

        lastWasCorrect = (!string.IsNullOrEmpty(correctAnswerLabel) && playerAnswerLabel == correctAnswerLabel)
                         || IsAnswerCorrect(playerAnswerValue)
                         || IsAnswerCorrect(playerAnswerLabel);

        if (lastWasCorrect)
        {
            MathSoundFX.PlayCorrect();
            if (ScoreManager.instance != null) ScoreManager.instance.AddScore(10);

            string pesan = $"Jawaban kamu '{playerAnswerLabel}' ({correctAnswerLabel}) adalah benar! " +
                           $"Jawaban: {correctAnswerValue}. Poin bertambah.";
            popupManager.ShowPopupBenar(pesan);
            Invoke(nameof(LanjutKeSoalBerikutnya), 2f);
        }
        else
        {
            MathSoundFX.PlayWrong();
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

    public virtual void OnTimeUp()
    {
        lastWasCorrect = false;

        if (string.IsNullOrEmpty(playerAnswerLabel))
        {
            MathSoundFX.PlayWrong();
            foreach (var obj in objekPilihanAktif)
                if (obj != null) Destroy(obj);
            objekPilihanAktif.Clear();

            bool isGameOver = healthManager.ReduceHealth();
            string pembahasan = $"PEMBAHASAN (WAKTU HABIS)\n\nSoal: {soalSaatIni}\n" +
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

    public void LanjutKeSoalBerikutnya()
    {
        ResetAnswerState();
        popupManager.HideAllPanels();
        GenerateRandomQuestion();
    }

    public void LanjutKeSoalBerikutnya(bool isCorrect)
    {
        ResetAnswerState();
        popupManager.HideAllPanels();
        GenerateRandomQuestion();
    }

    protected virtual string GetKategoriName()
    {
        return GetType().Name.Replace("Manager", "").Replace("controller", "");
    }

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
