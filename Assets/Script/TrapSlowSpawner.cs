using System.Collections;
using UnityEngine;

public class TrapSpawner : MonoBehaviour
{
    public GameObject trapPrefab; // Prefab của bẫy
    public Transform[] spawnPoints; // Các điểm đặt bẫy
    public float trapDuration = 10f; // Thời gian tồn tại của bẫy
    public float respawnDelay = 20f; // Thời gian chờ để xuất hiện lại

    private void Start()
    {
        StartCoroutine(SpawnTrapRoutine());
    }

    private IEnumerator SpawnTrapRoutine()
    {
        while (true)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            GameObject trap = Instantiate(trapPrefab, spawnPoint.position, Quaternion.identity);
            yield return new WaitForSeconds(trapDuration);
            Destroy(trap);
            yield return new WaitForSeconds(respawnDelay);
        }
    }
}