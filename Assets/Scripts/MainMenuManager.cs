using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manajer MainMenu: navigasi antar panel, selektor jenjang kelas (Fase A, B, C SD),
/// dan penampil badge bintang penguasaan materi (Mastery Stars).
/// </summary>
public class MainMenuManager : MonoBehaviour
{
    [Header("Panel Utama")]
    public GameObject startPanel;
    public GameObject soalCategoryPanel;
    public GameObject materiCategoryPanel;

    [Header("Panel Sub Kategori Soal")]
    public GameObject soalSubCategoryPanel_PerhitunganDasar;
    public GameObject soalSubCategoryPanel_PengolahanData;

    [Header("Panel Sub Kategori Materi")]
    public GameObject materiSubCategoryPanel_PerhitunganDasar;
    public GameObject materiSubCategoryPanel_PengolahanData;

    [Header("Panel Konten Materi di Scene Ini")]
    public GameObject materiPanel_Pecahan;

    [Header("Panel Kategori Baru (opsional - jika tidak di-assign, akan dibuat dinamis)")]
    public GameObject soalSubCategoryPanel_Lainnya;

    [Header("Navigasi")]
    public GameNavigation gameNavigation;

    private Dictionary<string, GameObject> dynamicPanels = new Dictionary<string, GameObject>();

    void Start()
    {
        // Terapkan tema Stitch UI ke seluruh panel utama dan sub-panel
        StitchUIThemeApplier.EnhanceMainMenu(this);

        // Pasang header status Stitch (Total Bintang & Identitas Siswa)
        SetupStitchHeader();

        // Pasang selektor jenjang kelas di panel Soal dan Materi
        SetupGradeLevelSelector(soalCategoryPanel);
        SetupGradeLevelSelector(materiCategoryPanel);

        // Perbarui badge bintang di tombol-tombol kuis
        RefreshMasteryBadges();

        if (MenuMemory.TargetPanelOnLoad != PanelTarget.None)
        {
            GameObject panelToActivate = FindPanelByEnum(MenuMemory.TargetPanelOnLoad);
            ShowPanel(panelToActivate);
            MenuMemory.TargetPanelOnLoad = PanelTarget.None;
        }
        else
        {
            ShowPanel(startPanel);
        }
    }

