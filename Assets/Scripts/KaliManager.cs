using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Kuis perkalian bilangan sesuai fase kurikulum SD (Kelas 1–6 SD).
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

        if (phase == GradePhase.FaseA)
        {
            // Kelas 1–2 SD: Penjumlahan berulang dasar, tabel 2–5
            num1 = Random.Range(2, 6);
            num2 = Random.Range(1, 6);
            jawabanBenar = num1 * num2;
            soalSaatIni = num1 + " x " + num2 + " = ...";
            langkahPenyelesaian = $"Perkalian dasar sebagai penjumlahan berulang:\n{num1} x {num2} = {GetRepeatedAdditionString(num2, num1)} = {jawabanBenar}";
        }
        else if (phase == GradePhase.FaseB)
        {
            // Kelas 3–4 SD: Tabel perkalian 2–10 dan missing-operand (a x ... = c)
            int format = Random.Range(0, 3);
            num1 = Random.Range(2, 11);
            num2 = Random.Range(2, 11);
            int prod = num1 * num2;

            if (format == 0)
            {
                soalSaatIni = num1 + " x ... = " + prod;
                jawabanBenar = num2;
                langkahPenyelesaian = $"Mencari faktor pengali:\n... = {prod} : {num1} = {num2}";
            }
            else if (format == 1)
            {
                soalSaatIni = "... x " + num2 + " = " + prod;
                jawabanBenar = num1;
                langkahPenyelesaian = $"Mencari faktor pengali:\n... = {prod} : {num2} = {num1}";
            }
            else
            {
                soalSaatIni = num1 + " x " + num2 + " = ...";
                jawabanBenar = prod;
                langkahPenyelesaian = $"Tabel perkalian:\n{num1} x {num2} = {prod}";
            }
        }
        else
        {
            // Kelas 5–6 SD: Perkalian multi-digit dan bilangan bulat negatif
            bool withNegative = Random.value > 0.4f;
            if (withNegative)
            {
                num1 = Random.Range(-12, 15);
                if (num1 == 0) num1 = -6;
                num2 = Random.Range(-12, 15);
                if (num2 == 0) num2 = 7;
                jawabanBenar = num1 * num2;
                string strNum1 = num1 < 0 ? $"({num1})" : num1.ToString();
                string strNum2 = num2 < 0 ? $"({num2})" : num2.ToString();
                soalSaatIni = $"{strNum1} x {strNum2} = ...";
                string rule = (num1 < 0 && num2 < 0) ? "Negatif x Negatif = Positif" :
                              (num1 < 0 || num2 < 0) ? "Negatif x Positif = Negatif" : "Positif x Positif = Positif";
                langkahPenyelesaian = $"Aturan tanda: {rule}\n{strNum1} x {strNum2} = {jawabanBenar}";
            }
            else
            {
                num1 = Random.Range(12, 50);
                num2 = Random.Range(11, 25);
                jawabanBenar = num1 * num2;
                soalSaatIni = $"{num1} x {num2} = ...";
                langkahPenyelesaian = $"Perkalian bersusun (Kelas 5-6 SD):\n{num1} x {num2} = {jawabanBenar}";
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
        while (pilihan.Count < 11)
        {
            int pengecoh = jawabanBenar + Random.Range(-10, 11);
            if (pengecoh != jawabanBenar && !pilihan.Contains(pengecoh))
                pilihan.Add(pengecoh);
        }
        SpawnPilihan(pilihan, jawabanBenar);
    }

    private string GetRepeatedAdditionString(int val, int count)
    {
        List<string> list = new List<string>();
        for (int i = 0; i < count; i++) list.Add(val.ToString());
        return string.Join(" + ", list.ToArray());
    }
}
