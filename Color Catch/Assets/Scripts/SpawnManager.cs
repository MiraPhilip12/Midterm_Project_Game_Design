using UnityEngine;
using System.Collections;

public class SpawnManager : MonoBehaviour
{
    public GameObject collectiblePrefab;
    public int maxSimultaneous = 10;
    public float spawnInterval = 1.2f;
    public Vector3 areaCenter = Vector3.zero;
    public Vector2 areaSize = new Vector2(24f, 24f); // X and Z extents

    public string[] colorOptions = new string[] { "Gold", "Silver", "Bronze" };

    void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (collectiblePrefab == null) yield return null;
            if (CountExistingCollectibles() >= maxSimultaneous) continue;

            Vector3 pos = new Vector3(
                Random.Range(areaCenter.x - areaSize.x / 2f, areaCenter.x + areaSize.x / 2f),
                0.4f,
                Random.Range(areaCenter.z - areaSize.y / 2f, areaCenter.z + areaSize.y / 2f)
            );

            GameObject go = Instantiate(collectiblePrefab, pos, Quaternion.identity, transform);
            Collectible c = go.GetComponent<Collectible>();
            if (c != null)
            {
                string color = colorOptions[Random.Range(0, colorOptions.Length)];
                c.colorName = color;
                // Optionally change material based on color here:
                Renderer r = go.GetComponent<Renderer>();
                if (r != null)
                {
                    // create simple tint by instantiating material instance
                    Material matInstance = new Material(r.sharedMaterial);
                    if (color == "Gold") matInstance.color = new Color(1f, 0.85f, 0f);
                    else if (color == "Silver") matInstance.color = new Color(0.75f, 0.75f, 0.8f);
                    else matInstance.color = new Color(0.8f, 0.5f, 0.2f);
                    r.material = matInstance;
                }
            }
        }
    }

    int CountExistingCollectibles()
    {
        return transform.childCount;
    }
}
