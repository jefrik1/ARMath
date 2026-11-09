using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Kuis volume: kubus, balok, tabung, prisma.
/// </summary>
public class VolumeManager : BaseQuizManager<string>
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

        if (TryLoadFromJson("Volume"))
        {
            questionText.text = soalSaatIni;
            return;
        }

        int tipe = Random.Range(0, 4);
        string soal = "";
        string jawaban = "";
        string langkah = "";

        switch (tipe)
        {
            case 0: // Volume kubus
                {
                    int s = Random.Range(2, 12);
                    int volume = s * s * s;
                    soal = "Volume kubus dengan sisi " + s + " cm adalah ... cm³.";
                    jawaban = volume.ToString();
                    langkah = "Volume = s³ = " + s + " x " + s + " x " + s + " = " + volume + " cm³.";
                    break;
                }
            case 1: // Volume balok
                {
                    int p = Random.Range(4, 15);
                    int l = Random.Range(3, 12);
                    int t = Random.Range(2, 10);
                    int volume = p * l * t;
                    soal = "Volume balok p=" + p + " cm, l=" + l + " cm, t=" + t + " cm adalah ... cm³.";
                    jawaban = volume.ToString();
                    langkah = "Volume = p x l x t = " + p + " x " + l + " x " + t + " = " + volume + " cm³.";
                    break;
                }
            case 2: // Volume tabung (sederhana, phi = 3.14)
                {
                    int r = Random.Range(2, 10);
                    int t = Random.Range(5, 20);
                    float phi = 3.14f;
                    float volume = phi * r * r * t;
                    int volBulat = Mathf.RoundToInt(volume);
                    soal = "Volume tabung r=" + r + " cm, t=" + t + " cm (π=3.14) adalah ... cm³.";
                    jawaban = volBulat.ToString();
                    langkah = "Volume = π x r² x t = 3.14 x " + r + "² x " + t +
                              " = 3.14 x " + (r * r) + " x " + t + " = " + volBulat + " cm³.";
                    break;
                }
            default: // Volume prisma segitiga
                {
                    int a = Random.Range(4, 15);
                    int tSegitiga = Random.Range(3, 12);
                    int tPrisma = Random.Range(5, 15);
                    int volume = (a * tSegitiga / 2) * tPrisma;
                    soal = "Volume prisma segitiga alas=" + a + " cm, t segitiga=" + tSegitiga +
                          " cm, t prisma=" + tPrisma + " cm adalah ... cm³.";
                    jawaban = volume.ToString();
                    langkah = "Volume = (alas x t segitiga / 2) x t prisma = (" + a + " x " + tSegitiga +
                              " / 2) x " + tPrisma + " = " + volume + " cm³.";
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
                int offset = Random.Range(-100, 101);
                int nilai = num + offset;
                if (nilai > 0 && nilai != num && !pilihan.Contains(nilai.ToString()))
                    pilihan.Add(nilai.ToString());
            }
        }
        if (pilihan.Count < 2) pilihan.Add("100");
        if (pilihan.Count < 3) pilihan.Add("500");
        SpawnPilihan(pilihan, jawabanBenar);
    }
}