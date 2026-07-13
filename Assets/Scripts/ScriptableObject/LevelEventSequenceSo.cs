using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName ="NewEventSequence", menuName = "Game/ Event Sequence")]
public class LevelEventSequenceSo : ScriptableObject
{
    public string sequenceName;
    [TextArea] public string description;


    public float startTime = 0;
    public bool shouldOtherEventsGoTORest = false;

    public WaweType sequenceBaseOn;
    public float durationIfTimeBase;
    public int countDownIfUnitBase;
    public RespawnType respawnTypeCountDownIfUnitBase;

    public List<eventData> eventDatas = new List<eventData>();


}

[System.Serializable]
public class eventData
{
    public EventType eventType;

    public float startTimeAfterEventStarted;
    public float duration;

    public RespawnBox respawnBox;

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