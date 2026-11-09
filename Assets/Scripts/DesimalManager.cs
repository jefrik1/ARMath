using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Kuis desimal: operasi tambah, kurang, kali, bagi desimal.
/// </summary>
public class DesimalManager : BaseQuizManager<string>
{
    public override void GenerateRandomQuestion()
    {
        answerText.text = "Jawaban Anda: -";
        checkButton.interactable = false;
        undoButton.gameObject.SetActive(false);
        foreach (var obj in objekPilihanAktif)
            if (obj != null) Destroy(obj);
        objekPilihanAktif.Clear();
        playerAnswerLabel = "";
        objekTerpilih = null;

        // Coba load dari JSON dulu
        if (TryLoadFromJson("Desimal"))
        {
            questionText.text = soalSaatIni;
            return;
        }

        int op = Random.Range(0, 4);
        string soal = "";
        string jawaban = "";
        string langkah = "";

        switch (op)
        {
            case 0: // tambah desimal
                {
                    float a = Random.Range(0.1f, 9.9f);
                    float b = Random.Range(0.1f, 9.9f);
                    a = Mathf.Round(a * 10) / 10f;
                    b = Mathf.Round(b * 10) / 10f;
                    float h = a + b;
                    soal = a + " + " + b + " = ...";
                    jawaban = h.ToString("F1");
                    langkah = "Samakan kolom desimal, jumlahkan: " + a + " + " + b + " = " + jawaban;
                    break;
                }
            case 1: // kurang desimal
                {
                    float a = Random.Range(1.0f, 10.0f);
                    float b = Random.Range(0.1f, a - 0.1f);
                    a = Mathf.Round(a * 10) / 10f;
                    b = Mathf.Round(b * 10) / 10f;
                    float h = a - b;
                    soal = a + " - " + b + " = ...";
                    jawaban = h.ToString("F1");
                    langkah = "Samakan kolom desimal, kurangkan: " + a + " - " + b + " = " + jawaban;
                    break;
                }
            case 2: // kali desimal (sederhana)
                {
                    int a = Random.Range(1, 9);
                    int b = Random.Range(1, 9);
                    float fa = a / 10f;
                    float fb = b / 10f;
                    float h = fa * fb;
                    soal = fa.ToString("F1") + " x " + fb.ToString("F1") + " = ...";
                    jawaban = h.ToString("F2");
                    langkah = "Kalikan seperti bilangan bulat, lalu geser koma: " +
                              a + " x " + b + " = " + (a * b) + ", koma digeser 2 dari kanan = " + jawaban;
                    break;
                }
            default: // bagi desimal sederhana
                {
                    int pembagi = Random.Range(1, 9);
                    int hasil = Random.Range(1, 9);
                    float div = pembagi * hasil / 10f;
                    soal = div.ToString("F1") + " / " + (pembagi / 10f).ToString("F1") + " = ...";
                    jawaban = hasil.ToString();
                    langkah = "Balik pembagi: " + div + " x " + (10f / pembagi).ToString("F1") + " = " + jawaban;
                    break;
                }
        }

        soalSaatIni = soal;
        langkahPenyelesaian = langkah;
        questionText.text = soal;
        BuatPilihan(jawaban);
    }

    public override bool IsAnswerCorrect(string playerAnswer)
    {
        if (float.TryParse(playerAnswer, out float pa) && float.TryParse(correctAnswerValue, out float ca))
        {
            return Mathf.Abs(pa - ca) < 0.01f;
        }
        return playerAnswer.Trim() == correctAnswerValue.Trim();
    }

    protected override string ConvertAnswerToString(string answer) => answer;

    private void BuatPilihan(string jawabanBenar)
    {
        List<string> pilihan = new List<string> { jawabanBenar };
        while (pilihan.Count < 4)
        {
            string[] parts = jawabanBenar.Split('.');
            if (parts.Length == 2 && int.TryParse(parts[0], out int num) && int.TryParse(parts[1], out int den))
            {
                int offset = Random.Range(-5, 6);
                int newNum = num + offset;
                int newDen = den;
                if (newDen > 0)
                {
                    string distractor = newNum + "." + newDen.ToString("D1");
                    if (!pilihan.Contains(distractor) && !distractor.Equals(jawabanBenar))
                        pilihan.Add(distractor);
                }
            }
            else
            {
                // fallback: random number nearby
                if (float.TryParse(jawabanBenar, out float jb))
                {
                    float d = jb + Random.Range(-2f, 2f);
                    d = Mathf.Round(d * 10) / 10f;
                    string ds = d.ToString("F1");
                    if (!pilihan.Contains(ds) && !ds.Equals(jawabanBenar))
                        pilihan.Add(ds);
                }
            }
        }
        SpawnPilihan(pilihan, jawabanBenar);
    }
}