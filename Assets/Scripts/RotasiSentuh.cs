using UnityEngine;

public class RotasiSentuh : MonoBehaviour
{
    [Header("Kecepatan Putar")]
    public float kecepatanPutar = 100f;

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            float rotasiX = Input.GetAxis("Mouse X") * kecepatanPutar * Time.deltaTime;
            float rotasiY = Input.GetAxis("Mouse Y") * kecepatanPutar * Time.deltaTime;

            transform.Rotate(Vector3.up, -rotasiX, Space.Self);

            transform.Rotate(Vector3.right, rotasiY, Space.Self);
        }
    }
}