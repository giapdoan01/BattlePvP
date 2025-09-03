using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkillInputHandler : MonoBehaviour
{
    [Header("Cooldowns")]
    public float cooldownH = 1f;
    public float cooldownJ = 1f;
    public float cooldownK = 1f;
    public float cooldownL = 1f;
    public float cooldownEnemyAttack = 1f;

    [Header("Character UI")]
    public GameObject characterUIPrefab;

    private Dictionary<string, bool> canUseSkill = new Dictionary<string, bool>();

    public Animator animator;
    private SpriteRenderer spriteRenderer;
    public bool isFacingRight = false;

    public SkillFactory skillFactory;

    public bool UseAIControl = true;

    void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        spriteRenderer = GetComponent<SpriteRenderer>();

        skillFactory = GetComponent<SkillFactory>();

        // Mặc định tất cả skill đều dùng được
        canUseSkill["H"] = true;
        canUseSkill["J"] = true;
        canUseSkill["K"] = true;
        canUseSkill["L"] = true;

        SpawnCharacterUI();
    }

    void Update()
    {
        FlipCheck();
        if (!UseAIControl)
        {
            if (Input.GetKeyDown(KeyCode.H) && canUseSkill["H"])
                TriggerSkill("H", cooldownH);

            if (Input.GetKeyDown(KeyCode.J) && canUseSkill["J"])
                TriggerSkill("J", cooldownJ);

            if (Input.GetKeyDown(KeyCode.K) && canUseSkill["K"])
                TriggerSkill("K", cooldownK);

            if (Input.GetKeyDown(KeyCode.L) && canUseSkill["L"])
                TriggerSkill("L", cooldownL);
        }
    }

    private void TriggerSkill(string key, float cooldown)
    {
        canUseSkill[key] = false; // Khóa skill
        animator.SetTrigger("Skill" + key); // Gọi animation
        StartCoroutine(StartCooldown(key, cooldown));
    }

    IEnumerator StartCooldown(string key, float cooldown)
    {
        yield return new WaitForSeconds(cooldown);
        canUseSkill[key] = true; // Mở lại skill
    }

    void SpawnCharacterUI()
    {
        if (characterUIPrefab != null)
        {
            GameObject ui = Instantiate(characterUIPrefab);
            CharacterSkillUI skillUI = ui.GetComponent<CharacterSkillUI>();
            if (skillUI != null)
            {
                skillUI.SetSkillHandler(this);
            }
        }
    }
    // Các hàm này sẽ được gọi từ Animation Event
    public void SpawnSkillH()
    {
        skillFactory.CreateSkill("H", gameObject, isFacingRight);
    }

    public void SpawnSkillJ()
    {
        skillFactory.CreateSkill("J", gameObject, isFacingRight);
    }

    public void SpawnSkillK()
    {
        skillFactory.CreateSkill("K", gameObject, isFacingRight);
    }

    public void SpawnSkillL()
    {
        skillFactory.CreateSkill("L", gameObject, isFacingRight);
    }

    void FlipCheck()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        if (horizontal > 0 && !isFacingRight)
            Flip(1);
        else if (horizontal < 0 && isFacingRight)
            Flip(-1);
    }

    void Flip(int direction)
    {
        isFacingRight = (direction == 1);
        spriteRenderer.flipX = !isFacingRight;
    }

    public bool CanUseSkill(string key)
    {
        return canUseSkill.ContainsKey(key) && canUseSkill[key];
    }

    public void TriggerSkill(string key)
    {
        float cooldown = 1f;
        switch (key)
        {
            case "H": cooldown = cooldownH; break;
            case "J": cooldown = cooldownJ; break;
            case "K": cooldown = cooldownK; break;
            case "L": cooldown = cooldownL; break;
        }
        if (CanUseSkill(key))
            TriggerSkill(key, cooldown);
    }
}
