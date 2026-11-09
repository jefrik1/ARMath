using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Spawns answer choice objects around the player with non-overlapping positions.
/// Mendukung object pooling untuk mengurangi GC allocation.
/// </summary>
public class ItemSpawner : MonoBehaviour
{
    [Header("Pengaturan Objek")]
    public GameObject prefabPilihan;
    public float minSpawnRadius = 1.5f;
    public float maxSpawnRadius = 5f;
    public float minObjectDistance = 0.75f;

    [Header("Pooling (opsional)")]
    [Tooltip("Jika > 0, gunakan pooling. Jika 0, instantiate/destroy biasa.")]
    public int maxPoolSize = 12;

    [Header("Referensi")]
    public Transform playerTransform;

    // Pool storage
    private Queue<GameObject> pool = new Queue<GameObject>();
    private bool usePool = false;

    private void Start()
    {
        usePool = maxPoolSize > 0;
        if (usePool)
        {
            for (int i = 0; i < maxPoolSize; i++)
            {
                GameObject obj = Instantiate(prefabPilihan, transform.position, Quaternion.identity, transform);
                obj.SetActive(false);
                Collider col = obj.GetComponent<Collider>();
                if (col != null) col.enabled = false;
                pool.Enqueue(obj);
            }
        }
    }

    // --- int (angka) ---
    public List<GameObject> SpawnPilihan(List<int> daftarNilai, string[] daftarHuruf, IQuizManager manager)
    {
        List<GameObject> spawnedObjects = new List<GameObject>();
        List<Vector3> spawnedPositions = new List<Vector3>();
        int count = Mathf.Min(daftarNilai.Count, daftarHuruf.Length);

        for (int i = 0; i < count; i++)
        {
            Vector3 spawnPosition = GetNonOverlappingPosition(spawnedPositions);
            spawnedPositions.Add(spawnPosition);

            GameObject instance = GetFromPoolOrInstantiate(spawnPosition);
            ObjekA scriptPilihan = instance.GetComponent<ObjekA>();
            if (scriptPilihan != null)
                scriptPilihan.Setup(daftarHuruf[i], daftarNilai[i], manager);
            spawnedObjects.Add(instance);
        }
        return spawnedObjects;
    }

    // --- string (pecahan) ---
    public List<GameObject> SpawnPilihan(List<string> daftarNilai, string[] daftarHuruf, IQuizManager manager)
    {
        List<GameObject> spawnedObjects = new List<GameObject>();
        List<Vector3> spawnedPositions = new List<Vector3>();
        int count = Mathf.Min(daftarNilai.Count, daftarHuruf.Length);

        for (int i = 0; i < count; i++)
        {
            Vector3 spawnPosition = GetNonOverlappingPosition(spawnedPositions);
            spawnedPositions.Add(spawnPosition);

            GameObject instance = GetFromPoolOrInstantiate(spawnPosition);
            ObjekA scriptPilihan = instance.GetComponent<ObjekA>();
            if (scriptPilihan != null)
                scriptPilihan.Setup(daftarHuruf[i], daftarNilai[i], manager);
            spawnedObjects.Add(instance);
        }
        return spawnedObjects;
    }

    // --- float (desimal / bangun ruang) ---
    public List<GameObject> SpawnPilihan(List<float> daftarNilai, string[] daftarHuruf, IQuizManager manager)
    {
        List<GameObject> spawnedObjects = new List<GameObject>();
        List<Vector3> spawnedPositions = new List<Vector3>();
        int count = Mathf.Min(daftarNilai.Count, daftarHuruf.Length);

        for (int i = 0; i < count; i++)
        {
            Vector3 spawnPosition = GetNonOverlappingPosition(spawnedPositions);
            spawnedPositions.Add(spawnPosition);

            GameObject instance = GetFromPoolOrInstantiate(spawnPosition);
            ObjekA scriptPilihan = instance.GetComponent<ObjekA>();
            if (scriptPilihan != null)
                scriptPilihan.Setup(daftarHuruf[i], daftarNilai[i], manager);
            spawnedObjects.Add(instance);
        }
        return spawnedObjects;
    }

    // --- string[] (dari BaseQuizManager.SpawnPilihan) ---
    public List<GameObject> SpawnPilihan(string[] daftarNilai, string[] daftarHuruf, IQuizManager manager)
    {
        List<GameObject> spawnedObjects = new List<GameObject>();
        List<Vector3> spawnedPositions = new List<Vector3>();
        int count = Mathf.Min(daftarNilai.Length, daftarHuruf.Length);

        for (int i = 0; i < count; i++)
        {
            Vector3 spawnPosition = GetNonOverlappingPosition(spawnedPositions);
            spawnedPositions.Add(spawnPosition);

            GameObject instance = GetFromPoolOrInstantiate(spawnPosition);
            ObjekA scriptPilihan = instance.GetComponent<ObjekA>();
            if (scriptPilihan != null)
                scriptPilihan.Setup(daftarHuruf[i], daftarNilai[i], manager);
            spawnedObjects.Add(instance);
        }
        return spawnedObjects;
    }

    // --- return objek ke pool atau destroy ===
    public void ReturnToPool(GameObject obj)
    {
        if (obj == null) return;

        if (usePool && pool.Count < maxPoolSize)
        {
            obj.SetActive(false);
            Collider col = obj.GetComponent<Collider>();
            if (col != null) col.enabled = false;

            ObjekA script = obj.GetComponent<ObjekA>();
            if (script != null)
            {
                script.pilihanHuruf = null;
                script.quizManager = null;
            }
            pool.Enqueue(obj);
        }
        else
        {
            Destroy(obj);
        }
    }

    private GameObject GetFromPoolOrInstantiate(Vector3 position)
    {
        GameObject obj;

        if (usePool && pool.Count > 0)
        {
            obj = pool.Dequeue();
            obj.transform.position = position;
            obj.SetActive(true);
            Collider col = obj.GetComponent<Collider>();
            if (col != null) col.enabled = true;
        }
        else
        {
            obj = Instantiate(prefabPilihan, position, Quaternion.identity);
        }

        return obj;
    }

    // --- helper: posisi acak yang tidak tumpang tindih ---
    private Vector3 GetNonOverlappingPosition(List<Vector3> existingPositions)
    {
        int maxAttempts = 20;
        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            Vector2 randomDirection = Random.insideUnitCircle.normalized;
            float randomDistance = Random.Range(minSpawnRadius, maxSpawnRadius);
            Vector3 offset = new Vector3(randomDirection.x, 0, randomDirection.y) * randomDistance;

            Vector3 newPosition = playerTransform.position + offset;
            newPosition.y = 0f;

            bool positionIsValid = true;
            foreach (Vector3 pos in existingPositions)
            {
                if (Vector3.Distance(newPosition, pos) < minObjectDistance)
                {
                    positionIsValid = false;
                    break;
                }
            }
            if (positionIsValid)
                return newPosition;
        }

        Debug.LogWarning("Gagal menemukan posisi ideal, objek mungkin berdekatan.");
        Vector2 finalDirection = Random.insideUnitCircle.normalized;
        float finalDistance = Random.Range(minSpawnRadius, maxSpawnRadius);
        Vector3 finalOffset = new Vector3(finalDirection.x, 0, finalDirection.y) * finalDistance;
        Vector3 finalPosition = playerTransform.position + finalOffset;
        finalPosition.y = 0f;
        return finalPosition;
    }
}