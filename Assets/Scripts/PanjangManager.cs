using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Kuis satuan panjang: km, m, cm, mm.
/// </summary>
public class PanjangManager : BaseQuizManager<string>
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
            case 0: // km ke m
                int km = Random.Range(1, 20);
                jawaban = (km * 1000).ToString();
                soal = km + " km = ... m";
                langkah = km + " km = " + km + " x 1000 = " + jawaban + " m";
                break;
            case 1: // m ke cm
                int m = Random.Range(1, 15);
                jawaban = (m * 100).ToString();
                soal = m + " m = ... cm";
                langkah = m + " m = " + m + " x 100 = " + jawaban + " cm";
                break;
            case 2: // cm ke mm
                int cm = Random.Range(1, 30);
                jawaban = (cm * 10).ToString();
                soal = cm + " cm = ... mm";
                langkah = cm + " cm = " + cm + " x 10 = " + jawaban + " mm";
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
        return playerAnswer == correctAnswerValue;
    }

    protected override string ConvertAnswerToString(string answer) => answer;

    private void BuatPilihan(string jawabanBenar)
    {
        List<string> pilihan = new List<string> { jawabanBenar };
        int jbInt = int.Parse(jawabanBenar);
        while (pilihan.Count < 4)
        {
            int offset = Random.Range(-50, 51);
            int nilai = jbInt + offset;
            if (nilai > 0 && nilai != jbInt && !pilihan.Contains(nilai.ToString()))
                pilihan.Add(nilai.ToString());
        }
        SpawnPilihan(pilihan, jawabanBenar);
    }
}