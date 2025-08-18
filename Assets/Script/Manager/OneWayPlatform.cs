using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class OneWayPlatform : MonoBehaviour
{
    [Header("🎛️ Platform Settings")]
    [SerializeField] private float disableTime = 0.5f;
    [Tooltip("Thời gian tắt collision khi player xuyên qua")]

    [Header("🎯 Detection Settings")]
    [SerializeField] private float horizontalRange = 1f;
    [Tooltip("Phạm vi phát hiện player theo chiều ngang")]

    [SerializeField] private float verticalOffset = 0.3f;
    [Tooltip("Khoảng cách để xác định player ở trên/dưới platform")]

    [Header("🎮 Input Settings")]
    [SerializeField] private KeyCode dropDownKey = KeyCode.S;
    [Tooltip("Phím để rớt xuống")]

    [Header("🐛 Debug Settings")]
    [SerializeField] private bool showDebugLogs = true;
    [SerializeField] private bool showDebugGizmos = true;
    [SerializeField] private bool showPlayerInfo = true;

    // Components
    private BoxCollider2D platformCollider;
    private SpriteRenderer spriteRenderer;
    private PlayerController player;

    // Visual feedback
    private Color originalColor;
    private bool isDisabled = false;

    void Start()
    {
        InitializeComponents();
        FindPlayer();
        SaveOriginalSettings();
    }

    void Update()
    {
        if (player == null || platformCollider == null) return;

        if (showPlayerInfo)
            ShowPlayerDebugInfo();

        // Chỉ kiểm tra khi platform đang bật
        if (platformCollider.enabled)
        {
            CheckJumpThroughFromBelow();  // Tính năng 1: Nhảy từ dưới lên
            CheckDropDownFromAbove();     // Tính năng 2: Ấn S để rớt xuống
        }
    }

    void InitializeComponents()
    {
        platformCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (platformCollider == null)
        {
            Debug.LogError("❌ OneWayPlatform cần BoxCollider2D component!");
            enabled = false;
            return;
        }
    }

    void FindPlayer()
    {
        // Kiểm tra character đã spawn chưa
        if (LoadPrefabChracter.SpawnedCharacter == null)
        {
            Debug.LogError("❌ Character chưa được spawn!");
            StartCoroutine(WaitForCharacterSpawn());
            return;
        }

        PlayerController foundPlayer = LoadPrefabChracter.SpawnedCharacter.GetComponent<PlayerController>();

        if (foundPlayer == null)
        {
            Debug.LogError($"❌ GameObject '{LoadPrefabChracter.SpawnedCharacter.name}' không có PlayerController!");
            enabled = false;
            return;
        }

        player = foundPlayer;
    }

    private System.Collections.IEnumerator WaitForCharacterSpawn()
    {
        float timeout = 5f;
        float elapsed = 0f;

        while (LoadPrefabChracter.SpawnedCharacter == null && elapsed < timeout)
        {
            yield return new WaitForEndOfFrame();
            elapsed += Time.unscaledDeltaTime;
        }

        if (LoadPrefabChracter.SpawnedCharacter != null)
        {
            FindPlayer(); // Retry
        }
        else
        {
            Debug.LogError("❌ Timeout - Character không được spawn sau 5 giây!");
            enabled = false;
        }
    }



    void SaveOriginalSettings()
    {
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
    }


    void CheckJumpThroughFromBelow()
    {
        // Kiểm tra điều kiện
        bool playerBelow = IsPlayerBelow();
        bool playerMovingUp = player.IsMovingUp();
        bool playerInRange = IsPlayerInHorizontalRange();

        if (playerBelow && playerMovingUp && playerInRange)
        {
            StartCoroutine(DisablePlatformTemporarily("JumpThrough"));
        }
    }


    void CheckDropDownFromAbove()
    {
        // Kiểm tra input
        bool pressingDrop = Input.GetKeyDown(dropDownKey) || player.IsPressingDown();

        if (pressingDrop && IsPlayerOnTop())
        {
            StartCoroutine(DisablePlatformTemporarily("DropDown"));
        }
    }


    bool IsPlayerBelow()
    {
        Vector3 playerPos = player.GetTransform().position;
        Vector3 platformPos = transform.position;

        bool below = playerPos.y < (platformPos.y - verticalOffset);

        return below;
    }

    bool IsPlayerOnTop()
    {
        Vector3 playerPos = player.GetTransform().position;
        Vector3 platformPos = transform.position;

        // Player ở trên platform
        bool onTop = playerPos.y > (platformPos.y + verticalOffset) &&
                     playerPos.y < (platformPos.y + 1.5f);

        // Player trong phạm vi ngang
        bool inRange = IsPlayerInHorizontalRange();

        // Player không đang bay lên cao
        bool notFlyingUp = !player.IsMovingUp() || player.GetVelocity().y < 2f;

        // Player đang trên mặt đất
        bool grounded = player.IsGrounded();

        bool result = onTop && inRange && notFlyingUp && grounded;

        return result;
    }


    bool IsPlayerInHorizontalRange()
    {
        Vector3 playerPos = player.GetTransform().position;
        Vector3 platformPos = transform.position;
        Bounds platformBounds = GetComponent<Renderer>().bounds;

        float distanceX = Mathf.Abs(playerPos.x - platformPos.x);
        float maxDistance = (platformBounds.size.x / 2) + horizontalRange;

        bool inRange = distanceX < maxDistance;

        return inRange;
    }



    IEnumerator DisablePlatformTemporarily(string reason)
    {
        if (isDisabled) yield break; // Tránh gọi nhiều lần

        isDisabled = true;

        // Tắt collider
        platformCollider.enabled = false;

        // Đổi màu để thấy rõ
        if (spriteRenderer != null)
            spriteRenderer.color = Color.gray;

        // Đợi thời gian tối thiểu
        yield return new WaitForSeconds(disableTime);

        // Đợi đến khi player không còn trong platform
        int waitCount = 0;
        while (IsPlayerInsidePlatform() && waitCount < 50) // Tránh vòng lặp vô hạn
        {
            yield return new WaitForSeconds(0.1f);
            waitCount++;
        }

        // Bật lại collider
        platformCollider.enabled = true;

        // Khôi phục màu
        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;

        isDisabled = false;

    }

    bool IsPlayerInsidePlatform()
    {
        if (player == null) return false;

        Bounds platformBounds = GetComponent<Renderer>().bounds;
        Bounds playerBounds = player.GetRenderer().bounds;

        bool intersects = platformBounds.Intersects(playerBounds);

        return intersects;
    }


    void ShowPlayerDebugInfo()
    {
        if (player == null) return;

        Vector2 vel = player.GetVelocity();
        bool grounded = player.IsGrounded();
        bool movingUp = player.IsMovingUp();
    }

    void OnDrawGizmosSelected()
    {
        if (!showDebugGizmos) return;

        Vector3 pos = transform.position;
        Bounds bounds = GetComponent<Renderer>().bounds;
        Vector3 size = bounds.size;

        // Platform chính (màu vàng)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(pos, size);

        // Vùng "trên platform" (màu xanh lá)
        Gizmos.color = Color.green;
        Vector3 topArea = new Vector3(size.x + horizontalRange * 2, 1f, 1f);
        Gizmos.DrawWireCube(pos + Vector3.up * (verticalOffset + 0.5f), topArea);

        // Vùng "dưới platform" (màu xanh dương)
        Gizmos.color = Color.blue;
        Vector3 bottomArea = new Vector3(size.x + horizontalRange * 2, 1f, 1f);
        Gizmos.DrawWireCube(pos + Vector3.down * (verticalOffset + 0.5f), bottomArea);

        // Hiển thị trạng thái
        if (Application.isPlaying && platformCollider != null)
        {
            Gizmos.color = platformCollider.enabled ? Color.white : Color.red;
            Gizmos.DrawWireCube(pos, size * 1.1f);
        }
    }
}