using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerCarController : MonoBehaviour
{
    public float forwardSpeed = 8f;
    public float reverseSpeed = 4f;
    public float turnSpeed = 120f; // degrees per second
    Rigidbody rb;
    float moveInput;
    float turnInput;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Input: arrow keys or WASD
        moveInput = Input.GetAxis("Vertical");    // W/S or up/down
        turnInput = Input.GetAxis("Horizontal");  // A/D or left/right
    }

    void FixedUpdate()
    {
        // move forward/back
        Vector3 forward = transform.forward;
        float speed = (moveInput >= 0) ? forwardSpeed : reverseSpeed;
        Vector3 vel = forward * moveInput * speed;
        Vector3 newPos = rb.position + vel * Time.fixedDeltaTime;
        rb.MovePosition(newPos);

        // rotate
        float turn = turnInput * turnSpeed * Time.fixedDeltaTime;
        Quaternion rot = rb.rotation * Quaternion.Euler(0f, turn, 0f);
        rb.MoveRotation(rot);
    }

    void OnCollisionEnter(Collision collision)
    {
        // You can handle bump audio/sfx here
        // If collides with hazard (non-trigger) the HazardDamage script will be invoked on collided object.
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Collectible"))
        {
            Collectible c = other.GetComponent<Collectible>();
            if (c != null)
            {
                c.OnPickedUp();
            }
        }
    }
}
