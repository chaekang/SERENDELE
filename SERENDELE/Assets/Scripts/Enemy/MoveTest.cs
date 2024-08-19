using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    void Move()
    {

        float speed = 10;

        // °È±â
        if (Input.GetKey(KeyCode.UpArrow))
        {
            if (Input.GetKey(KeyCode.A))
                transform.Translate(-speed * Time.deltaTime, 0, speed * Time.deltaTime);
            else
                transform.Translate(0, 0, speed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            //Debug.Log("isMoving");
            transform.Translate(-speed * Time.deltaTime, 0, 0);
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            //Debug.Log("isMoving");
            transform.Translate(0, 0, -speed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
            transform.Translate(speed * Time.deltaTime, 0, 0);
    }
}
