using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIWinManager : MonoBehaviour
{
    [SerializeField] private GameObject BossRaditz;
    [SerializeField] private GameObject UIWinPanel;
    void Start()
    {
        UIWinPanel.SetActive(false);
    }
    void Update()
    {
        if (BossRaditz == null)
        {
            StartCoroutine(WinGame());
        }
    }
    IEnumerator WinGame()
    {
        UIWinPanel.SetActive(true);
        yield return new WaitForSeconds(5f);
        GameManager.Instance.LoadSceneClean("MainGame");
    }
}
