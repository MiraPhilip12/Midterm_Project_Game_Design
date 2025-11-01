using UnityEngine;

public class HazardDamage : MonoBehaviour
{
    [Tooltip("Points to subtract when player collides")]
    public int penaltyPoints = 5;
    [Tooltip("Cooldown in seconds so repeated collisions don't continuously drain score")]
    public float cooldown = 0.8f;

    float lastHitTime = -999f;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            if (Time.time - lastHitTime < cooldown) return;
            lastHitTime = Time.time;

            // Apply penalty via GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ApplyHazardPenalty(penaltyPoints, gameObject.name);
            }

            // Optional: small bounce effect on player
            Rigidbody rb = collision.rigidbody;
            if (rb != null)
            {
                Vector3 away = (rb.position - transform.position).normalized;
                rb.AddForce(away * 150f); // tweak force
            }

            // Optionally play sfx or particle on hazard
        }
    }

    // If your bushes use trigger colliders instead:
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (Time.time - lastHitTime < cooldown) return;
        lastHitTime = Time.time;

        if (GameManager.Instance != null)
            GameManager.Instance.ApplyHazardPenalty(penaltyPoints, gameObject.name);
    }
}
