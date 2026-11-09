using UnityEngine;

public enum GradePhase
{
    FaseA = 0, // Kelas 1–2 SD
    FaseB = 1, // Kelas 3–4 SD
    FaseC = 2  // Kelas 5–6 SD
}

/// <summary>
/// Konfigurasi fase kelas sekolah dasar (SD) untuk kurikulum kuis matematika.
/// Tersimpan di PlayerPrefs agar konsisten antar scene.
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
            case GradePhase.FaseA: return "Kelas 1–2 (Fase A)";
            case GradePhase.FaseB: return "Kelas 3–4 (Fase B)";
            case GradePhase.FaseC: return "Kelas 5–6 (Fase C)";
            default: return "Kelas 3–4 (Fase B)";
        }
    }
}
