using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manajer Materi Pengolahan Data dengan Visualisasi Grafik Batang 3D AR Prosedural (Matematika SD).
/// </summary>
public class PDataManager : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Drag TextMeshPro - Judul di sini")]
    public TextMeshProUGUI textJudul;

    [Tooltip("Drag TextMeshPro - Isi Materi di sini")]
    public TextMeshProUGUI textIsi;

    [Tooltip("Masukkan 3 Tombol Menu (Modus, Median, Mean)")]
    public Button[] tombolMenu;

    [Header("3D AR Visualizer")]
    [Tooltip("Anchor atau Target AR opsional. Jika kosong, visualizer ditempatkan otomatis.")]
    public Transform arTargetRoot;

    private struct DataMateri
    {
        public string judul;
        public string isi;
    }

    private DataMateri[] listMateri;
    private GameObject currentVisualizerRoot;

    void Start()
    {
        InisialisasiMateri();
        SetupTombol();
        TampilkanMateri(0);
    }

    void SetupTombol()
    {
        for (int i = 0; i < tombolMenu.Length; i++)
        {
            int index = i;
            tombolMenu[i].onClick.AddListener(() => TampilkanMateri(index));
        }
    }

    void TampilkanMateri(int index)
    {
        if (index >= 0 && index < listMateri.Length)
        {
            textJudul.text = listMateri[index].judul;
            textIsi.text = listMateri[index].isi;
            SpawnVisualizer(index);
        }
    }

    void InisialisasiMateri()
    {
        listMateri = new DataMateri[3];

        // Modus
        listMateri[0].judul = "Modus";
        listMateri[0].isi = "<b>Modus</b> adalah nilai yang paling sering muncul dalam kumpulan data.\n\n" +
            "<b>Contoh Data:</b>\n" +
            "6, 6, 7, 8, 8, 8, 9\n\n" +
            "<b>Frekuensi Kemunculan:</b>\n" +
            "• Angka 6: muncul 2 kali\n" +
            "• Angka 7: muncul 1 kali\n" +
            "• <b>Angka 8: muncul 3 kali (terbanyak)</b>\n" +
            "• Angka 9: muncul 1 kali\n\n" +
            "<b>Maka, Modus = 8</b>\n" +
            "<i>(Lihat diagram batang 3D: Batang angka 8 disorot warna hijau terang)</i>";

        // Median
        listMateri[1].judul = "Median";
        listMateri[1].isi = "<b>Median</b> adalah nilai tengah dari kumpulan data setelah diurutkan dari terkecil ke terbesar.\n\n" +
            "<b>Contoh Data Ganjil:</b>\n" +
            "Data mentah: 7, 4, 18, 10, 14\n" +
            "1. Urutkan dahulu: <b>4, 7, 10, 14, 18</b>\n" +
            "2. Banyak data n = 5 (ganjil)\n" +
            "3. Nilai tepat di tengah adalah urutan ke-3 yaitu <b>10</b>.\n\n" +
            "<b>Maka, Median = 10</b>\n" +
            "<i>(Lihat batang 3D berwarna emas di tengah)</i>";

        // Mean
        listMateri[2].judul = "Mean (Rata-rata)";
        listMateri[2].isi = "<b>Mean</b> adalah nilai rata-rata dari sekumpulan data.\n" +
            "Rumus: <b>Jumlah Semua Nilai ÷ Banyak Data</b>\n\n" +
            "<b>Contoh Data:</b>\n" +
            "10, 12, 16, 11, 15, 16, 17, 10, 18, 13 (10 siswa)\n\n" +
            "1. Jumlahkan semua:\n" +
            "   10+12+16+11+15+16+17+10+18+13 = 138\n" +
            "2. Banyak data = 10\n" +
            "3. Mean = 138 ÷ 10 = <b>13,8</b>\n\n" +
            "<i>(Lihat bidang horizontal 3D berwarna toska: Garis rata-rata meratakan semua data)</i>";
    }

    private void SpawnVisualizer(int index)
    {
        if (currentVisualizerRoot != null)
        {
            Destroy(currentVisualizerRoot);
        }

        currentVisualizerRoot = new GameObject("VisualizerStatistik3D");
        SetupRootTransform(currentVisualizerRoot);
        currentVisualizerRoot.AddComponent<RotasiSentuh>();

        switch (index)
        {
            case 0:
                BuildModusVisualizer(currentVisualizerRoot.transform);
                break;
            case 1:
                BuildMedianVisualizer(currentVisualizerRoot.transform);
                break;
            case 2:
                BuildMeanVisualizer(currentVisualizerRoot.transform);
                break;
        }
    }

    private void SetupRootTransform(GameObject root)
    {
        if (arTargetRoot != null)
        {
            root.transform.SetParent(arTargetRoot, false);
            root.transform.localPosition = new Vector3(0, 0.1f, 0);
            root.transform.localRotation = Quaternion.identity;
            return;
        }

        GameObject targetInScene = GameObject.Find("TargetPD");
        if (targetInScene != null)
        {
            root.transform.SetParent(targetInScene.transform, false);
            root.transform.localPosition = new Vector3(0, 0.1f, 0);
            root.transform.localRotation = Quaternion.identity;
            return;
        }

        Camera cam = Camera.main;
        if (cam != null)
        {
            root.transform.position = cam.transform.position + cam.transform.forward * 2.3f + cam.transform.up * 0.1f;
            root.transform.rotation = Quaternion.LookRotation(cam.transform.forward, cam.transform.up);
        }
        else
        {
            root.transform.position = new Vector3(0, 0, 2f);
        }
    }

    /// <summary>
    /// Tab 0: Visualisasi Modus (Data: 6[2x], 7[1x], 8[3x - Modus], 9[1x]).
    /// Batang 8 diberi warna hijau cerah & aksen bintang/penanda.
    /// </summary>
    private void BuildModusVisualizer(Transform parent)
    {
        int[] nilai = { 6, 7, 8, 9 };
        int[] frekuensi = { 2, 1, 3, 1 };
        float barWidth = 0.35f;
        float spacing = 0.55f;
        float heightScale = 0.35f;
        float startX = -((nilai.Length - 1) * spacing) * 0.5f;

        for (int i = 0; i < nilai.Length; i++)
        {
            float posX = startX + i * spacing;
            float h = frekuensi[i] * heightScale;
            bool isModus = (nilai[i] == 8);

            GameObject bar = GameObject.CreatePrimitive(PrimitiveType.Cube);
            bar.name = $"Bar_Nilai_{nilai[i]}";
            bar.transform.SetParent(parent, false);
            bar.transform.localPosition = new Vector3(posX, h * 0.5f, 0);
            bar.transform.localScale = new Vector3(barWidth, h, barWidth);

            if (isModus)
            {
                SetMaterialColor(bar, new Color(0.2f, 0.95f, 0.35f));

                // Mahkota / penanda di atas batang modus
                GameObject crown = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                crown.name = "CrownModus";
                crown.transform.SetParent(parent, false);
                crown.transform.localPosition = new Vector3(posX, h + 0.15f, 0);
                crown.transform.localScale = Vector3.one * 0.18f;
                SetMaterialColor(crown, new Color(1f, 0.85f, 0.1f));

                Create3DLabel("MODUS!\nNilai 8 (3x)", new Vector3(posX, h + 0.45f, 0), parent, new Color(0.2f, 1f, 0.4f), 3.0f);
            }
            else
            {
                SetMaterialColor(bar, new Color(0.35f, 0.6f, 0.9f));
                Create3DLabel($"Nilai {nilai[i]}\n({frekuensi[i]}x)", new Vector3(posX, h + 0.25f, 0), parent, Color.white, 2.2f);
            }
        }

        Create3DLabel("Modus = 8 (Frekuensi Tertinggi)", new Vector3(0, 1.8f, 0), parent, new Color(1f, 0.9f, 0.2f), 3.6f);
    }

    /// <summary>
    /// Tab 1: Visualisasi Median (Data Terurut: 4, 7, 10 [Median], 14, 18).
    /// Batang tengah ke-3 (10) disorot warna Emas.
    /// </summary>
    private void BuildMedianVisualizer(Transform parent)
    {
        int[] dataSorted = { 4, 7, 10, 14, 18 };
        float barWidth = 0.32f;
        float spacing = 0.52f;
        float heightScale = 0.08f;
        float startX = -((dataSorted.Length - 1) * spacing) * 0.5f;

        for (int i = 0; i < dataSorted.Length; i++)
        {
            float posX = startX + i * spacing;
            float h = dataSorted[i] * heightScale;
            bool isMedian = (i == 2); // Indeks tengah

            GameObject bar = GameObject.CreatePrimitive(PrimitiveType.Cube);
            bar.name = $"Bar_Median_{dataSorted[i]}";
            bar.transform.SetParent(parent, false);
            bar.transform.localPosition = new Vector3(posX, h * 0.5f, 0);
            bar.transform.localScale = new Vector3(barWidth, h, barWidth);

            if (isMedian)
            {
                SetMaterialColor(bar, new Color(1f, 0.85f, 0.1f));
                Create3DLabel($"MEDIAN = {dataSorted[i]}\n(Urutan ke-3)", new Vector3(posX, h + 0.35f, 0), parent, new Color(1f, 0.9f, 0.2f), 3.0f);
            }
            else
            {
                SetMaterialColor(bar, new Color(0.45f, 0.55f, 0.75f));
                Create3DLabel($"ke-{i + 1}\n({dataSorted[i]})", new Vector3(posX, h + 0.25f, 0), parent, Color.white, 2.2f);
            }
        }

        Create3DLabel("Median: Nilai Tepat di Tengah Data Terurut", new Vector3(0, 2.0f, 0), parent, new Color(0.3f, 0.9f, 1f), 3.5f);
    }

    /// <summary>
    /// Tab 2: Visualisasi Mean (10 Data dengan bidang horizontal rata-rata setinggi 13,8).
    /// </summary>
    private void BuildMeanVisualizer(Transform parent)
    {
        int[] data = { 10, 12, 16, 11, 15, 16, 17, 10, 18, 13 };
        float barWidth = 0.2f;
        float spacing = 0.32f;
        float heightScale = 0.07f;
        float startX = -((data.Length - 1) * spacing) * 0.5f;

        for (int i = 0; i < data.Length; i++)
        {
            float posX = startX + i * spacing;
            float h = data[i] * heightScale;

            GameObject bar = GameObject.CreatePrimitive(PrimitiveType.Cube);
            bar.name = $"Bar_Data_{i}_{data[i]}";
            bar.transform.SetParent(parent, false);
            bar.transform.localPosition = new Vector3(posX, h * 0.5f, 0);
            bar.transform.localScale = new Vector3(barWidth, h, barWidth);
            SetMaterialColor(bar, new Color(0.35f, 0.65f, 0.95f));
        }

        // Garis/Bidang Horizontal Rata-Rata (Mean = 13.8)
        float meanValue = 13.8f;
        float meanY = meanValue * heightScale;
        float totalWidth = (data.Length * spacing) + 0.2f;

        GameObject meanPlane = GameObject.CreatePrimitive(PrimitiveType.Cube);
        meanPlane.name = "MeanHorizontalLine";
        meanPlane.transform.SetParent(parent, false);
        meanPlane.transform.localPosition = new Vector3(0, meanY, 0);
        meanPlane.transform.localScale = new Vector3(totalWidth, 0.04f, 0.35f);
        SetMaterialColor(meanPlane, new Color(0.1f, 0.95f, 0.85f));

        Create3DLabel("--- Garis Rata-Rata (Mean = 13,8) ---", new Vector3(0, meanY + 0.22f, 0), parent, new Color(0.1f, 1f, 0.9f), 2.8f);
        Create3DLabel("Mean = Total Nilai (138) ÷ Banyak Data (10) = 13,8", new Vector3(0, 1.8f, 0), parent, new Color(1f, 0.9f, 0.2f), 3.4f);
    }

    private void SetMaterialColor(GameObject obj, Color color)
    {
        Renderer r = obj.GetComponent<Renderer>();
        if (r != null)
        {
            Shader s = Shader.Find("Standard") ?? Shader.Find("Mobile/Diffuse") ?? Shader.Find("Sprites/Default");
            Material mat = new Material(s) { color = color };
            r.material = mat;
        }
    }

    private GameObject Create3DLabel(string text, Vector3 localPos, Transform parent, Color color, float size = 2.5f)
    {
        GameObject labelObj = new GameObject("Label3D");
        labelObj.transform.SetParent(parent, false);
        labelObj.transform.localPosition = localPos;
        TextMeshPro tmp = labelObj.AddComponent<TextMeshPro>();
        tmp.text = text;
        tmp.fontSize = size;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = color;
        labelObj.transform.localScale = Vector3.one * 0.35f;
        return labelObj;
    }
}
