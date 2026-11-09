using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;

/// <summary>
/// Kuis data pengolahan: mean, median, modus dari data yang diberikan.
/// </summary>
public class DataManager : MonoBehaviour, IQuizManager
{
    [Header("Referensi UI")]
    public TMP_Text questionText;
    public TMP_Text answerText;
    public Button checkButton;
    public Button undoButton;

    [Header("Manajer Lain")]
    public PopupManager popupManager;
    public HealthManager healthManager;
    public ItemSpawner itemSpawner;

    // huruf label jawaban benar (A, B, ...)
    protected string correctAnswerLabel;
    protected string playerAnswerLabel;
    protected string soalSaatIni;
    protected string langkahPenyelesaian;
    protected string correctAnswerValue;

    protected List<GameObject> objekPilihanAktif = new List<GameObject>();
    protected GameObject objekTerpilih;
    private string[] huruf = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K" };

    // State quiz
    private int jawabanSoalBenar;
    private string pilihanBenar;
    private int[] pilihanAngka;
    private bool lastWasCorrect;

    void Start()
    {
        checkButton.onClick.AddListener(CekJawaban);
        undoButton.onClick.AddListener(UndoJawaban);
        if (ScoreManager.instance != null) ScoreManager.instance.ResetScore();
        GenerateRandomQuestion();
    }

    public void GenerateRandomQuestion()
    {
        answerText.text = "Jawaban Anda: -";
        checkButton.interactable = false;
        undoButton.gameObject.SetActive(false);
        foreach (var obj in objekPilihanAktif)
            if (obj != null) Destroy(obj);
        objekPilihanAktif.Clear();
        playerAnswerLabel = "";
        objekTerpilih = null;

        // Pilih tipe soal: mean, median, atau modus
        int tipe = Random.Range(0, 3);

        if (tipe == 0)
        {
            GenerateMeanQuestion();
        }
        else if (tipe == 1)
        {
            GenerateMedianQuestion();
        }
        else
        {
            GenerateModusQuestion();
        }
    }

    private void GenerateMeanQuestion()
    {
        int count = Random.Range(5, 11);
        int targetMean = Random.Range(10, 50);
        int total = targetMean * count;

        List<int> numbers = new List<int>();
        int sum = 0;
        for (int i = 0; i < count - 1; i++)
        {
            int n = Random.Range(5, targetMean + 10);
            numbers.Add(n);
            sum += n;
        }
        int last = total - sum;
        if (last <= 0)
        {
            GenerateMeanQuestion();
            return;
        }
        numbers.Add(last);

        numbers = numbers.OrderBy(x => Random.value).ToList();
        string dataStr = string.Join(", ", numbers);
        jawabanSoalBenar = targetMean;
        soalSaatIni = "Rata-rata (mean) dari data berikut:\n" + dataStr + "\nadalah ...";
        langkahPenyelesaian = "Langkah: Jumlahkan semua data = " + total + ", lalu bagi dengan banyak data (" + count + ") = " + targetMean + ".";
        correctAnswerValue = jawabanSoalBenar.ToString();
        questionText.text = soalSaatIni;
        BuatPilihanMean(jawabanSoalBenar);
    }

    private void GenerateMedianQuestion()
    {
        int count;
        int tipe = Random.Range(0, 2);
        if (tipe == 0) count = 5;
        else count = 7;

        List<int> numbers = new List<int>();
        for (int i = 0; i < count; i++)
            numbers.Add(Random.Range(1, 100));

        string dataStr = string.Join(", ", numbers);
        numbers.Sort();
        int median = numbers[count / 2];
        soalSaatIni = "Median dari data berikut (sudah diurutkan):\n" + dataStr + "\nadalah ...";
        langkahPenyelesaian = "Langkah: Data diurutkan, ambil nilai tengah (indeks " + (count / 2) + "). Median = " + median + ".";
        correctAnswerValue = median.ToString();
        questionText.text = soalSaatIni;
        BuatPilihanMedian(median);
    }

    private void GenerateModusQuestion()
    {
        int count = Random.Range(8, 15);
        int uniqueCount = Random.Range(3, 6);
        List<int> numbers = new List<int>();

        while (numbers.Count < uniqueCount)
        {
            int n = Random.Range(1, 50);
            if (!numbers.Contains(n)) numbers.Add(n);
        }

        int modus = numbers[Random.Range(0, numbers.Count)];
        while (numbers.Count < count) numbers.Add(modus);

        numbers = numbers.OrderBy(x => Random.value).ToList();
        string dataStr = string.Join(", ", numbers);
        soalSaatIni = "Modus dari data berikut (nilai yang paling sering muncul):\n" + dataStr + "\nadalah ...";
        langkahPenyelesaian = "Langkah: Cari nilai yang paling sering muncul. Modus = " + modus + ".";
        correctAnswerValue = modus.ToString();
        questionText.text = soalSaatIni;
        BuatPilihanModus(modus);
    }

    private void BuatPilihanMean(int jawaban)
    {
        List<int> pilihan = new List<int> { jawaban };
        while (pilihan.Count < 11)
        {
            int pengecoh = jawaban + Random.Range(-10, 11);
            if (pengecoh > 0 && pengecoh != jawaban && !pilihan.Contains(pengecoh))
                pilihan.Add(pengecoh);
        }
        pilihan = pilihan.OrderBy(x => Random.value).ToList();
        for (int i = 0; i < pilihan.Count; i++)
            if (pilihan[i] == jawaban) pilihanBenar = huruf[i];

        objekPilihanAktif = itemSpawner.SpawnPilihan(pilihan, huruf, this);
    }

