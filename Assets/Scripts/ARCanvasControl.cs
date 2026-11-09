using UnityEngine;
using Vuforia;

public class ARCanvasControl : MonoBehaviour
{
    [Tooltip("Drag objek Canvas kamu ke sini")]
    public GameObject canvasMateri;

    private ObserverBehaviour mObserverBehaviour;

    void Start()
    {
        // Cari komponen ObserverBehaviour di objek  (Image Target)
        mObserverBehaviour = GetComponent<ObserverBehaviour>();

        if (mObserverBehaviour)
        {
            // Daftarkan fungsi  ke event Vuforia
            mObserverBehaviour.OnTargetStatusChanged += OnTargetStatusChanged;
        }
    }

    // Fungsi ini otomatis dipanggil Vuforia saat status marker berubah
    private void OnTargetStatusChanged(ObserverBehaviour behaviour, TargetStatus targetStatus)
    {
        bool tracked = targetStatus.Status == Status.TRACKED || targetStatus.Status == Status.EXTENDED_TRACKED;
        if (canvasMateri != null)
        {
            canvasMateri.SetActive(tracked);
        }
    }

    // Pembersihan event saat script hancur (opsional tapi bagus untuk performa)
    void OnDestroy()
    {
        if (mObserverBehaviour)
        {
            mObserverBehaviour.OnTargetStatusChanged -= OnTargetStatusChanged;
        }
    }
}