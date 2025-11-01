using UnityEngine;

public class Collectible : MonoBehaviour
{
    public string colorName = "Gold"; // set per prefab or by SpawnManager
    public int pointValue = 10;
    public AudioClip pickupSFX;
    public ParticleSystem pickupVFX;

    // Called by PlayerCarController OnTriggerEnter
    public void OnPickedUp()
    {
        // Compare with GameManager target
        if (GameManager.Instance == null) return;

        bool correct = colorName == GameManager.Instance.targetColor;
        if (correct)
        {
            GameManager.Instance.AddScore(pointValue);
            // play correct sound
        }
        else
        {
            GameManager.Instance.AddScore(-pointValue);
            GameManager.Instance.ShowFeedback($"-{pointValue} (Wrong color)");
        }

        if (pickupVFX) Instantiate(pickupVFX, transform.position, Quaternion.identity);
        if (pickupSFX) AudioSource.PlayClipAtPoint(pickupSFX, transform.position);

        Destroy(gameObject);
    }
}
