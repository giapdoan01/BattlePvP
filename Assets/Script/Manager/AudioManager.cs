using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource backgroundMusic;
    [SerializeField] private AudioSource SFXSound;

    [Header("Audio Clips")]
    public AudioClip backgroundMusicClip;
    public AudioClip selectSFX;
    public AudioClip startGameSFX;

    void Start()
    {
        backgroundMusic.clip = backgroundMusicClip;
        backgroundMusic.Play();
    }
    public void playAudioSFX(AudioClip clip)
    {
        SFXSound.PlayOneShot(clip);
    }
}
