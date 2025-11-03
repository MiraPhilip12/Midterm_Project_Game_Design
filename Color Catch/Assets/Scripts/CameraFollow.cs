using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public Vector3 offset = new Vector3(0, 3, -6);
    public float smoothSpeed = 5f;

    void LateUpdate()
    {
        if (player == null) return;

        // Calculate desired position behind the player
        Vector3 desiredPosition = player.position + player.TransformDirection(offset);

        // Smoothly move camera to desired position
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // Always look at the player
        transform.LookAt(player.position);
    }
}