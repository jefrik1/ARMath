using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Kuis perkalian bilangan sesuai fase kurikulum SD (Kelas 1–6 SD)
/// dengan 50% soal cerita kontekstual dan sistem petunjuk.
/// </summary>
public class KaliManager : BaseQuizManager<int>
{
    public override void GenerateRandomQuestion()
    {
        ResetAnswerState();
        foreach (var obj in objekPilihanAktif) { if (obj != null) Destroy(obj); }
        objekPilihanAktif.Clear();

        GradePhase phase = GradeLevelConfig.CurrentPhase;
        int num1, num2, jawabanBenar;
        bool isSoalCerita = Random.value < 0.5f;
        string nama = GradeLevelConfig.GetRandomStudentName();
        string barang = GradeLevelConfig.GetRandomItemName();

        if (phase == GradePhase.FaseA)
        {
            // Kelas 1–2 SD: Penjumlahan berulang dasar, tabel 2–5
            num1 = Random.Range(2, 6);
            num2 = Random.Range(1, 6);
            jawabanBenar = num1 * num2;

            if (isSoalCerita)
            {
                soalSaatIni = $"{nama} membeli {num1} kantong {barang}. Jika setiap kantong berisi {num2} buah {barang}, berapa banyak seluruh {barang} yang dibeli {nama}?";
            }
            else
            {
                soalSaatIni = $"{num1} x {num2} = ...";
            }

            langkahPenyelesaian = $"Perkalian dasar sebagai penjumlahan berulang:\n" +
                                  $"{num1} x {num2} = {GetRepeatedAdditionString(num2, num1)} = {jawabanBenar}";
            petunjukSoal = $"Perkalian adalah penjumlahan berulang:\nTambahkan angka {num2} sebanyak {num1} kali.";
        }
        else if (phase == GradePhase.FaseB)
        {
            // Kelas 3–4 SD: Tabel perkalian 2–10 dan missing-operand (a x ... = c)
            int format = Random.Range(0, 3);
            if (format == 0)
            {
                // Missing operand: a x ... = c
                num1 = Random.Range(3, 10);
                jawabanBenar = Random.Range(2, 10);
                int hasil = num1 * jawabanBenar;

                if (isSoalCerita)
                {
                    soalSaatIni = $"Ada {num1} meja di ruang kelas. Total pensil di seluruh meja ada {hasil} buah. Jika setiap meja memiliki jumlah pensil yang sama, berapa pensil di tiap meja?";
                }
                else
                {
                    soalSaatIni = $"{num1} x ... = {hasil}";
                }

                langkahPenyelesaian = $"Mencari faktor pengali yang hilang:\n" +
                                      $"{num1} x n = {hasil}\n" +
                                      $"n = {hasil} ÷ {num1} = {jawabanBenar}";
                petunjukSoal = $"Untuk mencari pengali yang hilang, bagi hasil ({hasil}) dengan angka yang diketahui ({num1}).";
            }
            else
            {
                num1 = Random.Range(4, 13);
                num2 = Random.Range(6, 16);
                jawabanBenar = num1 * num2;

                if (isSoalCerita)
                {
                    soalSaatIni = $"Sebuah toko menyusun {num1} baris kotak susu. Setiap baris berisi {num2} kotak susu. Berapa total kotak susu yang ada di toko tersebut?";
                }
                else
                {
                    soalSaatIni = $"{num1} x {num2} = ...";
                }

                langkahPenyelesaian = $"Perkalian bersusun:\n{num1} x {num2} = {jawabanBenar}";
                petunjukSoal = "Gunakan perkalian bersusun ke bawah: kalikan satuan terlebih dahulu, lalu puluhannya.";
            }
        }
        else
        {
            // Kelas 5–6 SD: Perkalian bilangan bulat bertanda (+ x -, - x -)
            int format = Random.Range(0, 2);
            if (format == 0)
            {
                // Negatif x Positif
                num1 = Random.Range(-12, -2);
                num2 = Random.Range(2, 10);
                jawabanBenar = num1 * num2;

                if (isSoalCerita)
                {
                    soalSaatIni = $"Sebuah kapal selam menyelam turun {Mathf.Abs(num1)} meter setiap menit ({num1} m/menit). Berapa meter perubahan kedalamannya setelah {num2} menit?";
                }
                else
                {
                    soalSaatIni = $"{num1} x {num2} = ...";
                }

                langkahPenyelesaian = $"Perkalian Bilangan Bulat Negatif x Positif:\n" +
                                      $"(-) x (+) = (-)\n" +
                                      $"{num1} x {num2} = {jawabanBenar}";
                petunjukSoal = "Aturan perkalian tanda:\nBilangan negatif dikali bilangan positif menghasilkan bilangan negatif (-).";
            }
            else
            {
                // Negatif x Negatif
                num1 = Random.Range(-10, -2);
                num2 = Random.Range(-10, -2);
                jawabanBenar = num1 * num2;

                soalSaatIni = $"({num1}) x ({num2}) = ...";
                langkahPenyelesaian = $"Perkalian Bilangan Bulat Negatif x Negatif:\n" +
                                      $"(-) x (-) = (+)\n" +
                                      $"({num1}) x ({num2}) = {jawabanBenar}";
                petunjukSoal = "Aturan perkalian tanda:\nBilangan negatif dikali bilangan negatif menghasilkan bilangan positif (+).";
            }
        }

        questionText.text = soalSaatIni;
        BuatPilihan(jawabanBenar);
    }

    private string GetRepeatedAdditionString(int val, int count)
    {
        List<string> list = new List<string>();
        for (int i = 0; i < count; i++) list.Add(val.ToString());
        return string.Join(" + ", list.ToArray());
    }

    public override bool IsAnswerCorrect(string playerAnswer)
    {
        if (int.TryParse(playerAnswer, out int pa) && int.TryParse(correctAnswerValue, out int ca))
            return pa == ca;
        return false;
    }

    private void BuatPilihan(int jawabanBenar)
    {
        List<int> pilihan = new List<int> { jawabanBenar };
        int maxAttempts = 100;
        int attempts = 0;
        while (pilihan.Count < 11 && attempts++ < maxAttempts)
        {
            int pengecoh = jawabanBenar + Random.Range(-10, 11);
            if (pengecoh != jawabanBenar && !pilihan.Contains(pengecoh))
                pilihan.Add(pengecoh);
        }
        while (pilihan.Count < 11)
        {
            int fallback = jawabanBenar + pilihan.Count * 3;
            if (!pilihan.Contains(fallback)) pilihan.Add(fallback);
        }
        SpawnPilihan(pilihan, jawabanBenar);
    }
}
