using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum SoundType
{
    CADENAS,
    CAIDA_TOTEM,
    CLICK,
    HOJAS,
    MAIN
}
public enum MusicType
{
    MENU,
    TIENDA,
    BATLLA,
}


[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] soundList;
    [SerializeField] private AudioClip[] musicList;
    private static AudioManager instance;
    public AudioSource audioSource;
    public AudioSource musicSource;

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
}
