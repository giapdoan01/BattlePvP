using System.Collections;
using UnityEngine;

public class GohanSkillK : MonoBehaviour, ISkill
{
    [Header("Teleport Settings")]
    [SerializeField] private float teleportDistance = 5f;
    [SerializeField] private float teleportDuration = 0.2f;
    [SerializeField] private LayerMask obstacleLayerMask = -1;

    private GameObject owner;
    private bool facingRight;
    private Rigidbody2D ownerRigidbody;
    private Collider2D ownerCollider;

    public void Initialize(GameObject skillOwner, bool ownerFacingRight)
    {
        owner = skillOwner;
        facingRight = ownerFacingRight;
        ownerRigidbody = owner.GetComponent<Rigidbody2D>();
        ownerCollider = owner.GetComponent<Collider2D>();

        StartCoroutine(ExecuteTeleport());
    }

    private IEnumerator ExecuteTeleport()
    {
        
        Vector3 startPosition = owner.transform.position;
        Vector3 rawDestination = CalculateTeleportDestination();
        
        bool isValidPosition = IsValidTeleportPosition(rawDestination);
        Vector3 finalDestination = isValidPosition ? rawDestination : FindNearestValidPosition(rawDestination);

        
        if (Vector3.Distance(startPosition, finalDestination) < 0.01f)
        {
            DestroySelf();
            yield break;
        }

        
        SetOwnerPhysics(false);
        yield return new WaitForSeconds(0.1f);

        
        PerformTeleport(finalDestination);

        
        yield return new WaitForSeconds(teleportDuration);

        
        SetOwnerPhysics(true);

        
        DestroySelf();
    }

    private Vector3 CalculateTeleportDestination()
    {
        Vector3 currentPosition = owner.transform.position;
        Vector3 direction = facingRight ? Vector3.right : Vector3.left;
        return currentPosition + (direction * teleportDistance);
    }

    private bool IsValidTeleportPosition(Vector3 position)
    {
        if (ownerCollider == null) return true;

        bool ownerColliderWasEnabled = ownerCollider.enabled;
        ownerCollider.enabled = false;

        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(
            position,
            ownerCollider.bounds.size,
            0f,
            obstacleLayerMask
        );

        ownerCollider.enabled = ownerColliderWasEnabled;

        foreach (Collider2D hitCollider in hitColliders)
        {
            if (!IsCameraBounds(hitCollider))
            {
                return false; 
            }
        }

        return true; 
    }

    private Vector3 FindNearestValidPosition(Vector3 targetPosition)
    {
        Vector3 currentPosition = owner.transform.position;
        Vector3 directionVector = targetPosition - currentPosition;
        Vector3 direction = directionVector.normalized;
        float maxDistance = directionVector.magnitude;

        if (ownerCollider == null) return targetPosition;

        bool ownerColliderWasEnabled = ownerCollider.enabled;
        ownerCollider.enabled = false;

        RaycastHit2D[] hits = Physics2D.RaycastAll(
            currentPosition,
            direction,
            maxDistance,
            obstacleLayerMask
        );

        ownerCollider.enabled = ownerColliderWasEnabled;

        foreach (RaycastHit2D hit in hits)
        {
            if (!IsCameraBounds(hit.collider))
            {
                float safeDistance = 0.5f;
                float adjustedDistance = Mathf.Max(0, hit.distance - safeDistance);
                return currentPosition + (direction * adjustedDistance);
            }
        }

        return targetPosition;
    }

    private bool IsCameraBounds(Collider2D collider)
    {
        if (collider == null) return false;
        string colliderName = collider.name.ToLower();
        return colliderName.Contains("camerabounds") || colliderName.Contains("boundary");
    }

    private void PerformTeleport(Vector3 destination)
    {
        owner.transform.position = destination;

        if (ownerRigidbody != null)
        {
            ownerRigidbody.linearVelocity = Vector2.zero;
        }
    }

    private void SetOwnerPhysics(bool enabled)
    {
        if (ownerCollider != null)
        {
            ownerCollider.enabled = enabled;
        }

        if (ownerRigidbody != null)
        {
            ownerRigidbody.simulated = enabled;
            if (!enabled)
            {
                ownerRigidbody.linearVelocity = Vector2.zero;
            }
        }
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }
}
