using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject skeletonPrefab; // Assign the skeleton prefab here
    public Transform spawnPoint; // Optional: Set a spawn point in the scene

    private void Update()
    {
        // Spawn a skeleton when the "S" key is pressed
        if (Input.GetKeyDown(KeyCode.J))
        {
            SpawnSkeleton();
        }
    }

    void SpawnSkeleton()
    {
        if (skeletonPrefab != null)
        {
            Vector3 spawnPosition = spawnPoint != null ? spawnPoint.position : transform.position;
            Instantiate(skeletonPrefab, spawnPosition, Quaternion.identity);
            Debug.Log("Skeleton spawned!");
        }
        else
        {
            Debug.LogError("Skeleton prefab is not assigned.");
        }
    }
}
