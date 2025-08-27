using UnityEngine;

public interface IBossSkill
{
    // Basic skill properties
    string SkillName { get; }
    float Damage { get; }
    float Range { get; }
    float Duration { get; }
    bool IsActive { get; }

    // Owner management
    void SetOwner(GameObject owner);
    GameObject GetOwner();

    // Direction and targeting
    void SetDirection(int direction); // -1 for left, 1 for right
    void SetTarget(Transform target);

    // Skill lifecycle
    void ActivateSkill();
    void DeactivateSkill();
    void UpdateSkill();

    // Collision and effects
    void OnHitTarget(GameObject target);
    void OnSkillComplete();
}
