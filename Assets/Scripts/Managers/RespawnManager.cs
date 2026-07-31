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
    }

    private Target RespawnTargetOn(RespawnData targetData, RespawnBox respawnBox, float duration = 0)
    {
        GameObject myPrefab = targetData.prefab;

        if (myPrefab == null)
            myPrefab = unitManager.GetBasicUnitData(targetData.respawnType).prefab;

        Target myTarget = respawnBox.RespawnRandomSide(myPrefab);

        myTarget.SetMyBox(respawnBox);
        myTarget.SetMyRespawnManager(this);

        if (duration != 0)
            myTarget.SetMyDuration(duration);

        unitManager.TrackTarget(myTarget);

        isSceenClear = false;


        return myTarget;
    }

    private void RespawnTargetRandomly(RespawnData respawnData)
    {
        GameObject prefab = respawnData.prefab;

        if (prefab == null)
            prefab = unitManager.GetBasicUnitData(respawnData.respawnType).prefab;

        if (shouldCountDown)
            CheckForCountDown(respawnData.respawnType);

        bool isTakerWithHostage = respawnData.respawnType == RespawnType.TakerWithHostage;

        RespawnBox box = boxManager.GetBox(isTakerWithHostage);
        Debug.Log(box == null);
        if (box == null)
            return;
        Target newTarget = RespawnTargetOn(prefab, box);

        if (respawnData.respawnType == RespawnType.Hostage)
            RespawnTargetRandomly(unitManager.GetBasicUnitData(RespawnType.BaseEnemy));

        if (isTakerWithHostage)
            SetUpHostageTaker(box, newTarget);
        
    }

    private void SetUpHostageTaker(RespawnBox box, Target newTarget)
    {
        EnemyWithHostage taker = newTarget.GetComponent<EnemyWithHostage>();
        GameObject hostagePrefab = currentRespawnInfo.Find(x => x.respawnType == RespawnType.Hostage).prefab;
        if (hostagePrefab == null)
            hostagePrefab = unitManager.GetBasicUnitData(RespawnType.Hostage).prefab;

        Hostage newHostage = RespawnTargetOn(hostagePrefab, box).GetComponent<Hostage>();

        unitManager.RemoveTrack(newHostage);
        taker.Setup(newHostage);
        newHostage.Setup(taker);
    }

    private Target RespawnTargetOn(GameObject prefab, RespawnBox box, bool usingLastSide = false)
    {
        Target newTarget = box.RespawnRandomSide(prefab, usingLastSide);

        unitManager.TrackTarget(newTarget);
        isSceenClear = false;

        newTarget.SetMyBox(box);
        newTarget.SetMyRespawnManager(this);

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

        ReplcaeRespawnInfo(newRespawns);

        SetNewRespawnTime(newMinMaxIntervalSummon);

        boxManager.ActivateRandomBoxes(newActiveBoxes);

        SetNewRespawnNumber(newMinMaxParallarRespawns);

        CheckAndSetRespawnBase(newCountDown, newCountDownType);

        isOnRandomRespawn = true;
    }

    private void ReplcaeRespawnInfo(List<RespawnData> newRespawns)
    {
        currentRespawnInfo.Clear();
        currentRespawnInfo.AddRange(newRespawns);
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


