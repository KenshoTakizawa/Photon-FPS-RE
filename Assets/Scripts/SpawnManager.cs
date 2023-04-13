using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public Transform[] spawnPositons;

    private void Start()
    {
        foreach (var pos in spawnPositons)
        {
            pos.gameObject.SetActive(false);
        }
    }

    public Transform GetSpawnPoint()
    {
        return spawnPositons[Random.Range(0, spawnPositons.Length)];
    }
}
