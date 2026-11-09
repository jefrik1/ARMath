using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Kuis perbandingan & proporsi: menentukan nilai yang belum diketahui dalam proporsi.
/// </summary>
public class PerbandinganManager : BaseQuizManager<string>
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

        if (TryLoadFromJson("Perbandingan"))
        {
            questionText.text = soalSaatIni;
            return;
        }

        int tipe = Random.Range(0, 3);
        string soal = "";
        string jawaban = "";
        string langkah = "";

        switch (tipe)
        {
            case 0: // a : b = c : x  (cari x)
                {
                    int a = Random.Range(2, 10);
                    int b = Random.Range(2, 10);
                    int c = Random.Range(2, 10);
                    // a/b = c/x  => x = c*b/a, harus integer
                    int x = (c * b) / a;
                    if (x == 0 || (c * b) % a != 0)
                    {
                        // retry dengan nilai yang lebih cocok
                        a = Random.Range(2, 5);
                        b = Random.Range(2, 5);
                        c = a * Random.Range(1, 5);
                        x = c / a * b;
                    }
                    soal = a + " : " + b + " = " + c + " : ...";
                    jawaban = x.ToString();
                    langkah = "a : b = c : x  =>  x = (c x b) / a = (" + c + " x " + b + ") / " + a + " = " + jawaban;
                    break;
                }
            case 1: // skala perbandingan sederhana: 3 apel : 2 jeruk, jika 9 apel berapa jeruk?
                {
                    int faktor = Random.Range(2, 5);
                    int a = faktor;
                    int b = Random.Range(2, 5);
                    int c = a * Random.Range(2, 5);
                    int x = c / a * b;
                    soal = "Perbandingan " + a + " : " + b + ". Jika ada " + c + " bagian pertama, berapa bagian kedua?";
                    jawaban = x.ToString();
                    langkah = "Faktor pengali = " + c + " / " + a + " = " + (c / a) +
                              ". Bagian kedua = " + b + " x " + (c / a) + " = " + jawaban;
                    break;
                }
            default: // perbandingan senilai: x : y = p : q, cari total atau selisih
                {
                    int p = Random.Range(2, 8);
                    int q = Random.Range(2, 8);
                    int x = p * Random.Range(2, 5);
                    int y = q * Random.Range(2, 5);
                    int t = Random.Range(0, 2);
                    if (t == 0)
                    {
                        soal = "Perbandingan " + x + " : " + y + " disederhanakan menjadi ... : ...";
                        int fpb = FPB(x, y);
                        jawaban = (x / fpb) + " : " + (y / fpb);
                    }
                    else
                    {
                        int jumlah = x + y;
                        soal = "Jumlah " + x + " dan " + y + " adalah ...";
                        jawaban = jumlah.ToString();
                        langkah = "Jumlah = " + x + " + " + y + " = " + jawaban;
                    }
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

    private int FPB(int a, int b)
    {
        while (b != 0)
        {
            int t = b;
            b = a % b;
            a = t;
        }
        return a;
    }

    private void BuatPilihan(string jawabanBenar)
    {
        List<string> pilihan = new List<string> { jawabanBenar };
        while (pilihan.Count < 4)
        {
            string[] parts = jawabanBenar.Split(':');
            if (parts.Length == 2 && int.TryParse(parts[0], out int a) && int.TryParse(parts[1], out int b))
            {
                int newA = a + Random.Range(-3, 4);
                int newB = b + Random.Range(-3, 4);
                if (newA > 0 && newB > 0)
                {
                    string d = newA + " : " + newB;
                    if (!pilihan.Contains(d) && !d.Equals(jawabanBenar))
                        pilihan.Add(d);
                }
            }
            else if (int.TryParse(jawabanBenar, out int num))
            {
                int offset = Random.Range(-5, 6);
                int nilai = num + offset;
                if (nilai > 0 && nilai != num && !pilihan.Contains(nilai.ToString()))
                    pilihan.Add(nilai.ToString());
            }
            else
            {
                break; // tidak bisa generate distractor
            }
        }
        if (pilihan.Count < 2) pilihan.Add("1");
        if (pilihan.Count < 3) pilihan.Add("0");
        SpawnPilihan(pilihan, jawabanBenar);
    }
}