using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class CharacterSelector : MonoBehaviour
{
    public GameObject[] CharacterPrefabs;       // Prefab nhân vật tương ứng
    public Button[] ButtonCharacters;           // Các nút chọn nhân vật
    public Button Startgame;
    AudioManager audioManager;
    public GameObject particleSelectEffect; // Hiệu ứng hạt khi chọn nhân vật
    private int characterSelectedIndex = 0;
    void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }
    void Start()
    {
        // Gán sự kiện cho từng nút chọn nhân vật
        for (int i = 0; i < ButtonCharacters.Length; i++)
        {
            int index = i;
            ButtonCharacters[i].onClick.AddListener(() => SelectCharacter(index));
        }

        Startgame.onClick.AddListener(StartGame);
        SelectCharacter(0);
    }

    public void SelectCharacter(int index)
    {
        characterSelectedIndex = index;
        audioManager.playAudioSFX(audioManager.selectSFX);
        for (int i = 0; i < ButtonCharacters.Length; i++)
        {
            Transform selectedObj = ButtonCharacters[i].transform.Find("Selected");
            Transform unselectedObj = ButtonCharacters[i].transform.Find("Unselected");

            if (selectedObj != null && unselectedObj != null)
            {
                bool isSelected = (i == index);
                selectedObj.gameObject.SetActive(isSelected);
                unselectedObj.gameObject.SetActive(!isSelected);
            }
        }
    }

    public void StartGame()
    {
        audioManager.playAudioSFX(audioManager.startGameSFX);
        PlayerPrefs.SetInt("SelectCharacter", characterSelectedIndex);
        Debug.Log($"Đã chọn {characterSelectedIndex}");
        SceneManager.LoadScene("MainGameScene");
    }
}
