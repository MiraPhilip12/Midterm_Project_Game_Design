using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerCarController : MonoBehaviour
{
    public float forwardSpeed = 20f;
    public float reverseSpeed = 10f;
    public float turnSpeed = 120f;

    private Rigidbody rb;
    private float moveInput;
    private float turnInput;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.mass = 1000f;
    }

    void Update()
    {
        moveInput = Input.GetAxis("Vertical");
        turnInput = Input.GetAxis("Horizontal");
    }

    void FixedUpdate()
    {
        if (GameManager.Instance != null && GameManager.Instance.gameOver) return;

        if (moveInput != 0)
        {
            Vector3 forward = transform.forward;
            float speed = (moveInput >= 0) ? forwardSpeed : reverseSpeed;
            Vector3 movement = forward * moveInput * speed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + movement);
        }

        if (turnInput != 0)
        {
            float turn = turnInput * turnSpeed * Time.fixedDeltaTime;
            Quaternion rotation = rb.rotation * Quaternion.Euler(0f, turn, 0f);
            rb.MoveRotation(rotation);
        }
    }

    // Strong collision detection with barriers and bushes
    void OnCollisionEnter(Collision collision)
    {
        HandleCollision(collision.collider);
    }

    void OnTriggerEnter(Collider other)
    {
        HandleCollision(other);
    }

    void HandleCollision(Collider collider)
    {
        if (collider.CompareTag("Barrier") || collider.CompareTag("Bush"))
        {
            // Strong force to stop the car
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            // Apply strong knockback
            Vector3 awayDirection = (transform.position - collider.transform.position).normalized;
            rb.AddForce(awayDirection * 15f, ForceMode.VelocityChange);

            Debug.Log($"Collided with: {collider.gameObject.name}");
        }

        // Police collision - instant game over
        if (collider.CompareTag("Police"))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.EndGame(false); // Instant game over
            }
        }
    }
}