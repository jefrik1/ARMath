using UnityEngine;
using System.Reflection;

/// <summary>
/// Generator audio prosedural C# untuk efek suara matematika di aplikasi Mathlfix.
/// Mengintegrasikan SoundFXManager jika tersedia, atau membuat sintesis gelombang audio runtime procedural
/// tanpa memerlukan file audio eksternal (.wav/.mp3), menjaga repositori tetap sangat kecil (< 100MB).
/// </summary>
public class MathSoundFX : MonoBehaviour
{
    public static MathSoundFX Instance { get; private set; }
    private AudioSource audioSource;
    private const float PI = 3.14159265f;

    private static MethodInfo createClipMethod;
    private static MethodInfo setDataClipMethod;
    private static bool reflectionInitialized = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // 2D Sound

        InitReflection();
    }

    private static void InitReflection()
    {
        if (reflectionInitialized) return;
        reflectionInitialized = true;
        createClipMethod = typeof(AudioClip).GetMethod("Create", new System.Type[] { typeof(string), typeof(int), typeof(int), typeof(int), typeof(bool) });
        setDataClipMethod = typeof(AudioClip).GetMethod("SetData", new System.Type[] { typeof(float[]), typeof(int) });
    }

    private static void EnsureInstance()
    {
        if (Instance == null)
        {
            GameObject go = new GameObject("MathSoundFX");
            Instance = go.AddComponent<MathSoundFX>();
        }
    }

    /// <summary>
    /// Efek nada akord mayor naik saat jawaban benar.
    /// </summary>
    public static void PlayCorrect()
    {
        if (SoundFXManager.Instance != null && SoundFXManager.Instance.correctClip != null)
        {
            SoundFXManager.Instance.PlayCorrect();
            return;
        }

        EnsureInstance();
        if (Instance != null)
        {
            Instance.PlayChime(new float[] { 523.25f, 659.25f, 783.99f }, 0.14f, 0.45f);
        }
    }

    /// <summary>
    /// Efek suara buzz lembut menurun saat jawaban salah.
    /// </summary>
    public static void PlayWrong()
    {
        if (SoundFXManager.Instance != null && SoundFXManager.Instance.wrongClip != null)
        {
            SoundFXManager.Instance.PlayWrong();
            return;
        }

        EnsureInstance();
        if (Instance != null)
        {
            Instance.PlayChime(new float[] { 220f, 180f, 140f }, 0.16f, 0.35f, true);
        }
    }

    /// <summary>
    /// Suara klik tombol responsif.
    /// </summary>
    public static void PlayClick()
    {
        if (SoundFXManager.Instance != null && SoundFXManager.Instance.clickClip != null)
        {
            SoundFXManager.Instance.PlayClick();
            return;
        }

        EnsureInstance();
        if (Instance != null)
        {
            Instance.PlayChime(new float[] { 880f }, 0.05f, 0.25f);
        }
    }

    /// <summary>
    /// Efek nada kemenangan/high score.
    /// </summary>
    public static void PlayHighScore()
    {
        if (SoundFXManager.Instance != null && SoundFXManager.Instance.achievementClip != null)
        {
            SoundFXManager.Instance.PlayAchievement();
            return;
        }

        EnsureInstance();
        if (Instance != null)
        {
            Instance.PlayChime(new float[] { 523.25f, 659.25f, 783.99f, 1046.50f }, 0.16f, 0.5f);
        }
    }

    private void PlayChime(float[] frequencies, float noteDuration, float volume, bool isSawtooth = false)
    {
        InitReflection();
        if (createClipMethod == null || setDataClipMethod == null || audioSource == null) return;

        int sampleRate = 44100;
        int noteSamples = (int)(sampleRate * noteDuration);
        int totalSamples = noteSamples * frequencies.Length;
        float[] samples = new float[totalSamples];

        for (int note = 0; note < frequencies.Length; note++)
        {
            float freq = frequencies[note];
            int startSample = note * noteSamples;

            for (int i = 0; i < noteSamples; i++)
            {
                float t = (float)i / sampleRate;
                float decay = 1f - ((float)i / noteSamples);
                float wave;
                if (isSawtooth)
                {
                    // Gelombang gergaji lembut
                    wave = (2f * (t * freq - Mathf.Floor(t * freq + 0.5f))) * decay;
                }
                else
                {
                    // Sinus murni dengan harmonik lembut
                    wave = (Mathf.Sin(2f * PI * freq * t) + 0.25f * Mathf.Sin(4f * PI * freq * t)) * decay;
                }
                samples[startSample + i] = wave * volume;
            }
        }

        try
        {
            object clipObj = createClipMethod.Invoke(null, new object[] { "SFX_Tone", totalSamples, 1, sampleRate, false });
            if (clipObj is AudioClip clip)
            {
                setDataClipMethod.Invoke(clip, new object[] { samples, 0 });
                audioSource.PlayOneShot(clip);
            }
        }
        catch
        {
            // Fallback graceful jika audio tidak tersedia di platform
        }
    }
}
