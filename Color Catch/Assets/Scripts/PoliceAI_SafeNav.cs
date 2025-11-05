using UnityEngine;

public class PoliceAI : MonoBehaviour
{
    public Transform player;
    public float followSpeed = 30f;
    public float rotationSpeed = 160f;
    public float minDistance = 3f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Find player automatically if not assigned
        if (player == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }
    }

    void FixedUpdate()
    {
        if (player == null) return;
        if (GameManager.Instance != null && GameManager.Instance.gameOver) return;

        // Calculate direction to player
        Vector3 directionToPlayer = player.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        // Only move if not too close to player
        if (distanceToPlayer > minDistance)
        {
            // Move towards player with same speed
            Vector3 moveDirection = directionToPlayer.normalized;
            rb.MovePosition(rb.position + moveDirection * followSpeed * Time.fixedDeltaTime);

            // Rotate to face player with same rotation speed
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            Quaternion newRotation = Quaternion.RotateTowards(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
            rb.MoveRotation(newRotation);
        }
    }
    
    void OnCollisionEnter(Collision collision)
    {
        // Check if collided with police
        if (collision.gameObject.CompareTag("Player"))
        {
            // End game immediately
            if (collision.gameObject.CompareTag("Player"))
            {
                GameManager.Instance.EndGame(false);
            }
        }
    }
}