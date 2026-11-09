using UnityEngine;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Kuis bangun ruang sesuai kurikulum SD (Fase A, B, C) dengan visualisasi 3D preview & label dimensi (Step 3.3).
/// </summary>
public class BangunRuangManager : BaseQuizManager<float>
{
    private enum BangunRuang
    {
        Kubus, Balok, Bola, Kerucut, Tabung,
        PrismaSegitiga, LimasSegitiga, LimasSegiempat,
        LimasSegilima, LimasSegienam
    }

    private enum TipePertanyaan { Sifat, Volume, LuasPermukaan }

    private GameObject previewObject;

    public override void GenerateRandomQuestion()
    {
        ResetAnswerState();
        foreach (var obj in objekPilihanAktif) { if (obj != null) Destroy(obj); }
        objekPilihanAktif.Clear();

        if (previewObject != null)
        {
            Destroy(previewObject);
            previewObject = null;
        }

        GradePhase phase = GradeLevelConfig.CurrentPhase;
        float jawaban = 0f;

        if (phase == GradePhase.FaseA)
        {
            // Kelas 1–2 SD: Sifat-sifat dasar bangun ruang sederhana (Kubus, Balok, Bola, Tabung, Kerucut)
            BangunRuang[] basicShapes = new BangunRuang[]
            {
                BangunRuang.Kubus, BangunRuang.Balok, BangunRuang.Bola, BangunRuang.Tabung, BangunRuang.Kerucut
            };
            BangunRuang bangun = basicShapes[Random.Range(0, basicShapes.Length)];

            switch (bangun)
            {
                case BangunRuang.Kubus:
                    int qk = Random.Range(0, 3);
                    if (qk == 0) { soalSaatIni = "Berapa jumlah sisi pada Kubus?"; jawaban = 6f; langkahPenyelesaian = "Kubus memiliki 6 sisi berbentuk persegi."; }
                    else if (qk == 1) { soalSaatIni = "Berapa jumlah rusuk pada Kubus?"; jawaban = 12f; langkahPenyelesaian = "Kubus memiliki 12 rusuk sama panjang."; }
                    else { soalSaatIni = "Berapa jumlah titik sudut pada Kubus?"; jawaban = 8f; langkahPenyelesaian = "Kubus memiliki 8 titik sudut."; }
                    Spawn3DPreview(PrimitiveType.Cube, Vector3.one * 1.5f, "Kubus");
                    break;

                case BangunRuang.Balok:
                    int qb = Random.Range(0, 3);
                    if (qb == 0) { soalSaatIni = "Berapa jumlah sisi pada Balok?"; jawaban = 6f; langkahPenyelesaian = "Balok memiliki 6 sisi (3 pasang sisi yang sama)."; }
                    else if (qb == 1) { soalSaatIni = "Berapa jumlah rusuk pada Balok?"; jawaban = 12f; langkahPenyelesaian = "Balok memiliki 12 rusuk."; }
                    else { soalSaatIni = "Berapa jumlah titik sudut pada Balok?"; jawaban = 8f; langkahPenyelesaian = "Balok memiliki 8 titik sudut."; }
                    Spawn3DPreview(PrimitiveType.Cube, new Vector3(2.2f, 1.2f, 1.4f), "Balok");
                    break;

                case BangunRuang.Bola:
                    soalSaatIni = "Berapa jumlah sisi lengkung pada Bola?";
                    jawaban = 1f;
                    langkahPenyelesaian = "Bola hanya memiliki 1 sisi lengkung dan tidak memiliki titik sudut maupun rusuk.";
                    Spawn3DPreview(PrimitiveType.Sphere, Vector3.one * 1.6f, "Bola");
                    break;

                case BangunRuang.Tabung:
                    int qt = Random.Range(0, 2);
                    if (qt == 0) { soalSaatIni = "Berapa jumlah sisi pada Tabung?"; jawaban = 3f; langkahPenyelesaian = "Tabung memiliki 3 sisi: 1 alas lingkaran, 1 tutup lingkaran, dan 1 selimut."; }
                    else { soalSaatIni = "Berapa jumlah rusuk lengkung pada Tabung?"; jawaban = 2f; langkahPenyelesaian = "Tabung memiliki 2 rusuk lengkung (pada tepi alas dan tutup)."; }
                    Spawn3DPreview(PrimitiveType.Cylinder, new Vector3(1.5f, 1.2f, 1.5f), "Tabung");
                    break;

                default: // Kerucut
                    int qkc = Random.Range(0, 2);
                    if (qkc == 0) { soalSaatIni = "Berapa jumlah titik puncak pada Kerucut?"; jawaban = 1f; langkahPenyelesaian = "Kerucut memiliki 1 titik puncak di bagian atas."; }
                    else { soalSaatIni = "Berapa jumlah sisi pada Kerucut?"; jawaban = 2f; langkahPenyelesaian = "Kerucut memiliki 2 sisi: 1 alas lingkaran dan 1 selimut kerucut."; }
                    Spawn3DPreview(PrimitiveType.Cylinder, new Vector3(1.4f, 1.2f, 1.4f), "Kerucut");
                    break;
            }
        }
        else if (phase == GradePhase.FaseB)
        {
            // Kelas 3–4 SD: Sifat Prisma & Limas + Volume dasar Kubus / Balok
            int mode = Random.Range(0, 4);
            if (mode == 0)
            {
                // Volume Kubus dasar
                int s = Random.Range(2, 6);
                soalSaatIni = $"Berapa Volume Kubus jika panjang rusuknya {s} cm?";
                jawaban = s * s * s;
                langkahPenyelesaian = $"Volume Kubus = s x s x s\nV = {s} x {s} x {s} = {jawaban} cm³";
                Spawn3DPreview(PrimitiveType.Cube, Vector3.one * 1.6f, $"s = {s} cm");
            }
            else if (mode == 1)
            {
                // Volume Balok dasar
                int p = Random.Range(4, 9), l = Random.Range(2, 5), t = Random.Range(2, 5);
                soalSaatIni = $"Volume Balok dengan p={p} cm, l={l} cm, t={t} cm adalah ... cm³";
                jawaban = p * l * t;
                langkahPenyelesaian = $"Volume Balok = p x l x t\nV = {p} x {l} x {t} = {jawaban} cm³";
                Spawn3DPreview(PrimitiveType.Cube, new Vector3(2.2f, 1.2f, 1.4f), $"p={p} cm, l={l} cm, t={t} cm");
            }
            else if (mode == 2)
            {
                // Sifat Prisma Segitiga
                int q = Random.Range(0, 3);
                if (q == 0) { soalSaatIni = "Berapa jumlah sisi pada Prisma Segitiga?"; jawaban = 5f; langkahPenyelesaian = "Prisma Segitiga memiliki 5 sisi: 2 alas segitiga + 3 sisi tegak persegi panjang."; }
                else if (q == 1) { soalSaatIni = "Berapa jumlah rusuk pada Prisma Segitiga?"; jawaban = 9f; langkahPenyelesaian = "Prisma Segitiga memiliki 9 rusuk."; }
                else { soalSaatIni = "Berapa jumlah titik sudut pada Prisma Segitiga?"; jawaban = 6f; langkahPenyelesaian = "Prisma Segitiga memiliki 6 titik sudut (3 di atas, 3 di bawah)."; }
                Spawn3DPreview(PrimitiveType.Cylinder, new Vector3(1.3f, 1.4f, 1.3f), "Prisma Segitiga");
            }
            else
            {
                // Sifat Limas Segiempat
                int q = Random.Range(0, 3);
                if (q == 0) { soalSaatIni = "Berapa jumlah titik sudut pada Limas Segiempat?"; jawaban = 5f; langkahPenyelesaian = "Limas Segiempat memiliki 5 titik sudut (4 di alas + 1 di puncak)."; }
                else if (q == 1) { soalSaatIni = "Berapa jumlah rusuk pada Limas Segiempat?"; jawaban = 8f; langkahPenyelesaian = "Limas Segiempat memiliki 8 rusuk (4 di alas + 4 tegak)."; }
                else { soalSaatIni = "Berapa jumlah sisi pada Limas Segiempat?"; jawaban = 5f; langkahPenyelesaian = "Limas Segiempat memiliki 5 sisi: 1 alas + 4 sisi tegak segitiga."; }
                Spawn3DPreview(PrimitiveType.Cube, new Vector3(1.5f, 1.2f, 1.5f), "Limas Segiempat");
            }
        }
        else
        {
            // Kelas 5–6 SD: Volume dan Luas Permukaan lengkap
            BangunRuang bangun = (BangunRuang)Random.Range(0, 6);
            bool isLuas = Random.value > 0.5f;

            switch (bangun)
            {
                case BangunRuang.Kubus:
                    int s = Random.Range(3, 8);
                    if (isLuas)
                    {
                        soalSaatIni = $"Luas Permukaan Kubus dengan rusuk {s} cm adalah ... cm²";
                        jawaban = 6 * s * s;
                        langkahPenyelesaian = $"Luas Permukaan Kubus = 6 x s²\nL = 6 x ({s} x {s}) = {jawaban} cm²";
                    }
                    else
                    {
                        soalSaatIni = $"Volume Kubus dengan rusuk {s} cm adalah ... cm³";
                        jawaban = s * s * s;
                        langkahPenyelesaian = $"Volume Kubus = s³ = {s} x {s} x {s} = {jawaban} cm³";
                    }
                    Spawn3DPreview(PrimitiveType.Cube, Vector3.one * 1.5f, $"s = {s} cm");
                    break;

                case BangunRuang.Balok:
                    int p = Random.Range(5, 10), l = Random.Range(3, 6), t = Random.Range(2, 5);
                    if (isLuas)
                    {
                        soalSaatIni = $"Luas Permukaan Balok (p={p}, l={l}, t={t}) adalah ... cm²";
                        jawaban = 2 * (p * l + p * t + l * t);
                        langkahPenyelesaian = $"Luas Balok = 2 x (p x l + p x t + l x t)\nL = 2 x ({p*l} + {p*t} + {l*t}) = {jawaban} cm²";
                    }
                    else
                    {
                        soalSaatIni = $"Volume Balok (p={p}, l={l}, t={t}) adalah ... cm³";
                        jawaban = p * l * t;
                        langkahPenyelesaian = $"Volume Balok = p x l x t = {p} x {l} x {t} = {jawaban} cm³";
                    }
                    Spawn3DPreview(PrimitiveType.Cube, new Vector3(2.3f, 1.2f, 1.5f), $"p={p} cm, l={l} cm, t={t} cm");
                    break;

                case BangunRuang.Tabung:
                    int r = Random.Range(7, 14);
                    int tt = Random.Range(10, 16);
                    soalSaatIni = $"Volume Tabung (r={r} cm, t={tt} cm, π=22/7) adalah ... cm³";
                    jawaban = Mathf.Round((22f / 7f) * r * r * tt);
                    langkahPenyelesaian = $"Volume Tabung = π x r² x t\nV = 22/7 x {r} x {r} x {tt} = {jawaban} cm³";
                    Spawn3DPreview(PrimitiveType.Cylinder, new Vector3(1.5f, 1.4f, 1.5f), $"r={r} cm, t={tt} cm");
                    break;

                case BangunRuang.PrismaSegitiga:
                    int a = Random.Range(4, 9), tSegitiga = Random.Range(3, 7), tPrisma = Random.Range(8, 15);
                    soalSaatIni = $"Volume Prisma Segitiga (alas segitiga={a} cm, tinggi segitiga={tSegitiga} cm, t prisma={tPrisma} cm)";
                    jawaban = 0.5f * a * tSegitiga * tPrisma;
                    langkahPenyelesaian = $"Volume = Luas Alas x Tinggi Prisma\nV = (1/2 x {a} x {tSegitiga}) x {tPrisma} = {jawaban} cm³";
                    Spawn3DPreview(PrimitiveType.Cylinder, new Vector3(1.3f, 1.4f, 1.3f), $"a={a} cm, t={tPrisma} cm");
                    break;

                case BangunRuang.Kerucut:
                    int rk = 7;
                    int tk = Random.Range(6, 15);
                    soalSaatIni = $"Volume Kerucut (r={rk} cm, t={tk} cm, π=22/7) adalah ... cm³";
                    jawaban = Mathf.Round((1f / 3f) * (22f / 7f) * rk * rk * tk);
                    langkahPenyelesaian = $"Volume Kerucut = 1/3 x π x r² x t\nV = 1/3 x 22/7 x 49 x {tk} = {jawaban} cm³";
                    Spawn3DPreview(PrimitiveType.Cylinder, new Vector3(1.3f, 1.3f, 1.3f), $"r={rk} cm, t={tk} cm");
                    break;

                default: // Bola
                    soalSaatIni = "Berapa jumlah titik sudut pada bangun Bola?";
                    jawaban = 0f;
                    langkahPenyelesaian = "Bola merupakan bangun ruang sisi lengkung dan memiliki 0 titik sudut.";
                    Spawn3DPreview(PrimitiveType.Sphere, Vector3.one * 1.5f, "r = 7 cm");
                    break;
            }
        }

        questionText.text = soalSaatIni;
        BuatPilihan(jawaban);
    }

