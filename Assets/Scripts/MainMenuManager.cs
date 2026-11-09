using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    public GameObject soalSubCategoryPanel_Lainnya; // panel fallback untuk kategori baru

    [Header("Navigasi")]
    public GameNavigation gameNavigation;

    private Dictionary<string, GameObject> dynamicPanels = new Dictionary<string, GameObject>();

    void Start()
    {
        SetupGradeLevelSelector();

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

    private void SetupGradeLevelSelector()
    {
        if (soalCategoryPanel == null) return;
        Transform existing = soalCategoryPanel.transform.Find("GradeLevelSelectorBar");
        if (existing != null) return;

        // Container GameObject with RectTransform & HorizontalLayoutGroup
        GameObject bar = new GameObject("GradeLevelSelectorBar", typeof(RectTransform), typeof(HorizontalLayoutGroup));
        bar.transform.SetParent(soalCategoryPanel.transform, false);

        RectTransform rt = bar.GetComponent<RectTransform>();
        // Posisikan di bagian atas panel Soal
        rt.anchorMin = new Vector2(0.08f, 0.88f);
        rt.anchorMax = new Vector2(0.92f, 0.96f);
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        HorizontalLayoutGroup hlg = bar.GetComponent<HorizontalLayoutGroup>();
        hlg.spacing = 15;
        hlg.childAlignment = TextAnchor.MiddleCenter;
        hlg.childControlWidth = true;
        hlg.childControlHeight = true;
        hlg.childForceExpandWidth = true;
        hlg.childForceExpandHeight = true;

        List<Button> buttons = new List<Button>();
        List<Image> buttonImages = new List<Image>();
        List<TMP_Text> buttonTexts = new List<TMP_Text>();

        GradePhase[] phases = new GradePhase[] { GradePhase.FaseA, GradePhase.FaseB, GradePhase.FaseC };
        string[] labels = new string[] { "Kelas 1–2 (Fase A)", "Kelas 3–4 (Fase B)", "Kelas 5–6 (Fase C)" };

        for (int i = 0; i < phases.Length; i++)
        {
            GradePhase p = phases[i];
            GameObject btnGo = new GameObject("Btn_" + p, typeof(RectTransform), typeof(Image), typeof(Button));
            btnGo.transform.SetParent(bar.transform, false);

            Image img = btnGo.GetComponent<Image>();
            Button btn = btnGo.GetComponent<Button>();

            GameObject textGo = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
            textGo.transform.SetParent(btnGo.transform, false);
            TextMeshProUGUI tmp = textGo.GetComponent<TextMeshProUGUI>();
            tmp.text = labels[i];
            tmp.fontSize = 20;
            tmp.fontStyle = FontStyles.Bold;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.white;

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
                UpdateGradeButtonStyles(phases, buttonImages, buttonTexts);
            });
        }

        UpdateGradeButtonStyles(phases, buttonImages, buttonTexts);
    }

    private void UpdateGradeButtonStyles(GradePhase[] phases, List<Image> buttonImages, List<TMP_Text> buttonTexts)
    {
        GradePhase current = GradeLevelConfig.CurrentPhase;
        Color activeColor = new Color(0.12f, 0.58f, 0.95f, 1f); // Vibrant blue
        Color inactiveColor = new Color(0.25f, 0.28f, 0.35f, 0.85f); // Slate gray

        for (int i = 0; i < phases.Length; i++)
        {
            if (phases[i] == current)
            {
                buttonImages[i].color = activeColor;
                buttonTexts[i].color = Color.white;
            }
            else
            {
                buttonImages[i].color = inactiveColor;
                buttonTexts[i].color = new Color(0.82f, 0.85f, 0.9f, 0.85f);
            }
        }
    }

    private GameObject FindPanelByName(string name)
    {
        if (soalCategoryPanel != null && soalCategoryPanel.name == name) return soalCategoryPanel;
        if (materiCategoryPanel != null && materiCategoryPanel.name == name) return materiCategoryPanel;

        if (soalSubCategoryPanel_PerhitunganDasar != null &&
            soalSubCategoryPanel_PerhitunganDasar.name == name)
            return soalSubCategoryPanel_PerhitunganDasar;
        if (soalSubCategoryPanel_PengolahanData != null &&
            soalSubCategoryPanel_PengolahanData.name == name)
            return soalSubCategoryPanel_PengolahanData;
        if (materiSubCategoryPanel_PerhitunganDasar != null &&
            materiSubCategoryPanel_PerhitunganDasar.name == name)
            return materiSubCategoryPanel_PerhitunganDasar;
        if (materiSubCategoryPanel_PengolahanData != null &&
            materiSubCategoryPanel_PengolahanData.name == name)
            return materiSubCategoryPanel_PengolahanData;

        Debug.LogWarning("Panel dengan nama '" + name + "' tidak ditemukan! Kembali ke Start Panel.");
        return startPanel;
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

    /// <summary>
    /// Buka kategori atau langsung load scene.
    /// </summary>
    public void OpenSoalSubCategory(string category)
    {
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
        if (category == "PengolahanData")
            ShowPanel(materiSubCategoryPanel_PengolahanData);
        else if (category == "PerhitunganDasar")
            ShowPanel(materiSubCategoryPanel_PerhitunganDasar);
    }

    /// <summary>
    /// Buka kategori soal baru (dinamis). Jika panel belum di-assign di Inspector,
    /// gunakan panel fallback lainnya.
    /// </summary>
    public void OpenNewCategory(string categoryName)
    {
        GameObject panel = soalSubCategoryPanel_Lainnya != null ? soalSubCategoryPanel_Lainnya : startPanel;

        if (!dynamicPanels.ContainsKey(categoryName))
            dynamicPanels[categoryName] = panel;

        ShowPanel(panel);
        Debug.Log("[MainMenu] Membuka kategori baru: " + categoryName);
    }

    /// <summary>
    /// Kembali dari panel kategori dinamis ke kategori soal utama.
    /// </summary>
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

    /// <summary>
    /// Dapatkan daftar semua kategori yang tersedia.
    /// </summary>
    public string[] GetAllKategori()
    {
        return KATEGORI;
    }
}
