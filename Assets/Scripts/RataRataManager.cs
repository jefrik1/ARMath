using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Kuis rata-rata (mean) data statistika SD sesuai fase kurikulum.
/// </summary>
public class RataRataManager : MonoBehaviour, IQuizManager
{
    [Header("Referensi UI")]
    public TMP_Text questionText;
    public TMP_Text answerText;
    public Button checkButton;
    public Button undoButton;

    [Header("Lainnya")]
    public PopupManager popupManager;
    public HealthManager healthManager;
    public ItemSpawner itemSpawner;

    private int jawabanSoalBenar;
    private string pilihanBenar;
    private string jawabanPilihanPemain;
    private int nilaiJawabanPemain;
    private string soalSaatIni;
    private string langkahPenyelesaian;

    private List<GameObject> objekPilihanAktif = new List<GameObject>();
    private GameObject objekTerpilih;
    private string[] huruf = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K" };

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
        foreach (var obj in objekPilihanAktif) if (obj != null) Destroy(obj);
        objekPilihanAktif.Clear();
        jawabanPilihanPemain = "";
        nilaiJawabanPemain = 0;
        objekTerpilih = null;

        GradePhase phase = GradeLevelConfig.CurrentPhase;
        int jumlahElemen = (phase == GradePhase.FaseC) ? Random.Range(5, 9) : Random.Range(3, 6);
        int rataRata = (phase == GradePhase.FaseC) ? Random.Range(10, 40) : Random.Range(4, 12);
        int totalSum = rataRata * jumlahElemen;

        List<int> numbers = new List<int>();
        int currentSum = 0;

        for (int i = 0; i < jumlahElemen - 1; i++)
        {
            int deviation = Random.Range(-4, 5);
            int val = Mathf.Max(1, rataRata + deviation);
            numbers.Add(val);
            currentSum += val;
        }

        int lastVal = totalSum - currentSum;
        if (lastVal <= 0)
        {
            lastVal = rataRata;
            totalSum = currentSum + lastVal;
            rataRata = Mathf.RoundToInt((float)totalSum / jumlahElemen);
        }
        numbers.Add(lastVal);
        numbers = numbers.OrderBy(x => Random.value).ToList();

        jawabanSoalBenar = rataRata;
        string numberString = string.Join(", ", numbers);

        soalSaatIni = $"Hitung rata-rata (mean) dari data berikut:\n{numberString}";
        langkahPenyelesaian = $"Rumus Mean = Jumlah seluruh data / Banyak data\n" +
                              $"Jumlah data = {string.Join(" + ", numbers)} = {totalSum}\n" +
                              $"Banyak data = {jumlahElemen}\n" +
                              $"Mean = {totalSum} / {jumlahElemen} = {jawabanSoalBenar}";

        questionText.text = soalSaatIni;
        BuatPilihan(jawabanSoalBenar);
    }

    public void SetJawabanPilihan(string pilihan, int nilai, GameObject objekYangDipilih)
    {
        jawabanPilihanPemain = pilihan;
        nilaiJawabanPemain = nilai;
        objekTerpilih = objekYangDipilih;
        answerText.text = "Jawaban Anda: " + nilai;
        checkButton.interactable = true;
        undoButton.gameObject.SetActive(true);
    }

    public void SetJawabanPilihan(string pilihan, string nilai, GameObject objekYangDipilih)
    {
        jawabanPilihanPemain = pilihan;
        int.TryParse(nilai, out nilaiJawabanPemain);
        objekTerpilih = objekYangDipilih;
        answerText.text = "Jawaban Anda: " + nilai;
        checkButton.interactable = true;
        undoButton.gameObject.SetActive(true);
    }

    public void SetJawabanPilihan(string pilihan, float nilai, GameObject objekYangDipilih)
    {
        jawabanPilihanPemain = pilihan;
        nilaiJawabanPemain = Mathf.RoundToInt(nilai);
        objekTerpilih = objekYangDipilih;
        answerText.text = "Jawaban Anda: " + nilai.ToString("F1");
        checkButton.interactable = true;
        undoButton.gameObject.SetActive(true);
    }

    public void UndoJawaban()
    {
        if (objekTerpilih == null) return;
        jawabanPilihanPemain = "";
        nilaiJawabanPemain = 0;
        answerText.text = "Jawaban Anda: -";
        objekTerpilih = null;
        checkButton.interactable = false;
        undoButton.gameObject.SetActive(false);
    }

    public void CekJawaban()
    {
        if (string.IsNullOrEmpty(jawabanPilihanPemain))
        {
            popupManager.ShowPopup("Pilih jawaban dulu!", false);
            return;
        }

        foreach (var obj in objekPilihanAktif)
        {
            if (obj != null) Destroy(obj);
        }
        objekPilihanAktif.Clear();

        // Validasi ganda: via label huruf (A-K) atau nilai integer
        bool lastWasCorrect = (jawabanPilihanPemain == pilihanBenar) || (nilaiJawabanPemain == jawabanSoalBenar);

        if (lastWasCorrect)
        {
            if (ScoreManager.instance != null)
            {
                ScoreManager.instance.AddScore(10);
            }

            string pesanKonfirmasi = $"Jawaban kamu '{jawabanPilihanPemain}' ({nilaiJawabanPemain}) adalah benar!\nPoin bertambah.";
            popupManager.ShowPopupBenar(pesanKonfirmasi);
            Invoke(nameof(LanjutKeSoalBerikutnya), 2f);
        }
        else
        {
            bool isGameOver = healthManager.ReduceHealth();
            string pembahasan = $"PEMBAHASAN\n\nSoal: {soalSaatIni}\nJawaban Anda: {nilaiJawabanPemain} ({jawabanPilihanPemain})\nJawaban Benar: {jawabanSoalBenar} ({pilihanBenar})\n\n{langkahPenyelesaian}";

            if (isGameOver)
            {
                string finalMessage = $"Game Over! \uD83D\uDE22\nSkor Anda: {ScoreManager.instance?.Score ?? 0}";
                popupManager.ShowPopup(finalMessage, false, true);
            }
            else
            {
                popupManager.ShowPembahasan(pembahasan);
            }
        }
    }

    public void LanjutKeSoalBerikutnya()
    {
        popupManager.HideAllPanels();
        GenerateRandomQuestion();
    }

    private void BuatPilihan(int jawabanBenar)
    {
        List<int> pilihan = new List<int> { jawabanBenar };
        while (pilihan.Count < 4)
        {
            int pengecoh = jawabanBenar + Random.Range(-5, 6);
            if (pengecoh > 0 && pengecoh != jawabanBenar && !pilihan.Contains(pengecoh))
                pilihan.Add(pengecoh);
        }

        for (int i = pilihan.Count - 1; i > 0; i--)
        {
            int r = Random.Range(0, i + 1);
            int tmp = pilihan[i]; pilihan[i] = pilihan[r]; pilihan[r] = tmp;
        }

        pilihanBenar = null;
        for (int i = 0; i < pilihan.Count && i < huruf.Length; i++)
        {
            if (pilihan[i] == jawabanBenar)
            {
                pilihanBenar = huruf[i];
                break;
            }
        }

        string[] strs = pilihan.ConvertAll(x => x.ToString()).ToArray();
        objekPilihanAktif = itemSpawner.SpawnPilihan(strs, huruf, this);
    }

    public bool IsAnswerCorrect(string playerAnswer)
    {
        if (int.TryParse(playerAnswer, out int pa))
            return pa == jawabanSoalBenar;
        return playerAnswer == pilihanBenar;
    }
}
