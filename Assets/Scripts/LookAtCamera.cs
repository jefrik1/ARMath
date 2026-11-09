using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        // Cari kamera utama di scene (biasanya AR Camera)
        mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        if (mainCamera != null)
        {
            // Membuat objek ini menghadap ke arah kamera
            /*
             * Trik: transform.LookAt membuat objek melihat ke target.
             * Jika kita menyuruhnya melihat kamera, teks akan terbalik (karena melihat 'ke dalam' kamera).
             * Jadi, kita menyuruhnya melihat ke arah yang berlawanan dari kamera.
             */
            transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
                             mainCamera.transform.rotation * Vector3.up);
        }
    }
}