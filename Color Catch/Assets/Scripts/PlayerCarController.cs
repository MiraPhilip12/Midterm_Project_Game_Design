using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerCarController : MonoBehaviour
{
    public float forwardSpeed = 14f;   // increased
    public float reverseSpeed = 7f;
    public float turnSpeed = 160f;
    Rigidbody rb;
    float moveInput;
    float turnInput;

    void Awake() { rb = GetComponent<Rigidbody>(); }

    void Update()
    {
        moveInput = Input.GetAxis("Vertical");    // W/S or Up/Down
        turnInput = Input.GetAxis("Horizontal");  // A/D or Left/Right
    }

    void FixedUpdate()
    {
        Vector3 forward = transform.forward;
        float speed = (moveInput >= 0) ? forwardSpeed : reverseSpeed;
        Vector3 vel = forward * moveInput * speed;
        rb.MovePosition(rb.position + vel * Time.fixedDeltaTime);

        float turn = turnInput * turnSpeed * Time.fixedDeltaTime;
        Quaternion rot = rb.rotation * Quaternion.Euler(0f, turn, 0f);
        rb.MoveRotation(rot);
    }
}
