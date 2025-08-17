using UnityEngine;

public class SkillFactory : MonoBehaviour
{
    [Header("Skill Prefabs")]
    public GameObject skillH;
    public GameObject skillJ;
    public GameObject skillK;
    public GameObject skillL;
    

    [Header("Right-facing Spawn Points")]
    public Transform spawnPointHRight;
    public Transform spawnPointJRight;
    public Transform spawnPointKRight;
    public Transform spawnPointLRight;
    

    [Header("Left-facing Spawn Points")]
    public Transform spawnPointHLeft;
    public Transform spawnPointJLeft;
    public Transform spawnPointKLeft;
    public Transform spawnPointLLeft;
    

    public GameObject CreateSkill(string skillKey, GameObject owner, bool facingRight)
    {
        GameObject prefab = null;
        Transform spawnPoint = null;

        switch (skillKey)
        {
            case "H":
                prefab = skillH;
                spawnPoint = facingRight ? spawnPointHRight : spawnPointHLeft;
                break;
            case "J":
                prefab = skillJ;
                spawnPoint = facingRight ? spawnPointJRight : spawnPointJLeft;
                break;
            case "K":
                prefab = skillK;
                spawnPoint = facingRight ? spawnPointKRight : spawnPointKLeft;
                break;
            case "L":
                prefab = skillL;
                spawnPoint = facingRight ? spawnPointLRight : spawnPointLLeft;
                break;
            

        }

        if (prefab == null || spawnPoint == null) return null;

        GameObject skill = Instantiate(prefab, spawnPoint.position, Quaternion.identity);

        ISkill skillInterface = skill.GetComponent<ISkill>();
        if (skillInterface != null)
        {
            skillInterface.Initialize(owner, facingRight);
        }

        return skill;
    }
}
