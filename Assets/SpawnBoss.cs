using System.Collections;
using UnityEngine;
using Unity.Cinemachine;

public class RaditzBossTrigger : MonoBehaviour
{
    [Header("Boss Settings")]
    public GameObject raditzBoss;
    public Animator raditzAnimator;
    public float introAnimationDuration = 3f;
    public LoadPrefabChracter loadPrefabCharacter;
    
    [Header("Animation Parameters")]
    [Tooltip("Tên của boolean parameter trong Animator để kích hoạt animation intro")]
    public string openingAnimationParam = "IsOpening";
    
    [Header("Camera Settings")]
    public CinemachineCamera virtualCamera;
    public Transform bossFocusPoint;
    
    [Header("Audio")]
    public AudioClip introSound;
    public AudioClip bossMusic;
    private AudioSource audioSource;
    
    // Lưu trữ thông tin camera ban đầu
    private Transform originalCameraFollow;
    private Transform originalCameraLookAt;
    private BossRaditzAI raditzBossAI;
    
    
    void Awake()
    {
        raditzBossAI = raditzBoss.GetComponent<BossRaditzAI>();
        // Thêm AudioSource nếu chưa có
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
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

    IEnumerator ActivateBossBattle()
    {
        // Hiện boss
        if (raditzBoss != null)
        {
            raditzBoss.SetActive(true);
        }

        bossFocusPoint.localScale = new Vector3(-1, 1, 1);
        
        raditzBossAI.detectionRange = 3f;
        // Lưu trữ thông tin camera ban đầu
        

        // Phát âm thanh intro nếu có
        if (introSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(introSound);
        }

        // Kích hoạt animation intro bằng cách đặt boolean parameter
        if (raditzAnimator != null)
        {
            raditzAnimator.SetBool(openingAnimationParam, true);
            Debug.Log("Đang chạy hoạt ảnh intro của boss Raditz");
        }
        if (virtualCamera != null)
        {

            // Chuyển camera focus sang boss
            if (bossFocusPoint != null)
            {
                virtualCamera.Follow = bossFocusPoint; // Dịch chuyển lên trên một chút để
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
        raditzBossAI.detectionRange = 15f;

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
        if (bossMusic != null && audioSource != null)
        {
            audioSource.clip = bossMusic;
            audioSource.loop = true;
            audioSource.Play();
        }


        Debug.Log("Trận đấu với boss Raditz đã bắt đầu!");
        gameObject.SetActive(false);
    }
}
