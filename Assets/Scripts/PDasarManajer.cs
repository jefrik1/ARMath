using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manajer Materi Perhitungan Dasar dengan Visualisasi Garis Bilangan 3D AR Prosedural (Matematika SD).
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

    // Implementasi IQuizManager
    public void SetJawabanPilihan(string p, int n, GameObject o) { }
    public void SetJawabanPilihan(string p, string n, GameObject o) { }
    public void SetJawabanPilihan(string p, float n, GameObject o) { }
    public void UndoJawaban() { }
    public bool IsAnswerCorrect(string a) => false;

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
        listMateri = new DataMateri[6];

        // Pertambahan
        listMateri[0].judul = "Pertambahan";
        listMateri[0].isi = "Pertambahan adalah proses menggabungkan dua bilangan untuk mendapatkan jumlahnya.\n\n" +
            "<b>Aturan Garis Bilangan:</b>\n" +
            "• Tambah bilangan positif: Melompat ke arah <b>KANAN</b> (+)\n" +
            "• Tambah bilangan negatif: Melompat ke arah <b>KIRI</b> (-)\n\n" +
            "<b>Contoh:</b>\n" +
            "-3 + (-6) = <b>-9</b>\n" +
            "<i>(Mulai di -3, melompat 6 langkah ke KIRI, mendarat di -9)</i>";

        // Pengurangan
        listMateri[1].judul = "Pengurangan";
        listMateri[1].isi = "Pengurangan adalah proses mengambil sejumlah nilai dari bilangan asal.\n\n" +
            "<b>Kunci Penting:</b>\n" +
            "Mengurangi bilangan negatif sama dengan <b>MENAMBAH</b> bilangan positif!\n" +
            "a - (-b) = a + b\n\n" +
            "<b>Contoh:</b>\n" +
            "-9 - (-13) = -9 + 13 = <b>4</b>\n" +
            "<i>(Mulai di -9, berbalik arah lalu melompat 13 langkah ke KANAN, mendarat di +4)</i>";

        // Perkalian
        listMateri[2].judul = "Perkalian";
        listMateri[2].isi = "Perkalian adalah <b>penjumlahan berulang</b> dari bilangan yang sama.\n\n" +
            "<b>Contoh: 3 x 2</b>\n" +
            "Artinya melompat sejauh 2 langkah sebanyak 3 kali berturut-turut:\n" +
            "0 → 2 → 4 → <b>6</b>\n\n" +
            "<b>Aturan Tanda:</b>\n" +
            "• (+) x (+) = (+)\n" +
            "• (+) x (-) = (-)\n" +
            "• (-) x (-) = (+)";

        // Pembagian
        listMateri[3].judul = "Pembagian";
        listMateri[3].isi = "Pembagian adalah <b>pengurangan berulang</b> sampai bernilai nol (0).\n\n" +
            "<b>Contoh: 6 ÷ 2</b>\n" +
            "6 dikurangi 2 terus menerus:\n" +
            "1. 6 - 2 = 4 (langkah 1)\n" +
            "2. 4 - 2 = 2 (langkah 2)\n" +
            "3. 2 - 2 = 0 (langkah 3)\n" +
            "Dibutuhkan 3 langkah lompatan mundur, maka <b>6 ÷ 2 = 3</b>.";

        // Operasi Campuran
        listMateri[4].judul = "Operasi Campuran";
        listMateri[4].isi = "<b>Urutan Hierarki Operasi (KABATAKU / PEMDAS):</b>\n" +
            "1. Tanda Kurung (Parentheses)\n" +
            "2. Perkalian & Pembagian (Kiri ke Kanan)\n" +
            "3. Penjumlahan & Pengurangan (Kiri ke Kanan)\n\n" +
            "<b>Contoh Soal:</b>\n" +
            "(-12 x 8) + 40 ÷ 10\n" +
            "= -96 + 40 ÷ 10\n" +
            "= -96 + 4\n" +
            "= <b>-92</b>";

        // Sifat-sifat Operasi
        listMateri[5].judul = "Sifat-sifat Operasi";
        listMateri[5].isi = "<b>1. Sifat Komutatif (Pertukaran)</b>\n" +
            "a + b = b + a  (Contoh: 2 + 3 = 3 + 2 = 5)\n" +
            "a x b = b x a  (Contoh: 4 x 5 = 5 x 4 = 20)\n\n" +
            "<b>2. Sifat Asosiatif (Pengelompokan)</b>\n" +
            "(a + b) + c = a + (b + c)\n" +
            "(a x b) x c = a x (b x c)";
    }

    private void SpawnVisualizer(int index)
    {
        if (currentVisualizerRoot != null)
        {
            Destroy(currentVisualizerRoot);
        }

        currentVisualizerRoot = new GameObject("VisualizerGarisBilangan3D");
        SetupRootTransform(currentVisualizerRoot);
        currentVisualizerRoot.AddComponent<RotasiSentuh>();

        switch (index)
        {
            case 0:
                BuildGarisBilanganWithJump(-3f, -9f, -6, "Tambah -6 (Lompat 6 langkah ke KIRI) = -9");
                break;
            case 1:
                BuildGarisBilanganWithJump(-9f, 4f, 13, "- (-13) = +13 (Lompat 13 langkah ke KANAN) = 4");
                break;
            case 2:
                BuildPerkalianVisualizer();
                break;
            case 3:
                BuildPembagianVisualizer();
                break;
            case 4:
                BuildOperasiCampuranVisualizer();
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
            root.transform.position = cam.transform.position + cam.transform.forward * 2.4f + cam.transform.up * 0.1f;
            root.transform.rotation = Quaternion.LookRotation(cam.transform.forward, cam.transform.up);
        }
        else
        {
            root.transform.position = new Vector3(0, 0, 2f);
        }
    }

    /// <summary>
    /// Membangun sumbu garis bilangan dari minVal ke maxVal dengan tanda strip & angka 3D.
    /// </summary>
    private GameObject CreateNumberLineAxis(int minVal = -10, int maxVal = 10, float stepWidth = 0.22f)
    {
        GameObject axisRoot = new GameObject("AxisRoot");
        axisRoot.transform.SetParent(currentVisualizerRoot.transform, false);

        int count = maxVal - minVal + 1;
        float totalLength = (count - 1) * stepWidth;

        // Batang Garis Utama (Silinder)
        GameObject rod = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        rod.name = "MainRod";
        rod.transform.SetParent(axisRoot.transform, false);
        rod.transform.localPosition = Vector3.zero;
        rod.transform.localRotation = Quaternion.Euler(0, 0, 90f);
        rod.transform.localScale = new Vector3(0.04f, totalLength * 0.5f, 0.04f);
        SetMaterialColor(rod, new Color(0.7f, 0.75f, 0.85f));

        // Ticks dan Label Angka
        for (int v = minVal; v <= maxVal; v++)
        {
            float posX = (v - minVal) * stepWidth - (totalLength * 0.5f);
            bool isZero = (v == 0);

            // Tick mark (kubus vertikal kecil)
            GameObject tick = GameObject.CreatePrimitive(PrimitiveType.Cube);
            tick.name = $"Tick_{v}";
            tick.transform.SetParent(axisRoot.transform, false);
            tick.transform.localPosition = new Vector3(posX, 0, 0);
            tick.transform.localScale = new Vector3(0.02f, isZero ? 0.16f : 0.09f, 0.02f);
            SetMaterialColor(tick, isZero ? new Color(1f, 0.85f, 0.1f) : Color.white);

            // Tampilkan label angka hanya untuk kelipatan tertentu agar tidak menumpuk
            if (v % 2 == 0 || isZero)
            {
                Create3DLabel(v.ToString(), new Vector3(posX, -0.15f, 0), axisRoot.transform, isZero ? new Color(1f, 0.9f, 0.2f) : Color.white, 2.0f);
            }
        }

        return axisRoot;
    }

    private float GetXPositionForValue(float val, int minVal = -10, int maxVal = 10, float stepWidth = 0.22f)
    {
        int count = maxVal - minVal + 1;
        float totalLength = (count - 1) * stepWidth;
        return (val - minVal) * stepWidth - (totalLength * 0.5f);
    }

    private void BuildGarisBilanganWithJump(float startVal, float endVal, int jumpSteps, string deskripsi)
    {
        CreateNumberLineAxis();

        float xStart = GetXPositionForValue(startVal);
        float xEnd = GetXPositionForValue(endVal);

        // Bola Posisi Awal (Biru / Kuning)
        GameObject sphereStart = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphereStart.name = "SphereStart";
        sphereStart.transform.SetParent(currentVisualizerRoot.transform, false);
        sphereStart.transform.localPosition = new Vector3(xStart, 0.12f, 0);
        sphereStart.transform.localScale = Vector3.one * 0.14f;
        SetMaterialColor(sphereStart, new Color(0.2f, 0.7f, 1f));
        Create3DLabel($"Mulai ({startVal})", new Vector3(xStart, 0.35f, 0), currentVisualizerRoot.transform, new Color(0.3f, 0.8f, 1f), 2.4f);

        // Bola Posisi Akhir (Hijau)
        GameObject sphereEnd = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphereEnd.name = "SphereEnd";
        sphereEnd.transform.SetParent(currentVisualizerRoot.transform, false);
        sphereEnd.transform.localPosition = new Vector3(xEnd, 0.12f, 0);
        sphereEnd.transform.localScale = Vector3.one * 0.15f;
        SetMaterialColor(sphereEnd, new Color(0.2f, 0.95f, 0.35f));
        Create3DLabel($"Hasil ({endVal})", new Vector3(xEnd, 0.35f, 0), currentVisualizerRoot.transform, new Color(0.2f, 1f, 0.4f), 2.6f);

        // Busur Lompatan (Parabolic Arc)
        CreateParabolicArc(xStart, xEnd, 0.55f, currentVisualizerRoot.transform, new Color(1f, 0.65f, 0.1f));

        Create3DLabel(deskripsi, new Vector3(0, 1.1f, 0), currentVisualizerRoot.transform, new Color(1f, 0.9f, 0.2f), 3.2f);
    }

    private void BuildPerkalianVisualizer()
    {
        CreateNumberLineAxis();

        // 3 x 2: 3 lompatan berukuran +2 (0 -> 2 -> 4 -> 6)
        float[] points = { 0f, 2f, 4f, 6f };
        for (int i = 0; i < points.Length - 1; i++)
        {
            float xA = GetXPositionForValue(points[i]);
            float xB = GetXPositionForValue(points[i + 1]);
            CreateParabolicArc(xA, xB, 0.35f, currentVisualizerRoot.transform, new Color(0.2f, 0.8f, 1f));

            // Titik singgah
            GameObject s = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            s.transform.SetParent(currentVisualizerRoot.transform, false);
            s.transform.localPosition = new Vector3(xA, 0.08f, 0);
            s.transform.localScale = Vector3.one * 0.1f;
            SetMaterialColor(s, Color.white);
        }

        // Bola Hasil di 6
        float xFinal = GetXPositionForValue(6f);
        GameObject sFinal = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sFinal.transform.SetParent(currentVisualizerRoot.transform, false);
        sFinal.transform.localPosition = new Vector3(xFinal, 0.12f, 0);
        sFinal.transform.localScale = Vector3.one * 0.16f;
        SetMaterialColor(sFinal, new Color(0.2f, 0.95f, 0.35f));
        Create3DLabel("Hasil = 6", new Vector3(xFinal, 0.35f, 0), currentVisualizerRoot.transform, new Color(0.3f, 1f, 0.4f), 2.8f);

        Create3DLabel("Perkalian 3 x 2: Lompat 2 sebanyak 3 kali = 6", new Vector3(0, 1.0f, 0), currentVisualizerRoot.transform, new Color(1f, 0.9f, 0.2f), 3.2f);
    }

    private void BuildPembagianVisualizer()
    {
        CreateNumberLineAxis();

        // 6 : 2: 3 lompatan mundur berukuran -2 (6 -> 4 -> 2 -> 0)
        float[] points = { 6f, 4f, 2f, 0f };
        for (int i = 0; i < points.Length - 1; i++)
        {
            float xA = GetXPositionForValue(points[i]);
            float xB = GetXPositionForValue(points[i + 1]);
            CreateParabolicArc(xA, xB, 0.35f, currentVisualizerRoot.transform, new Color(1f, 0.5f, 0.2f));

            GameObject s = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            s.transform.SetParent(currentVisualizerRoot.transform, false);
            s.transform.localPosition = new Vector3(xA, 0.08f, 0);
            s.transform.localScale = Vector3.one * 0.1f;
            SetMaterialColor(s, Color.white);
        }

        float xEnd = GetXPositionForValue(0f);
        GameObject sEnd = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sEnd.transform.SetParent(currentVisualizerRoot.transform, false);
        sEnd.transform.localPosition = new Vector3(xEnd, 0.12f, 0);
        sEnd.transform.localScale = Vector3.one * 0.15f;
        SetMaterialColor(sEnd, new Color(0.2f, 0.95f, 0.35f));

        Create3DLabel("Pembagian 6 ÷ 2: Pengurangan berulang dengan 2 (3 langkah) = 3", new Vector3(0, 1.0f, 0), currentVisualizerRoot.transform, new Color(1f, 0.9f, 0.2f), 3.0f);
    }

    private void BuildOperasiCampuranVisualizer()
    {
        // Tangga Prioritas Hierarki Operasi KABATAKU
        string[] steps = {
            "1. Tanda Kurung: (-12 x 8) = -96",
            "2. Pembagian: 40 ÷ 10 = 4",
            "3. Penjumlahan: -96 + 4 = -92"
        };

        for (int i = 0; i < steps.Length; i++)
        {
            GameObject bar = GameObject.CreatePrimitive(PrimitiveType.Cube);
            bar.name = $"StepBar_{i}";
            bar.transform.SetParent(currentVisualizerRoot.transform, false);
            bar.transform.localPosition = new Vector3(0, 0.7f - (i * 0.45f), 0);
            bar.transform.localScale = new Vector3(2.4f, 0.22f, 0.1f);
            SetMaterialColor(bar, (i == 2) ? new Color(0.2f, 0.85f, 0.35f) : new Color(0.25f, 0.5f, 0.85f));

            Create3DLabel(steps[i], new Vector3(0, 0.7f - (i * 0.45f), -0.08f), currentVisualizerRoot.transform, Color.white, 2.6f);
        }

        Create3DLabel("Hierarki Operasi: Kurung → Kali/Bagi → Tambah/Kurang", new Vector3(0, 1.25f, 0), currentVisualizerRoot.transform, new Color(1f, 0.9f, 0.2f), 3.2f);
    }

    private void BuildSifatKomutatifVisualizer()
    {
        // Dua Garis Bilangan Paralel: 2 + 3 = 5 vs 3 + 2 = 5
        CreateNumberLineAxis(0, 6, 0.3f);

        float x0 = GetXPositionForValue(0f, 0, 6, 0.3f);
        float x2 = GetXPositionForValue(2f, 0, 6, 0.3f);
        float x3 = GetXPositionForValue(3f, 0, 6, 0.3f);
        float x5 = GetXPositionForValue(5f, 0, 6, 0.3f);

        // Atas: 2 + 3 = 5
        CreateParabolicArc(x0, x2, 0.25f, currentVisualizerRoot.transform, new Color(0.2f, 0.7f, 1f));
        CreateParabolicArc(x2, x5, 0.35f, currentVisualizerRoot.transform, new Color(1f, 0.6f, 0.1f));
        Create3DLabel("Atas: (2) + (3) = 5", new Vector3(0, 0.75f, 0), currentVisualizerRoot.transform, new Color(0.3f, 0.9f, 1f), 2.5f);

        // Bawah: 3 + 2 = 5 (terbalik arc)
        CreateParabolicArc(x0, x3, -0.3f, currentVisualizerRoot.transform, new Color(1f, 0.6f, 0.1f));
        CreateParabolicArc(x3, x5, -0.2f, currentVisualizerRoot.transform, new Color(0.2f, 0.7f, 1f));
        Create3DLabel("Bawah: (3) + (2) = 5", new Vector3(0, -0.65f, 0), currentVisualizerRoot.transform, new Color(1f, 0.7f, 0.2f), 2.5f);

        // Titik temu di 5
        GameObject sTujuan = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sTujuan.transform.SetParent(currentVisualizerRoot.transform, false);
        sTujuan.transform.localPosition = new Vector3(x5, 0, 0);
        sTujuan.transform.localScale = Vector3.one * 0.16f;
        SetMaterialColor(sTujuan, new Color(0.2f, 0.95f, 0.35f));

        Create3DLabel("Sifat Komutatif: a + b = b + a (Keduanya Mendarat di 5)", new Vector3(0, 1.25f, 0), currentVisualizerRoot.transform, new Color(1f, 0.9f, 0.2f), 3.2f);
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
            // Kurva parabola: 4 * h * t * (1 - t)
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
