using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Choice : MonoBehaviour
{
    public string name;  // NPC 이름
    public string question;   // 질문
    public string[] answers;  // 답변
}
