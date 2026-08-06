using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    private PlayerController player;
    private BoxManager boxManager;
    private UnitManager unitManager;

    [Header("Setup")]
    //public List<RespawnData> respawnInfo = new List<RespawnData>();
    public List<RespawnData> currentRespawnInfo = new List<RespawnData>();
    private Vector2Int minMaxAvailableRespawns = new Vector2Int(1, 1);

    [Header("RandomRespawnSetup")]
    private Vector2 respawnTime = new Vector2(1, 3);

    public System.Action jobDone;

    //private List<Target> targetTracker = new List<Target>();

    private bool isOnRandomRespawn = false;

    private int CountDown = 10;
    private bool shouldCountDown = true;
    public bool isSummonAll { private set; get; } = false;
    private RespawnType countDownType = RespawnType.BaseEnemy;

    private float nextTimeToRespawn = 0;

    private bool onEventMode;
    public bool isSceenClear { private set; get; }
    private bool isEventEnded;

    private int waveActiveBoxes;
    private bool startingNextWawe;

    private void Awake()
    {
        player = FindFirstObjectByType<PlayerController>();


        boxManager = GetComponent<BoxManager>();
        if (boxManager == null )
            gameObject.AddComponent<BoxManager>();

        unitManager = GetComponent<UnitManager>();

    }

    private void Update()
    {
        if (boxManager.ManagingBoxes)
            return;

        if (unitManager.onWaitingRoomForSceneClear)
            return;

        if (startingNextWawe)
        {
            boxManager.ActivateRandomBoxes(waveActiveBoxes);
            startingNextWawe = false;
            return;
        }

        bool flowControl = CheckRandomConditionals();
        if (!flowControl)
        {
            return;
        }

        RespawnRandomInterval();

    }
    public void OnLeavingTheBox(RespawnBox respawnBox, Target boxUser)
    {
        boxManager.ReturnBox(respawnBox);

        isSceenClear = unitManager.RemoveTrack(boxUser); 

    }


    public void SetupBoxes(Transform BoxHolder)
    {
        boxManager.Setup(BoxHolder, player);
    }

    private bool CheckRandomConditionals()
    {
        if (onEventMode)
            return false;

        if (isOnRandomRespawn == false)
            return false;

        if (shouldCountDown && isSummonAll)
            return false;

        return true;
    }

    private void RespawnRandomInterval()
    {
        if (Time.time > nextTimeToRespawn)
        {
            bool isRespawned = RandomRespawn();

            if (isRespawned)
            {
                float cooldown = Random.Range(respawnTime.x, respawnTime.y);
                nextTimeToRespawn = Time.time + cooldown;
            }
            else
                nextTimeToRespawn = Time.time + (respawnTime.x == 0 ? 1 : respawnTime.x);
        }
    }

    private bool RandomRespawn()
    {
        int targetRespawnNum = Random.Range(minMaxAvailableRespawns.x, minMaxAvailableRespawns.y);
        for (int i = 0; i < targetRespawnNum; i++)
        {
            if (shouldCountDown && isSummonAll)
                break;

            bool isRespawned = RespawnRandomly();
            if (isRespawned == false)
                return false;
        }
        return true;
    }

    private bool RespawnRandomly()
    {
        RespawnData respawnToSummon = GetRandomRespawn();
        
        if (respawnToSummon == null)
            return false;

        RespawnTargetRandomly(respawnToSummon);

        return true;
    }

    private RespawnData GetRandomRespawn()
    {

        float random = Random.Range(0f, 1f);

        float currentChance = 0;

        RespawnData resault = null;

        foreach (var possibleRespawn in currentRespawnInfo)
        {
            currentChance += possibleRespawn.respawnProb;

            if (random <= currentChance)
            {
                resault = possibleRespawn;

                return resault;
            }
        }
        return resault;
    }

    public void ExecuteEvent(LevelEventSequenceSo sq)
    {
        onEventMode = true;
        StartCoroutine(EventCo(sq.eventDatas));
    }

    private IEnumerator EventCo(List<eventData> datas)
    {
        isEventEnded = false;

        foreach (var data in datas)
        {
            yield return new WaitForSeconds(data.startTimeAfterEventStarted);
            ExecuteEventData(data);
        }

        isEventEnded = true;
    }

    private void ExecuteEventData(eventData data)
    {
        RespawnBox myBox = boxManager.GetBox(data.respawnBoxName);

        if (myBox == null)
        {
            Debug.Log("This Name is Not on THis scene plz Change it : __" + data.respawnBoxName);

            return;
        }
        

        Target newTarget = RespawnTargetOn(data.respawnData, myBox, data.duration);

        if (newTarget == null)
            Debug.Log("Could not respawn on box " +  newTarget.name + "box: " + myBox.gameObject.name);

    }

    public void BreakRespawn()
    {
        isOnRandomRespawn = false;
    }
    
    public void ClearScene()
    {
        BreakRespawn();
        boxManager.DeActiveAllBoxes();
        boxManager.ResetBoxes();
        unitManager.ClearTheScene();
    }

    private void RespawnTargetRandomly(RespawnData respawnData)
    {
        if (shouldCountDown)
            CheckForCountDown(respawnData.respawnType);

        bool isTakerWithHostage = respawnData.respawnType == RespawnType.TakerWithHostage;

        RespawnBox box = boxManager.GetBox(isTakerWithHostage);
        if (box == null)
            return;
        Target newTarget = RespawnTargetOn(respawnData, box);

        if (respawnData.respawnType == RespawnType.Hostage)
            RespawnTargetRandomly(unitManager.GetBasicUnitData(RespawnType.BaseEnemy));

        if (isTakerWithHostage)
            SetUpHostageTaker(box, newTarget);
        
    }

    private void SetUpHostageTaker(RespawnBox box, Target newTarget)
    {
        EnemyWithHostage taker = newTarget.GetComponent<EnemyWithHostage>();
        

        RespawnData hostageData = currentRespawnInfo.Find(x => x.respawnType == RespawnType.Hostage);


        Hostage newHostage = RespawnTargetOn(hostageData, box).GetComponent<Hostage>();

        unitManager.RemoveTrack(newHostage);
        taker.Setup(newHostage);
        newHostage.Setup(taker);
    }
    private Target RespawnTargetOn(RespawnData targetData, RespawnBox respawnBox, float duration = 0, bool usingLastSide = false)
    {
        GameObject myPrefab = targetData.prefab;

        if (myPrefab == null)
            myPrefab = unitManager.GetBasicUnitData(targetData.respawnType).prefab;

        Target newTarget = respawnBox.RespawnRandomSide(myPrefab, usingLastSide);

        newTarget.SetUpTarget(player, this, respawnBox, targetData.respawnType);

        if (duration != 0)
            newTarget.SetMyDuration(duration);

        unitManager.TrackTarget(newTarget);

        isSceenClear = false;


        return newTarget;
    }   

    private void CheckForCountDown(RespawnType type)
    {
        if (countDownType == type)
            CountDown--;

        if (CountDown <= 0)
        {
            isSummonAll = true;
            jobDone?.Invoke();
        }
    }

    #region SetupRandom
    public void SetupRandom(List<RespawnData> newRespawns,
        int newActiveBoxes,
        Vector2 newMinMaxIntervalSummon,
        Vector2Int newMinMaxParallarRespawns,
        int newCountDown = 0, RespawnType newCountDownType = RespawnType.BaseEnemy)
    {
        waveActiveBoxes = newActiveBoxes;
        startingNextWawe = true;

        unitManager.SetOnWaitingForSceneClear();


        ReplcaeRespawnInfo(newRespawns);

        SetNewRespawnTime(newMinMaxIntervalSummon);

        SetNewRespawnNumber(newMinMaxParallarRespawns);

        CheckAndSetRespawnBase(newCountDown, newCountDownType);

        isOnRandomRespawn = true;
    }

    

    private void ReplcaeRespawnInfo(List<RespawnData> newRespawns)
    {
        currentRespawnInfo.Clear();
        currentRespawnInfo.AddRange(newRespawns);

        foreach (var basicData in unitManager.basicRespawnUnitInfo)
        {
            RespawnData check = currentRespawnInfo.Find(x => x.respawnType == basicData.respawnType);
            if ( check == null)
                currentRespawnInfo.Add(basicData);
        }
    }

    private void SetNewRespawnTime(Vector2 newMinMaxIntervalSummon)
    {
        respawnTime = newMinMaxIntervalSummon;
        if (respawnTime == Vector2.zero)
            respawnTime = new Vector2(1, 1);
    }

    private void SetNewRespawnNumber(Vector2Int newMinMaxParallarRespawns)
    {
        minMaxAvailableRespawns = newMinMaxParallarRespawns;
        if (minMaxAvailableRespawns == Vector2Int.zero)
            minMaxAvailableRespawns = new Vector2Int(1, 1);
    }

    private void CheckAndSetRespawnBase(int newCountDown, RespawnType newCountDownType)
    {
        if (newCountDown == 0)
        {
            shouldCountDown = false;
        }
        else
        {
            CountDown = newCountDown;
            countDownType = newCountDownType;
            shouldCountDown = true;
        }
    }

  
    #endregion
}


