using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public Vector3 offset = new Vector3(0, 8, -10);
    public float smoothSpeed = 5f;
    public float rotationSpeed = 3f;

    void LateUpdate()
    {
        if (player == null) return;

        // Calculate desired position behind the player
        Vector3 desiredPosition = player.position + player.TransformDirection(offset);

        // Smooth position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;

        // Always look at player
        Quaternion targetRotation = Quaternion.LookRotation(player.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}