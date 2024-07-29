using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Teleport : MonoBehaviour
{
    public void ToMAP()
    {
        SceneManager.LoadScene("NURI4");
        //SceneManager 메서드의 LoadScene 함수를 통해 NURI4.scene으로 씬 전환
    }

    public void ToDUNGEON1()
    {
        SceneManager.LoadScene("BUILDING 1");
        //SceneManager 메서드의 LoadScene 함수를 통해 BUILDING 1.scene으로 씬 전환
    }
}
