using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    public int level;
    public int enemy_num;
    public int count;
    public GameObject enemy;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(startSpawn());
    }

    IEnumerator startSpawn()
    {
        int xaxis;
        int zaxis;

        while (count < enemy_num)
        {
            xaxis = Random.Range(-9, 82);
            zaxis = Random.Range(-95, 9);
            Instantiate(enemy, new Vector3(xaxis, 1, zaxis), Quaternion.identity);
            yield return
            count += 1;
        }
    }

}
