using UnityEngine;
using System.Collections;

public class GohanSkillL : MonoBehaviour, ISkill
{
    [Header("Skill L Settings")]
    public float buffDuration = 30f;

    [Header("New Cooldowns While Buff Active")]
    public float overrideCooldownJ = 1f;
    public float overrideCooldownK = 1f;

    [Header("Aura")]
    public GameObject auraPrefab;           // Prefab hào quang
    public Vector3 auraOffset = Vector3.zero;

    private GameObject owner;
    private bool facingRight;
    private SkillInputHandler inputHandle;

    private float originalCooldownJ;
    private float originalCooldownK;

    private GameObject auraInstance;
    private bool buffApplied = false;

    // Ngăn stack: dùng flag static theo owner (cũng có thể dùng tag / component)
    private static readonly string ACTIVE_FLAG_NAME = "_SkillL_ActiveFlag";

    public void Initialize(GameObject skillOwner, bool ownerFacingRight)
    {
        owner = skillOwner;
        facingRight = ownerFacingRight;

        // Kiểm tra đã có SkillL đang chạy?
        if (owner.transform.Find(ACTIVE_FLAG_NAME) != null)
        {
            // Nếu muốn REFRESH thời gian thì có thể gọi Refresh() ở đây thay vì return.
            Destroy(gameObject);
            return;
        }

        inputHandle = owner.GetComponent<SkillInputHandler>();
        if (inputHandle == null)
        {
            Debug.LogWarning("SkillInputHandle not found on owner -> cancel SkillL");
            Destroy(gameObject);
            return;
        }

        ApplyBuff();
    }

    private void ApplyBuff()
    {
        if (buffApplied) return;

        // Tạo flag Object để đánh dấu đang có buff (đơn giản, không hiển thị)
        var flag = new GameObject(ACTIVE_FLAG_NAME);
        flag.transform.SetParent(owner.transform, false);

        // Lưu cooldown gốc
        originalCooldownJ = inputHandle.cooldownJ;
        originalCooldownK = inputHandle.cooldownK;

        // Ghi đè
        inputHandle.cooldownJ = overrideCooldownJ;
        inputHandle.cooldownK = overrideCooldownK;

        // Tạo Aura
        if (auraPrefab != null)
        {
            auraInstance = Instantiate(auraPrefab, owner.transform);
            auraInstance.transform.localPosition = auraOffset;

            // Chỉnh sorting để đè lên nhân vật
            var ownerSr = owner.GetComponentInChildren<SpriteRenderer>();
            var auraSr = auraInstance.GetComponentInChildren<SpriteRenderer>();
            if (ownerSr != null && auraSr != null)
            {
                auraSr.sortingLayerID = ownerSr.sortingLayerID;
                auraSr.sortingOrder = ownerSr.sortingOrder -1;
            }
        }

        buffApplied = true;
        StartCoroutine(BuffLife());
    }

    private IEnumerator BuffLife()
    {
        yield return new WaitForSeconds(buffDuration);
        RemoveBuff();
        Destroy(gameObject);
    }

    private void RemoveBuff()
    {
        if (!buffApplied) return;

        // Trả cooldown cũ
        if (inputHandle != null)
        {
            inputHandle.cooldownJ = originalCooldownJ;
            inputHandle.cooldownK = originalCooldownK;
        }

        if (auraInstance != null)
        {
            Destroy(auraInstance);
            auraInstance = null;
        }

        // Xoá flag
        if (owner != null)
        {
            var flag = owner.transform.Find(ACTIVE_FLAG_NAME);
            if (flag != null) Destroy(flag.gameObject);
        }

        buffApplied = false;
    }


    private void OnDestroy()
    {
        // Nếu bị huỷ sớm vẫn trả trạng thái
        if (buffApplied)
        {
            RemoveBuff();
        }
    }

}
