using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Kuis skala: menghitung jarak sesungguhnya dari peta, atau sebaliknya.
/// Contoh: skala 1:100.000, jarak di peta 3 cm, berapa km jarak sesungguhnya?
/// </summary>
public class SkalaManager : BaseQuizManager<string>
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

        if (TryLoadFromJson("Skala"))
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
            case 0: // dari peta ke sebenarnya (cm ke km)
                {
                    int skala = Random.Range(10000, 1000000);
                    skala = Mathf.RoundToInt(skala / 10000f) * 10000; // bulatkan ke puluhan ribu
                    if (skala < 10000) skala = 10000;
                    int chessMap = Random.Range(1, 10);
                    int sebenarnya = chessMap * skala;
                    int hasilKm = sebenarnya / 100000;
                    soal = "Skala peta 1 : " + skala + ". Jarak 2 kota di peta " + chessMap + " cm. Jarak sebenarnya adalah ... km.";
                    jawaban = hasilKm.ToString();
                    langkah = "Jarak sebenarnya = " + chessMap + " x " + skala + " = " + sebenarnya + " cm = " +
                              hasilKm + " km.";
                    break;
                }
            case 1: // dari sebenarnya ke peta (km ke cm)
                {
                    int skala = Random.Range(50000, 200000);
                    skala = Mathf.RoundToInt(skala / 10000f) * 10000;
                    int jarakKm = Random.Range(1, 20);
                    int jarakCm = (jarakKm * 100000) / skala;
                    if (jarakCm == 0) jarakCm = 1;
                    soal = "Skala peta 1 : " + skala + ". Jarak 2 kota " + jarakKm + " km. Berapa cm di peta?";
                    jawaban = jarakCm.ToString();
                    langkah = "Jarak di peta = " + jarakKm + " km x 100.000 / " + skala + " = " +
                              (jarakKm * 100000) + " / " + skala + " = " + jarakCm + " cm.";
                    break;
                }
            default: // skala gambar/arsitektur (1 : x)
                {
                    int skala = Random.Range(50, 200);
                    int ukuranGambar = Random.Range(1, 10);
                    int ukuranSejati = ukuranGambar * skala;
                    soal = "Skala denah 1 : " + skala + ". Di denah ukuran ruang " + ukuranGambar +
                          " cm. Ukuran sebenarnya ... cm?";
                    jawaban = ukuranSejati.ToString();
                    langkah = "Ukuran sebenarnya = " + ukuranGambar + " x " + skala + " = " + ukuranSejati + " cm.";
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
        if (pilihan.Count < 3) pilihan.Add("100");
        SpawnPilihan(pilihan, jawabanBenar);
    }
}