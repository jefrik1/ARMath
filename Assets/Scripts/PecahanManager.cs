using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Kuis pecahan sesuai fase kurikulum SD (Kelas 1–6 SD)
/// dengan 50% soal cerita kontekstual dan sistem petunjuk.
/// </summary>
public class PecahanManager : BaseQuizManager<string>
{
    public struct Pecahan
    {
        public int pembilang, penyebut;

        public Pecahan(int p, int y)
        {
            pembilang = p;
            penyebut = y == 0 ? 1 : y;
        }

        public void Sederhanakan()
        {
            if (penyebut == 0) return;
            if (penyebut < 0) { pembilang = -pembilang; penyebut = -penyebut; }
            int fpb = FPB(pembilang, penyebut);
            if (fpb > 0)
            {
                pembilang /= fpb;
                penyebut /= fpb;
            }
        }

        public override string ToString() => pembilang + "/" + penyebut;
    }

    public static int FPB(int a, int b) => b == 0 ? System.Math.Abs(a) : FPB(b, a % b);
    public static int KPK(int a, int b) => System.Math.Abs(a * b) / (FPB(a, b) == 0 ? 1 : FPB(a, b));

    Pecahan Tambah(Pecahan a, Pecahan b) =>
        new Pecahan(a.pembilang * b.penyebut + b.pembilang * a.penyebut, a.penyebut * b.penyebut);

    Pecahan Kurang(Pecahan a, Pecahan b) =>
        new Pecahan(a.pembilang * b.penyebut - b.pembilang * a.penyebut, a.penyebut * b.penyebut);

    Pecahan Kali(Pecahan a, Pecahan b) =>
        new Pecahan(a.pembilang * b.pembilang, a.penyebut * b.penyebut);

    Pecahan Bagi(Pecahan a, Pecahan b) =>
        new Pecahan(a.pembilang * b.penyebut, a.penyebut * b.pembilang);

