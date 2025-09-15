using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class VolumeSetting : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider soundSlider;
    [SerializeField] private Slider SFXSlider;

    void Start()
    {
        soundSlider.value = PlayerPrefs.GetFloat("SoundVolume", 1f);
        SFXSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
    }
    public void SetSoundVolume()
    {
        float volume = soundSlider.value;
        audioMixer.SetFloat("SoundVolume", Mathf.Log10(volume) * 20);
    }
    public void SetSFXVolume()
    {
        float volume = SFXSlider.value;
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
    }
}
