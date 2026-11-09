using UnityEngine;

/// <summary>
/// Interface yang diimplementasi semua quiz manager.
/// Dipakai oleh ItemSpawner untuk callback saat player menyentuh pilihan jawaban.
/// </summary>
public interface IQuizManager
{
    void SetJawabanPilihan(string pilihan, int nilai, GameObject objekYangDipilih);
    void SetJawabanPilihan(string pilihan, string nilai, GameObject objekYangDipilih);
    void SetJawabanPilihan(string pilihan, float nilai, GameObject objekYangDipilih);
    void UndoJawaban();
    bool IsAnswerCorrect(string playerAnswer);
}