using UnityEngine;
using UnityEngine.UI;

public class CharacterSkillUI : MonoBehaviour
{
    [Header("Skill Icons")]
    public Image skillHIcon;
    public Image skillJIcon;
    public Image skillKIcon;
    public Image skillLIcon;

    private SkillInputHandler skillHandler;

    public void SetSkillHandler(SkillInputHandler handler)
    {
        skillHandler = handler;
        Debug.Log($"✅ UI connected to: {handler.gameObject.name}");
    }

    void Update()
    {
        if (skillHandler != null)
        {
            UpdateSkillIcons();
        }
    }

    void UpdateSkillIcons()
    {
        UpdateIconColor(skillHIcon, skillHandler.CanUseSkill("H"));
        UpdateIconColor(skillJIcon, skillHandler.CanUseSkill("J"));
        UpdateIconColor(skillKIcon, skillHandler.CanUseSkill("K"));
        UpdateIconColor(skillLIcon, skillHandler.CanUseSkill("L"));
    }

    void UpdateIconColor(Image icon, bool canUse)
    {
        if (icon != null)
        {
            icon.color = canUse ? Color.white : Color.gray;
        }
    }
}
