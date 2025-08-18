using Unity.Cinemachine;
using UnityEngine;

public class LoadPrefabChracter : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private CinemachineCamera virtualCamera;
    public GameObject[] characterPrefabs;

    // ✅ THÊM STATIC REFERENCE
    public static GameObject SpawnedCharacter { get; private set; }

    void Start()
    {
        if (spawnPoint == null || virtualCamera == null)
        {
            Debug.LogError("SpawnPoint or Camera is not allocated in Inspector!");
            return;
        }

        int selectedIndex = PlayerPrefs.GetInt("SelectCharacter", 0);
        GameObject spawnCharacter = Instantiate(characterPrefabs[selectedIndex], spawnPoint.localPosition, Quaternion.identity);

        // ✅ LUU REFERENCE
        SpawnedCharacter = spawnCharacter;

        virtualCamera.Follow = spawnCharacter.transform;
        virtualCamera.LookAt = spawnCharacter.transform;

        // ✅ THÔNG BÁO CHO CÁC SCRIPT KHÁC
        Debug.Log($"🎮 Character spawned: {spawnCharacter.name}");
    }
}
