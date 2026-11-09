using UnityEngine;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Kuis bangun ruang & bangun datar sesuai kurikulum SD (Fase A, B, C)
/// dengan 50% soal cerita kontekstual, label dimensi 3D, dan sistem petunjuk.
/// </summary>
public class BangunRuangManager : BaseQuizManager<float>
{
    private enum BangunRuang
    {
        Kubus, Balok, Bola, Kerucut, Tabung,
        PrismaSegitiga, LimasSegitiga, LimasSegiempat
    }

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
        bool isSoalCerita = Random.value < 0.5f;
        string nama = GradeLevelConfig.GetRandomStudentName();

        if (phase == GradePhase.FaseA)
        {
            // Kelas 1–2 SD: Mengenal bentuk dan sifat bangun ruang (sisi, rusuk, titik sudut)
            BangunRuang[] basicShapes = new BangunRuang[]
            {
                BangunRuang.Kubus, BangunRuang.Balok, BangunRuang.Bola, BangunRuang.Tabung, BangunRuang.Kerucut
            };
            BangunRuang bangun = basicShapes[Random.Range(0, basicShapes.Length)];

            switch (bangun)
            {
                case BangunRuang.Kubus:
                    int qk = Random.Range(0, 3);
                    if (qk == 0)
                    {
                        jawaban = 6f;
                        soalSaatIni = isSoalCerita
                            ? $"{nama} membungkus kotak kado berbentuk kubus dengan kertas kado. Berapa banyak sisi datar yang harus dibungkus?"
                            : "Berapa jumlah sisi pada bangun Kubus?";
                        langkahPenyelesaian = "Kubus memiliki 6 sisi datar berbentuk persegi yang sama besar.";
                        petunjukSoal = "Kubus memiliki sisi atas, bawah, depan, belakang, kanan, dan kiri (total 6 sisi).";
                    }
                    else if (qk == 1)
                    {
                        jawaban = 12f;
                        soalSaatIni = isSoalCerita
                            ? $"Kerangka sebuah sangkar berbentuk kubus terbuat dari kawat. Berapa banyak kawat rusuk pada kubus tersebut?"
                            : "Berapa jumlah rusuk pada bangun Kubus?";
                        langkahPenyelesaian = "Kubus memiliki 12 rusuk yang sama panjang.";
                        petunjukSoal = "Hitung garis rusuk: 4 di alas, 4 di atas, dan 4 rusuk tegak (total 12 rusuk).";
                    }
                    else
                    {
                        jawaban = 8f;
                        soalSaatIni = "Berapa banyak titik sudut (pojok) pada bangun Kubus?";
                        langkahPenyelesaian = "Kubus memiliki 8 titik sudut (4 di bagian atas dan 4 di bagian bawah).";
                        petunjukSoal = "Titik sudut adalah pojok pertemuan rusuk: 4 di atas dan 4 di bawah (total 8).";
                    }
                    Spawn3DPreview(PrimitiveType.Cube, Vector3.one * 1.5f, "Kubus");
                    break;

                case BangunRuang.Balok:
                    int qb = Random.Range(0, 2);
                    if (qb == 0)
                    {
                        jawaban = 6f;
                        soalSaatIni = isSoalCerita
                            ? $"Sebuah kardus mie instan berbentuk balok. Berapa banyak sisi bidang pada kardus tersebut?"
                            : "Berapa jumlah sisi pada bangun Balok?";
                        langkahPenyelesaian = "Balok memiliki 6 sisi (3 pasang sisi yang berhadapan sama besar).";
                        petunjukSoal = "Sama seperti kubus, balok memiliki 6 sisi bidang.";
                    }
                    else
                    {
                        jawaban = 12f;
                        soalSaatIni = "Berapa banyak jumlah rusuk pada bangun Balok?";
                        langkahPenyelesaian = "Balok memiliki 12 rusuk (4 panjang, 4 lebar, dan 4 tinggi).";
                        petunjukSoal = "Balok memiliki 12 rusuk (4 rusuk panjang, 4 rusuk lebar, dan 4 rusuk tinggi).";
                    }
                    Spawn3DPreview(PrimitiveType.Cube, new Vector3(2.2f, 1.2f, 1.4f), "Balok");
                    break;

                case BangunRuang.Bola:
                    jawaban = 1f;
                    soalSaatIni = isSoalCerita
                        ? $"{nama} sedang bermain bola sepak. Berapa jumlah sisi lengkung pada sebuah bola?"
                        : "Berapa jumlah sisi lengkung pada bangun ruang Bola?";
                    langkahPenyelesaian = "Bola hanya memiliki 1 sisi lengkung dan tidak memiliki rusuk maupun titik sudut.";
                    petunjukSoal = "Bola tidak memiliki sudut maupun rusuk, hanya 1 bidang lengkung utuh.";
                    Spawn3DPreview(PrimitiveType.Sphere, Vector3.one * 1.6f, "Bola");
                    break;

                case BangunRuang.Tabung:
                    jawaban = 3f;
                    soalSaatIni = isSoalCerita
                        ? $"Sebuah kaleng susu berbentuk tabung. Berapa jumlah seluruh sisi (alas, tutup, selimut) pada kaleng tersebut?"
                        : "Berapa jumlah sisi pada bangun ruang Tabung?";
                    langkahPenyelesaian = "Tabung memiliki 3 sisi: 1 sisi alas lingkaran, 1 sisi tutup lingkaran, dan 1 sisi selimut lengkung.";
                    petunjukSoal = "Hitung alas (lingkaran), tutup (lingkaran), dan selimut melingkar (total 3 sisi).";
                    Spawn3DPreview(PrimitiveType.Cylinder, new Vector3(1.5f, 1.2f, 1.5f), "Tabung");
                    break;

                default: // Kerucut
                    jawaban = 1f;
                    soalSaatIni = isSoalCerita
                        ? $"Topi ulang tahun berbentuk kerucut. Berapa banyak titik puncak pada topi tersebut?"
                        : "Berapa jumlah titik puncak pada bangun ruang Kerucut?";
                    langkahPenyelesaian = "Kerucut memiliki 1 titik puncak di bagian paling atas.";
                    petunjukSoal = "Kerucut memiliki tepat 1 titik sudut puncak di ujung atas.";
                    Spawn3DPreview(PrimitiveType.Cylinder, new Vector3(1.4f, 1.2f, 1.4f), "Kerucut");
                    break;
            }
        }
        else if (phase == GradePhase.FaseB)
        {
            // Kelas 3–4 SD: Keliling & Luas Bangun Datar serta Sifat Prisma / Limas
            int mode = Random.Range(0, 4);
            if (mode == 0)
            {
                // Bangun Datar: Keliling Persegi Panjang
                int p = Random.Range(6, 15), l = Random.Range(3, 8);
                jawaban = 2 * (p + l);
                soalSaatIni = isSoalCerita
                    ? $"Sebuah kolam renang mini berbentuk persegi panjang memiliki panjang {p} m dan lebar {l} m. Berapa meter keliling kolam tersebut?"
                    : $"Keliling persegi panjang dengan panjang {p} cm dan lebar {l} cm adalah ... cm";
                langkahPenyelesaian = $"Keliling Persegi Panjang = 2 x (p + l)\nK = 2 x ({p} + {l}) = 2 x {p + l} = {jawaban} cm";
                petunjukSoal = "Rumus Keliling Persegi Panjang: K = 2 x (panjang + lebar).";
                Spawn3DPreview(PrimitiveType.Cube, new Vector3(2.2f, 0.2f, 1.4f), $"p={p} m, l={l} m");
            }
            else if (mode == 1)
            {
                // Volume Kubus dasar
                int s = Random.Range(2, 6);
                jawaban = s * s * s;
                soalSaatIni = isSoalCerita
                    ? $"Sebuah bak penampungan berbentuk kubus memiliki rusuk {s} m. Berapa m³ volume air jika bak terisi penuh?"
                    : $"Berapa Volume Kubus jika panjang rusuknya {s} cm?";
                langkahPenyelesaian = $"Volume Kubus = s x s x s\nV = {s} x {s} x {s} = {jawaban} cm³";
                petunjukSoal = "Rumus Volume Kubus: V = sisi x sisi x sisi (s³).";
                Spawn3DPreview(PrimitiveType.Cube, Vector3.one * 1.6f, $"s = {s} cm");
            }
            else if (mode == 2)
            {
                // Volume Balok dasar
                int p = Random.Range(4, 8), l = Random.Range(2, 5), t = Random.Range(2, 5);
                jawaban = p * l * t;
                soalSaatIni = isSoalCerita
                    ? $"Sebuah akuarium balok memiliki ukuran panjang {p} cm, lebar {l} cm, dan tinggi {t} cm. Berapa cm³ kapasitas akuarium?"
                    : $"Volume Balok dengan p={p} cm, l={l} cm, t={t} cm adalah ... cm³";
                langkahPenyelesaian = $"Volume Balok = p x l x t\nV = {p} x {l} x {t} = {jawaban} cm³";
                petunjukSoal = "Rumus Volume Balok: V = panjang x lebar x tinggi.";
                Spawn3DPreview(PrimitiveType.Cube, new Vector3(2.2f, 1.2f, 1.4f), $"p={p}, l={l}, t={t}");
            }
            else
            {
                // Sifat Prisma Segitiga
                jawaban = 9f;
                soalSaatIni = isSoalCerita
                    ? $"Tenda kemah pramuka berbentuk prisma segitiga. Berapa jumlah seluruh rusuk pada tenda tersebut?"
                    : "Berapa banyak jumlah rusuk pada bangun Prisma Segitiga?";
                langkahPenyelesaian = "Prisma Segitiga memiliki 9 rusuk: 3 rusuk alas, 3 rusuk tutup, dan 3 rusuk tegak.";
                petunjukSoal = "Prisma segitiga memiliki 3 rusuk di alas segitiga, 3 di tutup segitiga, dan 3 rusuk tegak penyangga.";
                Spawn3DPreview(PrimitiveType.Cylinder, new Vector3(1.3f, 1.4f, 1.3f), "Prisma Segitiga");
            }
        }
        else
        {
            // Kelas 5–6 SD: Volume dan Luas Permukaan lengkap
            BangunRuang bangun = (BangunRuang)Random.Range(0, 5);
            bool isLuas = Random.value > 0.5f;

            switch (bangun)
            {
                case BangunRuang.Kubus:
                    int s = Random.Range(3, 8);
                    if (isLuas)
                    {
                        jawaban = 6 * s * s;
                        soalSaatIni = isSoalCerita
                            ? $"Sebuah kardus kubus dengan panjang rusuk {s} cm akan dicat seluruh permukaannya. Berapa cm² luas permukaan yang dicat?"
                            : $"Luas Permukaan Kubus dengan rusuk {s} cm adalah ... cm²";
                        langkahPenyelesaian = $"Luas Permukaan Kubus = 6 x s²\nL = 6 x ({s} x {s}) = {jawaban} cm²";
                        petunjukSoal = "Kubus memiliki 6 sisi persegi yang sama. Luas = 6 x (sisi x sisi).";
                    }
                    else
                    {
                        jawaban = s * s * s;
                        soalSaatIni = isSoalCerita
                            ? $"Sebuah kotak perhiasan berbentuk kubus memiliki rusuk {s} cm. Berapa volume kotak perhiasan tersebut?"
                            : $"Volume Kubus dengan rusuk {s} cm adalah ... cm³";
                        langkahPenyelesaian = $"Volume Kubus = s³ = {s} x {s} x {s} = {jawaban} cm³";
                        petunjukSoal = "Volume kubus = s³ (rusuk dikalikan tiga kali).";
                    }
                    Spawn3DPreview(PrimitiveType.Cube, Vector3.one * 1.5f, $"s = {s} cm");
                    break;

                case BangunRuang.Balok:
                    int p = Random.Range(5, 10), l = Random.Range(3, 6), t = Random.Range(2, 5);
                    if (isLuas)
                    {
                        jawaban = 2 * (p * l + p * t + l * t);
                        soalSaatIni = $"Luas Permukaan Balok (p={p} cm, l={l} cm, t={t} cm) adalah ... cm²";
                        langkahPenyelesaian = $"Luas Balok = 2 x (p x l + p x t + l x t)\nL = 2 x ({p * l} + {p * t} + {l * t}) = {jawaban} cm²";
                        petunjukSoal = "Rumus Luas Permukaan Balok: L = 2 x (p×l + p×t + l×t).";
                    }
                    else
                    {
                        jawaban = p * l * t;
                        soalSaatIni = isSoalCerita
                            ? $"Sebuah bak mandi balok memiliki panjang {p} cm, lebar {l} cm, dan tinggi {t} cm. Berapa cm³ volume bak tersebut?"
                            : $"Volume Balok (p={p} cm, l={l} cm, t={t} cm) adalah ... cm³";
                        langkahPenyelesaian = $"Volume Balok = p x l x t = {p} x {l} x {t} = {jawaban} cm³";
                        petunjukSoal = "Rumus Volume Balok: V = panjang x lebar x tinggi.";
                    }
                    Spawn3DPreview(PrimitiveType.Cube, new Vector3(2.3f, 1.2f, 1.5f), $"p={p}, l={l}, t={t}");
                    break;

                case BangunRuang.Tabung:
                    int r = 7;
                    int tt = Random.Range(10, 15);
                    jawaban = Mathf.Round((22f / 7f) * r * r * tt);
                    soalSaatIni = isSoalCerita
                        ? $"Sebuah drum minyak berbentuk tabung memiliki jari-jari {r} cm dan tinggi {tt} cm (π = 22/7). Berapa cm³ volume drum minyak tersebut?"
                        : $"Volume Tabung (r={r} cm, t={tt} cm, π=22/7) adalah ... cm³";
                    langkahPenyelesaian = $"Volume Tabung = π x r² x t\nV = 22/7 x 49 x {tt} = 154 x {tt} = {jawaban} cm³";
                    petunjukSoal = "Rumus Volume Tabung: V = π x r² x t. Gunakan π = 22/7.";
                    Spawn3DPreview(PrimitiveType.Cylinder, new Vector3(1.5f, 1.4f, 1.5f), $"r={r} cm, t={tt} cm");
                    break;

                case BangunRuang.PrismaSegitiga:
                    int a = Random.Range(4, 8), tAlas = Random.Range(3, 6), tPrisma = Random.Range(8, 12);
                    jawaban = 0.5f * a * tAlas * tPrisma;
                    soalSaatIni = $"Volume Prisma Segitiga (alas segitiga={a} cm, tinggi alas={tAlas} cm, tinggi prisma={tPrisma} cm)";
                    langkahPenyelesaian = $"Volume = Luas Alas x Tinggi Prisma\nV = (1/2 x {a} x {tAlas}) x {tPrisma} = {jawaban} cm³";
                    petunjukSoal = "Volume Prisma = Luas Alas (1/2 × a × t) × Tinggi Prisma.";
                    Spawn3DPreview(PrimitiveType.Cylinder, new Vector3(1.3f, 1.4f, 1.3f), $"a={a}, t_alas={tAlas}, t={tPrisma}");
                    break;

                default: // Kerucut
                    int rk = 7;
                    int tk = Random.Range(6, 15);
                    jawaban = Mathf.Round((1f / 3f) * (22f / 7f) * rk * rk * tk);
                    soalSaatIni = isSoalCerita
                        ? $"Sebuah nasi tumpeng berbentuk kerucut memiliki jari-jari alas {rk} cm dan tinggi {tk} cm (π=22/7). Berapa volume tumpeng tersebut?"
                        : $"Volume Kerucut (r={rk} cm, t={tk} cm, π=22/7) adalah ... cm³";
                    langkahPenyelesaian = $"Volume Kerucut = 1/3 x π x r² x t\nV = 1/3 x 22/7 x 49 x {tk} = {jawaban} cm³";
                    petunjukSoal = "Volume Kerucut = 1/3 × Volume Tabung = 1/3 × π × r² × t.";
                    Spawn3DPreview(PrimitiveType.Cylinder, new Vector3(1.3f, 1.3f, 1.3f), $"r={rk} cm, t={tk} cm");
                    break;
            }
        }

