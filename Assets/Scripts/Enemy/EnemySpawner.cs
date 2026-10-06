using UnityEngine;
using System.Collections; // Required for Coroutines

public class EnemySpawner : MonoBehaviour
{
    // --- Editor-Adjustable Parameters ---
    [Header("Enemy Configuration")]
    [Tooltip("The prefab of the enemy capsule to spawn.")]
    [SerializeField] private GameObject enemyPrefab;
    
    [Header("Spawning Controls")]
    [Tooltip("Delay before the first wave starts.")]
    [SerializeField] private float startDelay = 2f;
    [Tooltip("Time delay between each wave.")]
    [SerializeField] private float waveInterval = 10f;
    [Tooltip("Base number of enemies to spawn in the first wave.")]
    [SerializeField] private int baseEnemyCount = 5;

    [Header("Spawn Position")]
    [Tooltip("The fixed Y height at which all enemies will spawn.")]
    [SerializeField] private float fixedSpawnHeight = 3.0f;
    
    // --- Spawn Locations ---
    [Header("Spawn Locations")]
    [Tooltip("Array of empty GameObjects defining spawn points.")]
    [SerializeField] private Transform[] spawnPoints;
    
    // --- Internal State ---
    private int currentWave = 0;

    void Start()
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("Enemy Prefab is not assigned in the Spawner!");
            enabled = false;
            return;
        }

        if (spawnPoints.Length == 0)
        {
            Debug.LogWarning("No spawn points assigned. Spawning at (0,0,0) instead.");
        }

        // Start the continuous spawning loop
        StartCoroutine(SpawnWavesRoutine());
    }

    IEnumerator SpawnWavesRoutine()
    {
        // Wait for initial delay before starting
        yield return new WaitForSeconds(startDelay);

        while (true)
        {
            currentWave++;
            int enemiesToSpawn = CalculateEnemyCount(currentWave);
            
            Debug.Log($"Starting Wave {currentWave} - Spawning {enemiesToSpawn} enemies.");

            StartCoroutine(SpawnWave(enemiesToSpawn));

            // Wait for the next wave interval
            yield return new WaitForSeconds(waveInterval);
        }
    }

    int CalculateEnemyCount(int waveNumber)
    {
        // Simple scaling: add one more enemy per wave
        return baseEnemyCount + (waveNumber - 1);
    }

    IEnumerator SpawnWave(int count)
    {
        for (int i = 0; i < count; i++)
        {
            // Choose a random spawn point
            Transform spawnPoint = GetRandomSpawnPoint();
            
            // Get the base spawn position from the Transform
            Vector3 spawnPosition = spawnPoint.position;
            
            // CRITICAL FIX: Overwrite the Y component with the fixed height
            spawnPosition.y = fixedSpawnHeight;

            // Instantiate the enemy at the corrected position
            GameObject newEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

            // Optional: Delay individual enemy spawns slightly for visual effect
            yield return new WaitForSeconds(1.0f); 
        }
    }

    Transform GetRandomSpawnPoint()
    {
        if (spawnPoints.Length == 0)
        {
            // If no spawn points are assigned, return the spawner's transform as a fallback
            return transform;
        }
        
        // Return a randomly selected spawn point
        int randomIndex = Random.Range(0, spawnPoints.Length);
        return spawnPoints[randomIndex];
    }
}
