using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Kuis satuan berat: kg, g, ons, lb.
/// </summary>
public class BeratManager : BaseQuizManager<string>
{
    public override void GenerateRandomQuestion()
    {
        answerText.text = "Jawaban Anda: -";
        checkButton.interactable = false;
        undoButton.gameObject.SetActive(false);
        foreach (var obj in objekPilihanAktif) { if (obj != null) Destroy(obj); }
        objekPilihanAktif.Clear();
        playerAnswerLabel = "";
        objekTerpilih = null;

        int tipe = Random.Range(0, 3);
        string soal = "";
        string jawaban = "";
        string langkah = "";

        switch (tipe)
        {
            case 0: // kg ke g
                int kg = Random.Range(1, 20);
                jawaban = (kg * 1000).ToString();
                soal = kg + " kg = ... g";
                langkah = kg + " kg = " + kg + " x 1000 = " + jawaban + " g";
                break;
            case 1: // g ke kg (desimal)
                int g = Random.Range(100, 5000);
                float kgVal = g / 1000f;
                jawaban = kgVal.ToString("F2");
                soal = g + " g = ... kg";
                langkah = g + " g = " + g + " / 1000 = " + jawaban + " kg";
                break;
            case 2: // kg ke ons (1 kg = 10 ons)
                int kg2 = Random.Range(1, 15);
                jawaban = (kg2 * 10).ToString();
                soal = kg2 + " kg = ... ons";
                langkah = kg2 + " kg = " + kg2 + " x 10 = " + jawaban + " ons";
                break;
        }

        correctAnswerValue = jawaban; correctAnswerLabel = "A"; 
        soalSaatIni = soal;
        langkahPenyelesaian = langkah;
        questionText.text = soal;
        BuatPilihan(jawaban);
    }

    public override bool IsAnswerCorrect(string playerAnswer)
    {
        if (float.TryParse(playerAnswer, out float pa) && float.TryParse(correctAnswerValue, out float ca))
            return Mathf.Abs(pa - ca) < 0.01f;
        return false;
    }

    protected override string ConvertAnswerToString(string answer) => answer;

    private void BuatPilihan(string jawabanBenar)
    {
        List<string> pilihan = new List<string> { jawabanBenar };
        float jb = float.Parse(jawabanBenar);
        while (pilihan.Count < 4)
        {
            float offset = Random.Range(-100, 101);
            float nilai = jb + offset;
            if (nilai > 0 && Mathf.Abs(nilai - jb) > 0.01f && !pilihan.Contains(nilai.ToString("F2")))
                pilihan.Add(nilai.ToString("F2"));
        }
        SpawnPilihan(pilihan, jawabanBenar);
    }
}