        questionText.text = soalSaatIni;
        BuatPilihan(jawaban);
    }

    private void Spawn3DPreview(PrimitiveType type, Vector3 scale, string dimensionLabel)
    {
        previewObject = GameObject.CreatePrimitive(type);
        previewObject.name = "Shape3DPreview";

        Collider col = previewObject.GetComponent<Collider>();
        if (col != null) Destroy(col);

        previewObject.transform.position = new Vector3(0f, 0.4f, 2.5f);
        previewObject.transform.localScale = scale * 0.4f;

        Material mat = Resources.Load<Material>("ModelBangunRuang/Biru");
        if (mat == null)
        {
            Renderer rend = previewObject.GetComponent<Renderer>();
            if (rend != null)
            {
                Shader s = Shader.Find("Standard") ?? Shader.Find("Mobile/Diffuse") ?? Shader.Find("Sprites/Default");
                if (s != null)
                {
                    Material m = new Material(s) { color = new Color(0.18f, 0.58f, 0.95f, 0.95f) };
                    rend.material = m;
                }
            }
        }
        else
        {
            previewObject.GetComponent<Renderer>().material = mat;
        }

        if (previewObject.GetComponent<RotasiSentuh>() == null)
        {
            previewObject.AddComponent<RotasiSentuh>();
        }

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
