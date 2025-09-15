using UnityEngine;

public class PauseGame : MonoBehaviour
{
    public GameObject pauseMenuUI; // gán Panel PauseMenu vào
    public GameObject ESCimage; // gán hình ESC vào
    public GameObject ImageText; // gán hình Resume vào
    public static bool isPaused = false;

    void Start()
    {
        pauseMenuUI.SetActive(false); // ẩn menu pause lúc bắt đầu
    }

    void Update()
    {
        // Nhấn ESC để bật/tắt pause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                Pause();
        }
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        ESCimage.SetActive(true); 
        ImageText.SetActive(true);
        Time.timeScale = 1f;   // game chạy lại
        isPaused = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        
        ESCimage.SetActive(false); 
        ImageText.SetActive(false);
        Time.timeScale = 0f;   // dừng game
        isPaused = true;
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}
