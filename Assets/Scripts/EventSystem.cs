using UnityEngine;
using UnityEngine.EventSystems;

public class EventSystemChecker : MonoBehaviour
{
    void Awake()
    {
        EventSystem[] systems = FindObjectsOfType<EventSystem>();
        if (systems.Length > 1)
        {
            // Matikan event system ini biar tidak dobel
            Destroy(gameObject);
        }
    }
}