    private void BuatPilihanMedian(int jawaban)
    {
        List<int> pilihan = new List<int> { jawaban };
        while (pilihan.Count < 11)
        {
            int pengecoh = jawaban + Random.Range(-10, 11);
            if (pengecoh > 0 && pengecoh != jawaban && !pilihan.Contains(pengecoh))
                pilihan.Add(pengecoh);
        }
        pilihan = pilihan.OrderBy(x => Random.value).ToList();
        for (int i = 0; i < pilihan.Count; i++)
            if (pilihan[i] == jawaban) pilihanBenar = huruf[i];

        objekPilihanAktif = itemSpawner.SpawnPilihan(pilihan, huruf, this);
    }

    private void BuatPilihanModus(int jawaban)
    {
        List<int> pilihan = new List<int> { jawaban };
        while (pilihan.Count < 11)
        {
            int pengecoh = jawaban + Random.Range(-10, 11);
            if (pengecoh > 0 && pengecoh != jawaban && !pilihan.Contains(pengecoh))
                pilihan.Add(pengecoh);
        }
        pilihan = pilihan.OrderBy(x => Random.value).ToList();
        for (int i = 0; i < pilihan.Count; i++)
            if (pilihan[i] == jawaban) pilihanBenar = huruf[i];

        objekPilihanAktif = itemSpawner.SpawnPilihan(pilihan, huruf, this);
    }

    public bool IsAnswerCorrect(string playerAnswer)
    {
        return playerAnswer == correctAnswerValue;
    }

    public void SetJawabanPilihan(string pilihan, int nilai, GameObject objekYangDipilih)
    {
        playerAnswerLabel = pilihan;
        objekTerpilih = objekYangDipilih;
        answerText.text = "Jawaban Anda: " + nilai;
        checkButton.interactable = true;
        undoButton.gameObject.SetActive(true);
    }

    public void SetJawabanPilihan(string pilihan, string nilai, GameObject objekYangDipilih)
    {
        playerAnswerLabel = pilihan;
        objekTerpilih = objekYangDipilih;
        answerText.text = "Jawaban Anda: " + nilai;
        checkButton.interactable = true;
        undoButton.gameObject.SetActive(true);
    }

    public void SetJawabanPilihan(string pilihan, float nilai, GameObject objekYangDipilih)
    {
        playerAnswerLabel = pilihan;
        objekTerpilih = objekYangDipilih;
        answerText.text = "Jawaban Anda: " + nilai.ToString("F1");
        checkButton.interactable = true;
        undoButton.gameObject.SetActive(true);
    }

    public void UndoJawaban()
    {
        if (objekTerpilih == null) return;
        playerAnswerLabel = "";
        objekTerpilih = null;
        answerText.text = "Jawaban Anda: -";
        checkButton.interactable = false;
        undoButton.gameObject.SetActive(false);
    }

    public void CekJawaban()
    {
        if (string.IsNullOrEmpty(playerAnswerLabel))
        {
            if (popupManager != null) popupManager.ShowPopup("Pilih jawaban dulu!", false);
            return;
        }

        foreach (var obj in objekPilihanAktif)
            if (obj != null) Destroy(obj);
        objekPilihanAktif.Clear();

        int playerAnswerInt = 0;
        int.TryParse(playerAnswerLabel, out playerAnswerInt);

        lastWasCorrect = (playerAnswerInt == jawabanSoalBenar);

        if (lastWasCorrect)
        {
            if (ScoreManager.instance != null) ScoreManager.instance.AddScore(10);
            string pesan = "Jawaban kamu '" + playerAnswerLabel + "' (" + pilihanBenar + ") adalah benar! Poin bertambah.";
            if (popupManager != null) popupManager.ShowPopupBenar(pesan);
            Invoke(nameof(LanjutKeSoalBerikutnya), 2f);
        }
        else
        {
            bool isGameOver = false;
            if (healthManager != null) isGameOver = healthManager.ReduceHealth();
            string pembahasan = "PEMBAHASAN\n\nSoal: " + soalSaatIni + "\nJawaban Anda: " + playerAnswerLabel + "\nJawaban Benar: " + pilihanBenar + " (" + correctAnswerValue + ")\n\n" + langkahPenyelesaian;

            if (isGameOver)
            {
                string msg = "Game Over!\nSkor Anda: " + (ScoreManager.instance?.Score ?? 0);
                if (popupManager != null) popupManager.ShowPopup(msg, false, true);
            }
            else
            {
                if (popupManager != null) popupManager.ShowPembahasan(pembahasan);
            }
        }
    }

    public void LanjutKeSoalBerikutnya()
    {
        if (popupManager != null) popupManager.HideAllPanels();
        GenerateRandomQuestion();
    }

    public virtual void OnTimeUp()
    {
        // Implementasi opsional untuk timer
        lastWasCorrect = false;
        if (string.IsNullOrEmpty(playerAnswerLabel))
        {
            foreach (var obj in objekPilihanAktif)
                if (obj != null) Destroy(obj);
            objekPilihanAktif.Clear();

            bool isGameOver = false;
            if (healthManager != null) isGameOver = healthManager.ReduceHealth();

            if (isGameOver)
            {
                string msg = "Game Over!\nSkor Anda: " + (ScoreManager.instance?.Score ?? 0);
                if (popupManager != null) popupManager.ShowPopup(msg, false, true);
            }
        }
        else
        {
            CekJawaban();
        }
    }

    protected virtual string GetKategoriName() => "Data";
    protected virtual string GetSoalId() => soalSaatIni.GetHashCode().ToString() + "_Data";
}