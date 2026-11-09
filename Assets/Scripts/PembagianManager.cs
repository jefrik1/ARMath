using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Kuis pembagian bilangan sesuai fase kurikulum SD (Kelas 1–6 SD)
/// dengan 50% soal cerita kontekstual dan sistem petunjuk.
/// </summary>
public class PembagianManager : BaseQuizManager<int>
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
            // Kelas 1–2 SD: Pembagian sebagai pengurangan berulang / pembagian rata
            num2 = Random.Range(2, 6);
            jawabanBenar = Random.Range(1, 6);
            num1 = num2 * jawabanBenar;

            if (isSoalCerita)
            {
                soalSaatIni = $"{nama} membagikan {num1} buah {barang} secara merata kepada {num2} orang temannya. Berapa buah {barang} yang diterima masing-masing teman?";
            }
            else
            {
                soalSaatIni = $"{num1} : {num2} = ...";
            }

            langkahPenyelesaian = $"Pembagian sebagai pengurangan berulang:\n" +
                                  $"Kurangkan {num1} dengan {num2} sampai habis (bernilai 0).\n" +
                                  $"{num1} : {num2} = {jawabanBenar}";
            petunjukSoal = $"Bagi sama rata, atau hitung berapa kali angka {num2} dapat dikurangkan dari {num1} sampai habis.";
        }
        else if (phase == GradePhase.FaseB)
        {
            // Kelas 3–4 SD: Pembagian bersusun porogapit (hasil puluhan) & missing-operand
            int format = Random.Range(0, 3);
            if (format == 0)
            {
                jawabanBenar = Random.Range(3, 10);
                num2 = Random.Range(3, 10);
                num1 = num2 * jawabanBenar;

                if (isSoalCerita)
                {
                    soalSaatIni = $"Pak Guru memiliki {num1} lembar kertas tugas. Kertas tersebut dimasukkan ke dalam beberapa map dengan isi {num2} lembar per map. Berapa banyak map yang terisi?";
                }
                else
                {
                    soalSaatIni = $"{num1} : ... = {num2}";
                }

                langkahPenyelesaian = $"Mencari pembagi yang hilang:\n" +
                                      $"{num1} : n = {num2}\n" +
                                      $"n = {num1} ÷ {num2} = {jawabanBenar}";
                petunjukSoal = $"Untuk mencari pembagi yang hilang, bagi bilangan awal ({num1}) dengan hasil bagi ({num2}).";
            }
            else
            {
                num2 = Random.Range(4, 9);
                jawabanBenar = Random.Range(11, 25);
                num1 = num2 * jawabanBenar;

                if (isSoalCerita)
                {
                    soalSaatIni = $"Sebanyak {num1} siswa akan dibagi ke dalam {num2} kelompok belajar yang sama banyak. Berapa siswa anggota pada setiap kelompok?";
                }
                else
                {
                    soalSaatIni = $"{num1} : {num2} = ...";
                }

                langkahPenyelesaian = $"Pembagian bersusun (porogapit):\n{num1} : {num2} = {jawabanBenar}";
                petunjukSoal = "Gunakan cara pembagian bersusun (porogapit): bagi angka puluhannya terlebih dahulu, lalu satuannya.";
            }
        }
        else
        {
            // Kelas 5–6 SD: Pembagian bilangan bulat bertanda (+ : -, - : -)
            int format = Random.Range(0, 2);
            if (format == 0)
            {
                // Negatif : Positif
                jawabanBenar = Random.Range(-12, -2);
                num2 = Random.Range(2, 9);
                num1 = num2 * jawabanBenar;

                if (isSoalCerita)
                {
                    soalSaatIni = $"Suhu di lemari es mengalami penurunan total sebesar {Mathf.Abs(num1)}°C secara bertahap dalam waktu {num2} jam. Berapa rata-rata perubahan suhu per jam?";
                }
                else
                {
                    soalSaatIni = $"{num1} : {num2} = ...";
                }

                langkahPenyelesaian = $"Pembagian Bilangan Bulat Negatif : Positif:\n" +
                                      $"(-) : (+) = (-)\n" +
                                      $"{num1} : {num2} = {jawabanBenar}";
                petunjukSoal = "Aturan tanda pembagian:\nBilangan negatif dibagi bilangan positif menghasilkan bilangan negatif (-).";
            }
            else
            {
                // Negatif : Negatif
                jawabanBenar = Random.Range(2, 12);
                num2 = Random.Range(-9, -2);
                num1 = num2 * jawabanBenar;

                soalSaatIni = $"({num1}) : ({num2}) = ...";
                langkahPenyelesaian = $"Pembagian Bilangan Bulat Negatif : Negatif:\n" +
                                      $"(-) : (-) = (+)\n" +
                                      $"({num1}) : ({num2}) = {jawabanBenar}";
                petunjukSoal = "Aturan tanda pembagian:\nBilangan negatif dibagi bilangan negatif menghasilkan bilangan positif (+).";
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
            int pengecoh = jawabanBenar + Random.Range(-8, 9);
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
