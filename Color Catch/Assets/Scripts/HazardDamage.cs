using UnityEngine;

// HazardDamage.cs
// Attach to static bushes/barriers. Use either a collider (non-trigger) or trigger collider.
public class HazardDamage : MonoBehaviour
{
    [Tooltip("Points to subtract when player collides")]
    public int penaltyPoints = 5;

    [Tooltip("Cooldown in seconds so repeated collisions don't continuously drain score")]
    public float cooldown = 0.8f;

    float lastHitTime = -999f;

    void OnCollisionEnter(Collision collision)
    {
        TryApplyPenalty(collision.collider);
    }

    void OnTriggerEnter(Collider other)
    {
        TryApplyPenalty(other);
    }

    void TryApplyPenalty(Collider col)
    {
        if (!col.CompareTag("Player")) return;
        if (Time.time - lastHitTime < cooldown) return;
        lastHitTime = Time.time;

        if (GameManager.Instance == null)
        {
            Debug.LogWarning("[HazardDamage] GameManager.Instance is null. Make sure a GameManager exists in the scene.");
            return;
        }

        // Call the new GameManager API
        GameManager.Instance.OnHazardHit(penaltyPoints, gameObject.name);

        // Optional: small knockback effect
        Rigidbody rb = col.attachedRigidbody;
        if (rb != null)
        {
            Vector3 away = (rb.position - transform.position).normalized;
            rb.AddForce(away * 220f);
        }
    }
}
