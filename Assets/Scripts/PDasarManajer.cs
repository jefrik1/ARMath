using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manajer Materi Perhitungan Dasar dengan Visualisasi 3D AR Prosedural (Kurikulum SD):
/// 1. Garis Bilangan 3D (Number Line & KPK)
/// 2. Balok Nilai Tempat 3D (Dienes Blocks: Satuan, Puluhan, Ratusan)
/// 3. Jam Analog 3D & Tangga Satuan 3D (km -> mm)
/// 4. Perkalian & Pembagian Berulang
/// 5. Sifat Komutatif Garis Bilangan Paralel
/// </summary>
public class PDasarManajer : MonoBehaviour, IQuizManager
{
    [Header("Referensi UI")]
    [Tooltip("Drag TextMeshPro - Judul di sini")]
    public TextMeshProUGUI textJudul;

    [Tooltip("Drag TextMeshPro - Isi Materi (Scroll View Content) di sini")]
    public TextMeshProUGUI textIsi;

    [Tooltip("Masukkan 6 Tombol Menu di sini sesuai urutan materi")]
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

    public void SetJawabanPilihan(string p, int n, GameObject o) { }
    public void SetJawabanPilihan(string p, string n, GameObject o) { }
    public void SetJawabanPilihan(string p, float n, GameObject o) { }
    public void UndoJawaban() { }
    public bool IsAnswerCorrect(string a) => false;

