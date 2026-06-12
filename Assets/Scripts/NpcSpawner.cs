using UnityEngine;
using System.Collections;

public class NPCSpawner : MonoBehaviour
{
    [Header("Spawn Locations")]
    [SerializeField] private Transform leftSpawnPoint;
    [SerializeField] private Transform rightSpawnPoint;
    [SerializeField] private Transform leftTargetPosition;
    [SerializeField] private Transform rightTargetPosition;

    [Header("NPC Prefabs")]
    [SerializeField] private GameObject[] npcPrefabs;
    
    [Header("Spawn Settings")]
    [SerializeField] private float minSpawnDelay = 2f;
    [SerializeField] private float maxSpawnDelay = 5f;

    private bool _canSpawn = true;

    void Start()
    {
        StartCoroutine(SpawnLoopRoutine());
    }

    private IEnumerator SpawnLoopRoutine()
    {
        while (_canSpawn)
        {
            float randomDelay = Random.Range(minSpawnDelay, maxSpawnDelay);
            yield return new WaitForSeconds(randomDelay);

            if (npcPrefabs.Length > 0)
            {
                GameObject chosenPrefab = npcPrefabs[Random.Range(0, npcPrefabs.Length)];
                SpawnNPC(chosenPrefab);
            }
        }
    }

    private void SpawnNPC(GameObject prefab)
    {
        bool spawnOnLeft = Random.value > 0.5f;
        
        Transform selectedSpawn = spawnOnLeft ? leftSpawnPoint : rightSpawnPoint;
        Transform selectedTarget = spawnOnLeft ? rightTargetPosition : leftTargetPosition;
        
        GameObject spawnedNPC = Instantiate(prefab, selectedSpawn.position, Quaternion.identity);
        
        NPCMovement movementScript = spawnedNPC.GetComponent<NPCMovement>();
        if (movementScript != null)
        {
            movementScript.SetupNPC(selectedTarget);
        }
        else
        {
            Debug.LogWarning("Spawned NPC is missing the NPCMovement script!");
        }
    }

    // Public method to stop spawning when the game ends
    public void StopSpawning()
    {
        _canSpawn = false;
    }
}