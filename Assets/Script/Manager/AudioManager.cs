using UnityEngine;
using System.Collections;

public class BackgroundMusicManager : MonoBehaviour
{
    public static BackgroundMusicManager Instance;
    
    [Header("Background Music")]
    [SerializeField] private AudioClip[] backgroundMusics;
    [SerializeField] private float volume = 0.5f;
    
    private AudioSource audioSource;
    private int currentMusicIndex = 0;
    
    void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SetupAudioSource();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void SetupAudioSource()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.volume = volume;
        
        // Tự động phát nhạc đầu tiên
        if (backgroundMusics.Length > 0)
        {
            PlayMusic(0);
        }
    }
    
    public void PlayMusic(int index)
    {
        if (index >= 0 && index < backgroundMusics.Length)
        {
            currentMusicIndex = index;
            audioSource.clip = backgroundMusics[index];
            audioSource.Play();
        }
    }
    
    public void StopMusic()
    {
        audioSource.Stop();
    }
    
    public void PauseMusic()
    {
        audioSource.Pause();
    }
    
    public void ResumeMusic()
    {
        audioSource.UnPause();
    }
    
    public void SetVolume(float newVolume)
    {
        volume = Mathf.Clamp01(newVolume);
        audioSource.volume = volume;
    }
}