    void Start()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas == null) canvas = FindObjectOfType<Canvas>();
        StitchUIThemeApplier.SetupMateriTopBar(canvas, "Materi AR: Perhitungan Dasar");

        if (textJudul != null && textJudul.transform.parent != null)
        {
            Image pImg = textJudul.transform.parent.GetComponent<Image>();
            if (pImg != null) StitchUIThemeApplier.ApplyCardStyle(pImg, StitchUIThemeApplier.SurfaceHud, StitchUIThemeApplier.PrimaryBlueLight, 22f, 2f);
        }

        InisialisasiMateri();
        SetupTombol();
        TampilkanMateri(0);
    }

    void SetupTombol()
    {
        for (int i = 0; i < tombolMenu.Length; i++)
        {
            int index = i;
            tombolMenu[i].onClick.AddListener(() =>
            {
                MathSoundFX.PlayClick();
                TampilkanMateri(index);
            });
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
        listMateri = new DataMateri[6];

        // 0: Pertambahan & Garis Bilangan
        listMateri[0].judul = "Garis Bilangan & Pertambahan";
        listMateri[0].isi = "<b>Garis Bilangan</b> membantu memahami arah penjumlahan dan pengurangan.\n\n" +
            "• Tambah bilangan positif: Melompat ke arah <b>KANAN (+)</b>\n" +
            "• Tambah bilangan negatif: Melompat ke arah <b>KIRI (-)</b>\n\n" +
            "<b>Contoh Soal:</b>\n" +
            "-3 + (-6) = <b>-9</b>\n" +
            "<i>(Mulai di angka -3, lalu melompat 6 langkah ke KIRI, mendarat tepat di -9)</i>";

        // 1: Balok Nilai Tempat (Dienes Blocks)
        listMateri[1].judul = "Nilai Tempat (Satuan, Puluhan, Ratusan)";
        listMateri[1].isi = "Setiap angka dalam bilangan memiliki <b>Nilai Tempat</b>:\n\n" +
            "1. <b>Satuan:</b> 1 kubus kecil (nilai 1 - 9)\n" +
            "2. <b>Puluhan:</b> 1 batang berisi 10 kubus (nilai 10 - 90)\n" +
            "3. <b>Ratusan:</b> 1 lempeng berisi 100 kubus (nilai 100 - 900)\n\n" +
            "<i>Perhatikan model 3D Dienes Block: Balok ratusan, batang puluhan, dan kubus satuan.</i>";

        // 2: Pengukuran: Jam Analog & Tangga Satuan
        listMateri[2].judul = "Pengukuran: Jam & Tangga Satuan";
        listMateri[2].isi = "<b>1. Jam Analog:</b>\n" +
            "• Jarum pendek = Jam (1 putaran = 12 jam)\n" +
            "• Jarum panjang = Menit (1 jam = 60 menit)\n\n" +
            "<b>2. Tangga Satuan Panjang (km → mm):</b>\n" +
            "• Setiap <b>TURUN 1 tangga</b>: dikali <b>10</b> (contoh: 1 m = 100 cm)\n" +
            "• Setiap <b>NAIK 1 tangga</b>: dibagi <b>10</b>\n\n" +
            "<i>Urutan: km - hm - dam - m - dm - cm - mm</i>";

        // 3: Perkalian & KPK
        listMateri[3].judul = "Perkalian & Kelipatan (KPK)";
        listMateri[3].isi = "Perkalian adalah <b>penjumlahan berulang</b> dari bilangan yang sama.\n\n" +
            "<b>Contoh: 3 x 2 = 6</b>\n" +
            "Artinya melompat sejauh 2 langkah sebanyak 3 kali:\n" +
            "0 → 2 → 4 → <b>6</b>\n\n" +
            "<b>KPK (Kelipatan Persekutuan Terkecil):</b>\n" +
            "Kelipatan 2: 2, 4, 6, 8, 10, ...\n" +
            "Kelipatan 3: 3, 6, 9, 12, ...\n" +
            "Titik temu terkecil adalah <b>6</b>.";

        // 4: Pembagian Bertahap
        listMateri[4].judul = "Pembagian (Pengurangan Berulang)";
        listMateri[4].isi = "Pembagian adalah proses <b>pengurangan berulang</b> sampai habis bernilai nol (0).\n\n" +
            "<b>Contoh: 6 ÷ 2</b>\n" +
            "1. 6 - 2 = 4 (langkah ke-1)\n" +
            "2. 4 - 2 = 2 (langkah ke-2)\n" +
            "3. 2 - 2 = 0 (langkah ke-3)\n\n" +
            "Dibutuhkan 3 kali pengurangan, sehingga <b>6 ÷ 2 = 3</b>.";

        // 5: Sifat Komutatif
        listMateri[5].judul = "Sifat Komutatif (Pertukaran)";
        listMateri[5].isi = "<b>Sifat Komutatif:</b>\n" +
            "Urutan bilangan tidak mengubah hasil penjumlahan dan perkalian.\n\n" +
            "• Penjumlahan: <b>a + b = b + a</b>\n" +
            "  Contoh: 2 + 3 = 5 sama dengan 3 + 2 = 5\n\n" +
            "• Perkalian: <b>a x b = b x a</b>\n" +
            "  Contoh: 3 x 4 = 12 sama dengan 4 x 3 = 12\n\n" +
            "<i>(Lihat visualisasi garis bilangan paralel yang mendarat di titik 5)</i>";
    }

    private void SpawnVisualizer(int index)
    {
        if (currentVisualizerRoot != null)
        {
            Destroy(currentVisualizerRoot);
        }

        currentVisualizerRoot = new GameObject("VisualizerPerhitunganDasar3D");
        SetupRootTransform(currentVisualizerRoot);
        currentVisualizerRoot.AddComponent<RotasiSentuh>();

        switch (index)
        {
            case 0:
                BuildGarisBilanganPenjumlahan();
                break;
            case 1:
                BuildBalokNilaiTempat();
                break;
            case 2:
                BuildJamDanTanggaSatuan();
                break;
            case 3:
                BuildPerkalianDanKPK();
                break;
            case 4:
                BuildPembagianVisualizer();
                break;
            case 5:
                BuildSifatKomutatifVisualizer();
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
    /// Tab 0: Garis Bilangan 3D dari -10 sampai +10 dengan busur lompatan -3 + (-6) = -9
    /// </summary>
    private void BuildGarisBilanganPenjumlahan()
    {
        CreateNumberLineAxis(-10, 10, 0.22f);
        float xStart = GetXPositionForValue(-3f, -10, 10, 0.22f);
        float xEnd = GetXPositionForValue(-9f, -10, 10, 0.22f);

        CreateMarkerSphere("StartSphere", new Vector3(xStart, 0.12f, 0), new Color(0.2f, 0.7f, 1f));
        Create3DLabel("Mulai (-3)", new Vector3(xStart, 0.35f, 0), currentVisualizerRoot.transform, new Color(0.3f, 0.8f, 1f), 2.2f);

        CreateMarkerSphere("EndSphere", new Vector3(xEnd, 0.12f, 0), new Color(0.2f, 0.95f, 0.35f));
        Create3DLabel("Hasil (-9)", new Vector3(xEnd, 0.35f, 0), currentVisualizerRoot.transform, new Color(0.2f, 1f, 0.4f), 2.5f);

        CreateParabolicArc(xStart, xEnd, 0.55f, currentVisualizerRoot.transform, new Color(1f, 0.65f, 0.1f));
        Create3DLabel("-3 + (-6) = Lompat 6 langkah ke KIRI = -9", new Vector3(0, 1.15f, 0), currentVisualizerRoot.transform, new Color(1f, 0.9f, 0.2f), 3.2f);
    }

    /// <summary>
    /// Tab 1: Balok Nilai Tempat 3D (Base-10 Dienes Blocks: Satuan, Puluhan, Ratusan)
    /// </summary>
    private void BuildBalokNilaiTempat()
    {
        // KIRI: Ratusan (1 lempeng 10x10)
        GameObject ratusan = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ratusan.name = "BlockRatusan_100";
        ratusan.transform.SetParent(currentVisualizerRoot.transform, false);
        ratusan.transform.localPosition = new Vector3(-0.9f, 0, 0);
        ratusan.transform.localScale = new Vector3(0.9f, 0.9f, 0.1f);
        SetMaterialColor(ratusan, new Color(0.25f, 0.65f, 0.95f));
        Create3DLabel("1 Ratusan\n(100 Satuan)", new Vector3(-0.9f, 0.65f, 0), currentVisualizerRoot.transform, new Color(0.3f, 0.8f, 1f), 2.4f);

        // TENGAH: Puluhan (1 batang 1x10)
        GameObject puluhan = GameObject.CreatePrimitive(PrimitiveType.Cube);
        puluhan.name = "BlockPuluhan_10";
        puluhan.transform.SetParent(currentVisualizerRoot.transform, false);
        puluhan.transform.localPosition = new Vector3(0.1f, 0, 0);
        puluhan.transform.localScale = new Vector3(0.12f, 0.9f, 0.1f);
        SetMaterialColor(puluhan, new Color(1f, 0.65f, 0.15f));
        Create3DLabel("1 Puluhan\n(10 Satuan)", new Vector3(0.1f, 0.65f, 0), currentVisualizerRoot.transform, new Color(1f, 0.7f, 0.2f), 2.4f);

        // KANAN: Satuan (1 kubus)
        GameObject satuan = GameObject.CreatePrimitive(PrimitiveType.Cube);
        satuan.name = "BlockSatuan_1";
        satuan.transform.SetParent(currentVisualizerRoot.transform, false);
        satuan.transform.localPosition = new Vector3(0.85f, -0.38f, 0);
        satuan.transform.localScale = new Vector3(0.12f, 0.12f, 0.1f);
        SetMaterialColor(satuan, new Color(0.2f, 0.95f, 0.35f));
        Create3DLabel("1 Satuan\n(1)", new Vector3(0.85f, -0.05f, 0), currentVisualizerRoot.transform, new Color(0.2f, 1f, 0.4f), 2.4f);

        Create3DLabel("Nilai Tempat: 1 Ratusan (100) + 1 Puluhan (10) + 1 Satuan (1) = 111", new Vector3(0, 1.15f, 0), currentVisualizerRoot.transform, new Color(1f, 0.9f, 0.2f), 3.2f);
    }

    /// <summary>
    /// Tab 2: Jam Analog 3D & Tangga Satuan 7 Langkah (km -> mm)
    /// </summary>
    private void BuildJamDanTanggaSatuan()
    {
        // KIRI: Jam Analog 3D
        GameObject clockRoot = new GameObject("ClockRoot");
        clockRoot.transform.SetParent(currentVisualizerRoot.transform, false);
        clockRoot.transform.localPosition = new Vector3(-0.95f, 0, 0);

        // Piringan Jam
        GameObject dial = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        dial.transform.SetParent(clockRoot.transform, false);
        dial.transform.localScale = new Vector3(0.85f, 0.04f, 0.85f);
        dial.transform.localRotation = Quaternion.Euler(90f, 0, 0);
        SetMaterialColor(dial, new Color(0.9f, 0.93f, 0.98f));

        // Jarum Pendek (Jam 3)
        GameObject hourHand = GameObject.CreatePrimitive(PrimitiveType.Cube);
        hourHand.transform.SetParent(clockRoot.transform, false);
        hourHand.transform.localPosition = new Vector3(0.12f, 0, -0.04f);
        hourHand.transform.localScale = new Vector3(0.22f, 0.035f, 0.02f);
        SetMaterialColor(hourHand, Color.black);

        // Jarum Panjang (Menit 12)
        GameObject minuteHand = GameObject.CreatePrimitive(PrimitiveType.Cube);
        minuteHand.transform.SetParent(clockRoot.transform, false);
        minuteHand.transform.localPosition = new Vector3(0, 0.18f, -0.04f);
        minuteHand.transform.localScale = new Vector3(0.025f, 0.32f, 0.02f);
        SetMaterialColor(minuteHand, new Color(0.85f, 0.15f, 0.15f));

        Create3DLabel("Jam 03.00", new Vector3(0, -0.55f, 0), clockRoot.transform, Color.white, 2.4f);

        // KANAN: Tangga Satuan 7 Langkah (km, hm, dam, m, dm, cm, mm)
        GameObject stairRoot = new GameObject("StairRoot");
        stairRoot.transform.SetParent(currentVisualizerRoot.transform, false);
        stairRoot.transform.localPosition = new Vector3(0.7f, -0.4f, 0);

        string[] units = { "km", "hm", "dam", "m", "dm", "cm", "mm" };
        float stepW = 0.16f;
        float stepH = 0.12f;

        for (int i = 0; i < units.Length; i++)
        {
            float sx = (i - 3) * stepW;
            float sy = (units.Length - 1 - i) * stepH;

            GameObject step = GameObject.CreatePrimitive(PrimitiveType.Cube);
            step.transform.SetParent(stairRoot.transform, false);
            step.transform.localPosition = new Vector3(sx, sy * 0.5f, 0);
            step.transform.localScale = new Vector3(stepW * 0.9f, sy + 0.05f, 0.2f);
            SetMaterialColor(step, (units[i] == "m") ? new Color(0.2f, 0.95f, 0.35f) : new Color(0.35f, 0.6f, 0.9f));

            Create3DLabel(units[i], new Vector3(sx, sy + 0.1f, 0), stairRoot.transform, Color.white, 1.8f);
        }

        Create3DLabel("Tangga Satuan: Turun 1 tangga = x 10 | Naik 1 tangga = : 10", new Vector3(0, 1.15f, 0), currentVisualizerRoot.transform, new Color(1f, 0.9f, 0.2f), 3.0f);
    }

    /// <summary>
    /// Tab 3: Garis Bilangan Perkalian Berulang & KPK (3 x 2 = 6)
    /// </summary>
    private void BuildPerkalianDanKPK()
    {
        CreateNumberLineAxis(0, 8, 0.26f);

        float[] points = { 0f, 2f, 4f, 6f };
        for (int i = 0; i < points.Length - 1; i++)
        {
            float xA = GetXPositionForValue(points[i], 0, 8, 0.26f);
            float xB = GetXPositionForValue(points[i + 1], 0, 8, 0.26f);
            CreateParabolicArc(xA, xB, 0.32f, currentVisualizerRoot.transform, new Color(0.2f, 0.8f, 1f));
            CreateMarkerSphere($"Stop_{i}", new Vector3(xA, 0.08f, 0), Color.white, 0.1f);
        }

        float xFinal = GetXPositionForValue(6f, 0, 8, 0.26f);
        CreateMarkerSphere("FinalSphere", new Vector3(xFinal, 0.12f, 0), new Color(0.2f, 0.95f, 0.35f), 0.15f);
        Create3DLabel("Hasil = 6", new Vector3(xFinal, 0.35f, 0), currentVisualizerRoot.transform, new Color(0.2f, 1f, 0.4f), 2.5f);

        Create3DLabel("Perkalian 3 x 2: Lompat 2 sebanyak 3 kali berturut-turut = 6", new Vector3(0, 1.1f, 0), currentVisualizerRoot.transform, new Color(1f, 0.9f, 0.2f), 3.2f);
    }

    /// <summary>
    /// Tab 4: Pembagian Bertahap (6 : 2 = 3)
    /// </summary>
    private void BuildPembagianVisualizer()
    {
        CreateNumberLineAxis(0, 8, 0.26f);

        float[] points = { 6f, 4f, 2f, 0f };
        for (int i = 0; i < points.Length - 1; i++)
        {
            float xA = GetXPositionForValue(points[i], 0, 8, 0.26f);
            float xB = GetXPositionForValue(points[i + 1], 0, 8, 0.26f);
            CreateParabolicArc(xA, xB, 0.32f, currentVisualizerRoot.transform, new Color(1f, 0.55f, 0.15f));
            CreateMarkerSphere($"Step_{i}", new Vector3(xA, 0.08f, 0), Color.white, 0.1f);
        }

        float xZero = GetXPositionForValue(0f, 0, 8, 0.26f);
        CreateMarkerSphere("ZeroSphere", new Vector3(xZero, 0.12f, 0), new Color(0.2f, 0.95f, 0.35f), 0.15f);

        Create3DLabel("Pembagian 6 ÷ 2: Pengurangan berulang dengan 2 (3 langkah habis) = 3", new Vector3(0, 1.1f, 0), currentVisualizerRoot.transform, new Color(1f, 0.9f, 0.2f), 3.0f);
    }

    /// <summary>
    /// Tab 5: Sifat Komutatif Garis Bilangan Paralel (2 + 3 = 5 vs 3 + 2 = 5)
    /// </summary>
    private void BuildSifatKomutatifVisualizer()
    {
        CreateNumberLineAxis(0, 6, 0.3f);

        float x0 = GetXPositionForValue(0f, 0, 6, 0.3f);
        float x2 = GetXPositionForValue(2f, 0, 6, 0.3f);
        float x3 = GetXPositionForValue(3f, 0, 6, 0.3f);
        float x5 = GetXPositionForValue(5f, 0, 6, 0.3f);

        // Atas: 2 + 3 = 5
        CreateParabolicArc(x0, x2, 0.25f, currentVisualizerRoot.transform, new Color(0.2f, 0.7f, 1f));
        CreateParabolicArc(x2, x5, 0.35f, currentVisualizerRoot.transform, new Color(1f, 0.6f, 0.1f));
        Create3DLabel("Atas: (2) + (3) = 5", new Vector3(0, 0.75f, 0), currentVisualizerRoot.transform, new Color(0.3f, 0.9f, 1f), 2.5f);

        // Bawah: 3 + 2 = 5
        CreateParabolicArc(x0, x3, -0.3f, currentVisualizerRoot.transform, new Color(1f, 0.6f, 0.1f));
        CreateParabolicArc(x3, x5, -0.2f, currentVisualizerRoot.transform, new Color(0.2f, 0.7f, 1f));
        Create3DLabel("Bawah: (3) + (2) = 5", new Vector3(0, -0.65f, 0), currentVisualizerRoot.transform, new Color(1f, 0.7f, 0.2f), 2.5f);

        CreateMarkerSphere("EndSphere5", new Vector3(x5, 0, 0), new Color(0.2f, 0.95f, 0.35f), 0.16f);
        Create3DLabel("Sifat Komutatif: a + b = b + a (Keduanya Mendarat di 5)", new Vector3(0, 1.25f, 0), currentVisualizerRoot.transform, new Color(1f, 0.9f, 0.2f), 3.2f);
    }

    private GameObject CreateNumberLineAxis(int minVal, int maxVal, float stepWidth)
    {
        GameObject axisRoot = new GameObject("AxisRoot");
        axisRoot.transform.SetParent(currentVisualizerRoot.transform, false);

        int count = maxVal - minVal + 1;
        float totalLength = (count - 1) * stepWidth;

        GameObject rod = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        rod.name = "MainRod";
        rod.transform.SetParent(axisRoot.transform, false);
        rod.transform.localPosition = Vector3.zero;
        rod.transform.localRotation = Quaternion.Euler(0, 0, 90f);
        rod.transform.localScale = new Vector3(0.04f, totalLength * 0.5f, 0.04f);
        SetMaterialColor(rod, new Color(0.7f, 0.75f, 0.85f));

        for (int v = minVal; v <= maxVal; v++)
        {
            float posX = (v - minVal) * stepWidth - (totalLength * 0.5f);
            bool isZero = (v == 0);

            GameObject tick = GameObject.CreatePrimitive(PrimitiveType.Cube);
            tick.name = $"Tick_{v}";
            tick.transform.SetParent(axisRoot.transform, false);
            tick.transform.localPosition = new Vector3(posX, 0, 0);
            tick.transform.localScale = new Vector3(0.02f, isZero ? 0.16f : 0.09f, 0.02f);
            SetMaterialColor(tick, isZero ? new Color(1f, 0.85f, 0.1f) : Color.white);

            if (v % 2 == 0 || isZero)
            {
                Create3DLabel(v.ToString(), new Vector3(posX, -0.15f, 0), axisRoot.transform, isZero ? new Color(1f, 0.9f, 0.2f) : Color.white, 2.0f);
            }
        }
        return axisRoot;
    }

    private float GetXPositionForValue(float val, int minVal, int maxVal, float stepWidth)
    {
        int count = maxVal - minVal + 1;
        float totalLength = (count - 1) * stepWidth;
        return (val - minVal) * stepWidth - (totalLength * 0.5f);
    }

    private GameObject CreateMarkerSphere(string name, Vector3 localPos, Color color, float scale = 0.14f)
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.name = name;
        sphere.transform.SetParent(currentVisualizerRoot.transform, false);
        sphere.transform.localPosition = localPos;
        sphere.transform.localScale = Vector3.one * scale;
        SetMaterialColor(sphere, color);
        return sphere;
    }

    private void CreateParabolicArc(float xStart, float xEnd, float arcHeight, Transform parent, Color color, int segments = 20)
    {
        GameObject arcObj = new GameObject("ParabolicArc");
        arcObj.transform.SetParent(parent, false);
        LineRenderer lr = arcObj.AddComponent<LineRenderer>();
        lr.positionCount = segments + 1;
        lr.widthMultiplier = 0.035f;

        Shader s = Shader.Find("Sprites/Default") ?? Shader.Find("Unlit/Color") ?? Shader.Find("Standard");
        Material mat = new Material(s) { color = color };
        lr.material = mat;

        for (int i = 0; i <= segments; i++)
        {
            float t = (float)i / segments;
            float x = Mathf.Lerp(xStart, xEnd, t);
            float y = 4f * arcHeight * t * (1f - t);
            lr.SetPosition(i, new Vector3(x, y + 0.08f, 0));
        }
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