    /// <summary>
    /// Injeksi top header bar Stitch (Total Bintang Penguasaan ★ 24/27 & Identitas ARMath).
    /// </summary>
    private void SetupStitchHeader()
    {
        if (startPanel == null) return;
        Transform existing = startPanel.transform.Find("StitchTopHeaderBar");
        if (existing != null) return;

        GameObject headerGo = new GameObject("StitchTopHeaderBar", typeof(RectTransform), typeof(Image));
        headerGo.transform.SetParent(startPanel.transform, false);

        RectTransform hrt = headerGo.GetComponent<RectTransform>();
        hrt.anchorMin = new Vector2(0.04f, 0.90f);
        hrt.anchorMax = new Vector2(0.96f, 0.98f);
        hrt.offsetMin = Vector2.zero;
        hrt.offsetMax = Vector2.zero;

        Image hImg = headerGo.GetComponent<Image>();
        StitchUIThemeApplier.ApplyHudBarStyle(hImg);

        // Sub-elemen: Logo & Judul
        GameObject brandGo = new GameObject("BrandTitle", typeof(RectTransform), typeof(TextMeshProUGUI));
        brandGo.transform.SetParent(headerGo.transform, false);
        RectTransform brt = brandGo.GetComponent<RectTransform>();
        brt.anchorMin = new Vector2(0.04f, 0.1f);
        brt.anchorMax = new Vector2(0.60f, 0.9f);
        brt.offsetMin = Vector2.zero;
        brt.offsetMax = Vector2.zero;

        TextMeshProUGUI brandTmp = brandGo.GetComponent<TextMeshProUGUI>();
        brandTmp.text = "<color=#1D8FE1><b>ARMath</b></color> <size=14><color=#404751>• Petualangan AR</color></size>";
        brandTmp.fontSize = 20;
        brandTmp.alignment = TextAlignmentOptions.MidlineLeft;

        // Sub-elemen: Total Bintang Pill
        GameObject starPillGo = new GameObject("StarPill", typeof(RectTransform), typeof(Image));
        starPillGo.transform.SetParent(headerGo.transform, false);
        RectTransform spRt = starPillGo.GetComponent<RectTransform>();
        spRt.anchorMin = new Vector2(0.68f, 0.15f);
        spRt.anchorMax = new Vector2(0.96f, 0.85f);
        spRt.offsetMin = Vector2.zero;
        spRt.offsetMax = Vector2.zero;

        Image spImg = starPillGo.GetComponent<Image>();
        spImg.sprite = StitchUIThemeApplier.GetPillSprite(StitchUIThemeApplier.TertiaryAmberLight, StitchUIThemeApplier.TertiaryAmber, 1.5f);
        spImg.type = Image.Type.Sliced;

        GameObject starTextGo = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
        starTextGo.transform.SetParent(starPillGo.transform, false);
        RectTransform stRt = starTextGo.GetComponent<RectTransform>();
        stRt.anchorMin = Vector2.zero;
        stRt.anchorMax = Vector2.one;
        stRt.offsetMin = Vector2.zero;
        stRt.offsetMax = Vector2.zero;

        TextMeshProUGUI starTmp = starTextGo.GetComponent<TextMeshProUGUI>();
        starTmp.text = GetTotalStarsString();
        starTmp.fontSize = 15;
        starTmp.fontStyle = FontStyles.Bold;
        starTmp.alignment = TextAlignmentOptions.Center;
        starTmp.color = StitchUIThemeApplier.TertiaryContainer;
    }

    private string GetTotalStarsString()
    {
        int earned = 0;
        int max = KATEGORI.Length * 3; // 27 stars
        foreach (string cat in KATEGORI)
        {
            earned += ScoreManager.GetStars("Soal" + cat);
        }
        return $"★ {earned}/{max}";
    }

    /// <summary>
    /// Injeksi bar selektor jenjang kelas uGUI Stitch (Fase A, Fase B, Fase C).
    /// </summary>
    private void SetupGradeLevelSelector(GameObject parentPanel)
    {
        if (parentPanel == null) return;
        Transform existing = parentPanel.transform.Find("GradeLevelSelectorBar");
        if (existing != null) return;

        GameObject bar = new GameObject("GradeLevelSelectorBar", typeof(RectTransform), typeof(Image), typeof(HorizontalLayoutGroup));
        bar.transform.SetParent(parentPanel.transform, false);

        RectTransform rt = bar.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.06f, 0.87f);
        rt.anchorMax = new Vector2(0.94f, 0.96f);
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        Image barImg = bar.GetComponent<Image>();
        barImg.sprite = StitchUIThemeApplier.GetRoundedRectSprite(20f, StitchUIThemeApplier.SurfaceContainer, StitchUIThemeApplier.PrimaryBlueLight, 1.5f);
        barImg.type = Image.Type.Sliced;

        HorizontalLayoutGroup hlg = bar.GetComponent<HorizontalLayoutGroup>();
        hlg.spacing = 8;
        hlg.padding = new RectOffset(6, 6, 6, 6);
        hlg.childAlignment = TextAnchor.MiddleCenter;
        hlg.childControlWidth = true;
        hlg.childControlHeight = true;
        hlg.childForceExpandWidth = true;
        hlg.childForceExpandHeight = true;

        List<Button> buttons = new List<Button>();
        List<Image> buttonImages = new List<Image>();
        List<TMP_Text> buttonTexts = new List<TMP_Text>();

        GradePhase[] phases = new GradePhase[] { GradePhase.FaseA, GradePhase.FaseB, GradePhase.FaseC };
        string[] titles = new string[] { "Fase A", "Fase B", "Fase C" };
        string[] subtitles = new string[] { "Kelas 1–2 SD", "Kelas 3–4 SD", "Kelas 5–6 SD" };

