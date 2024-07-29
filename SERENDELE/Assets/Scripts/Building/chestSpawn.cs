using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class chestSpawn : MonoBehaviour
{
    public GameObject chest;

    // Start is called before the first frame update
    void Start()
    {
        findPos();
    }
    
    public void findPos()
    {
        Vector3 pos;

        Vector3 a = new Vector3(53.15f, 1.21f, 9.83f);
        Vector3 b = new Vector3(-3, 0.28f, -16);
        Vector3 c = new Vector3(16.28f, 0.28f, -30.5f);
        Vector3 d = new Vector3(29.4f, 0.28f, -79);
        Vector3 e = new Vector3(-9, 0.28f, -78.4f);
        Vector3 f = new Vector3(77.72f, 0.28f, -54.6f);

        int num = Random.Range(1, 7);

        switch (num)
        {
            case 1: pos = a; break;
            case 2: pos = b; break;
            case 3: pos = c; break;
            case 4: pos = d; break;
            case 5: pos = e; break;
            case 6: pos = f; break;
            default: pos = new Vector3(0, 0, 0) ; break;
        }


        Instantiate(chest, pos, Quaternion.identity);
    }


}
