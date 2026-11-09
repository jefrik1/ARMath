using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Kuis menentukan median dari kumpulan data sesuai kurikulum Matematika SD
/// dengan 50% soal cerita kontekstual dan sistem petunjuk.
/// </summary>
public class MedianManager : BaseQuizManager<int>
{
    public override void GenerateRandomQuestion()
    {
        ResetAnswerState();
        foreach (var obj in objekPilihanAktif) { if (obj != null) Destroy(obj); }
        objekPilihanAktif.Clear();

        List<int> numbers = new List<int>();
        int jumlahData;
        int maxVal;

        GradePhase phase = GradeLevelConfig.CurrentPhase;
        if (phase == GradePhase.FaseA || phase == GradePhase.FaseB)
        {
            // Fase A & B (Kelas 1-4 SD): 3 atau 5 angka sederhana (1-20)
            jumlahData = Random.value < 0.5f ? 3 : 5;
            maxVal = 20;
        }
        else
        {
            // Fase C (Kelas 5-6 SD): 5, 7, 9, atau 11 data (1-100)
            int pilihanAcak = Random.Range(0, 4);
            switch (pilihanAcak)
            {
                case 0: jumlahData = 5; break;
                case 1: jumlahData = 7; break;
                case 2: jumlahData = 9; break;
                default: jumlahData = 11; break;
            }
            maxVal = 100;
        }

        for (int i = 0; i < jumlahData; i++)
        {
            numbers.Add(Random.Range(1, maxVal + 1));
        }

        string rawString = string.Join(", ", numbers);
        List<int> sortedNumbers = new List<int>(numbers);
        sortedNumbers.Sort();
        string sortedString = string.Join(", ", sortedNumbers);

        int posTengah = (jumlahData + 1) / 2;
        int jawaban = sortedNumbers[posTengah - 1];

        // 50% Soal Cerita Kontekstual vs 50% Numerik
        bool isSoalCerita = Random.value < 0.5f;
        string nama = GradeLevelConfig.GetRandomStudentName();

        if (isSoalCerita)
        {
            if (phase == GradePhase.FaseA || phase == GradePhase.FaseB)
            {
                soalSaatIni = $"{nama} mencatat banyak kelereng yang dimiliki {jumlahData} orang temannya:\n{rawString}\nBerapa nilai tengah (median) data kelereng tersebut?";
            }
            else
            {
                soalSaatIni = $"Data berat badan (kg) dari {jumlahData} siswa adalah sebagai berikut:\n{rawString}\nTentukan nilai median dari data tersebut!";
            }
        }
        else
        {
            soalSaatIni = $"Tentukan median dari kumpulan data berikut:\n{rawString}";
        }

        langkahPenyelesaian = "Langkah mencari Median:\n" +
            $"1. Urutkan data dari terkecil ke terbesar:\n   {sortedString}\n" +
            $"2. Karena jumlah data n = {jumlahData} (ganjil), median adalah data urutan ke-(n+1)/2 = ke-{posTengah}.\n" +
            $"3. Nilai pada urutan ke-{posTengah} adalah {jawaban}.\n" +
            $"Maka, Median = {jawaban}.";

        petunjukSoal = $"Langkah 1: Urutkan dulu semua data dari yang paling kecil.\nLangkah 2: Ambil angka yang berada tepat di tengah (urutan ke-{posTengah}).";

        questionText.text = soalSaatIni;
        BuatPilihan(jawaban);
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
            if (pengecoh >= 0 && pengecoh != jawabanBenar && !pilihan.Contains(pengecoh))
                pilihan.Add(pengecoh);
        }
        while (pilihan.Count < 11)
        {
            int fallback = Random.Range(1, 100);
            if (!pilihan.Contains(fallback)) pilihan.Add(fallback);
        }
        SpawnPilihan(pilihan, jawabanBenar);
    }
}
