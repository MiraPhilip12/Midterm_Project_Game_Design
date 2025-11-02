using UnityEngine;

public class CollectiblePickup : MonoBehaviour
{
    public enum ItemType { Coin, Bomb }
    public ItemType type = ItemType.Coin;
    public int points = 10;
    public AudioClip pickupSFX;
    public ParticleSystem pickupVFX;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (GameManager.Instance == null) return;

        if (type == ItemType.Coin) GameManager.Instance.AddScore(points);
        else GameManager.Instance.AddScore(-points);

        if (pickupVFX) Instantiate(pickupVFX, transform.position, Quaternion.identity);
        if (pickupSFX) AudioSource.PlayClipAtPoint(pickupSFX, transform.position);
        Destroy(gameObject);

        if (other.CompareTag("Player"))
        {
            // if this is a coin:
            GameManager.Instance.HandleCollectiblePicked(GameManager.ItemType.Coin, points);

            // if this is a bomb:
            GameManager.Instance.HandleCollectiblePicked(GameManager.ItemType.Bomb, points);

            Destroy(gameObject);
        }

    }
}