    /// <summary>
    /// Step 3.3: Spawn procedural 3D reference visualizer with 3D TextMeshPro dimension labels & touch rotation.
    /// </summary>
    private void Spawn3DPreview(PrimitiveType type, Vector3 scale, string dimensionLabel)
    {
        previewObject = GameObject.CreatePrimitive(type);
        previewObject.name = "Shape3DPreview";

        // Remove collider so it doesn't block Raycasts on answer choices
        Collider col = previewObject.GetComponent<Collider>();
        if (col != null) Destroy(col);

        // Position in front of the camera
        previewObject.transform.position = new Vector3(0f, 0.4f, 2.5f);
        previewObject.transform.localScale = scale * 0.4f;

        // Apply a clean material
        Material mat = Resources.Load<Material>("ModelBangunRuang/Biru");
        if (mat == null)
        {
            Renderer rend = previewObject.GetComponent<Renderer>();
            if (rend != null)
            {
                Shader s = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
                if (s != null)
                {
                    Material m = new Material(s);
                    m.color = new Color(0.18f, 0.58f, 0.95f, 0.95f);
                    rend.material = m;
                }
            }
        }
        else
        {
            previewObject.GetComponent<Renderer>().material = mat;
        }

        // Add RotasiSentuh for touch interactivity
        if (previewObject.GetComponent<RotasiSentuh>() == null)
        {
            previewObject.AddComponent<RotasiSentuh>();
        }

        // Attach floating 3D TextMeshPro dimension label
        if (!string.IsNullOrEmpty(dimensionLabel))
        {
            GameObject labelGo = new GameObject("DimensionLabel");
            labelGo.transform.SetParent(previewObject.transform, false);
            labelGo.transform.localPosition = new Vector3(0f, 1.4f, 0f);

            TextMeshPro tmp = labelGo.AddComponent<TextMeshPro>();
            tmp.text = dimensionLabel;
            tmp.fontSize = 7;
            tmp.fontStyle = FontStyles.Bold;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.yellow;

            // Make sure label faces camera
            labelGo.AddComponent<LookAtCamera>();
        }
    }

