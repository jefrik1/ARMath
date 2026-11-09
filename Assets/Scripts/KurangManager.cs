using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Kuis pengurangan bilangan sesuai fase kurikulum SD (Kelas 1–6 SD).
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

        if (phase == GradePhase.FaseA)
        {
            // Kelas 1–2 SD: Bilangan 1–50 tanpa negatif, hitung mundur
            num1 = Random.Range(5, 50);
            num2 = Random.Range(1, num1);
            jawabanBenar = num1 - num2;
            soalSaatIni = num1 + " - " + num2 + " = ...";
            langkahPenyelesaian = $"Pengurangan (Kelas 1-2 SD):\nHitung mundur dari {num1} sebanyak {num2} langkah.\n{num1} - {num2} = {jawabanBenar}";
        }
        else if (phase == GradePhase.FaseB)
        {
            // Kelas 3–4 SD: Bilangan hingga 500 dan missing-operand
            int format = Random.Range(0, 3);
            num1 = Random.Range(50, 450);
            num2 = Random.Range(10, num1);
            int diff = num1 - num2;

            if (format == 0)
            {
                soalSaatIni = num1 + " - ... = " + diff;
                jawabanBenar = num2;
                langkahPenyelesaian = $"Mencari pengurang:\n... = {num1} - {diff} = {num2}";
            }
            else if (format == 1)
            {
                soalSaatIni = "... - " + num2 + " = " + diff;
                jawabanBenar = num1;
                langkahPenyelesaian = $"Mencari bilangan yang dikurangi:\n... = {diff} + {num2} = {num1}";
            }
            else
            {
                soalSaatIni = num1 + " - " + num2 + " = ...";
                jawabanBenar = diff;
                langkahPenyelesaian = $"Pengurangan bersusun:\n{num1} - {num2} = {diff}";
            }
        }
        else
        {
            // Kelas 5–6 SD: Pengurangan bilangan bulat (termasuk negatif) & multi-digit
            bool withNegative = Random.value > 0.4f;
            if (withNegative)
            {
                num1 = Random.Range(-40, 80);
                num2 = Random.Range(-40, 80);
                jawabanBenar = num1 - num2;
                string strNum1 = num1 < 0 ? $"({num1})" : num1.ToString();
                string strNum2 = num2 < 0 ? $"({num2})" : num2.ToString();
                soalSaatIni = $"{strNum1} - {strNum2} = ...";
                langkahPenyelesaian = $"Pengurangan bilangan bulat (Kelas 5-6 SD):\n{strNum1} - {strNum2} = {strNum1} + ({-num2}) = {jawabanBenar}";
            }
            else
            {
                num1 = Random.Range(300, 999);
                num2 = Random.Range(100, num1);
                jawabanBenar = num1 - num2;
                soalSaatIni = $"{num1} - {num2} = ...";
                langkahPenyelesaian = $"Pengurangan ratusan:\n  {num1}\n- {num2}\n-------\n  {jawabanBenar}";
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
}
