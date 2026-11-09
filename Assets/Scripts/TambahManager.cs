using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Kuis penjumlahan bilangan sesuai fase kurikulum SD (Kelas 1–6 SD).
/// </summary>
public class TambahManager : BaseQuizManager<int>
{
    public override void GenerateRandomQuestion()
    {
        ResetAnswerState();
        foreach (var obj in objekPilihanAktif) { if (obj != null) Destroy(obj); }
        objekPilihanAktif.Clear();

        if (TryLoadFromJson("PerhitunganDasar"))
        {
            if (int.TryParse(correctAnswerValue, out int jsonJawaban))
            {
                questionText.text = soalSaatIni;
                return;
            }
        }

        GradePhase phase = GradeLevelConfig.CurrentPhase;
        int num1, num2, jawabanBenar;

        if (phase == GradePhase.FaseA)
        {
            // Kelas 1–2 SD: Bilangan cacah 1–50, tanpa negatif, bantuan visual / hitung maju
            num1 = Random.Range(1, 30);
            num2 = Random.Range(1, 21);
            jawabanBenar = num1 + num2;
            soalSaatIni = num1 + " + " + num2 + " = ...";
            langkahPenyelesaian = $"Penjumlahan (Kelas 1-2 SD):\nHitung maju dari {num1} sebanyak {num2} langkah.\n{num1} + {num2} = {jawabanBenar}";
        }
        else if (phase == GradePhase.FaseB)
        {
            // Kelas 3–4 SD: Bilangan hingga 500 dan missing-operand (a + ... = c)
            int format = Random.Range(0, 3);
            num1 = Random.Range(15, 250);
            num2 = Random.Range(15, 250);
            int sum = num1 + num2;

            if (format == 0)
            {
                soalSaatIni = num1 + " + ... = " + sum;
                jawabanBenar = num2;
                langkahPenyelesaian = $"Mencari bilangan yang hilang:\n... = {sum} - {num1} = {num2}";
            }
            else if (format == 1)
            {
                soalSaatIni = "... + " + num2 + " = " + sum;
                jawabanBenar = num1;
                langkahPenyelesaian = $"Mencari bilangan yang hilang:\n... = {sum} - {num2} = {num1}";
            }
            else
            {
                soalSaatIni = num1 + " + " + num2 + " = ...";
                jawabanBenar = sum;
                langkahPenyelesaian = $"Jumlahkan ratusan, puluhan, dan satuan:\n{num1} + {num2} = {sum}";
            }
        }
        else
        {
            // Kelas 5–6 SD: Multi-digit dan operasi bilangan bulat (termasuk negatif)
            bool withNegative = Random.value > 0.4f;
            if (withNegative)
            {
                num1 = Random.Range(-50, 100);
                num2 = Random.Range(-50, 100);
                jawabanBenar = num1 + num2;
                string strNum1 = num1 < 0 ? $"({num1})" : num1.ToString();
                string strNum2 = num2 < 0 ? $"({num2})" : num2.ToString();
                soalSaatIni = $"{strNum1} + {strNum2} = ...";
                langkahPenyelesaian = $"Operasi bilangan bulat (Kelas 5-6 SD):\n{strNum1} + {strNum2} = {jawabanBenar}";
            }
            else
            {
                num1 = Random.Range(100, 999);
                num2 = Random.Range(100, 999);
                jawabanBenar = num1 + num2;
                soalSaatIni = $"{num1} + {num2} = ...";
                langkahPenyelesaian = $"Penjumlahan bersusun (Kelas 5-6 SD):\n  {num1}\n+ {num2}\n-------\n  {jawabanBenar}";
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
