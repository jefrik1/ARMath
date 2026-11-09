using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Kuis irisan bilangan: irisan dan gabungan himpunan bilangan (Venn diagram sederhana).
/// </summary>
public class IrisanBilanganManager : BaseQuizManager<int>
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

        if (TryLoadFromJson("IrisanBilangan"))
        {
            questionText.text = soalSaatIni;
            return;
        }

        int tipe = Random.Range(0, 3);
        string soal = "";
        int jawaban = 0;
        string langkah = "";

        // Buat himpunan A dan B dengan irisan
        int irisan = Random.Range(2, 6);
        int aSaja = Random.Range(2, 5);
        int bSaja = Random.Range(2, 5);
        int nA = irisan + aSaja;
        int nB = irisan + bSaja;

        switch (tipe)
        {
            case 0: // |A ∪ B| = n(A) + n(B) - n(A ∩ B)
                {
                    soal = "Diketahui n(A) = " + nA + ", n(B) = " + nB + ", n(A ∩ B) = " + irisan +
                          ". n(A ∪ B) = ...";
                    jawaban = nA + nB - irisan;
                    langkah = "n(A ∪ B) = n(A) + n(B) - n(A ∩ B) = " + nA + " + " + nB + " - " + irisan +
                              " = " + jawaban;
                    break;
                }
            case 1: // |A ∩ B| dari n(A), n(B), n(A ∪ B)
                {
                    int nA2 = Random.Range(5, 12);
                    int nB2 = Random.Range(5, 12);
                    int nUnion = nA2 + nB2 - Random.Range(1, Mathf.Min(nA2, nB2));
                    int irisan2 = nA2 + nB2 - nUnion;
                    soal = "Diketahui n(A) = " + nA2 + ", n(B) = " + nB2 + ", n(A ∪ B) = " + nUnion +
                          ". n(A ∩ B) = ...";
                    jawaban = irisan2;
                    langkah = "n(A ∩ B) = n(A) + n(B) - n(A ∪ B) = " + nA2 + " + " + nB2 + " - " + nUnion +
                              " = " + jawaban;
                    break;
                }
            default: // Gabungan dua himpunan yang saling lepas
                {
                    int aHanya = Random.Range(3, 8);
                    int bHanya = Random.Range(3, 8);
                    soal = "Himpunan A memiliki " + aHanya + " elemen, himpunan B memiliki " + bHanya +
                          " elemen, dan A ∩ B = ∅ (saling lepas). n(A ∪ B) = ...";
                    jawaban = aHanya + bHanya;
                    langkah = "Karena A ∩ B = ∅, maka n(A ∪ B) = n(A) + n(B) = " + aHanya + " + " + bHanya + " = " + jawaban;
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
        if (int.TryParse(playerAnswer, out int pa) && int.TryParse(correctAnswerValue, out int ca))
            return pa == ca;
        return false;
    }

    protected override string ConvertAnswerToString(int answer) => answer.ToString();

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