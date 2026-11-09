using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Kuis menentukan modus dari kumpulan data sesuai kurikulum Matematika SD.
/// </summary>
public class ModusManager : BaseQuizManager<int>
{
    public override void GenerateRandomQuestion()
    {
        ResetAnswerState();
        foreach (var obj in objekPilihanAktif) { if (obj != null) Destroy(obj); }
        objekPilihanAktif.Clear();

        List<int> numbers = new List<int>();
        int totalData;
        int jumlahDataUnik;
        int maxVal;

        GradePhase phase = GradeLevelConfig.CurrentPhase;
        if (phase == GradePhase.FaseA || phase == GradePhase.FaseB)
        {
            // Fase A & B (Kelas 1-4 SD): 5-7 data, nilai 1-20, modus sangat jelas
            totalData = Random.Range(5, 8);
            jumlahDataUnik = Random.Range(2, 4);
            maxVal = 20;
        }
        else
        {
            // Fase C (Kelas 5-6 SD): 7-12 data, nilai 1-50
            totalData = Random.Range(7, 13);
            jumlahDataUnik = Random.Range(3, 5);
            maxVal = 50;
        }

        while (numbers.Count < jumlahDataUnik)
        {
            int num = Random.Range(1, maxVal + 1);
            if (!numbers.Contains(num)) numbers.Add(num);
        }

        int modus = numbers[Random.Range(0, numbers.Count)];
        // Pastikan modus muncul paling banyak
        int targetModusCount = (totalData / 2) + 1;
        int countModus = 0;
        foreach (int x in numbers) { if (x == modus) countModus++; }
        while (countModus < targetModusCount && numbers.Count < totalData)
        {
            numbers.Add(modus);
            countModus++;
        }
        while (numbers.Count < totalData)
        {
            int other = 0;
            foreach (int x in numbers)
            {
                if (x != modus) { other = x; break; }
            }
            if (other != 0) numbers.Add(other);
            else numbers.Add(Random.Range(1, maxVal + 1));
        }

        // Acak urutan data tampilan (Fisher-Yates)
        for (int i = numbers.Count - 1; i > 0; i--)
        {
            int rnd = Random.Range(0, i + 1);
            int temp = numbers[i];
            numbers[i] = numbers[rnd];
            numbers[rnd] = temp;
        }

        // Hitung frekuensi menggunakan Dictionary
        Dictionary<int, int> frekuensiMap = new Dictionary<int, int>();
        foreach (int x in numbers)
        {
            if (frekuensiMap.ContainsKey(x)) frekuensiMap[x]++;
            else frekuensiMap[x] = 1;
        }

        int modusCount = frekuensiMap[modus];
        string numberString = string.Join(", ", numbers.ConvertAll(n => n.ToString()).ToArray());
        soalSaatIni = $"Tentukan modus dari data berikut:\n{numberString}";

        List<string> freqLines = new List<string>();
        foreach (var kvp in frekuensiMap)
        {
            freqLines.Add($"   Angka {kvp.Key}: muncul {kvp.Value} kali");
        }

        langkahPenyelesaian = "Langkah mencari Modus:\n" +
            "Modus adalah nilai yang paling sering muncul (frekuensi tertinggi).\n" +
            "1. Hitung frekuensi setiap angka:\n" + string.Join("\n", freqLines.ToArray()) + "\n" +
            $"2. Angka {modus} memiliki kemunculan terbanyak ({modusCount} kali).\n" +
            $"Maka, Modus = {modus}.";

        questionText.text = soalSaatIni;
        BuatPilihan(modus);
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
            int fallback = Random.Range(1, 60);
            if (!pilihan.Contains(fallback)) pilihan.Add(fallback);
        }
        SpawnPilihan(pilihan, jawabanBenar);
    }
}
