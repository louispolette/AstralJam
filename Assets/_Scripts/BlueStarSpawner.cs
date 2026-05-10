using UnityEngine;

public class BlueStarSpawner : MonoBehaviour
{
    [Space]

    public float spawnRange = 3f;

    [Space]

    public BlueStar blueStarPrefab;

    [ContextMenu("Spawn Star")]
    public void SpawnStar()
    {
        float xPos = Random.Range(transform.position.x - spawnRange, transform.position.x + spawnRange);
        Instantiate(blueStarPrefab, new Vector3(xPos, transform.position.y, 0f), Quaternion.identity);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + Vector3.left * spawnRange, transform.position + Vector3.right * spawnRange);
    }
}
