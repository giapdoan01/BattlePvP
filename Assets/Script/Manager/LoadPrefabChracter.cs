using Unity.Cinemachine;
using UnityEngine;

public class LoadPrefabChracter : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private CinemachineCamera virtualCamera;
    public GameObject[] characterPrefabs;

    void Start()
    {
        if (spawnPoint == null || virtualCamera == null)
        {
            Debug.LogError("SpawnPoint hoặc Camera chưa được gán trong Inspector!");
            return;
        }

        spawnPoint.position = new Vector3(-109f, -13f, 0f);
        int selectedIndex = PlayerPrefs.GetInt("SelectCharacter", 0);
        GameObject spawnCharacter = Instantiate(characterPrefabs[selectedIndex], spawnPoint.position, Quaternion.identity);

        virtualCamera.Follow = spawnCharacter.transform;
        virtualCamera.LookAt = spawnCharacter.transform;

    }
}
