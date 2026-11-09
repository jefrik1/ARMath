using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

/// <summary>
/// Controller untuk papan coretan matematika (Scratchpad / Porogapit):
/// coretan halus di atas UI, tombol ganti warna cepat (Hitam, Biru, Merah),
/// hapus coretan (Clear), dan simpan hasil kerja.
/// </summary>
public class DrawingController : MonoBehaviour
{
    [Header("UI Reference")]
    public GameObject drawingPanel;
    public RawImage drawingArea;
    public Button clearButton;
    public Button openButton;
    public Button closeButton;
    public Button saveButton;
    public TMP_Text statusText;

    [Header("Pengaturan Coretan")]
    public Color drawColor = Color.black;
    public int brushSize = 4;

    [Header("Color Picker UI")]
    public Image colorPreviewImage;
    public Slider redSlider;
    public Slider greenSlider;
    public Slider blueSlider;

    [Header("Ekspor")]
    public string folderName = "Mathlfix_Drawings";

    private Texture2D texture;
    private RectTransform rectTransform;
    private Vector2? lastDrawPos = null;

    void Start()
    {
        StitchUIThemeApplier.EnhanceScratchpad(this);

        if (drawingPanel != null)
            drawingPanel.SetActive(false);

        if (drawingArea != null)
        {
            rectTransform = drawingArea.rectTransform;
            int width = Mathf.Max(256, (int)rectTransform.rect.width);
            int height = Mathf.Max(256, (int)rectTransform.rect.height);
            texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            drawingArea.texture = texture;
            ClearTexture();
        }

        if (clearButton != null)
            clearButton.onClick.AddListener(ClearTexture);
        if (openButton != null)
            openButton.onClick.AddListener(OpenCanvas);
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseCanvas);
        if (saveButton != null)
            saveButton.onClick.AddListener(SaveDrawingToPNG);

        if (redSlider != null)
            redSlider.onValueChanged.AddListener(delegate { UpdateBrushColor(); });
        if (greenSlider != null)
            greenSlider.onValueChanged.AddListener(delegate { UpdateBrushColor(); });
        if (blueSlider != null)
            blueSlider.onValueChanged.AddListener(delegate { UpdateBrushColor(); });

