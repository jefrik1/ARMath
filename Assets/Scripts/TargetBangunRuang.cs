using UnityEngine;
using Vuforia;
using Debug = UnityEngine.Debug;

public class TargetBangunRuang : DefaultObserverEventHandler
{
    public static TargetBangunRuang Aktif { get; private set; }

    [Header("Jenis Bangun Ruang")]
    public JenisBangunRuang jenisBangun;

    [Header("Objek 3D")]
    public GameObject objekModelUtuh;
    public GameObject objekRusuk;
    public GameObject objekJaring;
    public GameObject objekRumus;

    [Header("Animasi Jaring")]
    public Animator animatorJaring;
    public string namaStateJaring = "Jaring";

    [Header("Animasi Rumus")]
    public Animator animatorRumus;
    public string namaStateRumus = "Sisi";

    protected override void Start()
    {
        base.Start();
        MatikanSemuaObjek3D();
    }

    protected override void OnTrackingFound()
    {
        base.OnTrackingFound(); // Vuforia nyalakan 3D Object

        Debug.Log("✅ MARKER DITEMUKAN: " + jenisBangun);
        Aktif = this;

        // Reset tampilan ke model utuh
        TampilModelUtuh();

        // PANGGIL UI AGAR MUNCUL
        if (UIBangunRuang.Instance != null)
        {
            UIBangunRuang.Instance.TampilTombol(true);
            UIBangunRuang.Instance.TampilInfo(jenisBangun);
        }
    }

    protected override void OnTrackingLost()
    {
        base.OnTrackingLost(); // Vuforia matikan 3D Object

        Debug.Log("❌ MARKER HILANG: " + jenisBangun);

        // Reset status aktif
        if (Aktif == this)
        {
            Aktif = null;
        }

        if (UIBangunRuang.Instance != null)
        {
            UIBangunRuang.Instance.SembunyikanSemua();
        }
    }

    void MatikanSemuaObjek3D()
    {
        if (objekModelUtuh != null) objekModelUtuh.SetActive(false);
        if (objekRusuk != null) objekRusuk.SetActive(false);
        if (objekJaring != null) objekJaring.SetActive(false);
        if (objekRumus != null) objekRumus.SetActive(false);
    }

    public void TampilModelUtuh()
    {
        MatikanSemuaObjek3D();
        if (objekModelUtuh != null) objekModelUtuh.SetActive(true);
    }

    public void TampilRusuk()
    {
        MatikanSemuaObjek3D();
        if (objekRusuk != null) objekRusuk.SetActive(true);
    }

    public void TampilJaring()
    {
        MatikanSemuaObjek3D();
        if (objekJaring != null) objekJaring.SetActive(true);

        if (animatorJaring != null)
        {
            animatorJaring.Play(namaStateJaring, -1, 0f);
        }
    }

    public void TampilRumus()
    {
        MatikanSemuaObjek3D();
        if (objekRumus != null) objekRumus.SetActive(true);

        if (animatorRumus != null)
        {
            animatorRumus.Play(namaStateRumus, -1, 0f);
        }
    }
}