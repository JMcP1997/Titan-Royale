using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPointHolder : MonoBehaviour
{
    public static SpawnPointHolder Instance;

    [SerializeField] Transform[] spawnPoints;
    List<Transform> usedSpawnPoints = new List<Transform>();

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
    }

    public Transform[] GetSpawnPoints()
    {
        return spawnPoints;
    }
}