        InitializeColorPicker();
        SetupQuickColorButtons();
    }

    /// <summary>
    /// Buat tombol pill warna cepat (Hitam, Biru, Merah, Hijau, Kuning) Stitch di drawingPanel.
    /// </summary>
    private void SetupQuickColorButtons()
    {
        if (drawingPanel == null) return;
        Transform existing = drawingPanel.transform.Find("QuickColorPills");
        if (existing != null) return;

        GameObject container = new GameObject("QuickColorPills", typeof(RectTransform), typeof(HorizontalLayoutGroup));
        container.transform.SetParent(drawingPanel.transform, false);

        RectTransform crt = container.GetComponent<RectTransform>();
        crt.anchorMin = new Vector2(0.04f, 0.92f);
        crt.anchorMax = new Vector2(0.60f, 0.98f);
        crt.offsetMin = Vector2.zero;
        crt.offsetMax = Vector2.zero;

        HorizontalLayoutGroup hlg = container.GetComponent<HorizontalLayoutGroup>();
        hlg.spacing = 8;
        hlg.childAlignment = TextAnchor.MiddleLeft;
        hlg.childControlWidth = false;
        hlg.childControlHeight = false;

        CreateColorPill(container.transform, "Hitam", new Color(0.12f, 0.16f, 0.23f), () => SetColorHitam());
        CreateColorPill(container.transform, "Biru", StitchUIThemeApplier.PrimaryBlue, () => SetColorBiru());
        CreateColorPill(container.transform, "Merah", StitchUIThemeApplier.CoralRed, () => SetColorMerah());
        CreateColorPill(container.transform, "Hijau", StitchUIThemeApplier.SecondaryEmerald, () => SetDrawColor(StitchUIThemeApplier.SecondaryEmerald));
        CreateColorPill(container.transform, "Kuning", StitchUIThemeApplier.TertiaryAmber, () => SetDrawColor(StitchUIThemeApplier.TertiaryAmber));
    }

    private void CreateColorPill(Transform parent, string label, Color color, UnityEngine.Events.UnityAction action)
    {
        GameObject pill = new GameObject("Pill_" + label, typeof(RectTransform), typeof(Image), typeof(Button));
        pill.transform.SetParent(parent, false);

        RectTransform rt = pill.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(56, 30);

        Image img = pill.GetComponent<Image>();
        img.sprite = StitchUIThemeApplier.GetPillSprite(color, Color.white, 2f);
        img.type = Image.Type.Sliced;

        Button btn = pill.GetComponent<Button>();
        btn.onClick.AddListener(action);

        GameObject textGo = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
        textGo.transform.SetParent(pill.transform, false);
        TextMeshProUGUI tmp = textGo.GetComponent<TextMeshProUGUI>();
        tmp.text = label;
        tmp.fontSize = 12;
        tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;

        RectTransform textRt = textGo.GetComponent<RectTransform>();
        textRt.anchorMin = Vector2.zero;
        textRt.anchorMax = Vector2.one;
        textRt.offsetMin = Vector2.zero;
        textRt.offsetMax = Vector2.zero;
    }

    public void SetColorHitam() => SetDrawColor(new Color(0.12f, 0.16f, 0.23f));
    public void SetColorBiru() => SetDrawColor(StitchUIThemeApplier.PrimaryBlue);
    public void SetColorMerah() => SetDrawColor(StitchUIThemeApplier.CoralRed);

    public void SetDrawColor(Color c)
    {
        MathSoundFX.PlayClick();
        drawColor = c;
        if (colorPreviewImage != null)
            colorPreviewImage.color = drawColor;
        if (redSlider != null) redSlider.value = c.r;
        if (greenSlider != null) greenSlider.value = c.g;
        if (blueSlider != null) blueSlider.value = c.b;
    }

    void Update()
    {
        if (drawingPanel == null || !drawingPanel.activeInHierarchy || texture == null) return;

        // Mendukung input sentuhan jari (HP Android) dan mouse (Unity Editor)
        Vector2 screenPos;
        bool isDrawing = false;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            screenPos = touch.position;
            if (touch.phase == TouchPhase.Began)
            {
                lastDrawPos = null;
                isDrawing = true;
            }
            else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
            {
                isDrawing = true;
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                lastDrawPos = null;
            }
        }
        else if (Input.GetMouseButton(0))
        {
            screenPos = Input.mousePosition;
            isDrawing = true;
        }
        else
        {
            lastDrawPos = null;
            return;
        }

        if (isDrawing)
        {
            Vector2 localPoint;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform, screenPos, null, out localPoint))
            {
                int x = (int)(localPoint.x + rectTransform.rect.width * 0.5f);
                int y = (int)(localPoint.y + rectTransform.rect.height * 0.5f);

                if (x >= 0 && x < texture.width && y >= 0 && y < texture.height)
                {
                    if (lastDrawPos.HasValue)
                    {
                        DrawLine(lastDrawPos.Value, new Vector2(x, y));
                    }
                    else
                    {
                        DrawAt(x, y);
                        texture.Apply();
                    }
                    lastDrawPos = new Vector2(x, y);
                }
            }
        }
    }

    void DrawAt(int x, int y)
    {
        for (int i = -brushSize; i <= brushSize; i++)
        {
            for (int j = -brushSize; j <= brushSize; j++)
            {
                if (i * i + j * j <= brushSize * brushSize)
                {
                    int px = x + i;
                    int py = y + j;
                    if (px >= 0 && px < texture.width && py >= 0 && py < texture.height)
                        texture.SetPixel(px, py, drawColor);
                }
            }
        }
    }

    void DrawLine(Vector2 start, Vector2 end)
    {
        int steps = Mathf.Max(1, (int)Vector2.Distance(start, end));
        for (int i = 0; i <= steps; i++)
        {
            float t = (float)i / steps;
            int x = (int)Mathf.Lerp(start.x, end.x, t);
            int y = (int)Mathf.Lerp(start.y, end.y, t);
            DrawAt(x, y);
        }
        texture.Apply();
    }

    public void ClearTexture()
    {
        MathSoundFX.PlayClick();
        if (texture == null) return;
        int w = texture.width;
        int h = texture.height;
        Color32[] pixels = new Color32[w * h];
        Color32 paperColor = new Color32(251, 253, 255, 255); // #FBFDFF
        Color32 gridLineColor = new Color32(191, 219, 254, 110); // subtle blue grid line #BFDBFE

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                bool isGrid = (x % 24 == 0 || y % 24 == 0);
                pixels[y * w + x] = isGrid ? gridLineColor : paperColor;
            }
        }
        texture.SetPixels32(pixels);
        texture.Apply();
        SetStatus("Buku kotak coretan bersih.");
    }

    public void OpenCanvas()
    {
        MathSoundFX.PlayClick();
        if (drawingPanel != null)
            drawingPanel.SetActive(true);
    }

    public void CloseCanvas()
    {
        MathSoundFX.PlayClick();
        if (drawingPanel != null)
            drawingPanel.SetActive(false);
    }

    public void SetBrushSize(float newSize)
    {
        brushSize = Mathf.Clamp((int)newSize, 1, 20);
    }

    public void SaveDrawingToPNG()
    {
        MathSoundFX.PlayClick();
        if (texture == null)
        {
            SetStatus("Tidak ada gambar untuk disimpan.");
            return;
        }

        string folderPath = Path.Combine(Application.persistentDataPath, folderName);
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        string fileName = System.DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png";
        string fullPath = Path.Combine(folderPath, fileName);

        byte[] pngBytes = texture.EncodeToPNG();
        File.WriteAllBytes(fullPath, pngBytes);

        SetStatus("Gambar disimpan: " + fileName);
    }

    private void SetStatus(string message)
    {
        if (statusText != null)
            statusText.text = message;
    }

    void InitializeColorPicker()
    {
        if (redSlider != null) redSlider.value = drawColor.r;
        if (greenSlider != null) greenSlider.value = drawColor.g;
        if (blueSlider != null) blueSlider.value = drawColor.b;
        UpdateBrushColor();
    }

    public void UpdateBrushColor()
    {
        if (redSlider == null || greenSlider == null || blueSlider == null) return;
        drawColor = new Color(redSlider.value, greenSlider.value, blueSlider.value);
        if (colorPreviewImage != null)
            colorPreviewImage.color = drawColor;
    }

    private void OnDestroy()
    {
        if (texture != null)
        {
            Destroy(texture);
            texture = null;
        }
    }
}
