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

        // Find player automatically
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

        
        Vector3 directionToPlayer = player.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        if (distanceToPlayer > minDistance)
        {
            Vector3 moveDirection = directionToPlayer.normalized;
            rb.MovePosition(rb.position + moveDirection * followSpeed * Time.fixedDeltaTime);

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