using UnityEngine;

public class SkillRestriction : MonoBehaviour
{
    public bool retricSkillH = true;
    public bool retricSkillJ = true;
    public bool retricSkillK = true;
    public bool retricSkillL = true;

    // trạng thái đang dùng skill
    private bool isUsingSkillH = false;
    private bool isUsingSkillJ = false;
    private bool isUsingSkillK = false;
    private bool isUsingSkillL = false;

    void Awake()
    {
        // không cần tham chiếu ngược đến SkillInputHandler ở đây
    }

    // GỌI khi skill bắt đầu (ví dụ animation event hoặc SkillFactory spawn)
    public void OnSkillStarted(string key)
    {
        switch (key)
        {
            case "H": if (retricSkillH) isUsingSkillH = true; break;
            case "J": if (retricSkillJ) isUsingSkillJ = true; break;
            case "K": if (retricSkillK) isUsingSkillK = true; break;
            case "L": if (retricSkillL) isUsingSkillL = true; break;
        }
    }

    // GỌI khi skill kết thúc (animation event / skill prefab gọi)
    public void OnSkillFinished(string key)
    {
        switch (key)
        {
            case "H": isUsingSkillH = false; break;
            case "J": isUsingSkillJ = false; break;
            case "K": isUsingSkillK = false; break;
            case "L": isUsingSkillL = false; break;
        }
    }

    // Chỉ trả lời câu hỏi "theo luật restriction này thì có được dùng không?"
    public bool CanUseSkill(string key)
    {
        // Nếu đang xài J thì chặn các skill khác
        if (isUsingSkillJ && key != "J") return false;

        // Bạn có thể thêm rule khác ở đây nếu cần
        return true;
    }
}
