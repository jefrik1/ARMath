using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Load soal dari JSON di StreamingAssets/quiz_data.json.
/// </summary>
public static class SoalJsonLoader
{
    private static QuizData cachedData;
    private static bool isLoaded = false;

    /// <summary>
    /// Load dan cache data soal dari JSON.
    /// </summary>
    public static QuizData Load()
    {
        if (isLoaded && cachedData != null)
            return cachedData;

        string json = null;

        // 1. Prioritas: Load via Resources (kompatibel penuh dengan Android APK build & Editor)
        TextAsset textAsset = Resources.Load<TextAsset>("quiz_data");
        if (textAsset != null && !string.IsNullOrEmpty(textAsset.text))
        {
            json = textAsset.text;
        }
        else
        {
            // 2. Fallback: StreamingAssets (Editor & Standalone filesystem)
            string streamingPath = System.IO.Path.Combine(Application.streamingAssetsPath, "quiz_data.json");
            if (System.IO.File.Exists(streamingPath))
            {
                json = System.IO.File.ReadAllText(streamingPath);
            }
        }

        if (string.IsNullOrEmpty(json))
        {
            Debug.LogWarning("SoalJsonLoader: quiz_data.json tidak dapat ditemukan di Resources maupun StreamingAssets.");
            return null;
        }

        cachedData = JsonUtility.FromJson<QuizData>(json);
        isLoaded = true;

        if (cachedData == null)
        {
            Debug.LogWarning("SoalJsonLoader: gagal parse JSON");
        }

        return cachedData;
    }

    /// <summary>
    /// Dapatkan list soal untuk kategori tertentu (berdasarkan nama).
    /// </summary>
    public static List<SoalEntry> GetSoal(string kategoriNama)
    {
        QuizData data = Load();
        if (data == null || data.kategories == null) return new List<SoalEntry>();

        foreach (var kategori in data.kategories)
        {
            if (kategori.nama == kategoriNama)
                return kategori.soals ?? new List<SoalEntry>();
        }

        return new List<SoalEntry>();
    }

    /// <summary>
    /// Dapatkan soal acak untuk kategori tertentu.
    /// </summary>
    public static SoalEntry GetRandomSoal(string kategoriNama)
    {
        List<SoalEntry> soals = GetSoal(kategoriNama);
        if (soals == null || soals.Count == 0) return null;

        if (soals.Count == 1) return soals[0];

        int idx = Random.Range(0, soals.Count);
        return soals[idx];
    }

    /// <summary>
    /// Dapatkan semua kategori yang tersedia.
    /// </summary>
    public static List<string> GetAllKategori()
    {
        QuizData data = Load();
        if (data == null || data.kategories == null) return new List<string>();

        List<string> result = new List<string>();
        foreach (var k in data.kategories)
        {
            if (!string.IsNullOrEmpty(k.nama))
                result.Add(k.nama);
        }
        return result;
    }
}