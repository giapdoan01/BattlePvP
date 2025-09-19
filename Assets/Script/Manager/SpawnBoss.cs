using System.Collections;
using UnityEngine;
using Unity.Cinemachine;

public class RaditzBossTrigger : MonoBehaviour
{
    [Header("Boss Settings")]
    public GameObject raditzBoss;
    public Animator raditzAnimator;
    public float introAnimationDuration = 2.4f;
    public LoadPrefabChracter loadPrefabCharacter;
    
    [Header("Animation Parameters")]
    [Tooltip("Tên của boolean parameter trong Animator để kích hoạt animation intro")]
    public string openingAnimationParam = "IsOpening";
    
    [Header("Camera Settings")]
    public CinemachineCamera virtualCamera;
    public Transform bossFocusPoint;
    
    [Header("Audio")]
    public AudioClip bossRaditzMusic;
    private AudioSource audioRaditzSource;
    
    // Lưu trữ thông tin camera ban đầu
    private Transform originalCameraFollow;
    private Transform originalCameraLookAt;
    private BossRaditzAI raditzBossAI;
    
    
    void Awake()
    {
        raditzBossAI = raditzBoss.GetComponent<BossRaditzAI>();
        // Thêm AudioSource nếu chưa có
        audioRaditzSource = GetComponent<AudioSource>();
        if (audioRaditzSource == null)
        {
            audioRaditzSource = gameObject.AddComponent<AudioSource>();
        }
        
        // Ẩn boss ban đầu
        if (raditzBoss != null)
        {
            raditzBoss.SetActive(false);
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        // Kiểm tra nếu người chơi chạm vào trigger
        if (other.CompareTag("Player"))
        {
            // Kích hoạt trận đấu với boss
            StartCoroutine(ActivateBossBattle());
            
            
        }
    }

    //Coroutine Intro Boss Raditz và bắt đầu trận đấu
    IEnumerator ActivateBossBattle()
    {
        // Hiện boss
        if (raditzBoss != null)
        {
            raditzBoss.SetActive(true);
        }

        bossFocusPoint.localScale = new Vector3(-1, 1, 1);

        //Khóa di chuyển của người chơi trong thời gian hoạt ảnh intro
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
        
        raditzBossAI.detectionRange = 3f;
        // Lưu trữ thông tin camera ban đầu
        
        // Kích hoạt animation intro bằng cách đặt boolean parameter
        if (raditzAnimator != null)
        {
            raditzAnimator.SetBool(openingAnimationParam, true);
            Debug.Log("Đang chạy hoạt ảnh intro của boss Raditz");
        }
        yield return new WaitForSeconds(0.7f);
        if (virtualCamera != null)
        {

            // Chuyển camera focus sang boss
            if (bossFocusPoint != null)
            {
                virtualCamera.Follow = bossFocusPoint; 
                virtualCamera.LookAt = bossFocusPoint;
            }
            else if (raditzBoss != null)
            {
                virtualCamera.Follow = raditzBoss.transform;
                virtualCamera.LookAt = raditzBoss.transform;
            }
        }

        // Chờ hoạt ảnh intro kết thúc
        yield return new WaitForSeconds(introAnimationDuration);

        // Mở rộng phạm vi phát hiện của boss
        raditzBossAI.detectionRange = 30f;

        bossFocusPoint.localScale = new Vector3(1, 1, 1);
        // Tắt animation intro
        if (raditzAnimator != null)
        {
            raditzAnimator.SetBool(openingAnimationParam, false);
        }

        // Chuyển camera focus về người chơi
        if (virtualCamera != null)
        {
            if (loadPrefabCharacter != null && LoadPrefabChracter.SpawnedCharacter != null)
            {
                virtualCamera.Follow = LoadPrefabChracter.SpawnedCharacter.transform;
                virtualCamera.LookAt = LoadPrefabChracter.SpawnedCharacter.transform;
            }
            else
            {
                Debug.LogWarning("LoadPrefabChracter or SpawnedCharacter is null. Cannot reset camera focus to player.");
            }
        }

        // Phát nhạc boss
        if (bossRaditzMusic != null && audioRaditzSource != null)
        {
            audioRaditzSource.clip = bossRaditzMusic;
            audioRaditzSource.loop = true;
            audioRaditzSource.Play();
        }
        // Mở khóa di chuyển của người chơi
        if (loadPrefabCharacter != null && LoadPrefabChracter.SpawnedCharacter != null)
        {
            PlayerController playerController = LoadPrefabChracter.SpawnedCharacter.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.playerSpeed = 10f;
            }
            else
            {
                Debug.LogWarning("PlayerController component not found on SpawnedCharacter.");
            }
        }
        else
        {
            Debug.LogWarning("LoadPrefabChracter or SpawnedCharacter is null. Cannot enable player movement.");
        }

        Debug.Log("Trận đấu với boss Raditz đã bắt đầu!");
        Destroy(gameObject); // Xóa trigger sau khi kích hoạt
    }
}
