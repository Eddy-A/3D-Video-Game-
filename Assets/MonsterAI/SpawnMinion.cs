using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnMinion : MonoBehaviour
{
    public GameObject minion;
    public float spawnDistance = 1f;

    public void SpawnMinions()
    {
        Vector3[] directions = new Vector3[] { Vector3.right, Vector3.forward, Vector3.left, Vector3.back };
        for(int i = 0; i < 4; i++)
        {
            Instantiate(minion, transform.position + directions[i] * spawnDistance, Quaternion.identity);
        }
    }
}
