using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Centralized Stitch Design System Applier for Mathlfix (Project ID: 756251947286196394).
/// Menyediakan color tokens resmi, generator Sprite 9-slice procedural (anti-aliased rounded corners & borders),
/// dan fungsi styling otomatis untuk uGUI Canvas di MainMenu, Materi*, dan Soal*.
/// </summary>
public class StitchUIThemeApplier : MonoBehaviour
{
    public static StitchUIThemeApplier Instance { get; private set; }

    #region Color Tokens (Google Stitch Project 756251947286196394)
    public static readonly Color PrimaryBlue          = new Color(0.114f, 0.561f, 0.882f, 1f); // #1D8FE1
    public static readonly Color PrimaryBlueDeep      = new Color(0.141f, 0.431f, 0.651f, 1f); // #246EA6
    public static readonly Color PrimaryBlueSubtle    = new Color(0.937f, 0.965f, 1.000f, 1f); // #EFF6FF
    public static readonly Color PrimaryBlueLight     = new Color(0.749f, 0.859f, 0.996f, 1f); // #BFDBFE

    public static readonly Color SecondaryEmerald     = new Color(0.133f, 0.773f, 0.369f, 1f); // #22C55E
    public static readonly Color SecondaryEmeraldDark = new Color(0.082f, 0.502f, 0.239f, 1f); // #15803D
    public static readonly Color SecondaryContainer   = new Color(0.420f, 1.000f, 0.561f, 1f); // #6BFF8F

    public static readonly Color TertiaryAmber        = new Color(0.961f, 0.620f, 0.043f, 1f); // #F59E0B
    public static readonly Color TertiaryAmberLight   = new Color(0.996f, 0.953f, 0.780f, 1f); // #FEF3C7
    public static readonly Color TertiaryContainer    = new Color(0.639f, 0.404f, 0.000f, 1f); // #A36700

    public static readonly Color CoralRed             = new Color(0.937f, 0.267f, 0.267f, 1f); // #EF4444
    public static readonly Color CoralRedSubtle       = new Color(0.996f, 0.949f, 0.949f, 1f); // #FEF2F2

    public static readonly Color SurfaceHud           = new Color(0.973f, 0.980f, 0.988f, 0.90f); // rgba(248, 250, 252, 0.90)
    public static readonly Color SurfaceCanvas        = new Color(0.941f, 0.969f, 1.000f, 1f); // #F0F7FF
    public static readonly Color SurfaceLowest        = new Color(1.000f, 1.000f, 1.000f, 1f); // #FFFFFF
    public static readonly Color SurfaceLow           = new Color(0.941f, 0.953f, 1.000f, 1f); // #F0F3FF
    public static readonly Color SurfaceContainer     = new Color(0.906f, 0.933f, 1.000f, 1f); // #E7EEFF
    public static readonly Color SurfaceHigh          = new Color(0.871f, 0.910f, 1.000f, 1f); // #DEE8FF

    public static readonly Color OnSurface            = new Color(0.067f, 0.110f, 0.176f, 1f); // #111C2D
    public static readonly Color OnSurfaceVariant     = new Color(0.251f, 0.278f, 0.318f, 1f); // #404751
    public static readonly Color OutlineVariant       = new Color(0.749f, 0.780f, 0.827f, 1f); // #BFC7D3
    #endregion

