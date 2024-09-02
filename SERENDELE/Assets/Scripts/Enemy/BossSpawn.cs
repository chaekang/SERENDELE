using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpawn : MonoBehaviour
{
    [SerializeField]
    public GameObject BossPrefab;

    // Boss spawn position
    private Vector3 spawnPosition = new Vector3(20, 0, 15);

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnBoss());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator SpawnBoss()
    {
        yield return new WaitForSeconds(30f);
        Instantiate(BossPrefab, spawnPosition, Quaternion.identity);
    }
}
