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

        StitchUIThemeApplier.EnhanceQuizHUD(this);
        SetupStitchTopHUD();
        SetupHintButton();
        SetupScratchpadButton();
        GenerateRandomQuestion();
        ResetAnswerState();
    }

    /// <summary>
    /// Injeksi / penyesuaian Top HUD Stitch (Kembali, 3 Hearts, Skor, High Score).
    /// </summary>
    protected void SetupStitchTopHUD()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas == null) canvas = FindObjectOfType<Canvas>();
        if (canvas == null) return;

        Transform existing = canvas.transform.Find("StitchQuizTopHUD");
        if (existing != null) return;

        GameObject topHudGo = new GameObject("StitchQuizTopHUD", typeof(RectTransform), typeof(Image));
        topHudGo.transform.SetParent(canvas.transform, false);

        RectTransform rt = topHudGo.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.04f, 0.91f);
        rt.anchorMax = new Vector2(0.96f, 0.98f);
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        Image img = topHudGo.GetComponent<Image>();
        StitchUIThemeApplier.ApplyHudBarStyle(img);

        // Tombol Kembali
        GameObject backBtnGo = new GameObject("Btn_Kembali", typeof(RectTransform), typeof(Image), typeof(Button));
        backBtnGo.transform.SetParent(topHudGo.transform, false);
        RectTransform brt = backBtnGo.GetComponent<RectTransform>();
        brt.anchorMin = new Vector2(0.02f, 0.15f);
        brt.anchorMax = new Vector2(0.24f, 0.85f);
        brt.offsetMin = Vector2.zero;
        brt.offsetMax = Vector2.zero;

        Image bImg = backBtnGo.GetComponent<Image>();
        bImg.sprite = StitchUIThemeApplier.GetPillSprite(StitchUIThemeApplier.SurfaceLowest, StitchUIThemeApplier.PrimaryBlueLight, 1.5f);
        bImg.type = Image.Type.Sliced;

        GameObject bTextGo = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
        bTextGo.transform.SetParent(backBtnGo.transform, false);
        RectTransform btRt = bTextGo.GetComponent<RectTransform>();
        btRt.anchorMin = Vector2.zero;
        btRt.anchorMax = Vector2.one;
        btRt.offsetMin = Vector2.zero;
        btRt.offsetMax = Vector2.zero;
        TextMeshProUGUI bTmp = bTextGo.GetComponent<TextMeshProUGUI>();
        bTmp.text = "← Menu";
        bTmp.fontSize = 15;
        bTmp.fontStyle = FontStyles.Bold;
        bTmp.alignment = TextAlignmentOptions.Center;
        bTmp.color = StitchUIThemeApplier.OnSurface;

        Button backBtn = backBtnGo.GetComponent<Button>();
        backBtn.onClick.AddListener(() =>
        {
            MathSoundFX.PlayClick();
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        });

        // Skor Pill
        GameObject scoreGo = new GameObject("ScorePill", typeof(RectTransform), typeof(Image));
        scoreGo.transform.SetParent(topHudGo.transform, false);
        RectTransform srt = scoreGo.GetComponent<RectTransform>();
        srt.anchorMin = new Vector2(0.70f, 0.15f);
        srt.anchorMax = new Vector2(0.98f, 0.85f);
        srt.offsetMin = Vector2.zero;
        srt.offsetMax = Vector2.zero;

        Image sImg = scoreGo.GetComponent<Image>();
        sImg.sprite = StitchUIThemeApplier.GetPillSprite(StitchUIThemeApplier.TertiaryAmberLight, StitchUIThemeApplier.TertiaryAmber, 1.5f);
        sImg.type = Image.Type.Sliced;

        GameObject sTextGo = new GameObject("ScoreText", typeof(RectTransform), typeof(TextMeshProUGUI));
        sTextGo.transform.SetParent(scoreGo.transform, false);
        RectTransform stRt = sTextGo.GetComponent<RectTransform>();
        stRt.anchorMin = Vector2.zero;
        stRt.anchorMax = Vector2.one;
        stRt.offsetMin = Vector2.zero;
        stRt.offsetMax = Vector2.zero;
        TextMeshProUGUI sTmp = sTextGo.GetComponent<TextMeshProUGUI>();
        sTmp.text = "★ Skor: 0";
        sTmp.fontSize = 15;
        sTmp.fontStyle = FontStyles.Bold;
        sTmp.alignment = TextAlignmentOptions.Center;
        sTmp.color = StitchUIThemeApplier.TertiaryContainer;
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
        hintRt.sizeDelta = new Vector2(checkRt.sizeDelta.x * 0.85f, checkRt.sizeDelta.y);
        hintRt.anchoredPosition = checkRt.anchoredPosition + new Vector2(checkRt.sizeDelta.x + 15f, 0f);

        Image img = hintGo.GetComponent<Image>();
        img.sprite = StitchUIThemeApplier.GetPillSprite(StitchUIThemeApplier.TertiaryAmberLight, StitchUIThemeApplier.TertiaryAmber, 1.5f);
        img.type = Image.Type.Sliced;

        GameObject textGo = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
        textGo.transform.SetParent(hintGo.transform, false);
        TextMeshProUGUI tmp = textGo.GetComponent<TextMeshProUGUI>();
        tmp.text = "💡 Petunjuk";
        tmp.fontSize = 18;
        tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = StitchUIThemeApplier.TertiaryContainer;

        RectTransform textRt = textGo.GetComponent<RectTransform>();
        textRt.anchorMin = Vector2.zero;
        textRt.anchorMax = Vector2.one;
        textRt.offsetMin = Vector2.zero;
        textRt.offsetMax = Vector2.zero;

        hintButton = hintGo.GetComponent<Button>();
        hintButton.onClick.AddListener(TampilkanPetunjuk);
    }

    /// <summary>
    /// Buat tombol "✏️ Coretan" Stitch secara dinamis untuk membuka Scratchpad DrawingController.
    /// </summary>
    protected void SetupScratchpadButton()
    {
        if (checkButton == null) return;
        Transform parent = checkButton.transform.parent;
        if (parent == null) return;

        Transform existing = parent.Find("Btn_Coretan");
        if (existing != null) return;

        GameObject drawBtnGo = new GameObject("Btn_Coretan", typeof(RectTransform), typeof(Image), typeof(Button));
        drawBtnGo.transform.SetParent(parent, false);

        RectTransform checkRt = checkButton.GetComponent<RectTransform>();
        RectTransform drawRt = drawBtnGo.GetComponent<RectTransform>();

        drawRt.anchorMin = checkRt.anchorMin;
        drawRt.anchorMax = checkRt.anchorMax;
        drawRt.pivot = checkRt.pivot;
        drawRt.sizeDelta = new Vector2(checkRt.sizeDelta.x * 0.85f, checkRt.sizeDelta.y);
        drawRt.anchoredPosition = checkRt.anchoredPosition - new Vector2(checkRt.sizeDelta.x + 15f, 0f);

        Image img = drawBtnGo.GetComponent<Image>();
        img.sprite = StitchUIThemeApplier.GetPillSprite(StitchUIThemeApplier.PrimaryBlueSubtle, StitchUIThemeApplier.PrimaryBlue, 1.5f);
        img.type = Image.Type.Sliced;

        GameObject textGo = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
        textGo.transform.SetParent(drawBtnGo.transform, false);
        TextMeshProUGUI tmp = textGo.GetComponent<TextMeshProUGUI>();
        tmp.text = "✏️ Coretan";
        tmp.fontSize = 18;
        tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = StitchUIThemeApplier.PrimaryBlueDeep;

        RectTransform textRt = textGo.GetComponent<RectTransform>();
        textRt.anchorMin = Vector2.zero;
        textRt.anchorMax = Vector2.one;
        textRt.offsetMin = Vector2.zero;
        textRt.offsetMax = Vector2.zero;

        Button drawBtn = drawBtnGo.GetComponent<Button>();
        drawBtn.onClick.AddListener(() =>
        {
            MathSoundFX.PlayClick();
            DrawingController dc = FindObjectOfType<DrawingController>();
            if (dc != null) dc.OpenCanvas();
        });
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

    private void UpdateStateD2()
    {
        if (answerText != null)
        {
            answerText.text = $"✓ Jawaban Terpilih: <b>{playerAnswerValue}</b> (Objek {playerAnswerLabel} Terkunci!)";
            answerText.color = Color.white;
            Transform strip = answerText.transform.parent;
            if (strip != null)
            {
                Image stripImg = strip.GetComponent<Image>();
                if (stripImg != null)
                    StitchUIThemeApplier.ApplyCardStyle(stripImg, StitchUIThemeApplier.SecondaryEmerald, StitchUIThemeApplier.SecondaryEmeraldDark, 16f, 2f);
            }
        }
        if (checkButton != null)
        {
            checkButton.interactable = true;
            TMP_Text btnTxt = checkButton.GetComponentInChildren<TMP_Text>();
            if (btnTxt != null) btnTxt.text = $"Cek Jawaban ({playerAnswerValue})";
        }
        if (undoButton != null) undoButton.gameObject.SetActive(true);
    }

    public void SetJawabanPilihan(string pilihan, int nilai, GameObject objekYangDipilih)
    {
        MathSoundFX.PlayClick();
        playerAnswerLabel = pilihan;
        playerAnswerValue = nilai.ToString();
        objekTerpilih = objekYangDipilih;
        UpdateStateD2();
    }

    public void SetJawabanPilihan(string pilihan, string nilai, GameObject objekYangDipilih)
    {
        MathSoundFX.PlayClick();
        playerAnswerLabel = pilihan;
        playerAnswerValue = nilai != null ? nilai : "";
        objekTerpilih = objekYangDipilih;
        UpdateStateD2();
    }

    public void SetJawabanPilihan(string pilihan, float nilai, GameObject objekYangDipilih)
    {
        MathSoundFX.PlayClick();
        playerAnswerLabel = pilihan;
        playerAnswerValue = nilai.ToString("F1");
        objekTerpilih = objekYangDipilih;
        UpdateStateD2();
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
        if (answerText != null)
        {
            answerText.text = "🚶 Berjalanlah mendekati objek jawaban 3D lalu sentuh untuk memilih!";
            answerText.color = StitchUIThemeApplier.PrimaryBlueDeep;
            Transform strip = answerText.transform.parent;
            if (strip != null)
            {
                Image stripImg = strip.GetComponent<Image>();
                if (stripImg != null)
                    StitchUIThemeApplier.ApplyCardStyle(stripImg, StitchUIThemeApplier.SurfaceLow, StitchUIThemeApplier.PrimaryBlueLight, 16f, 1f);
            }
        }
        if (checkButton != null)
        {
            checkButton.interactable = false;
            TMP_Text btnTxt = checkButton.GetComponentInChildren<TMP_Text>();
            if (btnTxt != null) btnTxt.text = "Dekati Objek Dulu";
        }
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
