using UnityEngine;
using System.Collections;
using Unity.Cinemachine;
using Unity.VisualScripting;

public class GuideTreeShootMonster : MonoBehaviour
{
    public GameObject guideText;
    public GameObject focusPoint;
    public LoadPrefabChracter loadPrefabCharacter;
    public CinemachineCamera virtualCamera;
    public Transform monsterFocusPoint;

    void Start()
    {
        guideText.SetActive(false);
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(StartGuid());
        }

    }
    IEnumerator StartGuid()
    {
        if (loadPrefabCharacter != null && LoadPrefabChracter.SpawnedCharacter != null)
        {
            PlayerController playerController = LoadPrefabChracter.SpawnedCharacter.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.playerSpeed = 0;
            }
            else
            {
                Debug.LogWarning("PlayerController component not found on SpawnedCharacter.");
            }
        }
        else
        {
            Debug.LogWarning("LoadPrefabChracter or SpawnedCharacter is null. Cannot disable player movement.");
        }

        guideText.SetActive(true);
        // Chuyển camera để tập trung vào quái vật
        virtualCamera.Follow = monsterFocusPoint;
        virtualCamera.LookAt = monsterFocusPoint;

        yield return new WaitForSeconds(3f);
        if (LoadPrefabChracter.SpawnedCharacter != null)
        {
            virtualCamera.Follow = LoadPrefabChracter.SpawnedCharacter.transform;
            virtualCamera.LookAt = LoadPrefabChracter.SpawnedCharacter.transform;
        }

        if (loadPrefabCharacter != null && LoadPrefabChracter.SpawnedCharacter != null)
        {
            PlayerController playerController = LoadPrefabChracter.SpawnedCharacter.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.playerSpeed = 10;
            }
            else
            {
                Debug.LogWarning("PlayerController component not found on SpawnedCharacter.");
            }
        }
        else
        {
            Debug.LogWarning("LoadPrefabChracter or SpawnedCharacter is null. Cannot disable player movement.");
        }

        guideText.SetActive(false);
        Destroy(guideText);
        focusPoint.SetActive(false);
        Destroy(focusPoint);
        Destroy(gameObject);
    }
}
