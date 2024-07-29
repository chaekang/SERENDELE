using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Transform[] spawnPoints;
    public GameObject enemy;

    public int xaxis;
    public int zaxis;
    public int enemy_num;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(startSpawn());
    }



    IEnumerator startSpawn()
    {
        while(enemy_num < 40)
        {
            xaxis = Random.Range(10, 55);
            zaxis = Random.Range(20, 60);
            Instantiate(enemy, new Vector3(xaxis, 1, zaxis), Quaternion.identity);
            yield return new WaitForSeconds(2);
            enemy_num += 1;
        }
    }
}
