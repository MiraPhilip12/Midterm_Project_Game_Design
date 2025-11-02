using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("Prefab")]
    public GameObject collectiblePrefab; // prefab root must have Collectible script

    [Header("Spawn area (center and size on X,Z)")]
    public Vector3 areaCenter = Vector3.zero;
    public Vector2 areaSize = new Vector2(24f, 24f);

    [Header("Spawn timing")]
    public float spawnInterval = 1.2f;
    public int maxSimultaneous = 12;

    [Header("Item settings")]
    public int coinPoints = 10;
    public int bombPoints = 10;
    public float spawnHeight = 0.4f; // Y position for spawned items

    [Header("Optional materials")]
    public Material coinMaterial; // optional: assign gold-like material
    public Material bombMaterial; // optional: assign red/dark material

    void Start()
    {
        if (collectiblePrefab == null)
        {
            Debug.LogWarning("[SpawnManager] collectiblePrefab not assigned.");
            return;
        }

        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (collectiblePrefab == null) continue;
            if (CountExistingCollectibles() >= maxSimultaneous) continue;

            Vector3 pos = new Vector3(
                Random.Range(areaCenter.x - areaSize.x / 2f, areaCenter.x + areaSize.x / 2f),
                spawnHeight,
                Random.Range(areaCenter.z - areaSize.y / 2f, areaCenter.z + areaSize.y / 2f)
            );

            GameObject go = Instantiate(collectiblePrefab, pos, Quaternion.identity, transform);
            Collectible c = go.GetComponent<Collectible>();
            if (c == null)
            {
                Debug.LogWarning("[SpawnManager] Spawned prefab does not have Collectible script on root. Please ensure collectiblePrefab root has the Collectible component.");
                Destroy(go);
                continue;
            }

            // Randomly decide coin or bomb (tweak probability as needed)
            bool isCoin = (Random.value > 0.28f); // ~72% coins, 28% bombs
            if (isCoin)
            {
                c.itemType = Collectible.LocalItemType.Coin;
                c.points = coinPoints;
                // optional tint
                ApplyMaterialIfPossible(go, coinMaterial, new Color(1f, 0.85f, 0f));
            }
            else
            {
                c.itemType = Collectible.LocalItemType.Bomb;
                c.points = bombPoints;
                ApplyMaterialIfPossible(go, bombMaterial, new Color(0.7f, 0.15f, 0.15f));
            }
        }
    }

    int CountExistingCollectibles()
    {
        // We assume spawned collectibles are children of this manager
        return transform.childCount;
    }

    void ApplyMaterialIfPossible(GameObject go, Material mat, Color fallbackColor)
    {
        // Try to tint the root renderer (works for primitives or simple models)
        Renderer r = go.GetComponent<Renderer>();
        if (r != null)
        {
            if (mat != null) r.material = mat;
            else
            {
                // create a small material instance and tint it
                Material inst = new Material(r.sharedMaterial);
                inst.color = fallbackColor;
                r.material = inst;
            }
            return;
        }

        // If no renderer on root, try children
        Renderer childR = go.GetComponentInChildren<Renderer>();
        if (childR != null)
        {
            if (mat != null) childR.material = mat;
            else
            {
                Material inst = new Material(childR.sharedMaterial);
                inst.color = fallbackColor;
                childR.material = inst;
            }
        }
    }
}
