using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Kuis perhitungan dasar campuran: tambah, kurang, kali, bagi dalam satu sesi.
/// Independent dari manager kategori lain. Ekstends BaseQuizManager<int>.
/// </summary>
public class MixedQuizManager : BaseQuizManager<int>
{
    private enum Operasi { Tambah, Kurang, Kali, Bagi }

    public override void GenerateRandomQuestion()
    {
        answerText.text = "Jawaban Anda: -";
        checkButton.interactable = false;
        undoButton.gameObject.SetActive(false);
        foreach (var obj in objekPilihanAktif) { if (obj != null) Destroy(obj); }
        objekPilihanAktif.Clear();
        playerAnswerLabel = "";
        objekTerpilih = null;
        if (correctAnswerValue != null)
            correctAnswerValue = null;

        Operasi op = (Operasi)Random.Range(0, 4);
        int jawaban = 0;
        int num1 = 0, num2 = 0;

        switch (op)
        {
            case Operasi.Tambah:
                num1 = Random.Range(1, 20);
                num2 = Random.Range(1, 20);
                jawaban = num1 + num2;
                soalSaatIni = num1 + " + " + num2 + " = ...";
                langkahPenyelesaian = "Langkah: " + num1 + " + " + num2 + " = " + jawaban;
                break;

            case Operasi.Kurang:
                num1 = Random.Range(10, 30);
                num2 = Random.Range(1, num1);
                jawaban = num1 - num2;
                soalSaatIni = num1 + " - " + num2 + " = ...";
                langkahPenyelesaian = "Langkah: " + num1 + " - " + num2 + " = " + jawaban;
                break;

            case Operasi.Kali:
                num1 = Random.Range(2, 11);
                num2 = Random.Range(2, 11);
                jawaban = num1 * num2;
                soalSaatIni = num1 + " x " + num2 + " = ...";
                langkahPenyelesaian = "Langkah: " + num1 + " x " + num2 + " = " + jawaban;
                break;

            case Operasi.Bagi:
                jawaban = Random.Range(2, 11);
                num2 = Random.Range(2, 11);
                num1 = jawaban * num2;
                soalSaatIni = num1 + " / " + num2 + " = ...";
                langkahPenyelesaian = "Langkah: " + num1 + " / " + num2 + " = " + jawaban;
                break;
        }

        questionText.text = soalSaatIni;
        BuatPilihan(jawaban);
    }

    public override bool IsAnswerCorrect(string playerAnswer)
    {
        return int.TryParse(playerAnswer, out int pa) && pa == int.Parse(correctAnswerValue);
    }

    private void BuatPilihan(int jawabanBenar)
    {
        List<int> pilihan = new List<int> { jawabanBenar };
        while (pilihan.Count < 11)
        {
            int pengecoh = jawabanBenar + Random.Range(-10, 11);
            if (pengecoh >= 0 && pengecoh != jawabanBenar && !pilihan.Contains(pengecoh))
                pilihan.Add(pengecoh);
        }
        SpawnPilihan(pilihan, jawabanBenar);
    }
}