using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Collections.Generic;

/// <summary>
/// Controller untuk canvas gambar: coretan, color picker, clear, open/close,
/// dan ekspor hasil ke PNG file.
/// </summary>
public class DrawingController : MonoBehaviour
{
    [Header("UI Reference")]
    public GameObject drawingPanel;
    public RawImage drawingArea;
    public Button clearButton;
    public Button openButton;
    public Button closeButton;
    public Button saveButton;            // tombol simpan (opsional)
    public TMP_Text statusText;          // teks status "Gambar disimpan: ..." (opsional)

    [Header("Pengaturan Coretan")]
    public Color drawColor = Color.black;
    public int brushSize = 5;

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
        if (drawingPanel != null)
            drawingPanel.SetActive(false);

        if (drawingArea != null)
        {
            rectTransform = drawingArea.rectTransform;
            texture = new Texture2D((int)rectTransform.rect.width, (int)rectTransform.rect.height, TextureFormat.RGBA32, false);
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
    }

    void Update()
    {
        if (drawingPanel == null || !drawingPanel.activeInHierarchy) return;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector2 localPoint;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform, touch.position, null, out localPoint))
            {
                int x = (int)(localPoint.x + rectTransform.rect.width / 2);
                int y = (int)(localPoint.y + rectTransform.rect.height / 2);

                if (touch.phase == TouchPhase.Began)
                {
                    DrawAt(x, y);
                    lastDrawPos = new Vector2(x, y);
                }
                else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                {
                    if (lastDrawPos.HasValue)
                        DrawLine(lastDrawPos.Value, new Vector2(x, y));
                    lastDrawPos = new Vector2(x, y);
                }
                else if (touch.phase == TouchPhase.Ended)
                {
                    lastDrawPos = null;
                }
            }
        }
    }

    void DrawAt(int x, int y)
    {
        for (int i = -brushSize; i < brushSize; i++)
        {
            for (int j = -brushSize; j < brushSize; j++)
            {
                if (i * i + j * j <= brushSize * brushSize)
                {
                    if (x + i >= 0 && x + i < texture.width &&
                        y + j >= 0 && y + j < texture.height)
                        texture.SetPixel(x + i, y + j, drawColor);
                }
            }
        }
    }

    void DrawLine(Vector2 start, Vector2 end)
    {
        int steps = (int)Vector2.Distance(start, end);
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
        if (texture == null) return;
        Color32[] pixels = texture.GetPixels32();
        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = Color.white;
        texture.SetPixels32(pixels);
        texture.Apply();
    }

    public void OpenCanvas()
    {
        if (drawingPanel != null)
            drawingPanel.SetActive(true);
    }

    public void CloseCanvas()
    {
        if (drawingPanel != null)
            drawingPanel.SetActive(false);
    }

    public void SetBrushSize(float newSize)
    {
        brushSize = (int)newSize;
    }

    // ===== EKSPOR KE PNG =====

    /// <summary>
    /// Simpan tekstur drawing ke file PNG di persistentDataPath.
    /// File disimpan di: Application.persistentDataPath / <folderName> / tanggal.png
    /// </summary>
    public void SaveDrawingToPNG()
    {
        if (texture == null)
        {
            SetStatus("Tidak ada gambar untuk disimpan.");
            return;
        }

        // Pastikan folder ada
        string folderPath = Path.Combine(Application.persistentDataPath, folderName);
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        // Nama file berdasarkan tanggal
        string fileName = System.DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png";
        string fullPath = Path.Combine(folderPath, fileName);

        // Encode dan simpan
        byte[] pngBytes = texture.EncodeToPNG();
        File.WriteAllBytes(fullPath, pngBytes);

        SetStatus("Gambar disimpan: " + fileName);
        Debug.Log("DrawingController: Gambar disimpan ke " + fullPath);

        // Optional: kontra notifikasi atau buka folder (tergantung platform)
    }

    private void SetStatus(string message)
    {
        if (statusText != null)
            statusText.text = message;
    }

    // ===== COLOR PICKER =====

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