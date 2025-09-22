using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class VolumeSetting : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider soundSlider;
    [SerializeField] private Slider SFXSlider;
    [SerializeField] private Button backToMenuButton;

    void Start()
    {
        soundSlider.value = PlayerPrefs.GetFloat("SoundVolume", 1f);
        SFXSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);

        backToMenuButton.onClick.AddListener(BackToMenu);
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
    void BackToMenu()
    {
        // Đảm bảo GameManager được tạo và sử dụng nó để load scene sạch
        GameManager.Instance.LoadSceneClean("MainGame");
    }

}
