using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject gameManagerObject = new GameObject("GameManager");
                _instance = gameManagerObject.AddComponent<GameManager>();
                DontDestroyOnLoad(gameManagerObject);
            }
            return _instance;
        }
    }

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    // Load scene mới hoàn toàn
    public void LoadSceneClean(string sceneName)
    {
        StartCoroutine(LoadSceneSequence(sceneName));
    }

    private IEnumerator LoadSceneSequence(string sceneName)
    {
        // reset state
        PauseGame.isPaused = false;
        Time.timeScale = 1f;
        PlayerPrefs.Save();

        // Nếu có ObjectPool cũ thì xoá luôn
        if (ObjectPool.Instance != null)
        {
            Destroy(ObjectPool.Instance.gameObject);
        }

        yield return null;

        // Load scene mới, xoá toàn bộ scene cũ
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
}
