using System;
using UnityEngine;

[Serializable]
public class SerializableNPCData
{
    public string NPCName;
}

[CreateAssetMenu(fileName = "NPC", menuName = "New NPC")]
public class NPCData : ScriptableObject
{
    public string NPCName;

    public SerializableNPCData ToSerializable()
    {
        return new SerializableNPCData
        {
            NPCName = NPCName,
        };
    }
}
