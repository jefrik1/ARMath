using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Kuis bangun datar: luas dan keliling persegi, persegi panjang, segitiga, trapesium, layang-layang, jajargenjang.
/// </summary>
public class BangunDatarManager : BaseQuizManager<string>
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

        if (TryLoadFromJson("BangunDatar"))
        {
            questionText.text = soalSaatIni;
            return;
        }

        int tipe = Random.Range(0, 6);
        string soal = "";
        string jawaban = "";
        string langkah = "";

        switch (tipe)
        {
            case 0: // Luas persegi
                {
                    int s = Random.Range(2, 15);
                    int luas = s * s;
                    soal = "Luas persegi dengan sisi " + s + " cm adalah ... cm².";
                    jawaban = luas.ToString();
                    langkah = "Luas = s x s = " + s + " x " + s + " = " + luas + " cm².";
                    break;
                }
            case 1: // Keliling persegi panjang
                {
                    int p = Random.Range(5, 20);
                    int l = Random.Range(3, 15);
                    int keliling = 2 * (p + l);
                    soal = "Keliling persegi panjang p=" + p + " cm, l=" + l + " cm adalah ... cm.";
                    jawaban = keliling.ToString();
                    langkah = "Keliling = 2 x (p + l) = 2 x (" + p + " + " + l + ") = 2 x " + (p + l) + " = " + keliling + " cm.";
                    break;
                }
            case 2: // Luas segitiga
                {
                    int a = Random.Range(4, 20);
                    int t = Random.Range(2, 15);
                    int luas = (a * t) / 2;
                    soal = "Luas segitiga dengan alas " + a + " cm dan tinggi " + t + " cm adalah ... cm².";
                    jawaban = luas.ToString();
                    langkah = "Luas = (alas x tinggi) / 2 = (" + a + " x " + t + ") / 2 = " + luas + " cm².";
                    break;
                }
            case 3: // Luas trapesium
                {
                    int a1 = Random.Range(4, 15);
                    int a2 = Random.Range(4, 15);
                    int t = Random.Range(3, 12);
                    int luas = ((a1 + a2) * t) / 2;
                    soal = "Luas trapesium dengan sisi sejajar " + a1 + " cm dan " + a2 +
                          " cm, tinggi " + t + " cm adalah ... cm².";
                    jawaban = luas.ToString();
                    langkah = "Luas = ((a1 + a2) x t) / 2 = ((" + a1 + " + " + a2 + ") x " + t + ") / 2 = " + luas + " cm².";
                    break;
                }
            case 4: // Keliling layang-layang
                {
                    int d1 = Random.Range(5, 15);
                    int d2 = Random.Range(5, 15);
                    int keliling = 2 * (d1 + d2);
                    soal = "Keliling layang-layang dengan diagonal " + d1 + " cm dan " + d2 +
                          " cm adalah ... cm.";
                    jawaban = keliling.ToString();
                    langkah = "Keliling = 2 x (diagonal1 + diagonal2) = 2 x (" + d1 + " + " + d2 + ") = " + keliling + " cm.";
                    break;
                }
            default: // Luas jajargenjang
                {
                    int a = Random.Range(4, 18);
                    int t = Random.Range(3, 12);
                    int luas = a * t;
                    soal = "Luas jajargenjang dengan alas " + a + " cm dan tinggi " + t + " cm adalah ... cm².";
                    jawaban = luas.ToString();
                    langkah = "Luas = alas x tinggi = " + a + " x " + t + " = " + luas + " cm².";
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
        return playerAnswer.Trim() == correctAnswerValue.Trim();
    }

    protected override string ConvertAnswerToString(string answer) => answer;

    private void BuatPilihan(string jawabanBenar)
    {
        List<string> pilihan = new List<string> { jawabanBenar };
        if (int.TryParse(jawabanBenar, out int num))
        {
            while (pilihan.Count < 4)
            {
                int offset = Random.Range(-30, 31);
                int nilai = num + offset;
                if (nilai > 0 && nilai != num && !pilihan.Contains(nilai.ToString()))
                    pilihan.Add(nilai.ToString());
            }
        }
        if (pilihan.Count < 2) pilihan.Add("1");
        if (pilihan.Count < 3) pilihan.Add("50");
        SpawnPilihan(pilihan, jawabanBenar);
    }
}