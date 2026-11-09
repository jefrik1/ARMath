using UnityEngine;

public enum GradePhase
{
    FaseA = 0, // Kelas 1–2 SD (Fondasi & Pengenalan Konkret)
    FaseB = 1, // Kelas 3–4 SD (Operasi Menengah & Pecahan Dasar)
    FaseC = 2  // Kelas 5–6 SD (Operasi Lanjut, Geometri Lengkap, & Analisis Data)
}

/// <summary>
/// Konfigurasi kurikulum Matematika SD (Kurikulum Merdeka 5 Domain:
/// Bilangan, Aljabar, Pengukuran, Geometri, dan Analisis Data).
/// Tersimpan di PlayerPrefs agar konsisten di seluruh scene kuis dan materi.
/// </summary>
public static class GradeLevelConfig
{
    private const string PREF_KEY = "Mathlfix_GradePhase";

    public static GradePhase CurrentPhase
    {
        get
        {
            int val = PlayerPrefs.GetInt(PREF_KEY, (int)GradePhase.FaseB);
            return (GradePhase)Mathf.Clamp(val, 0, 2);
        }
        set
        {
            PlayerPrefs.SetInt(PREF_KEY, (int)value);
            PlayerPrefs.Save();
        }
    }

    public static string GetPhaseLabel()
    {
        switch (CurrentPhase)
        {
            case GradePhase.FaseA: return "Kelas 1–2 SD (Fase A)";
            case GradePhase.FaseB: return "Kelas 3–4 SD (Fase B)";
            case GradePhase.FaseC: return "Kelas 5–6 SD (Fase C)";
            default: return "Kelas 3–4 SD (Fase B)";
        }
    }

    public static string GetPhaseShortName()
    {
        switch (CurrentPhase)
        {
            case GradePhase.FaseA: return "Kelas 1-2";
            case GradePhase.FaseB: return "Kelas 3-4";
            case GradePhase.FaseC: return "Kelas 5-6";
            default: return "Kelas 3-4";
        }
    }

    // Nama-nama siswa khas Indonesia untuk soal cerita kontekstual
    private static readonly string[] IndonesianNames = {
        "Budi", "Siti", "Edo", "Lani", "Dayu", "Udin", "Beni", "Putri", "Rian", "Dewi"
    };

    public static string GetRandomStudentName()
    {
        return IndonesianNames[Random.Range(0, IndonesianNames.Length)];
    }

    // Buah dan barang sehari-hari untuk soal cerita
    private static readonly string[] IndonesianItems = {
        "apel", "jeruk", "mangga", "pensil", "buku tulis", "kue bolu", "permen", "kelereng"
    };

    public static string GetRandomItemName()
    {
        return IndonesianItems[Random.Range(0, IndonesianItems.Length)];
    }
}
