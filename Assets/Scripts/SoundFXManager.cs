using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Simple Sound FX Manager: play satu-shot SFX dari AudioClip.
/// Gunakan Unity AudioSource untuk output.
/// </summary>
public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager Instance { get; private set; }

    [Header("Audio Clip SFX")]
    public AudioClip clickClip;        // suara klik tombol
    public AudioClip correctClip;      // suara jawaban benar
    public AudioClip wrongClip;        // suara jawaban salah
    public AudioClip popupClip;        // suara popup muncul
    public AudioClip achievementClip;  // suara badge unlock
    public AudioClip bgMusicClip;      // musik latar (opsional)

    [Header("Volume")]
    [Range(0f, 1f)]
    public float sfxVolume = 0.5f;
    [Range(0f, 1f)]
    public float musicVolume = 0.3f;

    private AudioSource sfxSource;
    private AudioSource musicSource;
    private bool musicEnabled = true;
    private float currentMusicVolume = 0.3f;
    private float currentSfxVolume = 0.5f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Buat AudioSource untuk SFX (playOneShot)
            GameObject sfxGO = new GameObject("SFXSource");
            sfxGO.transform.SetParent(transform);
            sfxSource = sfxGO.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
            sfxSource.spatialBlend = 0f;

            // Buat AudioSource untuk musik (loop)
            GameObject musicGO = new GameObject("MusicSource");
            musicGO.transform.SetParent(transform);
            musicSource = musicGO.AddComponent<AudioSource>();
            musicSource.playOnAwake = false;
            musicSource.loop = true;
            musicSource.spatialBlend = 0f;

            currentSfxVolume = sfxVolume;
            currentMusicVolume = musicVolume;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Jika ada bgMusicClip, mulai putar
        if (bgMusicClip != null)
        {
            PlayMusic();
        }
    }

    /// <summary>Play SFX satu-shot.</summary>
    public void PlaySFX(AudioClip clip)
    {
        if (clip == null || sfxSource == null) return;
        sfxSource.volume = currentSfxVolume;
        sfxSource.PlayOneShot(clip);
    }

    /// <summary>Play klik tombol.</summary>
    public void PlayClick()
    {
        PlaySFX(clickClip);
    }

    /// <summary>Play suara jawaban benar.</summary>
    public void PlayCorrect()
    {
        PlaySFX(correctClip);
    }

    /// <summary>Play suara jawaban salah.</summary>
    public void PlayWrong()
    {
        PlaySFX(wrongClip);
    }

    /// <summary>Play suara popup.</summary>
    public void PlayPopup()
    {
        PlaySFX(popupClip);
    }

    /// <summary>Play suara achievement.</summary>
    public void PlayAchievement()
    {
        PlaySFX(achievementClip);
    }

    /// <summary>Mulai putar musik latar.</summary>
    public void PlayMusic()
    {
        if (bgMusicClip == null || musicSource == null) return;
        musicSource.clip = bgMusicClip;
        musicSource.volume = currentMusicVolume;
        musicSource.Play();
        musicEnabled = true;
    }

    /// <summary>Stop musik.</summary>
    public void StopMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
            musicEnabled = false;
        }
    }

    /// <summary>Pause/Resume musik.</summary>
    public void ToggleMusic()
    {
        if (musicSource == null) return;
        if (musicEnabled)
        {
            musicSource.Pause();
            musicEnabled = false;
        }
        else
        {
            musicSource.UnPause();
            musicEnabled = true;
        }
    }

    /// <summary>Set volume musik.</summary>
    public void SetMusicVolume(float volume)
    {
        currentMusicVolume = Mathf.Clamp01(volume);
        if (musicSource != null)
            musicSource.volume = currentMusicVolume;
    }

    /// <summary>Set volume SFX.</summary>
    public void SetSFXVolume(float volume)
    {
        currentSfxVolume = Mathf.Clamp01(volume);
    }

    /// <summary>Get current music volume.</summary>
    public float GetMusicVolume() => currentMusicVolume;

    /// <summary>Get current SFX volume.</summary>
    public float GetSFXVolume() => currentSfxVolume;

    /// <summary>Cek apakah musik sedang diputar.</summary>
    public bool IsMusicPlaying()
    {
        return musicEnabled && musicSource != null && musicSource.isPlaying;
    }

    /// <summary>Simpan preferensi volume ke PlayerPrefs.</summary>
    public void SaveVolumePrefs()
    {
        {

            PlayerPrefs.SetFloat("sfx_volume", currentSfxVolume);
            PlayerPrefs.SetFloat("music_volume", currentMusicVolume);
            PlayerPrefs.SetInt("music_enabled", musicEnabled ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    /// <summary>Load preferensi volume dari PlayerPrefs.</summary>
    public void LoadVolumePrefs()
    {
        currentSfxVolume = PlayerPrefs.GetFloat("sfx_volume", 0.5f);
        currentMusicVolume = PlayerPrefs.GetFloat("music_volume", 0.3f);
        musicEnabled = PlayerPrefs.GetInt("music_enabled", 1) == 1;

        if (sfxSource != null) sfxSource.volume = currentSfxVolume;
        if (musicSource != null)
        {
            musicSource.volume = currentMusicVolume;
            if (musicEnabled && bgMusicClip != null && !musicSource.isPlaying)
                musicSource.Play();
        }
    }
}