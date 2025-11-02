// CameraFollow.cs
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0f, 18f, -12f);
    public float smooth = 8f;

    void LateUpdate()
    {
        if (target == null)
        {
            // try to auto-find the player by tag once
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null) target = player.transform;
            else return;
        }

        Vector3 desired = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desired, Time.deltaTime * smooth);
        transform.LookAt(target.position + Vector3.up * 0.6f);
    }
}