    public override bool IsAnswerCorrect(string playerAnswer)
    {
        if (float.TryParse(playerAnswer, out float pa) && float.TryParse(correctAnswerValue, out float ca))
            return Mathf.Abs(pa - ca) < 0.5f;
        return false;
    }

    protected override string ConvertAnswerToString(float answer) => answer.ToString("F0");

    private void BuatPilihan(float jawabanBenar)
    {
        bool isInteger = Mathf.Approximately(jawabanBenar, Mathf.Round(jawabanBenar));
        int jawabanInt = Mathf.RoundToInt(jawabanBenar);
        string[] h = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K" };

        List<int> pilihan = new List<int> { jawabanInt };
        int tries = 0;
        while (pilihan.Count < 4 && tries < 50)
        {
            tries++;
            int offset = Random.Range(1, 5) * (Random.value > 0.5f ? 1 : -1);
            int pengecoh = jawabanInt + offset;
            if (pengecoh >= 0 && !pilihan.Contains(pengecoh))
            {
                pilihan.Add(pengecoh);
            }
        }

        for (int i = pilihan.Count - 1; i > 0; i--)
        {
            int r = Random.Range(0, i + 1);
            int tmp = pilihan[i]; pilihan[i] = pilihan[r]; pilihan[r] = tmp;
        }

        string correctLabel = null;
        for (int i = 0; i < pilihan.Count && i < h.Length; i++)
        {
            if (pilihan[i] == jawabanInt)
            {
                correctLabel = h[i];
                break;
            }
        }

        string[] strs = pilihan.ConvertAll(x => x.ToString()).ToArray();
        objekPilihanAktif = itemSpawner.SpawnPilihan(strs, h, this);

        correctAnswerLabel = correctLabel;
        correctAnswerValue = jawabanInt.ToString();
    }
}
