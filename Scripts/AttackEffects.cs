using UnityEngine;
using System.Collections;
using Cinemachine;

public class AttackEffects : MonoBehaviour
{
    public GameObject[] rockPrefabs; // Array to store multiple rock prefabs
    public Transform[] rockSpawnPositions;
    public float minRockFallInterval = 0.3f;
    public float maxRockFallInterval = 1.0f;
    public float minFallSpeed = 2f;
    public float maxFallSpeed = 5f;

    public CinemachineImpulseSource impulseSource;

    public void TriggerRocks()
    {
        StartCoroutine(FallingRocks());
    }

    public void TriggerShockwave()
    {
        ScreenShake();
    }

    private IEnumerator FallingRocks()
    {
        for (int i = 0; i < 4; i++) // Spawn exactly 4 rocks each time
        {
            // Choose a random spawn position
            Transform spawnPos = rockSpawnPositions[Random.Range(0, rockSpawnPositions.Length)];

            // Choose a random rock prefab from the array
            GameObject selectedRockPrefab = rockPrefabs[Random.Range(0, rockPrefabs.Length)];

            // Instantiate the chosen rock prefab
            GameObject rock = Instantiate(selectedRockPrefab, spawnPos.position, Quaternion.identity);

            // Set a random fall speed for the rock
            Rock rockScript = rock.GetComponent<Rock>();
            if (rockScript != null)
            {
                rockScript.SetFallSpeed(Random.Range(minFallSpeed, maxFallSpeed));
            }

            // Wait for a random interval before spawning the next rock
            yield return new WaitForSeconds(Random.Range(minRockFallInterval, maxRockFallInterval));
        }
    }

    private void ScreenShake()
    {
        if (impulseSource != null)
        {
            impulseSource.GenerateImpulse();
        }
    }
}
