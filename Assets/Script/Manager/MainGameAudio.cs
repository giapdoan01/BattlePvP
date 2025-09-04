using UnityEngine;
public class MainMenuAudio : MonoBehaviour
{
    void Start()
    {
        // Phát nhạc main menu (index 0)
        BackgroundMusicManager.Instance?.PlayMusic(0);
    }

    public void StartGame()
    {
        // Chuyển scene và nhạc sẽ tự động tiếp tục
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }
}
