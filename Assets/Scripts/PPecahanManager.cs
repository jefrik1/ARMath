using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manajer Materi Pecahan dengan Visualisasi 3D AR Prosedural (Matematika SD).
/// </summary>
public class PPecahanManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI textJudul;
    public TextMeshProUGUI textIsi;

    [Tooltip("Masukkan 4 Tombol Menu di sini (Definisi, Jenis, Operasi, Contoh)")]
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
        listMateri = new DataMateri[4];

        listMateri[0].judul = "Definisi Pecahan";
        listMateri[0].isi = "<b>Pecahan</b> adalah bilangan yang menunjukkan bagian dari keseluruhan.\n\n" +
            "Pecahan terdiri dari dua bagian:\n" +
            "1. <b>Pembilang</b> (bagian atas): Bagian yang diambil/dimiliki\n" +
            "2. <b>Penyebut</b> (bagian bawah): Total seluruh bagian yang sama besar\n\n" +
            "<i>Perhatikan model 3D di layar: Silinder dibagi 2 sama besar, 1 bagian berwarna biru mewakili pecahan 1/2.</i>";

        listMateri[1].judul = "Jenis-Jenis Pecahan";
        listMateri[1].isi = "<b>1. Pecahan Biasa</b> (Contoh: 3/4)\n" +
            "Pembilang lebih kecil dari penyebut (bagian dari 1 keutuhan).\n\n" +
            "<b>2. Pecahan Campuran</b> (Contoh: 1 1/2)\n" +
            "Gabungan bilangan bulat dan pecahan (1 balok utuh + 1/2 balok).\n\n" +
            "<b>3. Pecahan Desimal</b> (Contoh: 0,5 = 1/2)\n\n" +
            "<b>4. Persen (%)</b> (Contoh: 75% = 3/4)";

        listMateri[2].judul = "Operasi pada Pecahan";
        listMateri[2].isi = "<b>Penjumlahan & Pengurangan:</b>\n" +
            "Hanya bisa dijumlahkan jika <b>penyebutnya sama</b>.\n" +
            "Jika berbeda, samakan penyebut terlebih dahulu menggunakan KPK.\n\n" +
            "<b>Perkalian:</b>\n" +
            "Kalikan langsung: Pembilang x Pembilang, Penyebut x Penyebut.\n\n" +
            "<b>Pembagian:</b>\n" +
            "Balik pecahan kedua, lalu kalikan langsung.";

        listMateri[3].judul = "Contoh Operasi Hitung";
        listMateri[3].isi = "<b>1. Penjumlahan (Samakan Penyebut)</b>\n" +
            "2/3 + 1/6\n" +
            "= 4/6 + 1/6\n" +
            "= <b>5/6</b> (4 bagian biru + 1 bagian oranye dari total 6 bagian)\n\n" +
            "<b>2. Perkalian (Langsung)</b>\n" +
            "3/4 x 2/5 = (3 x 2) / (4 x 5) = 6/20 = <b>3/10</b>\n\n" +
            "<b>3. Pembagian (Balik & Kali)</b>\n" +
            "3/4 : 2/5 = 3/4 x 5/2 = <b>15/8 = 1 7/8</b>";
    }

    private void SpawnVisualizer(int index)
    {
        if (currentVisualizerRoot != null)
        {
            Destroy(currentVisualizerRoot);
        }

        currentVisualizerRoot = new GameObject("VisualizerPecahan3D");
        SetupRootTransform(currentVisualizerRoot);

        // Tambahkan RotasiSentuh agar interaktif diputar jari/mouse
        currentVisualizerRoot.AddComponent<RotasiSentuh>();

        switch (index)
        {
            case 0:
                BuildDefinisiVisualizer(currentVisualizerRoot.transform);
                break;
            case 1:
                BuildJenisVisualizer(currentVisualizerRoot.transform);
                break;
            case 2:
            case 3:
                BuildOperasiVisualizer(currentVisualizerRoot.transform);
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
            root.transform.position = cam.transform.position + cam.transform.forward * 2.2f + cam.transform.up * 0.1f;
            root.transform.rotation = Quaternion.LookRotation(cam.transform.forward, cam.transform.up);
        }
        else
        {
            root.transform.position = new Vector3(0, 0, 2f);
        }
    }

    /// <summary>
    /// Tab 0: Definisi 1/2 (Silinder dipotong menjadi 2 bagian dengan celah kecil).
    /// </summary>
    private void BuildDefinisiVisualizer(Transform parent)
    {
        float radius = 0.6f;
        float height = 0.35f;

        // Belahan Biru (1/2 aktif - Pembilang)
        GameObject sliceA = CreatePieSliceObject("Half_Blue_1_2", 0f, 180f, radius, height, new Color(0.2f, 0.6f, 1f));
        sliceA.transform.SetParent(parent, false);
        sliceA.transform.localPosition = new Vector3(0, 0, 0.04f);

        // Belahan Abu-abu (1/2 pelengkap - Penyebut)
        GameObject sliceB = CreatePieSliceObject("Half_White_1_2", 180f, 360f, radius, height, new Color(0.85f, 0.88f, 0.92f));
        sliceB.transform.SetParent(parent, false);
        sliceB.transform.localPosition = new Vector3(0, 0, -0.04f);

        Create3DLabel("1/2\n(Pembilang: 1 Bagian)", new Vector3(0, height + 0.2f, 0.35f), parent, new Color(0.2f, 0.7f, 1f), 3.2f);
        Create3DLabel("1/2\n(Total Penyebut: 2)", new Vector3(0, height + 0.2f, -0.35f), parent, Color.white, 2.6f);
        Create3DLabel("Model Pecahan 1/2", new Vector3(0, height + 0.65f, 0), parent, new Color(1f, 0.9f, 0.2f), 3.8f);
    }

    /// <summary>
    /// Tab 1: Jenis-Jenis Pecahan (Pecahan Campuran 1 1/2 di kiri & Pecahan Biasa 3/4 di kanan).
    /// </summary>
    private void BuildJenisVisualizer(Transform parent)
    {
        // KIRI: Pecahan Campuran 1 1/2
        GameObject kiriRoot = new GameObject("PecahanCampuranRoot");
        kiriRoot.transform.SetParent(parent, false);
        kiriRoot.transform.localPosition = new Vector3(-0.9f, 0, 0);

        // Balok 1 Utuh
        GameObject cubeUtuh = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cubeUtuh.name = "CubeUtuh";
        cubeUtuh.transform.SetParent(kiriRoot.transform, false);
        cubeUtuh.transform.localPosition = new Vector3(-0.35f, 0, 0);
        cubeUtuh.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        SetMaterialColor(cubeUtuh, new Color(1f, 0.55f, 0.15f));

        // Balok 1/2
        GameObject cubeSetengah = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cubeSetengah.name = "CubeSetengah";
        cubeSetengah.transform.SetParent(kiriRoot.transform, false);
        cubeSetengah.transform.localPosition = new Vector3(0.2f, -0.125f, 0);
        cubeSetengah.transform.localScale = new Vector3(0.5f, 0.25f, 0.5f);
        SetMaterialColor(cubeSetengah, new Color(1f, 0.8f, 0.2f));

        Create3DLabel("1 Utuh", new Vector3(-0.35f, 0.4f, 0), kiriRoot.transform, Color.white, 2.4f);
        Create3DLabel("1/2", new Vector3(0.2f, 0.4f, 0), kiriRoot.transform, Color.white, 2.4f);
        Create3DLabel("Campuran:\n1 1/2", new Vector3(0, 0.75f, 0), kiriRoot.transform, new Color(1f, 0.7f, 0.2f), 3f);

        // KANAN: Pecahan Biasa 3/4 (Pie 4 Bagian: 3 Hijau, 1 Transparan)
        GameObject kananRoot = new GameObject("PecahanBiasaRoot");
        kananRoot.transform.SetParent(parent, false);
        kananRoot.transform.localPosition = new Vector3(0.9f, 0, 0);

        float r = 0.45f;
        float h = 0.25f;
        for (int i = 0; i < 4; i++)
        {
            float start = i * 90f;
            float end = (i + 1) * 90f;
            Color c = (i < 3) ? new Color(0.2f, 0.85f, 0.35f) : new Color(0.7f, 0.7f, 0.75f, 0.4f);
            GameObject slice = CreatePieSliceObject($"PieSlice_{i}", start, end, r, h, c);
            slice.transform.SetParent(kananRoot.transform, false);
            slice.transform.localPosition = Vector3.zero;
        }

        Create3DLabel("Pecahan Biasa:\n3/4", new Vector3(0, 0.75f, 0), kananRoot.transform, new Color(0.3f, 0.9f, 0.4f), 3f);
    }

    /// <summary>
    /// Tab 2 & 3: Operasi Penjumlahan (2/3 + 1/6 = 4/6 + 1/6 = 5/6).
    /// </summary>
    private void BuildOperasiVisualizer(Transform parent)
    {
        int totalSegments = 6;
        float barWidth = 0.32f;
        float barHeight = 0.35f;
        float barDepth = 0.35f;
        float startX = -(totalSegments * barWidth) * 0.5f + (barWidth * 0.5f);

        for (int i = 0; i < totalSegments; i++)
        {
            GameObject seg = GameObject.CreatePrimitive(PrimitiveType.Cube);
            seg.name = $"Segment_{i}";
            seg.transform.SetParent(parent, false);
            seg.transform.localPosition = new Vector3(startX + i * (barWidth + 0.02f), 0, 0);
            seg.transform.localScale = new Vector3(barWidth, barHeight, barDepth);

            if (i < 4)
            {
                SetMaterialColor(seg, new Color(0.2f, 0.55f, 1f));
            }
            else if (i == 4)
            {
                SetMaterialColor(seg, new Color(1f, 0.6f, 0.1f));
            }
            else
            {
                SetMaterialColor(seg, new Color(0.7f, 0.7f, 0.75f));
            }
        }

        Create3DLabel("2/3 = 4/6 (Biru)", new Vector3(-0.4f, 0.4f, 0), parent, new Color(0.3f, 0.7f, 1f), 2.5f);
        Create3DLabel("+ 1/6 (Oranye)", new Vector3(0.5f, 0.4f, 0), parent, new Color(1f, 0.7f, 0.2f), 2.5f);
        Create3DLabel("Hasil: 4/6 + 1/6 = 5/6 Bagian Terisi", new Vector3(0, 0.8f, 0), parent, new Color(0.3f, 1f, 0.5f), 3.4f);
    }

    private GameObject CreatePieSliceObject(string name, float startAngle, float endAngle, float radius, float height, Color color)
    {
        GameObject obj = new GameObject(name);
        MeshFilter mf = obj.AddComponent<MeshFilter>();
        MeshRenderer mr = obj.AddComponent<MeshRenderer>();
        mf.mesh = GeneratePieSliceMesh(startAngle, endAngle, radius, height);
        SetMaterialColor(obj, color);
        return obj;
    }

    private Mesh GeneratePieSliceMesh(float startDeg, float endDeg, float radius, float height, int segments = 24)
    {
        Mesh mesh = new Mesh();
        var verts = new System.Collections.Generic.List<Vector3>();
        var tris = new System.Collections.Generic.List<int>();

        float halfH = height * 0.5f;
        verts.Add(new Vector3(0, halfH, 0));  // 0: top center
        verts.Add(new Vector3(0, -halfH, 0)); // 1: bottom center

        int topRimStart = verts.Count;
        for (int i = 0; i <= segments; i++)
        {
            float t = (float)i / segments;
            float rad = Mathf.Lerp(startDeg, endDeg, t) * 0.0174532925f;
            verts.Add(new Vector3(Mathf.Sin(rad) * radius, halfH, Mathf.Cos(rad) * radius));
        }

        int botRimStart = verts.Count;
        for (int i = 0; i <= segments; i++)
        {
            float t = (float)i / segments;
            float rad = Mathf.Lerp(startDeg, endDeg, t) * 0.0174532925f;
            verts.Add(new Vector3(Mathf.Sin(rad) * radius, -halfH, Mathf.Cos(rad) * radius));
        }

        for (int i = 0; i < segments; i++)
        {
            tris.Add(0);
            tris.Add(topRimStart + i);
            tris.Add(topRimStart + i + 1);
        }

        for (int i = 0; i < segments; i++)
        {
            tris.Add(1);
            tris.Add(botRimStart + i + 1);
            tris.Add(botRimStart + i);
        }

        for (int i = 0; i < segments; i++)
        {
            int trA = topRimStart + i;
            int trB = topRimStart + i + 1;
            int brA = botRimStart + i;
            int brB = botRimStart + i + 1;

            tris.Add(trA);
            tris.Add(brA);
            tris.Add(trB);

            tris.Add(trB);
            tris.Add(brA);
            tris.Add(brB);
        }

        tris.Add(0);
        tris.Add(topRimStart);
        tris.Add(botRimStart);

        tris.Add(0);
        tris.Add(botRimStart);
        tris.Add(1);

        int topRimEnd = topRimStart + segments;
        int botRimEnd = botRimStart + segments;

        tris.Add(0);
        tris.Add(botRimEnd);
        tris.Add(topRimEnd);

        tris.Add(0);
        tris.Add(1);
        tris.Add(botRimEnd);

        mesh.SetVertices(verts);
        mesh.SetTriangles(tris, 0);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
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
