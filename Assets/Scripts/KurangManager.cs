using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Kuis pengurangan bilangan sesuai fase kurikulum SD (Kelas 1–6 SD)
/// dengan 50% soal cerita kontekstual dan sistem petunjuk.
/// </summary>
public class KurangManager : BaseQuizManager<int>
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
            // Kelas 1–2 SD: Pengurangan cacah 1–50 tanpa hasil negatif
            num1 = Random.Range(10, 45);
            num2 = Random.Range(1, num1);
            jawabanBenar = num1 - num2;

            if (isSoalCerita)
            {
                soalSaatIni = $"{nama} memiliki {num1} butir {barang}. Sebanyak {num2} butir diberikan kepada temannya. Berapa sisa {barang} {nama} sekarang?";
            }
            else
            {
                soalSaatIni = $"{num1} - {num2} = ...";
            }

            langkahPenyelesaian = $"Pengurangan Dasar (Kelas 1-2 SD):\n" +
                                  $"Mulai dari {num1}, lalu hitung mundur sebanyak {num2} langkah:\n" +
                                  $"{num1} - {num2} = {jawabanBenar}";
            petunjukSoal = $"Hitung mundur dari {num1} sebanyak {num2} langkah, atau kurangkan satuannya terlebih dahulu.";
        }
        else if (phase == GradePhase.FaseB)
        {
            // Kelas 3–4 SD: Pengurangan hingga 500 dan missing-operand (a - ... = c)
            int format = Random.Range(0, 3);
            if (format == 0)
            {
                // Missing operand: a - ... = c
                num1 = Random.Range(60, 250);
                jawabanBenar = Random.Range(15, num1 - 10);
                int sisa = num1 - jawabanBenar;

                if (isSoalCerita)
                {
                    soalSaatIni = $"Sebuah toko memiliki persediaan {num1} {barang}. Setelah terjual beberapa buah, tersisa {sisa} buah. Berapa buah {barang} yang telah terjual?";
                }
                else
                {
                    soalSaatIni = $"{num1} - ... = {sisa}";
                }

                langkahPenyelesaian = $"Mencari pengurang yang hilang:\n" +
                                      $"{num1} - n = {sisa}\n" +
                                      $"n = {num1} - {sisa} = {jawabanBenar}";
                petunjukSoal = $"Untuk mencari nilai yang hilang, kurangkan bilangan awal ({num1}) dengan bilangan sisa ({sisa}).";
            }
            else
            {
                num1 = Random.Range(80, 450);
                num2 = Random.Range(20, num1 - 5);
                jawabanBenar = num1 - num2;

                if (isSoalCerita)
                {
                    soalSaatIni = $"Dalam sebuah ujian terdapat {num1} peserta. Jika {num2} peserta telah selesai dan keluar ruangan, berapa peserta yang masih mengerjakan ujian?";
                }
                else
                {
                    soalSaatIni = $"{num1} - {num2} = ...";
                }

                langkahPenyelesaian = $"Pengurangan bersusun dengan teknik meminjam:\n" +
                                      $"{num1} - {num2} = {jawabanBenar}";
                petunjukSoal = "Kurangkan satuan dengan satuan (pinjam dari puluhan jika angka atas lebih kecil), lalu kurangkan puluhannya.";
            }
        }
        else
        {
            // Kelas 5–6 SD: Pengurangan melibatkan bilangan bulat negatif (a - (-b) = a + b)
            int format = Random.Range(0, 2);
            if (format == 0)
            {
                num1 = Random.Range(-15, 10);
                num2 = Random.Range(-15, -1);
                jawabanBenar = num1 - num2;

                if (isSoalCerita)
                {
                    soalSaatIni = $"Seorang penyelam berada di kedalaman {num1} meter. Posisi dasar terumbu karang adalah {num2} meter. Berapa selisih jarak antara penyelam dan terumbu karang?";
                }
                else
                {
                    soalSaatIni = $"{num1} - ({num2}) = ...";
                }

                langkahPenyelesaian = $"Pengurangan dengan Bilangan Negatif:\n" +
                                      $"Mengurangi negatif sama dengan menambah positif:\n" +
                                      $"{num1} - ({num2}) = {num1} + ({-num2}) = {jawabanBenar}";
                petunjukSoal = "Ingat sifat tanda:\nMinus bertemu minus berubah menjadi tanda tambah: a - (-b) = a + b.";
            }
            else
            {
                num1 = Random.Range(-25, -1);
                num2 = Random.Range(5, 30);
                jawabanBenar = num1 - num2;

                if (isSoalCerita)
                {
                    soalSaatIni = $"Suhu di freezer adalah {num1}°C. Karena pendingin dinaikkan kekuatannya, suhu turun lagi sebesar {num2}°C. Berapa suhu freezer sekarang?";
                }
                else
                {
                    soalSaatIni = $"{num1} - {num2} = ...";
                }

                langkahPenyelesaian = $"Pengurangan Bilangan Negatif dengan Positif:\n" +
                                      $"Pada garis bilangan bergerak ke kiri:\n" +
                                      $"{num1} - {num2} = {jawabanBenar}";
                petunjukSoal = "Mengurangi bilangan positif pada bilangan negatif berarti melompat semakin ke kiri pada garis bilangan.";
            }
        }

        questionText.text = soalSaatIni;
        BuatPilihan(jawabanBenar);
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
            int fallback = jawabanBenar + pilihan.Count * 2;
            if (!pilihan.Contains(fallback)) pilihan.Add(fallback);
        }
        SpawnPilihan(pilihan, jawabanBenar);
    }
}
