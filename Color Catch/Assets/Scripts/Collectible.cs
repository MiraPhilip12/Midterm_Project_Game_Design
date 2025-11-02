using UnityEngine;

// Collectible.cs
// Attach to the coin or bomb prefab (ensure the collider is IsTrigger = true and Rigidbody is kinematic)
public class Collectible : MonoBehaviour
{
    public enum LocalItemType { Coin, Bomb }

    [Tooltip("Set whether this prefab is a coin (good) or bomb (bad)")]
    public LocalItemType itemType = LocalItemType.Coin;

    [Tooltip("Points to add or subtract when picked")]
    public int points = 10;

    public AudioClip pickupSFX;
    public ParticleSystem pickupVFX;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (GameManager.Instance == null)
        {
            Debug.LogWarning("[Collectible] GameManager.Instance is null. Make sure a GameManager exists in the scene.");
            return;
        }

        // Map local enum to GameManager.ItemType and notify GameManager
        GameManager.ItemType gmType = (itemType == LocalItemType.Coin) ? GameManager.ItemType.Coin : GameManager.ItemType.Bomb;
        GameManager.Instance.HandleCollectiblePicked(gmType, points);

        if (pickupVFX) Instantiate(pickupVFX, transform.position, Quaternion.identity);
        if (pickupSFX) AudioSource.PlayClipAtPoint(pickupSFX, transform.position);

        Destroy(gameObject);
    }
}
