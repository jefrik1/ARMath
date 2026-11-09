using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Kuis pecahan sesuai fase kurikulum SD (Kelas 1–6 SD).
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
            pembilang /= fpb;
            penyebut /= fpb;
        }

        public override string ToString() => pembilang + "/" + penyebut;
    }

    public static int FPB(int a, int b) => b == 0 ? System.Math.Abs(a) : FPB(b, a % b);
    public static int KPK(int a, int b) => System.Math.Abs(a * b) / FPB(a, b);

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

        if (TryLoadFromJson("Pecahan"))
        {
            questionText.text = soalSaatIni;
            return;
        }

        GradePhase phase = GradeLevelConfig.CurrentPhase;
        Pecahan hasil;

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
                soalSaatIni = $"Pecahan manakah yang senilai dengan {p1}?";
                langkahPenyelesaian = $"Pecahan senilai didapat dengan mengalikan pembilang dan penyebut dengan bilangan yang sama:\n{p1} x {multiplier}/{multiplier} = {hasil}";
            }
            else
            {
                hasil = p1;
                soalSaatIni = $"Satu kue dibagi menjadi {den} bagian sama besar. Nilai satu bagian adalah ...";
                langkahPenyelesaian = $"Satu bagian dari total {den} bagian sama besar bernilai {p1}.";
            }
        }
        else if (phase == GradePhase.FaseB)
        {
            // Kelas 3–4 SD: Pecahan berpenyebut sama (penyebut <= 12) atau pecahan senilai
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
                soalSaatIni = $"{f1} + {f2} = ...";
                langkahPenyelesaian = $"Karena penyebut sudah sama ({den}), cukup jumlahkan pembilangnya:\n{p1}/{den} + {p2}/{den} = {p1 + p2}/{den} = {hasil}";
            }
            else
            {
                if (p1 < p2) { int tmp = p1; p1 = p2; p2 = tmp; }
                if (p1 == p2) p1++;
                Pecahan f1 = new Pecahan(p1, den);
                Pecahan f2 = new Pecahan(p2, den);
                hasil = new Pecahan(p1 - p2, den);
                hasil.Sederhanakan();
                soalSaatIni = $"{f1} - {f2} = ...";
                langkahPenyelesaian = $"Karena penyebut sudah sama ({den}), cukup kurangkan pembilangnya:\n{p1}/{den} - {p2}/{den} = {p1 - p2}/{den} = {hasil}";
            }
        }
        else
        {
            // Kelas 5–6 SD: Penjumlahan/pengurangan beda penyebut (KPK), perkalian, atau pembagian
            int op = Random.Range(0, 4);
            int b = Random.Range(2, 9);
            int a = Random.Range(1, b);
            int d = Random.Range(2, 9);
            while (d == b) d = Random.Range(2, 9); // pastikan beda penyebut
            int c = Random.Range(1, d);

            Pecahan f1 = new Pecahan(a, b); f1.Sederhanakan();
            Pecahan f2 = new Pecahan(c, d); f2.Sederhanakan();

            switch (op)
            {
                case 0:
                    hasil = Tambah(f1, f2);
                    hasil.Sederhanakan();
                    int kpkAdd = KPK(f1.penyebut, f2.penyebut);
                    soalSaatIni = $"{f1} + {f2} = ...";
                    langkahPenyelesaian = $"Samakan penyebut dengan KPK({f1.penyebut}, {f2.penyebut}) = {kpkAdd}:\n" +
                                          $"{f1.pembilang * (kpkAdd / f1.penyebut)}/{kpkAdd} + {f2.pembilang * (kpkAdd / f2.penyebut)}/{kpkAdd} = {hasil}";
                    break;
                case 1:
                    if ((float)f1.pembilang / f1.penyebut < (float)f2.pembilang / f2.penyebut)
                    {
                        var temp = f1; f1 = f2; f2 = temp;
                    }
                    hasil = Kurang(f1, f2);
                    hasil.Sederhanakan();
                    int kpkSub = KPK(f1.penyebut, f2.penyebut);
                    soalSaatIni = $"{f1} - {f2} = ...";
                    langkahPenyelesaian = $"Samakan penyebut dengan KPK({f1.penyebut}, {f2.penyebut}) = {kpkSub}:\n" +
                                          $"{f1.pembilang * (kpkSub / f1.penyebut)}/{kpkSub} - {f2.pembilang * (kpkSub / f2.penyebut)}/{kpkSub} = {hasil}";
                    break;
                case 2:
                    hasil = Kali(f1, f2);
                    hasil.Sederhanakan();
                    soalSaatIni = $"{f1} x {f2} = ...";
                    langkahPenyelesaian = $"Kalikan pembilang dengan pembilang, dan penyebut dengan penyebut:\n" +
                                          $"{f1.pembilang} x {f2.pembilang} / {f1.penyebut} x {f2.penyebut} = {hasil}";
                    break;
                default:
                    hasil = Bagi(f1, f2);
                    hasil.Sederhanakan();
                    soalSaatIni = $"{f1} : {f2} = ...";
                    langkahPenyelesaian = $"Balikkan pecahan pembagi lalu kalikan:\n" +
                                          $"{f1} x {f2.penyebut}/{f2.pembilang} = {hasil}";
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
