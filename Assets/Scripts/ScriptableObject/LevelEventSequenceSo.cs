using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName ="NewEventSequence", menuName = "Game/ Event Sequence")]
public class LevelEventSequenceSo : ScriptableObject
{
    public string sequenceName;
    [TextArea] public string description;


    public float startTime = 0;
    public bool shouldOtherEventsGoTORest = false;



    public List<eventData> eventDatas = new List<eventData>();


}

[System.Serializable]
public class eventData
{
    public EventType eventType;

    public float startTimeAfterEventStarted;
    public float duration;

    public string respawnBoxName;

    public RespawnData respawnData;

}

public enum EventType
{
    SpawnEnemy,
    PlaySound,
    ShowMessage,
    ChangeState,
    TriggerAnimation,
    Custom
}