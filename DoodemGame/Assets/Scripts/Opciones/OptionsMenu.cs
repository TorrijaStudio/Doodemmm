using System;
using UnityEngine;
using UnityEngine.UI;
using Slider = UnityEngine.UI.Slider;

namespace Opciones
{
    public class OptionsMenu : MonoBehaviour
    {
        [SerializeField] private Slider soundSlider;
        [SerializeField] private Scrollbar soundScrollbar;

        public void ChangeSlider(float value)
        {
            PersistenSoundInfo.instance.SetVolume(value);
        }

        private void Start()
        {
            if(soundSlider)
                soundSlider.value = PersistenSoundInfo.musicVolume;
            if(soundScrollbar)
                soundScrollbar.value = PersistenSoundInfo.musicVolume;
        }
    }
}