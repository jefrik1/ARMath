using UnityEngine;
using TMPro;

/// <summary>
/// Objek pilihan jawaban 3D dalam ruang Augmented Reality (Walk-to-Answer Mechanic).
/// Menampilkan teks pilihan dengan styling Stitch, indikator jarak real-time ("1.8 m"),
/// dan mendeteksi kedekatan fisik pemain via OnTriggerEnter (atau tap layar).
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

    private Transform camTransform;
    private bool isPickedUp = false;

    private void Start()
    {
        if (Camera.main != null)
            camTransform = Camera.main.transform;

        // Pastikan objek selalu menghadap kamera (Billboard)
        if (GetComponent<LookAtCamera>() == null)
            gameObject.AddComponent<LookAtCamera>();
    }

    private void Update()
    {
        if (camTransform == null && Camera.main != null)
            camTransform = Camera.main.transform;

        if (camTransform != null && teksPilihan != null)
        {
            float dist = Vector3.Distance(transform.position, camTransform.position);
            string valStr = menggunakanInt ? nilaiInt.ToString() : (menggunakanFloat ? nilaiFloat.ToString("F1") : nilaiString);

            string proximityBadge = isPickedUp
                ? "<color=#22C55E>✓ Terkunci!</color>"
                : (dist < 1.3f
                    ? $"<color=#22C55E>{dist:F1}m • Sangat Dekat!</color>"
                    : $"<color=#246EA6>{dist:F1}m</color>");

            teksPilihan.text = $"<b>{pilihanHuruf}. {valStr}</b>\n<size=11>{proximityBadge}</size>";
        }
    }

    /// Setup untuk kuis berbasis angka (int)
    public void Setup(string huruf, int nilai, IQuizManager manager)
    {
        pilihanHuruf = huruf;
        nilaiInt = nilai;
        menggunakanInt = true;
        menggunakanString = false;
        menggunakanFloat = false;
        quizManager = manager;
        isPickedUp = false;

        if (teksPilihan != null)
            teksPilihan.text = $"{huruf}. {nilaiInt}";
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
        isPickedUp = false;

        if (teksPilihan != null)
            teksPilihan.text = $"{huruf}. {nilaiString}";
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
        isPickedUp = false;

        if (teksPilihan != null)
            teksPilihan.text = $"{huruf}. {nilaiFloat:F1}";
    }

    private void OnTriggerEnter(Collider other)
    {
        // Deteksi tabrakan fisik dengan Player atau ARCamera saat siswa berjalan mendekati objek
        if ((other.CompareTag("Player") || other.CompareTag("MainCamera") || other.GetComponent<Camera>() != null) && quizManager != null)
        {
            TriggerSelection();
        }
    }

    private void OnMouseDown()
    {
        // Fallback sentuhan langsung di layar jika siswa tidak dapat berjalan jauh
        TriggerSelection();
    }

    private void TriggerSelection()
    {
        if (quizManager == null) return;
        isPickedUp = true;
        MathSoundFX.PlayClick();

        if (menggunakanInt)
            quizManager.SetJawabanPilihan(pilihanHuruf, nilaiInt, this.gameObject);
        else if (menggunakanString)
            quizManager.SetJawabanPilihan(pilihanHuruf, nilaiString, this.gameObject);
        else if (menggunakanFloat)
            quizManager.SetJawabanPilihan(pilihanHuruf, nilaiFloat, this.gameObject);
    }
}
