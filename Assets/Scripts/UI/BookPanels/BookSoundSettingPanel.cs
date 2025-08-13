using System;
using ToB.Core;
using UnityEngine;
using UnityEngine.UI;

namespace ToB.UI.BookPanels
{
    public class BookSoundSettingPanel : MonoBehaviour
    {
        [SerializeField] private Slider masterSlider;
        [SerializeField] private Slider bgmSlider;
        [SerializeField] private Slider sfxSlider;

        private void OnEnable()
        {
            masterSlider.value = AudioManager.MasterVolume / 100;
            bgmSlider.value = AudioManager.BackgroundVolume / 100;
            sfxSlider.value = AudioManager.EffectVolume / 100;
        }

        public void OnMasterVolumeChanged(float value)
        {
            AudioManager.MasterVolume = value * 100;
        }

        public void OnBGMVolumeChanged(float value)
        {
            AudioManager.BackgroundVolume = value * 100;
        }

        public void OnSFXVolumeChanged(float value)
        {
            AudioManager.EffectVolume = value * 100;
        }
    }
}