    // Cache procedural sprites agar alokasi memori minimal & cepat
    private static readonly Dictionary<string, Sprite> spriteCache = new Dictionary<string, Sprite>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    #region Procedural 9-Sliced Sprite Generator (Zero External Textures)
    /// <summary>
    /// Membuat atau mengambil dari cache Sprite 9-slice rounded rectangle dengan border stroke & anti-aliasing.
    /// </summary>
    public static Sprite GetRoundedRectSprite(float cornerRadius = 20f, Color? fillColor = null, Color? strokeColor = null, float strokeWidth = 0f)
    {
        Color fill = fillColor ?? SurfaceLowest;
        Color stroke = strokeColor ?? PrimaryBlueLight;

        int r = Mathf.Clamp(Mathf.RoundToInt(cornerRadius), 4, 48);
        int sw = Mathf.Clamp(Mathf.RoundToInt(strokeWidth), 0, 8);
        int pad = 2; // anti-aliasing margin
        int size = (r + pad) * 2 + 2; // minimal texture size for clean 9-slice

        string key = $"{r}_{sw}_{fill.r:F2}_{fill.g:F2}_{fill.b:F2}_{fill.a:F2}_{stroke.r:F2}_{stroke.g:F2}_{stroke.b:F2}_{stroke.a:F2}";
        if (spriteCache.TryGetValue(key, out Sprite cached))
            return cached;

        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;
        tex.wrapMode = TextureWrapMode.Clamp;

        Color[] pixels = new Color[size * size];
        float centerOffset = r + pad;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                // Tentukan quadrant corner
                float cx = (x < centerOffset) ? centerOffset : size - 1 - centerOffset;
                float cy = (y < centerOffset) ? centerOffset : size - 1 - centerOffset;

                float dx = Mathf.Abs(x - cx);
                float dy = Mathf.Abs(y - cy);
                float dist = Mathf.Sqrt(dx * dx + dy * dy);

                // Anti-aliased outer edge
                float alphaOuter = Mathf.Clamp01(r + 0.5f - dist);

                if (alphaOuter <= 0f)
                {
                    pixels[y * size + x] = Color.clear;
                    continue;
                }

                if (sw > 0)
                {
                    // Di area stroke
                    float innerEdge = r - sw;
                    float alphaInner = Mathf.Clamp01(innerEdge + 0.5f - dist);

                    // Interpolasi warna stroke ke fill
                    Color col = Color.Lerp(stroke, fill, alphaInner);
                    col.a *= alphaOuter;
                    pixels[y * size + x] = col;
                }
                else
                {
                    Color col = fill;
                    col.a *= alphaOuter;
                    pixels[y * size + x] = col;
                }
            }
        }

        tex.SetPixels(pixels);
        tex.Apply();

        float borderInset = r + pad;
        Vector4 border = new Vector4(borderInset, borderInset, borderInset, borderInset);
        Sprite sprite = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect, border);
        spriteCache[key] = sprite;
        return sprite;
    }

    /// <summary>
    /// Sprite pill penuh (rounded 9999px) untuk tombol aksi, chip jenjang, dan badge.
    /// </summary>
    public static Sprite GetPillSprite(Color? fillColor = null, Color? strokeColor = null, float strokeWidth = 0f)
    {
        return GetRoundedRectSprite(28f, fillColor, strokeColor, strokeWidth);
    }
    #endregion

    #region Component Styling Helpers
    /// <summary>
    /// Terapkan gaya Card Stitch (Background cream/putih, rounded 22px, border halus 2px #BFDBFE).
    /// </summary>
    public static void ApplyCardStyle(Image img, Color? bg = null, Color? border = null, float radius = 22f, float stroke = 2f)
    {
        if (img == null) return;
        Color bgColor = bg ?? SurfaceHud;
        Color borderColor = border ?? PrimaryBlueLight;
        img.sprite = GetRoundedRectSprite(radius, bgColor, borderColor, stroke);
        img.type = Image.Type.Sliced;
        img.color = Color.white;
    }

    /// <summary>
    /// Terapkan gaya Tactile Button Stitch (rounded full pill, 3D bottom press depth).
    /// </summary>
    public static void ApplyTactileButtonStyle(Button btn, Color normalBg, Color pressLipColor, Color textColor)
    {
        if (btn == null) return;
        Image img = btn.GetComponent<Image>();
        if (img != null)
        {
            img.sprite = GetPillSprite(normalBg, pressLipColor, 2f);
            img.type = Image.Type.Sliced;
            img.color = Color.white;
        }

        ColorBlock cb = btn.colors;
        cb.normalColor = Color.white;
        cb.highlightedColor = new Color(1f, 1f, 1f, 0.95f);
        cb.pressedColor = new Color(0.9f, 0.9f, 0.9f, 1f);
        cb.selectedColor = Color.white;
        btn.colors = cb;

        TMP_Text tmp = btn.GetComponentInChildren<TMP_Text>();
        if (tmp != null)
        {
            tmp.color = textColor;
            tmp.fontStyle = FontStyles.Bold;
        }
    }

    /// <summary>
    /// Terapkan gaya Glassmorphic Floating HUD Bar (transparan 88%, border halus).
    /// </summary>
    public static void ApplyHudBarStyle(Image img)
    {
        if (img == null) return;
        img.sprite = GetRoundedRectSprite(24f, SurfaceHud, PrimaryBlueLight, 1.5f);
        img.type = Image.Type.Sliced;
        img.color = Color.white;
    }

    /// <summary>
    /// Terapkan gaya pill badge status/chip (misal: "Marker Terdeteksi", "Kelas 1–2").
    /// </summary>
    public static void ApplyPillBadge(Image img, TMP_Text text, Color bgColor, Color textColor, Color? borderColor = null)
    {
        if (img != null)
        {
            img.sprite = GetPillSprite(bgColor, borderColor ?? bgColor, borderColor != null ? 1.5f : 0f);
            img.type = Image.Type.Sliced;
            img.color = Color.white;
        }
        if (text != null)
        {
            text.color = textColor;
            text.fontStyle = FontStyles.Bold;
        }
    }
    #endregion

    #region Scene Integrators
    /// <summary>
    /// Terapkan Screen A & B layout ke MainMenu.unity
    /// </summary>
    public static void EnhanceMainMenu(MainMenuManager menu)
    {
        if (menu == null) return;

        // 1. Terapkan card style ke panel-panel kategori
        GameObject[] panels = new GameObject[] {
            menu.startPanel,
            menu.soalCategoryPanel,
            menu.materiCategoryPanel,
            menu.soalSubCategoryPanel_PerhitunganDasar,
            menu.soalSubCategoryPanel_PengolahanData,
            menu.materiSubCategoryPanel_PerhitunganDasar,
            menu.materiSubCategoryPanel_PengolahanData
        };

        foreach (var p in panels)
        {
            if (p == null) continue;
            Image panelImg = p.GetComponent<Image>();
            if (panelImg != null && p != menu.startPanel)
            {
                ApplyCardStyle(panelImg, SurfaceHud, PrimaryBlueLight, 24f, 2f);
            }

            // Style seluruh tombol di dalam panel menjadi Stitch tactile cards
            Button[] buttons = p.GetComponentsInChildren<Button>(true);
            foreach (var b in buttons)
            {
                if (b.name.StartsWith("Btn_Fase") || b.name == "Btn_Kembali") continue;
                Image btnImg = b.GetComponent<Image>();
                if (btnImg != null)
                {
                    ApplyCardStyle(btnImg, SurfaceLowest, PrimaryBlueLight, 20f, 2f);
                }
                TMP_Text txt = b.GetComponentInChildren<TMP_Text>();
                if (txt != null)
                {
                    txt.color = OnSurface;
                    txt.fontStyle = FontStyles.Bold;
                }
            }
        }
    }

    /// <summary>
    /// Terapkan Screen C layout ke Materi*.unity (HUD atas, card rumus kolapsibel, dock 4 tombol).
    /// Menjaga viewport kamera 65%+ di tengah tetap 100% transparan.
    /// </summary>
    public static void EnhanceMateriHUD(UIBangunRuang ui)
    {
        if (ui == null) return;

        // Style panelInfo (Card Rumus & Konsep)
        if (ui.panelInfo != null)
        {
            Image infoImg = ui.panelInfo.GetComponent<Image>();
            if (infoImg != null)
                ApplyCardStyle(infoImg, SurfaceHud, PrimaryBlueLight, 22f, 2f);

            if (ui.teksJudul != null)
            {
                ui.teksJudul.color = PrimaryBlueDeep;
                ui.teksJudul.fontStyle = FontStyles.Bold;
            }
            if (ui.teksIsi != null)
            {
                ui.teksIsi.color = OnSurface;
            }
        }

        // Style panelTombol (Dock 4-Pill Tampilan AR di bawah)
        if (ui.panelTombol != null)
        {
            Image tombolPanelImg = ui.panelTombol.GetComponent<Image>();
            if (tombolPanelImg != null)
                ApplyHudBarStyle(tombolPanelImg);

            Button[] btns = ui.panelTombol.GetComponentsInChildren<Button>(true);
            foreach (var b in btns)
            {
                ApplyTactileButtonStyle(b, SurfaceContainer, PrimaryBlueDeep, OnSurface);
            }
        }
    }

    /// <summary>
    /// Injeksi top bar Stitch untuk materi AR (Tombol Kembali, Judul Materi, Badge Status Marker Terdeteksi).
    /// </summary>
    public static void SetupMateriTopBar(Canvas canvas, string title)
    {
        if (canvas == null) return;
        Transform existing = canvas.transform.Find("StitchMateriTopBar");
        if (existing != null) return;

        GameObject barGo = new GameObject("StitchMateriTopBar", typeof(RectTransform), typeof(Image));
        barGo.transform.SetParent(canvas.transform, false);

        RectTransform rt = barGo.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.04f, 0.91f);
        rt.anchorMax = new Vector2(0.96f, 0.98f);
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        Image img = barGo.GetComponent<Image>();
        ApplyHudBarStyle(img);

        // Tombol Kembali
        GameObject backBtnGo = new GameObject("Btn_Kembali", typeof(RectTransform), typeof(Image), typeof(Button));
        backBtnGo.transform.SetParent(barGo.transform, false);
        RectTransform brt = backBtnGo.GetComponent<RectTransform>();
        brt.anchorMin = new Vector2(0.02f, 0.15f);
        brt.anchorMax = new Vector2(0.24f, 0.85f);
        brt.offsetMin = Vector2.zero;
        brt.offsetMax = Vector2.zero;

        Image bImg = backBtnGo.GetComponent<Image>();
        bImg.sprite = GetPillSprite(SurfaceLowest, PrimaryBlueLight, 1.5f);
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
        bTmp.color = OnSurface;

        Button backBtn = backBtnGo.GetComponent<Button>();
        backBtn.onClick.AddListener(() =>
        {
            MathSoundFX.PlayClick();
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        });

        // Judul Sesi Belajar
        GameObject titleGo = new GameObject("TitleText", typeof(RectTransform), typeof(TextMeshProUGUI));
        titleGo.transform.SetParent(barGo.transform, false);
        RectTransform trt = titleGo.GetComponent<RectTransform>();
        trt.anchorMin = new Vector2(0.26f, 0.1f);
        trt.anchorMax = new Vector2(0.65f, 0.9f);
        trt.offsetMin = Vector2.zero;
        trt.offsetMax = Vector2.zero;

        TextMeshProUGUI titleTmp = titleGo.GetComponent<TextMeshProUGUI>();
        titleTmp.text = title;
        titleTmp.fontSize = 16;
        titleTmp.fontStyle = FontStyles.Bold;
        titleTmp.alignment = TextAlignmentOptions.MidlineLeft;
        titleTmp.color = PrimaryBlueDeep;

        // Badge Status Marker Terdeteksi
        GameObject badgeGo = new GameObject("MarkerBadge", typeof(RectTransform), typeof(Image));
        badgeGo.transform.SetParent(barGo.transform, false);
        RectTransform badgeRt = badgeGo.GetComponent<RectTransform>();
        badgeRt.anchorMin = new Vector2(0.66f, 0.15f);
        badgeRt.anchorMax = new Vector2(0.98f, 0.85f);
        badgeRt.offsetMin = Vector2.zero;
        badgeRt.offsetMax = Vector2.zero;

        Image badgeImg = badgeGo.GetComponent<Image>();
        badgeImg.sprite = GetPillSprite(SecondaryContainer, SecondaryEmeraldDark, 1.5f);
        badgeImg.type = Image.Type.Sliced;

        GameObject badgeTextGo = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
        badgeTextGo.transform.SetParent(badgeGo.transform, false);
        RectTransform badgeTextRt = badgeTextGo.GetComponent<RectTransform>();
        badgeTextRt.anchorMin = Vector2.zero;
        badgeTextRt.anchorMax = Vector2.one;
        badgeTextRt.offsetMin = Vector2.zero;
        badgeTextRt.offsetMax = Vector2.zero;

        TextMeshProUGUI badgeTmp = badgeTextGo.GetComponent<TextMeshProUGUI>();
        badgeTmp.text = "● Marker Aktif";
        badgeTmp.fontSize = 13;
        badgeTmp.fontStyle = FontStyles.Bold;
        badgeTmp.alignment = TextAlignmentOptions.Center;
        badgeTmp.color = SecondaryEmeraldDark;
    }

    /// <summary>
    /// Terapkan Screen D1 & D2 layout ke Soal*.unity (Top HUD hearts & score, floating question card, bottom actions).
    /// </summary>
    public static void EnhanceQuizHUD<T>(BaseQuizManager<T> quiz)
    {
        if (quiz == null) return;

        // 1. Question Card
        if (quiz.questionText != null)
        {
            Transform parent = quiz.questionText.transform.parent;
            if (parent != null)
            {
                Image cardImg = parent.GetComponent<Image>();
                if (cardImg != null)
                    ApplyCardStyle(cardImg, SurfaceHud, PrimaryBlueLight, 24f, 2f);
            }
            quiz.questionText.color = OnSurface;
            quiz.questionText.fontStyle = FontStyles.Bold;
        }

        // 2. Answer selection status strip
        if (quiz.answerText != null)
        {
            Transform strip = quiz.answerText.transform.parent;
            if (strip != null)
            {
                Image stripImg = strip.GetComponent<Image>();
                if (stripImg != null)
                    ApplyCardStyle(stripImg, PrimaryBlueSubtle, PrimaryBlueLight, 16f, 1f);
            }
            quiz.answerText.color = PrimaryBlueDeep;
        }

        // 3. Bottom Action Buttons (Check, Undo, Hint)
        if (quiz.checkButton != null)
        {
            ApplyTactileButtonStyle(quiz.checkButton, SecondaryEmerald, SecondaryEmeraldDark, Color.white);
        }
        if (quiz.undoButton != null)
        {
            ApplyTactileButtonStyle(quiz.undoButton, CoralRedSubtle, CoralRed, CoralRed);
        }
    }

    /// <summary>
    /// Terapkan Screen E layout ke Scratchpad (DrawingController)
    /// </summary>
    public static void EnhanceScratchpad(DrawingController drawing)
    {
        if (drawing == null || drawing.drawingPanel == null) return;

        Image panelImg = drawing.drawingPanel.GetComponent<Image>();
        if (panelImg != null)
            ApplyCardStyle(panelImg, SurfaceLowest, PrimaryBlueLight, 24f, 2.5f);

        if (drawing.clearButton != null)
            ApplyTactileButtonStyle(drawing.clearButton, CoralRedSubtle, CoralRed, CoralRed);
        if (drawing.closeButton != null)
            ApplyTactileButtonStyle(drawing.closeButton, SurfaceContainer, OutlineVariant, OnSurface);
        if (drawing.saveButton != null)
            ApplyTactileButtonStyle(drawing.saveButton, PrimaryBlueSubtle, PrimaryBlue, PrimaryBlue);
    }

    /// <summary>
    /// Terapkan Screen E layout ke Modals (PopupManager)
    /// </summary>
    public static void EnhanceModals(PopupManager popup)
    {
        if (popup == null) return;

        if (popup.panelBenar != null)
        {
            Image img = popup.panelBenar.GetComponent<Image>();
            if (img != null)
                ApplyCardStyle(img, SurfaceLowest, SecondaryEmerald, 28f, 3f);
        }

        if (popup.panelSalah != null)
        {
            Image img = popup.panelSalah.GetComponent<Image>();
            if (img != null)
                ApplyCardStyle(img, SurfaceLowest, CoralRed, 28f, 3f);
        }

        if (popup.panelGameOver != null)
        {
            Image img = popup.panelGameOver.GetComponent<Image>();
            if (img != null)
                ApplyCardStyle(img, SurfaceLowest, CoralRed, 28f, 3f);
        }

        if (popup.restartButton != null)
        {
            ApplyTactileButtonStyle(popup.restartButton, SecondaryEmerald, SecondaryEmeraldDark, Color.white);
        }
    }
    #endregion
}
