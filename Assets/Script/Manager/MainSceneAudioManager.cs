using UnityEngine;
using UnityEngine.Audio;

public class MainSceneAudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource backgroundMusic;
    [SerializeField] private AudioSource SFXSound;
    
    public AudioClip backgroundMusicClip;
    
    
    private void Start()
    {
        // Khởi tạo nhạc nền
        if (backgroundMusic != null && backgroundMusicClip != null)
        {
            backgroundMusic.clip = backgroundMusicClip;
            backgroundMusic.loop = true;
            backgroundMusic.Play();
        }
    }
    
    // Phát âm thanh hiệu ứng thông thường
    public void playAudioSFX(AudioClip clip)
    {
        if (SFXSound != null && clip != null)
        {
            SFXSound.PlayOneShot(clip);
        }
    }

}
