using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Kuis waktu: konversi satuan, membaca jam, durasi, selisih waktu.
/// </summary>
public class WaktuManager : BaseQuizManager<string>
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

        if (TryLoadFromJson("Waktu"))
        {
            questionText.text = soalSaatIni;
            return;
        }

        int tipe = Random.Range(0, 5);
        string soal = "";
        string jawaban = "";
        string langkah = "";

        switch (tipe)
        {
            case 0: // jam ke menit
                {
                    int jam = Random.Range(1, 12);
                    jawaban = (jam * 60).ToString();
                    soal = jam + " jam = ... menit";
                    langkah = jam + " jam = " + jam + " x 60 = " + jawaban + " menit";
                    break;
                }
            case 1: // menit ke detik
                {
                    int menit = Random.Range(1, 30);
                    jawaban = (menit * 60).ToString();
                    soal = menit + " menit = ... detik";
                    langkah = menit + " menit = " + menit + " x 60 = " + jawaban + " detik";
                    break;
                }
            case 2: // detik ke menit (desimal)
                {
                    int detik = Random.Range(60, 1800);
                    float mnt = detik / 60f;
                    jawaban = mnt.ToString("F1");
                    soal = detik + " detik = ... menit";
                    langkah = detik + " detik = " + detik + " / 60 = " + jawaban + " menit";
                    break;
                }
            case 3: // Durasi: mulai pukul X, berlangsung Y menit, selesai pukul berapa?
                {
                    int jamMulai = Random.Range(6, 10);
                    int menitMulai = Random.Range(0, 45);
                    menitMulai = Mathf.RoundToInt(menitMulai / 5f) * 5;
                    int durasi = Random.Range(30, 180);
                    durasi = Mathf.RoundToInt(durasi / 5f) * 5;
                    int totalMenit = jamMulai * 60 + menitMulai + durasi;
                    int jamSelesai = totalMenit / 60;
                    int menitSelesai = totalMenit % 60;
                    string ms = menitSelesai < 10 ? "0" + menitSelesai : menitSelesai.ToString();
                    soal = "Dimulai pukul " + jamMulai + ":" + (menitMulai < 10 ? "0" + menitMulai : menitMulai) +
                          ", berlangsung " + durasi + " menit. Selesai pukul ...?";
                    jawaban = jamSelesai + ":" + ms;
                    langkah = "Total menit = " + (jamMulai * 60 + menitMulai) + " + " + durasi +
                              " = " + totalMenit + " menit = pukul " + jamSelesai + ":" + ms + ".";
                    break;
                }
            default: // Jam berapa X jam Y menit setelah pukul A:B?
                {
                    int jamAwal = Random.Range(7, 11);
                    int menitAwal = Random.Range(0, 45);
                    menitAwal = Mathf.RoundToInt(menitAwal / 5f) * 5;
                    int jamTambah = Random.Range(1, 4);
                    int menitTambah = Random.Range(15, 90);
                    menitTambah = Mathf.RoundToInt(menitTambah / 5f) * 5;
                    int totalMenit = (jamAwal * 60 + menitAwal) + (jamTambah * 60 + menitTambah);
                    int jamHasil = totalMenit / 60;
                    if (jamHasil >= 24) jamHasil -= 24;
                    int menitHasil = totalMenit % 60;
                    string m1s = menitAwal < 10 ? "0" + menitAwal : menitAwal.ToString();
                    string m2s = menitHasil < 10 ? "0" + menitHasil : menitHasil.ToString();
                    soal = "Pukul " + jamAwal + ":" + m1s + " ditambah " + jamTambah + " jam " + menitTambah +
                          " menit = pukul ...?";
                    jawaban = jamHasil + ":" + m2s;
                    langkah = jamAwal + ":" + m1s + " + " + jamTambah + " jam " + menitTambah +
                              " menit = " + jamHasil + ":" + m2s + ".";
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
            return Mathf.Abs(pa - ca) < 0.1f;
        return playerAnswer.Trim() == correctAnswerValue.Trim();
    }

    protected override string ConvertAnswerToString(string answer) => answer;

    private void BuatPilihan(string jawabanBenar)
    {
        List<string> pilihan = new List<string> { jawabanBenar };
        if (jawabanBenar.Contains(":"))
        {
            // variasi waktu
            while (pilihan.Count < 4)
            {
                string[] parts = jawabanBenar.Split(':');
                if (parts.Length == 2 && int.TryParse(parts[0], out int j) && int.TryParse(parts[1], out int m))
                {
                    int newJ = j + Random.Range(-1, 2);
                    int newM = m + Random.Range(-10, 11);
                    if (newM >= 0 && newM < 60 && newJ >= 0 && newJ < 24)
                    {
                        string nm = newM < 10 ? "0" + newM : newM.ToString();
                        string d = newJ + ":" + nm;
                        if (!pilihan.Contains(d) && !d.Equals(jawabanBenar))
                            pilihan.Add(d);
                    }
                }
                else break;
            }
        }
        else
        {
            float jb = float.Parse(jawabanBenar);
            while (pilihan.Count < 4)
            {
                float offset = Random.Range(-30, 31);
                float nilai = jb + offset;
                if (nilai > 0 && Mathf.Abs(nilai - jb) > 0.1f && !pilihan.Contains(nilai.ToString("F1")))
                    pilihan.Add(nilai.ToString("F1"));
            }
        }
        if (pilihan.Count < 2) pilihan.Add("12:00");
        if (pilihan.Count < 3) pilihan.Add("15:30");
        SpawnPilihan(pilihan, jawabanBenar);
    }
}