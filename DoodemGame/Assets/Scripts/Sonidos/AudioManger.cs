using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum SoundType
{
    CADENAS,
    CAIDA_TOTEM,
    CLICK,
    HOJAS,
    MONEDA
}
public enum MusicType
{
    MENU,
    TIENDA,
    BATLLA,
    PAJAROS
}

public enum AmbienceType
{
    PAJAROS
}


[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] soundList;
    [SerializeField] private AudioClip[] musicList;
    [SerializeField] private AudioClip[] ambienceList;
    private static AudioManager instance;
    public AudioSource audioSource;
    public AudioSource musicSource;
    public AudioSource ambienceSource;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Mantener entre escenas
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        //musicSource.clip = background;
        //musicSource.Play();
        //audioSource = GetComponent<AudioSource>();

        
    }

    public static void PlaySound(SoundType sound, float volume = 1.0f)
    {
        instance.audioSource.PlayOneShot(instance.soundList[(int)sound], volume);
        Debug.Log("Hojas");
    }

    #region MUSICA
    public static void PlayMusic(MusicType music, float transitionTime = 1.0f)
    {
        if (instance == null || instance.musicSource == null) return;

        instance.StartCoroutine(instance.TransitionToNewMusic(instance.musicList[(int)music], transitionTime));
    }

    public static void StopMusic(float fadeOutTime = 1.0f)
    {
        if (instance == null || instance.musicSource == null) return;

        instance.StartCoroutine(instance.FadeOutMusic(fadeOutTime));
    }

    private IEnumerator TransitionToNewMusic(AudioClip newMusic, float transitionTime)
    {
        // Si ya hay música sonando, realiza un fade-out
        if (musicSource.isPlaying)
        {
            yield return StartCoroutine(FadeOutMusic(transitionTime));
        }

        // Cambiar la música y hacer fade-in
        musicSource.clip = newMusic;
        musicSource.Play();
        yield return StartCoroutine(FadeInMusic(transitionTime));
    }

    private IEnumerator FadeOutMusic(float duration)
    {
        float startVolume = musicSource.volume;

        while (musicSource.volume > 0)
        {
            musicSource.volume -= startVolume * Time.deltaTime / duration;
            yield return null;
        }

        musicSource.Stop();
        musicSource.volume = startVolume; // Restaurar volumen original para futuras músicas
    }

    private IEnumerator FadeInMusic(float duration)
    {
        float startVolume = 0.0f;
        musicSource.volume = startVolume;

        while (musicSource.volume < 0.5f) // Volumen máximo predeterminado
        {
            musicSource.volume += Time.deltaTime / duration;
            yield return null;
        }
    }
    #endregion

    #region AMBIENTE
    public static void PlayAmbience(AmbienceType ambience, float transitionTime = 1.0f)
    {
        if (instance == null || instance.ambienceSource == null) return;

        instance.StartCoroutine(instance.TransitionToNewAmbience(instance.ambienceList[(int)ambience], transitionTime));
    }

    public static void StopAmbience(float fadeOutTime = 1.0f)
    {
        if (instance == null || instance.ambienceSource == null) return;

        instance.StartCoroutine(instance.FadeOutAmbience(fadeOutTime));
    }

    private IEnumerator TransitionToNewAmbience(AudioClip newAmbience, float transitionTime)
    {
        // Si ya hay música sonando, realiza un fade-out
        if (ambienceSource.isPlaying)
        {
            yield return StartCoroutine(FadeOutAmbience(transitionTime));
        }

        // Cambiar la música y hacer fade-in
        ambienceSource.clip = newAmbience;
        ambienceSource.Play();
        yield return StartCoroutine(FadeInAmbience(transitionTime));
    }

    private IEnumerator FadeOutAmbience(float duration)
    {
        float startVolume = ambienceSource.volume;

        while (ambienceSource.volume > 0)
        {
            ambienceSource.volume -= startVolume * Time.deltaTime / duration;
            yield return null;
        }

        ambienceSource.Stop();
        ambienceSource.volume = startVolume; // Restaurar volumen original para futuras músicas
    }

    private IEnumerator FadeInAmbience(float duration)
    {
        float startVolume = 0.0f;
        ambienceSource.volume = startVolume;

        while (ambienceSource.volume < 0.5f) // Volumen máximo predeterminado
        {
            ambienceSource.volume += Time.deltaTime / duration;
            yield return null;
        }
    }
    #endregion
}