        for (int i = 0; i < phases.Length; i++)
        {
            GradePhase p = phases[i];
            GameObject btnGo = new GameObject("Btn_" + p, typeof(RectTransform), typeof(Image), typeof(Button));
            btnGo.transform.SetParent(bar.transform, false);

            Image img = btnGo.GetComponent<Image>();
            img.type = Image.Type.Sliced;
            Button btn = btnGo.GetComponent<Button>();

            GameObject textGo = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
            textGo.transform.SetParent(btnGo.transform, false);
            TextMeshProUGUI tmp = textGo.GetComponent<TextMeshProUGUI>();
            tmp.text = $"<b>{titles[i]}</b>\n<size=11>{subtitles[i]}</size>";
            tmp.fontSize = 15;
            tmp.alignment = TextAlignmentOptions.Center;

            RectTransform textRt = textGo.GetComponent<RectTransform>();
            textRt.anchorMin = Vector2.zero;
            textRt.anchorMax = Vector2.one;
            textRt.offsetMin = Vector2.zero;
            textRt.offsetMax = Vector2.zero;

            buttons.Add(btn);
            buttonImages.Add(img);
            buttonTexts.Add(tmp);

            btn.onClick.AddListener(() =>
            {
                GradeLevelConfig.CurrentPhase = p;
                MathSoundFX.PlayClick();
                UpdateGradeButtonStyles(phases, buttonImages, buttonTexts);
            });
        }

