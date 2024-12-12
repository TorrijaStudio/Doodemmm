using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Opciones
{
    public class PersistenSoundInfo : MonoBehaviour
    {
        public static float musicVolume = 1;

        public void SetVolume(float volume)
        {
            musicVolume = Mathf.Clamp(volume, 0, 100);
            SetSfxVolume();
        }
        public static PersistenSoundInfo instance;

        private readonly List<AudioConfig> _sfxAudioSources = new List<AudioConfig>();
        // private List<AudioConfig> _musicAudioSources = new List<AudioConfig>();
        
        public void Start()
        {
            if (!instance) instance = this;
            else Destroy(this);
            
            InitializeAudioSources();
        }
        
        private void InitializeAudioSources()
        {
            var audioSources = FindObjectsByType<AudioSource>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            // List<AudioSource> sfx = audioSources.Where(a =>
            //     (!a.CompareTag("MainMusic"))).ToList();
            // List<AudioSource> music = audioSources.Where(a =>
            //     (a.CompareTag("MainMusic"))).ToList();

            foreach(var s in audioSources)
            {
                _sfxAudioSources.Add(new AudioConfig(s, s.volume*2));
            }

            // foreach (AudioSource m in music)
            // {
            //     _musicAudiosources.Add(new AudioConfig(m, m.volume*2));
            // }
            SetSfxVolume();
            // SetMusicVolume();
        }
        private void SetSfxVolume()
        {
            Debug.Log("Setting volume to " + (musicVolume));
            foreach (AudioConfig source in _sfxAudioSources)
            {
                source.source.volume = source.ogVolume * (musicVolume);
            }
        }

        private struct AudioConfig
        {
            public readonly AudioSource source;
            public readonly float ogVolume;

            public AudioConfig(AudioSource source, float ogVolume)
            {
                this.source = source;
                this.ogVolume = ogVolume;
            }
        }

    }
}