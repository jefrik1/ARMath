using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Kuis pembagian bilangan sesuai fase kurikulum SD (Kelas 1–6 SD).
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

        if (phase == GradePhase.FaseA)
        {
            // Kelas 1–2 SD: Pembagian dasar (pembagian rata / pengurangan berulang) tabel 2–5
            num2 = Random.Range(2, 6);
            jawabanBenar = Random.Range(1, 6);
            num1 = jawabanBenar * num2;
            soalSaatIni = num1 + " : " + num2 + " = ...";
            langkahPenyelesaian = $"Pembagian sebagai pengurangan berulang:\nBagi {num1} benda ke dalam kelompok berisi {num2}.\nDidapatkan {jawabanBenar} kelompok.";
        }
        else if (phase == GradePhase.FaseB)
        {
            // Kelas 3–4 SD: Tabel pembagian 2–10 dan missing-operand (a : ... = c atau ... : b = c)
            int format = Random.Range(0, 3);
            num2 = Random.Range(2, 11);
            int quotient = Random.Range(2, 11);
            num1 = quotient * num2;

            if (format == 0)
            {
                soalSaatIni = num1 + " : ... = " + quotient;
                jawabanBenar = num2;
                langkahPenyelesaian = $"Mencari pembagi:\n... = {num1} : {quotient} = {num2}";
            }
            else if (format == 1)
            {
                soalSaatIni = "... : " + num2 + " = " + quotient;
                jawabanBenar = num1;
                langkahPenyelesaian = $"Mencari bilangan yang dibagi:\n... = {quotient} x {num2} = {num1}";
            }
            else
            {
                soalSaatIni = num1 + " : " + num2 + " = ...";
                jawabanBenar = quotient;
                langkahPenyelesaian = $"Tabel pembagian:\n{num1} : {num2} = {quotient}";
            }
        }
        else
        {
            // Kelas 5–6 SD: Pembagian ratusan / puluhan dan bilangan bulat negatif
            bool withNegative = Random.value > 0.4f;
            if (withNegative)
            {
                num2 = Random.Range(-12, 13);
                if (num2 == 0) num2 = 4;
                int quotient = Random.Range(-10, 11);
                if (quotient == 0) quotient = -3;
                num1 = quotient * num2;

                string strNum1 = num1 < 0 ? $"({num1})" : num1.ToString();
                string strNum2 = num2 < 0 ? $"({num2})" : num2.ToString();
                soalSaatIni = $"{strNum1} : {strNum2} = ...";
                jawabanBenar = quotient;

                string rule = (num1 < 0 && num2 < 0) ? "Negatif : Negatif = Positif" :
                              (num1 < 0 || num2 < 0) ? "Negatif : Positif = Negatif" : "Positif : Positif = Positif";
                langkahPenyelesaian = $"Aturan tanda: {rule}\n{strNum1} : {strNum2} = {jawabanBenar}";
            }
            else
            {
                num2 = Random.Range(11, 25);
                jawabanBenar = Random.Range(10, 35);
                num1 = jawabanBenar * num2;
                soalSaatIni = $"{num1} : {num2} = ...";
                langkahPenyelesaian = $"Pembagian bersusun (Porogapit):\n{num1} : {num2} = {jawabanBenar}";
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
