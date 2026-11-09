using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnButton : MonoBehaviour
{
    [Tooltip("Pilih panel tujuan di MainMenu.")]
    public PanelTarget targetPanel = PanelTarget.None;

    [Tooltip("Isi dengan nama scene MainMenu.")]
    public string mainMenuSceneName = "MainMenu";

    public void GoBackToMainMenu()
    {
        if (string.IsNullOrEmpty(mainMenuSceneName))
        {
            Debug.LogError("ReturnButton: mainMenuSceneName belum diisi!");
            return;
        }

        // Simpan enum ke memory
        MenuMemory.TargetPanelOnLoad = targetPanel;

        // Load MainMenu
        SceneManager.LoadScene(mainMenuSceneName);
    }
}