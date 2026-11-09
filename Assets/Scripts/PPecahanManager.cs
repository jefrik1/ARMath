using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manajer Materi Pecahan dengan Visualisasi 3D AR Prosedural (Kurikulum SD):
/// 1. Piringan Pecahan 3D (Fraction Disk / Pizza)
/// 2. Pecahan Senilai, Desimal & Persen (1/2 = 2/4 = 0.5 = 50%)
/// 3. Batang Pecahan 3D Dinamis dengan Tombol Interaktif [+] dan [-]
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

    // Nilai manipulasi interaktif untuk siswa
    private int interactivePembilang = 1;
    private int interactivePenyebut = 2;
    private int activeTab = 0;

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
            tombolMenu[i].onClick.AddListener(() =>
            {
                MathSoundFX.PlayClick();
                TampilkanMateri(index);
            });
        }
    }

    void TampilkanMateri(int index)
    {
        activeTab = index;
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
            "1. <b>Pembilang (atas):</b> Jumlah bagian yang diambil atau dimiliki.\n" +
            "2. <b>Penyebut (bawah):</b> Jumlah seluruh potongan yang sama besar.\n\n" +
            "<i>(Gunakan tombol [+] dan [-] pada model 3D untuk memanipulasi pecahan secara langsung!)</i>";

        listMateri[1].judul = "Pecahan Senilai, Desimal, & Persen";
        listMateri[1].isi = "Pecahan dapat dinyatakan dalam bentuk lain yang nilainya sama:\n\n" +
            "• <b>Pecahan Senilai:</b> 1/2 = 2/4 = 3/6\n" +
            "• <b>Bentuk Desimal:</b> 1/2 = 0,5 (1 ÷ 2)\n" +
            "• <b>Bentuk Persen (%):</b> 1/2 = 50% (50 per seratus)\n\n" +
            "<i>Perhatikan perbandingan visual disk 3D antara 1/2 dan 2/4 di layar.</i>";

        listMateri[2].judul = "Operasi Penjumlahan Pecahan";
        listMateri[2].isi = "<b>Penjumlahan Pecahan:</b>\n" +
            "Jika penyebut berbeda, cari <b>KPK</b> penyebutnya terlebih dahulu.\n\n" +
            "<b>Contoh:</b>\n" +
            "2/3 + 1/6\n" +
            "= 4/6 + 1/6\n" +
            "= <b>5/6</b>\n\n" +
            "<i>(Lihat batang 3D tersegmentasi 6 bagian: 4 bagian biru + 1 oranye = 5 bagian terisi)</i>";

        listMateri[3].judul = "Perkalian & Pembagian Pecahan";
        listMateri[3].isi = "<b>1. Perkalian Pecahan (Kalikan Langsung):</b>\n" +
            "3/4 x 2/5 = (3 x 2) / (4 x 5) = 6/20 = <b>3/10</b>\n\n" +
            "<b>2. Pembagian Pecahan (Balik Pecahan Kedua):</b>\n" +
            "3/4 ÷ 2/5 = 3/4 x 5/2 = <b>15/8 = 1 7/8</b>";
    }

    private void SpawnVisualizer(int index)
    {
        if (currentVisualizerRoot != null)
        {
            Destroy(currentVisualizerRoot);
        }

        currentVisualizerRoot = new GameObject("VisualizerPecahan3D");
        SetupRootTransform(currentVisualizerRoot);
        currentVisualizerRoot.AddComponent<RotasiSentuh>();

        switch (index)
        {
            case 0:
                BuildInteractiveFractionDisk();
                break;
            case 1:
                BuildPecahanSenilaiDesimalPersen();
                break;
            case 2:
            case 3:
                BuildOperasiBatangVisualizer();
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
    /// Tab 0: Piringan Pecahan 3D Interaktif dengan Tombol [+] dan [-]
    /// </summary>
    private void BuildInteractiveFractionDisk()
    {
        float radius = 0.6f;
        float height = 0.3f;
        float anglePerSlice = 360f / interactivePenyebut;

        for (int i = 0; i < interactivePenyebut; i++)
        {
            float start = i * anglePerSlice;
            float end = (i + 1) * anglePerSlice;
            bool isActive = (i < interactivePembilang);
            Color sliceColor = isActive ? new Color(0.18f, 0.6f, 0.95f) : new Color(0.85f, 0.88f, 0.92f);

            GameObject slice = CreatePieSliceObject($"Slice_{i}", start, end, radius, height, sliceColor);
            slice.transform.SetParent(currentVisualizerRoot.transform, false);

            if (isActive)
            {
                float midRad = (start + end) * 0.5f * 0.0174532925f;
                slice.transform.localPosition = new Vector3(Mathf.Sin(midRad) * 0.04f, 0, Mathf.Cos(midRad) * 0.04f);
            }
        }

        Create3DLabel($"Pecahan: {interactivePembilang}/{interactivePenyebut}", new Vector3(0, 0.65f, 0), currentVisualizerRoot.transform, new Color(1f, 0.85f, 0.1f), 3.4f);
        Create3DLabel($"Pembilang: {interactivePembilang} bagian | Penyebut: {interactivePenyebut} potong", new Vector3(0, -0.65f, 0), currentVisualizerRoot.transform, Color.white, 2.2f);

        CreateInteractiveControlPills();
    }

    private void CreateInteractiveControlPills()
    {
        GameObject controlsGo = new GameObject("FractionControls", typeof(RectTransform));
        controlsGo.transform.SetParent(currentVisualizerRoot.transform, false);
        controlsGo.transform.localPosition = new Vector3(0, -0.95f, 0);

        // Tombol Tambah Pembilang
        Create3DControlPill(controlsGo.transform, new Vector3(-0.45f, 0, 0), "+ Pembilang", () =>
        {
            if (interactivePembilang < interactivePenyebut)
            {
                interactivePembilang++;
                SpawnVisualizer(0);
            }
        });

        // Tombol Kurang Pembilang
        Create3DControlPill(controlsGo.transform, new Vector3(-0.15f, 0, 0), "- Pembilang", () =>
        {
            if (interactivePembilang > 1)
            {
                interactivePembilang--;
                SpawnVisualizer(0);
            }
        });

        // Tombol Tambah Penyebut
        Create3DControlPill(controlsGo.transform, new Vector3(0.15f, 0, 0), "+ Penyebut", () =>
        {
            if (interactivePenyebut < 8)
            {
                interactivePenyebut++;
                SpawnVisualizer(0);
            }
        });

        // Tombol Kurang Penyebut
        Create3DControlPill(controlsGo.transform, new Vector3(0.45f, 0, 0), "- Penyebut", () =>
        {
            if (interactivePenyebut > 2)
            {
                interactivePenyebut--;
                if (interactivePembilang > interactivePenyebut) interactivePembilang = interactivePenyebut;
                SpawnVisualizer(0);
            }
        });
    }

    private void Create3DControlPill(Transform parent, Vector3 localPos, string text, System.Action onClick)
    {
        GameObject pill = GameObject.CreatePrimitive(PrimitiveType.Cube);
        pill.name = "Pill_" + text;
        pill.transform.SetParent(parent, false);
        pill.transform.localPosition = localPos;
        pill.transform.localScale = new Vector3(0.25f, 0.12f, 0.05f);
        SetMaterialColor(pill, new Color(0.2f, 0.25f, 0.35f));

        Create3DLabel(text, new Vector3(0, 0, -0.06f), pill.transform, Color.white, 1.8f);

        // Interaksi klik mouse/touch pada kubus pill
        BoxCollider bc = pill.GetComponent<BoxCollider>();
        if (bc != null)
        {
            pill.AddComponent<PillClickHandler>().onClickAction = onClick;
        }
    }

    /// <summary>
    /// Tab 1: Pecahan Senilai & Desimal/Persen (1/2 = 2/4 = 0.5 = 50%)
    /// </summary>
    private void BuildPecahanSenilaiDesimalPersen()
    {
        // KIRI: Disk 1/2
        GameObject kiri = new GameObject("Disk_1_2");
        kiri.transform.SetParent(currentVisualizerRoot.transform, false);
        kiri.transform.localPosition = new Vector3(-0.75f, 0, 0);

        GameObject s1 = CreatePieSliceObject("Slice1", 0, 180, 0.45f, 0.2f, new Color(0.2f, 0.6f, 1f));
        s1.transform.SetParent(kiri.transform, false);
        GameObject s2 = CreatePieSliceObject("Slice2", 180, 360, 0.45f, 0.2f, new Color(0.85f, 0.88f, 0.92f));
        s2.transform.SetParent(kiri.transform, false);
        Create3DLabel("1/2\n= 0,5\n= 50%", new Vector3(0, 0.6f, 0), kiri.transform, new Color(0.3f, 0.8f, 1f), 2.5f);

        // KANAN: Disk 2/4 (Senilai)
        GameObject kanan = new GameObject("Disk_2_4");
        kanan.transform.SetParent(currentVisualizerRoot.transform, false);
        kanan.transform.localPosition = new Vector3(0.75f, 0, 0);

        for (int i = 0; i < 4; i++)
        {
            Color c = (i < 2) ? new Color(0.2f, 0.85f, 0.35f) : new Color(0.85f, 0.88f, 0.92f);
            GameObject sl = CreatePieSliceObject($"Slice4_{i}", i * 90f, (i + 1) * 90f, 0.45f, 0.2f, c);
            sl.transform.SetParent(kanan.transform, false);
        }
        Create3DLabel("2/4\n= 0,5\n= 50%", new Vector3(0, 0.6f, 0), kanan.transform, new Color(0.3f, 0.95f, 0.4f), 2.5f);

        Create3DLabel("Pecahan Senilai: 1/2 = 2/4 = 0,5 = 50%", new Vector3(0, 1.15f, 0), currentVisualizerRoot.transform, new Color(1f, 0.9f, 0.2f), 3.2f);
    }

    /// <summary>
    /// Tab 2 & 3: Batang Tersegmentasi Operasi Penjumlahan (2/3 + 1/6 = 4/6 + 1/6 = 5/6)
    /// </summary>
    private void BuildOperasiBatangVisualizer()
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
            seg.transform.SetParent(currentVisualizerRoot.transform, false);
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

        Create3DLabel("2/3 = 4/6 (Biru)", new Vector3(-0.4f, 0.4f, 0), currentVisualizerRoot.transform, new Color(0.3f, 0.7f, 1f), 2.5f);
        Create3DLabel("+ 1/6 (Oranye)", new Vector3(0.5f, 0.4f, 0), currentVisualizerRoot.transform, new Color(1f, 0.7f, 0.2f), 2.5f);
        Create3DLabel("Hasil: 4/6 + 1/6 = 5/6 Bagian Terisi", new Vector3(0, 0.95f, 0), currentVisualizerRoot.transform, new Color(0.3f, 1f, 0.5f), 3.4f);
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
        verts.Add(new Vector3(0, halfH, 0));
        verts.Add(new Vector3(0, -halfH, 0));

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

            tris.Add(trA); tris.Add(brA); tris.Add(trB);
            tris.Add(trB); tris.Add(brA); tris.Add(brB);
        }

        tris.Add(0); tris.Add(topRimStart); tris.Add(botRimStart);
        tris.Add(0); tris.Add(botRimStart); tris.Add(1);

        int topRimEnd = topRimStart + segments;
        int botRimEnd = botRimStart + segments;

        tris.Add(0); tris.Add(botRimEnd); tris.Add(topRimEnd);
        tris.Add(0); tris.Add(1); tris.Add(botRimEnd);

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

/// <summary>
/// Handler klik interaktif pada tombol pill 3D.
/// </summary>
public class PillClickHandler : MonoBehaviour
{
    public System.Action onClickAction;

    private void OnMouseDown()
    {
        MathSoundFX.PlayClick();
        onClickAction?.Invoke();
    }
}