        UpdateGradeButtonStyles(phases, buttonImages, buttonTexts);
    }

    private void UpdateGradeButtonStyles(GradePhase[] phases, List<Image> buttonImages, List<TMP_Text> buttonTexts)
    {
        GradePhase current = GradeLevelConfig.CurrentPhase;

        for (int i = 0; i < phases.Length; i++)
        {
            if (phases[i] == current)
            {
                // Active pill Stitch: PrimaryBlueDeep with white text
                buttonImages[i].sprite = StitchUIThemeApplier.GetRoundedRectSprite(16f, StitchUIThemeApplier.PrimaryBlueDeep, StitchUIThemeApplier.PrimaryBlueDeep, 0f);
                buttonTexts[i].color = Color.white;
            }
            else
            {
                // Inactive pill Stitch: SurfaceLowest with OnSurfaceVariant text
                buttonImages[i].sprite = StitchUIThemeApplier.GetRoundedRectSprite(16f, StitchUIThemeApplier.SurfaceLowest, StitchUIThemeApplier.PrimaryBlueLight, 1f);
                buttonTexts[i].color = StitchUIThemeApplier.OnSurfaceVariant;
            }
        }
    }

    /// <summary>
    /// Perbarui kartu topik kuis dengan layout Stitch: ★★★ rating indicator dan badge skor terbaik.
    /// </summary>
    public void RefreshMasteryBadges()
    {
        GameObject[] panels = new GameObject[] {
            soalCategoryPanel,
            soalSubCategoryPanel_PerhitunganDasar,
            soalSubCategoryPanel_PengolahanData
        };

        foreach (var p in panels)
        {
            if (p == null) continue;
            Button[] btns = p.GetComponentsInChildren<Button>(true);
            foreach (var b in btns)
            {
                if (b.name.StartsWith("Btn_Fase") || b.name == "Btn_Kembali") continue;

                TMP_Text tmp = b.GetComponentInChildren<TMP_Text>();
                if (tmp == null) continue;

                string currentText = tmp.text;
                foreach (string cat in KATEGORI)
                {
                    if (currentText.Contains(cat) || b.name.Contains(cat))
                    {
                        string sceneName = "Soal" + cat;
                        int stars = ScoreManager.GetStars(sceneName);
                        int hs = ScoreManager.GetHighScore(sceneName);

                        string starsDisplay = GetStarGlyphs(stars);
                        tmp.text = $"<b>{cat}</b>\n<color=#F59E0B><size=20>{starsDisplay}</size></color>\n<size=12><color=#246EA6>Skor: {hs}</color></size>";
                        break;
                    }
                }
            }
        }
    }

    private string GetStarGlyphs(int stars)
    {
        if (stars == 3) return "★★★";
        if (stars == 2) return "★★<color=#CBD5E1>★</color>";
        if (stars == 1) return "★<color=#CBD5E1>★★</color>";
        return "<color=#CBD5E1>★★★</color>";
    }

    public void ShowPanel(GameObject panelToShow)
    {
        if (startPanel) startPanel.SetActive(false);
        if (soalCategoryPanel) soalCategoryPanel.SetActive(false);
        if (soalSubCategoryPanel_PerhitunganDasar) soalSubCategoryPanel_PerhitunganDasar.SetActive(false);
        if (soalSubCategoryPanel_PengolahanData) soalSubCategoryPanel_PengolahanData.SetActive(false);
        if (materiCategoryPanel) materiCategoryPanel.SetActive(false);
        if (materiSubCategoryPanel_PerhitunganDasar) materiSubCategoryPanel_PerhitunganDasar.SetActive(false);
        if (materiSubCategoryPanel_PengolahanData) materiSubCategoryPanel_PengolahanData.SetActive(false);

        if (panelToShow)
        {
            panelToShow.SetActive(true);
            RefreshMasteryBadges();
        }
    }

    private GameObject FindPanelByEnum(PanelTarget target)
    {
        switch (target)
        {
            case PanelTarget.Start: return startPanel;
            case PanelTarget.Soal: return soalCategoryPanel;
            case PanelTarget.PerhitunganDasar: return soalSubCategoryPanel_PerhitunganDasar;
            case PanelTarget.PengolahanData: return soalSubCategoryPanel_PengolahanData;
            case PanelTarget.Materi: return materiCategoryPanel;
            case PanelTarget.Materi_PerhitunganDasar: return materiSubCategoryPanel_PerhitunganDasar;
            case PanelTarget.Materi_PengolahanData: return materiSubCategoryPanel_PengolahanData;
            default:
                Debug.LogWarning("Target panel " + target + " tidak dikenali. Kembali ke Start Panel.");
                return startPanel;
            }
    }

    public void OpenStart() => ShowPanel(startPanel);
    public void OpenSoalCategory() => ShowPanel(soalCategoryPanel);
    public void OpenMateriCategory() => ShowPanel(materiCategoryPanel);

    public void BackToSoalCategory() => ShowPanel(soalCategoryPanel);
    public void BackToMateriCategory() => ShowPanel(materiCategoryPanel);

    public void OpenSoalSubCategory(string category)
    {
        MathSoundFX.PlayClick();
        if (gameNavigation != null)
        {
            string sceneName = "Soal" + category;
            gameNavigation.LoadGameScene(sceneName);
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Soal" + category);
        }
    }

    public void OpenMateriSubCategory(string category)
    {
        MathSoundFX.PlayClick();
        if (category == "PengolahanData")
            ShowPanel(materiSubCategoryPanel_PengolahanData);
        else if (category == "PerhitunganDasar")
            ShowPanel(materiSubCategoryPanel_PerhitunganDasar);
    }

    public void OpenNewCategory(string categoryName)
    {
        MathSoundFX.PlayClick();
        GameObject panel = soalSubCategoryPanel_Lainnya != null ? soalSubCategoryPanel_Lainnya : startPanel;

        if (!dynamicPanels.ContainsKey(categoryName))
            dynamicPanels[categoryName] = panel;

        ShowPanel(panel);
    }

    public void BackFromDynamicCategory()
    {
        ShowPanel(soalCategoryPanel);
        dynamicPanels.Clear();
    }

    public static readonly string[] KATEGORI = new string[]
    {
        "Penjumlahan", "Pengurangan", "Perkalian", "Pembagian",
        "Pecahan", "BangunRuang", "Rata-Rata", "Median", "Modus"
    };

    public string[] GetAllKategori()
    {
        return KATEGORI;
    }
}
