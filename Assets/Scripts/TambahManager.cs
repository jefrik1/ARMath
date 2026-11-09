using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Kuis penjumlahan bilangan sesuai fase kurikulum SD (Kelas 1–6 SD)
/// dengan 50% soal cerita kontekstual dan sistem petunjuk.
/// </summary>
public class TambahManager : BaseQuizManager<int>
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
            // Kelas 1–2 SD: Bilangan cacah 1–50, tanpa negatif, bantuan hitung maju / nilai tempat
            num1 = Random.Range(1, 30);
            num2 = Random.Range(1, 21);
            jawabanBenar = num1 + num2;

            if (isSoalCerita)
            {
                soalSaatIni = $"{nama} memiliki {num1} buah {barang}. Ibu memberikan lagi {num2} buah {barang}. Berapa jumlah seluruh {barang} {nama} sekarang?";
            }
            else
            {
                soalSaatIni = $"{num1} + {num2} = ...";
            }

            langkahPenyelesaian = $"Penjumlahan (Kelas 1-2 SD):\n" +
                                  $"Mulai dari angka {num1}, lalu hitung maju {num2} langkah:\n" +
                                  $"{num1} + {num2} = {jawabanBenar}";
            petunjukSoal = $"Gabungkan kedua kelompok benda.\nHitung maju dari {num1} sebanyak {num2} langkah.";
        }
        else if (phase == GradePhase.FaseB)
        {
            // Kelas 3–4 SD: Bilangan hingga 500 dan missing-operand (a + ... = c)
            int format = Random.Range(0, 3);
            if (format == 0)
            {
                // Missing operand: a + ... = c
                num1 = Random.Range(15, 120);
                jawabanBenar = Random.Range(10, 80);
                int total = num1 + jawabanBenar;

                if (isSoalCerita)
                {
                    soalSaatIni = $"Sebuah keranjang berisi {num1} buah {barang}. Setelah ditambahkan beberapa {barang} lagi, totalnya menjadi {total} buah. Berapa buah {barang} yang baru ditambahkan?";
                }
                else
                {
                    soalSaatIni = $"{num1} + ... = {total}";
                }

                langkahPenyelesaian = $"Mencari angka yang hilang pada penjumlahan:\n" +
                                      $"{num1} + n = {total}\n" +
                                      $"n = {total} - {num1} = {jawabanBenar}";
                petunjukSoal = $"Untuk mencari nilai yang hilang, kurangkan bilangan hasil ({total}) dengan bilangan pertama ({num1}).";
            }
            else
            {
                num1 = Random.Range(40, 250);
                num2 = Random.Range(30, 250);
                jawabanBenar = num1 + num2;

                if (isSoalCerita)
                {
                    soalSaatIni = $"Di perpustakaan sekolah terdapat {num1} buku cerita dan {num2} buku pelajaran. Berapa jumlah total buku di perpustakaan?";
                }
                else
                {
                    soalSaatIni = $"{num1} + {num2} = ...";
                }

                langkahPenyelesaian = $"Penjumlahan bersusun ratusan dan puluhan:\n" +
                                      $"{num1} + {num2} = {jawabanBenar}";
                petunjukSoal = "Jumlahkan nilai satuan dengan satuan terlebih dahulu, lalu puluhan, kemudian ratusan.";
            }
        }
        else
        {
            // Kelas 5–6 SD: Operasi campuran & bilangan bulat bertanda / kontekstual suhu
            int format = Random.Range(0, 2);
            if (format == 0)
            {
                // Kontekstual suhu / bilangan negatif
                num1 = Random.Range(-25, 0);
                num2 = Random.Range(5, 35);
                jawabanBenar = num1 + num2;

                if (isSoalCerita)
                {
                    soalSaatIni = $"Suhu udara di dalam ruang pendingin mula-mula {num1}°C. Setelah pendingin dimatikan, suhu naik sebesar {num2}°C. Berapa suhu ruangan sekarang?";
                }
                else
                {
                    soalSaatIni = $"{num1} + {num2} = ...";
                }

                langkahPenyelesaian = $"Penjumlahan Bilangan Bulat Negatif & Positif:\n" +
                                      $"Mulai dari {num1}, bergerak ke kanan sejauh {num2} pada garis bilangan:\n" +
                                      $"{num1} + {num2} = {jawabanBenar}";
                petunjukSoal = "Pada garis bilangan, menambah bilangan positif berarti melompat ke arah kanan.";
            }
            else
            {
                num1 = Random.Range(100, 750);
                num2 = Random.Range(100, 750);
                jawabanBenar = num1 + num2;

                if (isSoalCerita)
                {
                    soalSaatIni = $"{nama} menabung sebesar Rp{num1}.000 pada bulan pertama dan Rp{num2}.000 pada bulan kedua. Berapa total tabungan {nama} (dalam ribuan Rupiah)?";
                }
                else
                {
                    soalSaatIni = $"{num1} + {num2} = ...";
                }

                langkahPenyelesaian = $"Penjumlahan Bilangan Bulat Besar:\n{num1} + {num2} = {jawabanBenar}";
                petunjukSoal = "Gunakan penjumlahan bersusun ke bawah dengan teliti menjaga simpanan angka puluhan/ratusan.";
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
