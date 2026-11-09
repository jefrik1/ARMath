using UnityEngine;
using TMPro;

/// <summary>
/// Objek pilihan jawaban yang bisa disentuh/Trigger oleh player.
/// Setup satu kali saat spawn, lalu panggil quiz manager saat player menyentuhnya.
/// </summary>
public class ObjekA : MonoBehaviour
{
    [Header("UI")]
    public string pilihanHuruf;
    public TMP_Text teksPilihan;

    public IQuizManager quizManager;
    private int nilaiInt;
    private string nilaiString;
    private float nilaiFloat;
    private bool menggunakanInt;
    private bool menggunakanString;
    private bool menggunakanFloat;

    /// Setup untuk kuis berbasis angka (int)
    public void Setup(string huruf, int nilai, IQuizManager manager)
    {
        pilihanHuruf = huruf;
        nilaiInt = nilai;
        menggunakanInt = true;
        menggunakanString = false;
        menggunakanFloat = false;
        quizManager = manager;

        if (teksPilihan != null)
            teksPilihan.text = nilaiInt.ToString();
    }

    /// Setup untuk kuis berbasis teks/string (pecahan)
    public void Setup(string huruf, string nilai, IQuizManager manager)
    {
        pilihanHuruf = huruf;
        nilaiString = nilai;
        menggunakanInt = false;
        menggunakanString = true;
        menggunakanFloat = false;
        quizManager = manager;

        if (teksPilihan != null)
            teksPilihan.text = nilaiString;
    }

    /// Setup untuk kuis berbasis desimal (float / bangun ruang)
    public void Setup(string huruf, float nilai, IQuizManager manager)
    {
        pilihanHuruf = huruf;
        nilaiFloat = nilai;
        menggunakanInt = false;
        menggunakanString = false;
        menggunakanFloat = true;
        quizManager = manager;

        if (teksPilihan != null)
            teksPilihan.text = nilaiFloat.ToString("F1");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && quizManager != null)
        {
            if (menggunakanInt)
                quizManager.SetJawabanPilihan(pilihanHuruf, nilaiInt, this.gameObject);
            else if (menggunakanString)
                quizManager.SetJawabanPilihan(pilihanHuruf, nilaiString, this.gameObject);
            else if (menggunakanFloat)
                quizManager.SetJawabanPilihan(pilihanHuruf, nilaiFloat, this.gameObject);
        }
    }
}