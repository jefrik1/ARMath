using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Data soal statis dari JSON di StreamingAssets/quiz_data.json.
/// Format JSON harus array kategories (bukan Dictionary, karena JsonUtility tidak support Dictionary).
/// </summary>
[System.Serializable]
public class QuizData
{
    public int version;
    public List<KategoriData> kategories;
}

[System.Serializable]
public class KategoriData
{
    public string nama;
    public string target;
    public List<SoalEntry> soals;
}

[System.Serializable]
public class SoalEntry
{
    public string soal;
    public string jawaban;
    public string[] pengecof;
    public string langkah;
}