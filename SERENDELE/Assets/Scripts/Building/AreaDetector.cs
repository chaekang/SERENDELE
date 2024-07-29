using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaDetector : MonoBehaviour
{
    private GameObject exit;
    // Start is called before the first frame update
    void Start()
    {
        exit = GameObject.Find("Button");
        exit.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        exit.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        exit.SetActive(false);
    }
}
