using UnityEngine;

public class SkillEnemyFactory : MonoBehaviour
{
    [Header("Skill Prefabs")]
    public GameObject shootBallPrefab;

    // Object Pool settings
    private static bool poolInitialized = false;
    private static string poolTag = "EnemyShootBall";
    private static int poolSize = 15;

    void Start()
    {
        InitializePool();
    }

    void InitializePool()
    {
        // Tạo ObjectPool singleton nếu chưa có
        if (ObjectPool.Instance == null)
        {
            GameObject poolObj = new GameObject("ObjectPool");
            poolObj.AddComponent<ObjectPool>();
        }

        // Tạo pool cho EnemyShootBall nếu chưa có
        if (!poolInitialized && shootBallPrefab != null)
        {
            ObjectPool.Instance.CreatePool(poolTag, shootBallPrefab, poolSize);
            poolInitialized = true;
            Debug.Log($"Created pool for {poolTag} with size {poolSize}");
        }
    }

    public GameObject CreateSkill(string skillKey, GameObject owner, Vector3 spawnPos, Vector2 direction)
    {
        switch (skillKey)
        {
            case "ShootBall":
                return CreateShootBall(owner, spawnPos, direction);
            default:
                Debug.LogWarning($"Skill key not found: {skillKey}");
                return null;
        }
    }

    GameObject CreateShootBall(GameObject owner, Vector3 spawnPos, Vector2 direction)
    {
        // Lấy object từ pool
        GameObject skillObj = ObjectPool.Instance.Get(poolTag, spawnPos, Quaternion.identity);

        if (skillObj != null)
        {
            EnemyShootBall skill = skillObj.GetComponent<EnemyShootBall>();
            if (skill != null)
            {
                skill.ActivateSkill(owner, direction);
                Debug.Log($"Got skill from pool: {skillObj.name}");
            }
        }
        else
        {
            Debug.LogWarning("Failed to get skill from pool!");
        }

        return skillObj;
    }
}
