using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class NPCObjct : MonoBehaviour, IInteractable
{
    public NPCData npcData;
    public string GetInteractPrompt()
    {
        Debug.Log(npcData.NPCName);
        return string.Format("{0}과(와) 대화하기", npcData.NPCName);
    }

    public void OnInteract()
    {
        Debug.Log("NPC 대화");
    }
}