    public override void GenerateRandomQuestion()
    {
        ResetAnswerState();
        foreach (var obj in objekPilihanAktif) { if (obj != null) Destroy(obj); }
        objekPilihanAktif.Clear();

        GradePhase phase = GradeLevelConfig.CurrentPhase;
        Pecahan hasil;
        bool isSoalCerita = Random.value < 0.5f;
        string nama = GradeLevelConfig.GetRandomStudentName();

        if (phase == GradePhase.FaseA)
        {
            // Kelas 1–2 SD: Pecahan sederhana pembilang 1 atau pecahan senilai dasar
            int den = Random.Range(2, 5); // 2, 3, 4
            Pecahan p1 = new Pecahan(1, den);
            bool isSenilai = Random.value > 0.5f;

            if (isSenilai)
            {
                int multiplier = Random.Range(2, 4);
                hasil = new Pecahan(1 * multiplier, den * multiplier);

                if (isSoalCerita)
                {
                    soalSaatIni = $"Siti membagi kue bolu menjadi {den} bagian ({p1}). {nama} ingin memotongnya lebih kecil menjadi {hasil.penyebut} bagian sama besar. Pecahan senilai yang tepat adalah ...";
                }
                else
                {
                    soalSaatIni = $"Pecahan manakah yang senilai dengan {p1}?";
                }

                langkahPenyelesaian = $"Pecahan senilai didapat dengan mengalikan pembilang dan penyebut dengan angka yang sama:\n{p1} x {multiplier}/{multiplier} = {hasil}";
                petunjukSoal = $"Kalikan angka atas (pembilang) dan angka bawah (penyebut) dengan bilangan yang sama ({multiplier}).";
            }
            else
            {
                hasil = p1;
                if (isSoalCerita)
                {
                    soalSaatIni = $"Ibu memotong sebuah pizza menjadi {den} potong sama besar. {nama} memakan 1 potong. Berapa bagian pizza yang dimakan {nama}?";
                }
                else
                {
                    soalSaatIni = $"Satu lingkaran utuh dibagi menjadi {den} bagian sama besar. Nilai 1 bagian tersebut adalah ...";
                }

                langkahPenyelesaian = $"Satu bagian dari total {den} bagian sama besar bernilai {p1}.";
                petunjukSoal = "Ingat: Pembilang (angka atas) adalah bagian yang diambil (1), Penyebut (angka bawah) adalah total seluruh potongan.";
            }
        }
        else if (phase == GradePhase.FaseB)
        {
            // Kelas 3–4 SD: Pecahan berpenyebut sama (penyebut <= 12)
            int den = Random.Range(3, 11);
            int p1 = Random.Range(1, den);
            int p2 = Random.Range(1, den);

            bool isTambah = Random.value > 0.5f;
            if (isTambah)
            {
                Pecahan f1 = new Pecahan(p1, den);
                Pecahan f2 = new Pecahan(p2, den);
                hasil = new Pecahan(p1 + p2, den);
                hasil.Sederhanakan();

                if (isSoalCerita)
                {
                    soalSaatIni = $"{nama} meminum {f1} liter air di pagi hari dan {f2} liter air di siang hari. Berapa total air yang diminum {nama}?";
                }
                else
                {
                    soalSaatIni = $"{f1} + {f2} = ...";
                }

                langkahPenyelesaian = $"Karena penyebutnya sudah sama ({den}), cukup jumlahkan pembilangnya:\n{p1}/{den} + {p2}/{den} = {p1 + p2}/{den} = {hasil}";
                petunjukSoal = $"Penyebut kedua pecahan sudah sama yaitu {den}. Jumlahkan pembilang ({p1} + {p2}), penyebutnya tetap sama.";
            }
            else
            {
                if (p1 < p2) { int tmp = p1; p1 = p2; p2 = tmp; }
                if (p1 == p2) p1++;
                Pecahan f1 = new Pecahan(p1, den);
                Pecahan f2 = new Pecahan(p2, den);
                hasil = new Pecahan(p1 - p2, den);
                hasil.Sederhanakan();

                if (isSoalCerita)
                {
                    soalSaatIni = $"Ada pita sepanjang {f1} meter. Dipakai untuk menghias kado sepanjang {f2} meter. Berapa sisa panjang pita sekarang?";
                }
                else
                {
                    soalSaatIni = $"{f1} - {f2} = ...";
                }

                langkahPenyelesaian = $"Karena penyebutnya sudah sama ({den}), cukup kurangkan pembilangnya:\n{p1}/{den} - {p2}/{den} = {p1 - p2}/{den} = {hasil}";
                petunjukSoal = $"Penyebut kedua pecahan sudah sama ({den}). Kurangkan pembilang ({p1} - {p2}).";
            }
        }
        else
        {
            // Kelas 5–6 SD: Penjumlahan/pengurangan beda penyebut (KPK), perkalian, atau pembagian
            int op = Random.Range(0, 4);
            int b = Random.Range(2, 8);
            int a = Random.Range(1, b);
            int d = Random.Range(2, 8);
            while (d == b) d = Random.Range(2, 8);
            int c = Random.Range(1, d);

            Pecahan f1 = new Pecahan(a, b); f1.Sederhanakan();
            Pecahan f2 = new Pecahan(c, d); f2.Sederhanakan();

            switch (op)
            {
                case 0:
                    hasil = Tambah(f1, f2);
                    hasil.Sederhanakan();
                    int kpkAdd = KPK(f1.penyebut, f2.penyebut);

                    if (isSoalCerita)
                    {
                        soalSaatIni = $"Pak Tani mencangkul {f1} bagian sawah di pagi hari dan {f2} bagian di sore hari. Berapa total bagian sawah yang sudah dicangkul?";
                    }
                    else
                    {
                        soalSaatIni = $"{f1} + {f2} = ...";
                    }

                    langkahPenyelesaian = $"Samakan penyebut dengan KPK({f1.penyebut}, {f2.penyebut}) = {kpkAdd}:\n" +
                                          $"{f1.pembilang * (kpkAdd / f1.penyebut)}/{kpkAdd} + {f2.pembilang * (kpkAdd / f2.penyebut)}/{kpkAdd} = {hasil}";
                    petunjukSoal = $"Cari KPK dari penyebut {f1.penyebut} dan {f2.penyebut} untuk menyamakan penyebut pecahan.";
                    break;

                case 1:
                    if ((float)f1.pembilang / f1.penyebut < (float)f2.pembilang / f2.penyebut)
                    {
                        var temp = f1; f1 = f2; f2 = temp;
                    }
                    hasil = Kurang(f1, f2);
                    hasil.Sederhanakan();
                    int kpkSub = KPK(f1.penyebut, f2.penyebut);

                    if (isSoalCerita)
                    {
                        soalSaatIni = $"Ibu memiliki persediaan tepung {f1} kg. Digunakan untuk membuat kue sebanyak {f2} kg. Berapa sisa tepung ibu?";
                    }
                    else
                    {
                        soalSaatIni = $"{f1} - {f2} = ...";
                    }

                    langkahPenyelesaian = $"Samakan penyebut dengan KPK({f1.penyebut}, {f2.penyebut}) = {kpkSub}:\n" +
                                          $"{f1.pembilang * (kpkSub / f1.penyebut)}/{kpkSub} - {f2.pembilang * (kpkSub / f2.penyebut)}/{kpkSub} = {hasil}";
                    petunjukSoal = $"Samakan penyebut kedua pecahan terlebih dahulu menggunakan KPK ({kpkSub}).";
                    break;

                case 2:
                    hasil = Kali(f1, f2);
                    hasil.Sederhanakan();
                    soalSaatIni = $"{f1} x {f2} = ...";
                    langkahPenyelesaian = $"Kalikan pembilang dengan pembilang, dan penyebut dengan penyebut:\n" +
                                          $"{f1.pembilang} x {f2.pembilang} / ({f1.penyebut} x {f2.penyebut}) = {hasil}";
                    petunjukSoal = "Pada perkalian pecahan, kalikan pembilang dengan pembilang dan penyebut dengan penyebut secara langsung.";
                    break;

                default:
                    hasil = Bagi(f1, f2);
                    hasil.Sederhanakan();
                    soalSaatIni = $"{f1} : {f2} = ...";
                    langkahPenyelesaian = $"Balikkan pecahan pembagi lalu kalikan:\n" +
                                          $"{f1} x {f2.penyebut}/{f2.pembilang} = {hasil}";
                    petunjukSoal = $"Ubah tanda bagi (:) menjadi kali (x), lalu balikkan pecahan kedua menjadi {f2.penyebut}/{f2.pembilang}.";
                    break;
            }
        }

        questionText.text = soalSaatIni;
        BuatPilihan(hasil.ToString());
    }

    public override bool IsAnswerCorrect(string playerAnswer)
    {
        return playerAnswer == correctAnswerValue;
    }

    protected override string ConvertAnswerToString(string answer) => answer;

    private void BuatPilihan(string jawabanBenar)
    {
        List<string> pilihan = new List<string> { jawabanBenar };
        int tries = 0;
        while (pilihan.Count < 4 && tries < 50)
        {
            tries++;
            string[] parts = jawabanBenar.Split('/');
            int num = int.Parse(parts[0]);
            int den = parts.Length > 1 ? int.Parse(parts[1]) : 1;
            int pNum = Mathf.Max(1, num + Random.Range(-4, 5));
            int pDen = Mathf.Max(2, den + Random.Range(-3, 4));

            Pecahan dist = new Pecahan(pNum, pDen);
            dist.Sederhanakan();
            string distStr = dist.ToString();
            if (!pilihan.Contains(distStr))
            {
                pilihan.Add(distStr);
            }
        }
        SpawnPilihan(pilihan, jawabanBenar);
    }
}
