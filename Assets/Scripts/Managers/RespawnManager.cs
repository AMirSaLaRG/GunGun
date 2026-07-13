using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class RespawnManager : MonoBehaviour
{
    PlayerController player;

    [Header("Setup")]
    private List<RespawnData> respawns = new List<RespawnData>();
    private int numActiveBoxes;
    private Vector2Int minMaxAvailableRespawns = new Vector2Int(1, 1);

    [Header("RandomRespawnSetup")]
    private Vector2 RespawnTime = new Vector2(1, 3);

    [Header("basicSetup")]
    [SerializeField] private List<RespawnData> respawnsBasicSetup = new List<RespawnData>();

    [Header("Prefabs")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject hostagePrefab;

    public System.Action jobDone;

    private RespawnBox[] myRespawnBoxes;
    private List<RespawnBox> activeEmptyBoxes = new List<RespawnBox>();
    private List<RespawnBox> boxes = new List<RespawnBox>();
    private List<RespawnBox> deActiveBoxes = new List<RespawnBox>();
    private List<Target> eventTracker = new List<Target>();

    private bool isOnRandomRespawn = false;

    private int CountDown = 10;
    private bool shouldCountDown = true;
    public bool isSummonAll { private set; get; } = false;
    private RespawnType countDownType = RespawnType.BaseEnemy;

    private float nextTimeToRespawn = 0;
    private bool wasLastRespawnHostage = false;

    private bool onEventMode;
    private bool isEventClear;
    private bool isEventDataExecuted;
    public System.Action onEventEnded;
    private float eventStartTime;
    
    private void Awake()
    {
        player = FindFirstObjectByType<PlayerController>();
        myRespawnBoxes = GetComponentsInChildren<RespawnBox>();

        if (myRespawnBoxes == null || myRespawnBoxes.Length == 0)
        {
            Debug.LogError("NO RESPAWN BOXES FOUND! Disabling RespawnManager.");
            enabled = false; // Stop the script from running
            return;
        }

        if (player == null)
        {
            Debug.Log("CouldNotFindPlayer");
            return;
        }

        foreach (var box in myRespawnBoxes)
            box.SetPlayer(player);

        boxes = myRespawnBoxes.ToList();
        deActiveBoxes = myRespawnBoxes.ToList();

    }

    private void Update()
    {
        if (onEventMode)
            return;

        if (isOnRandomRespawn == false)
            return;

        if (shouldCountDown && isSummonAll)
            return;



        RespawnRandomInterval();

    }

    public void ExecuteEvent(LevelEventSequenceSo sq)
    {
        eventStartTime = Time.time;
        onEventMode = true;

        StartCoroutine(EventCo(sq.eventDatas));
    }

    private IEnumerator EventCo(List<eventData> datas)
    {
        isEventDataExecuted = false;

        foreach (var data in datas)
        {
            yield return new WaitForSeconds(data.startTimeAfterEventStarted);
            ExecuteEventData(data);
        }

        isEventDataExecuted = true;
    }

    private void ExecuteEventData(eventData data)
    {
        string myBoxName = data.respawnBoxName;

        RespawnBox myBox = myRespawnBoxes.FirstOrDefault(x => x.gameObject.name == myBoxName);

        if (myBox == null)
        {
            Debug.Log("This Name is Not on THis scene plz Change it : __" + myBoxName);
            return;
        }

        RespawnData myRespawn = data.respawnData;
        GameObject myPrefab = myRespawn.prefab;

        if (myPrefab == null)
        {
            myPrefab = respawnsBasicSetup.Find(x => x.respawnType == myRespawn.respawnType).prefab;
        }

        ActiveBox(myBox);

        Target myTarget = myBox.RespawnRandomSide(myPrefab);

        myTarget.SetMyBox(myBox);
        myTarget.SetMyRespawnManager(this);

        if (data.duration != 0)
            myTarget.SetMyDuration(data.duration);

        eventTracker.Add(myTarget);

        isEventClear = false;

        myTarget.atEndAction += OnRespawnLeftTheBox;

    }

    public void SetupRandom(List<RespawnData> newRespawns, int newActiveBoxes,
        Vector2Int newMinMaxParallarRespawns,
        int newCountDown = 0, RespawnType newCountDownType = RespawnType.BaseEnemy)
    {
        respawns.Clear();
        respawns.AddRange(newRespawns);

        numActiveBoxes = newActiveBoxes;
        ActivateRandomBoxes(numActiveBoxes);


        minMaxAvailableRespawns = newMinMaxParallarRespawns;

        if (newCountDown != 0)
        {
            CountDown = newCountDown;
            countDownType = newCountDownType;
            shouldCountDown = true;
        }
        else
        {
            shouldCountDown = false;
        }

        isOnRandomRespawn = true;
    }

    public void BreakRespawn()
    {
        isOnRandomRespawn = false;
    }

    private void RespawnRandomInterval()
    {
        if (Time.time > nextTimeToRespawn)
        {
            bool isRespawned = RandomRespawn();

            if (isRespawned)
            {
                float cooldown = Random.Range(RespawnTime.x, RespawnTime.y);
                nextTimeToRespawn = Time.time + cooldown;
            }
            else
                nextTimeToRespawn = Time.time + (RespawnTime.x == 0 ? 1 : RespawnTime.x);

        }
    }

    private bool RandomRespawn()
    {
        int targetRespawnNum = Random.Range(minMaxAvailableRespawns.x, minMaxAvailableRespawns.y);
        for (int i = 0; i < targetRespawnNum; i++)
        {
            if (shouldCountDown && isSummonAll)
                break;

            bool isRespawned = REspawnRandomly();
            if (isRespawned == false)
                return false;
        }
        return true;
    }

    private bool REspawnRandomly()
    {

        RespawnData prefabToRespaw = GetRandomPrefab();

        if (prefabToRespaw == null)
            return false;

        RespawnSingleTargetRandomly(prefabToRespaw);

        return true;
    }

    private void RespawnSingleTargetRandomly(RespawnData respawnData)
    {
        RespawnBox box = ChoseRandomEmptyBox(false);
        GameObject prefab;

        if (activeEmptyBoxes.Contains(box) == false)
            return;

        if (respawnData.prefab == null)
        {
            prefab = enemyPrefab;
            if (shouldCountDown)
                CheckForCountDown(RespawnType.BaseEnemy);
        }
        else
        {
            prefab = respawnData.prefab;
            if (shouldCountDown)
                CheckForCountDown(respawnData.respawnType);
        }

        Target newTarget = box.RespawnRandomSide(prefab);
        newTarget.SetMyBox(box);
        newTarget.SetMyRespawnManager(this);

        newTarget.atEndAction += OnRespawnLeftTheBox;

        activeEmptyBoxes.Remove(box);
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

    private void ActivateRandomBoxes(int newNum)
    {
        int currentNum = activeEmptyBoxes.Count;
        int num = newNum - currentNum;
        if (num <= 0)
            return;

        if (num > deActiveBoxes.Count)
        {
            Debug.Log("There is No More Available Boxes");
            return;
        }

        List<RespawnBox> newActiveBoxes = new List<RespawnBox>();
        newActiveBoxes = GetRandomBox(num, deActiveBoxes);

        foreach (var box in newActiveBoxes)
        {
            box.ActiveThisBox();
            activeEmptyBoxes.Add(box);

            if (newActiveBoxes.Contains(box))
                deActiveBoxes.Remove(box);
        }
    }

    private void ActiveBox(RespawnBox box)
    {
        if (activeEmptyBoxes.Contains(box))
            return;
        box.ActiveThisBox();

        activeEmptyBoxes.Add(box);
        deActiveBoxes.Remove(box);
    }

    private List<RespawnBox> GetRandomBox(int num, List<RespawnBox> availableBoxes)
    {
        List<RespawnBox> returnBoxes = new List<RespawnBox>();

        int numBoxes = availableBoxes.Count;

        foreach (var i in HashRandomSelection(numBoxes, num))
        {
            returnBoxes.Add(availableBoxes[i]);
        }
        return returnBoxes;
    }

    private List<int> HashRandomSelection(int Range, int returnNum)
    {
        HashSet<int> resaults = new HashSet<int>();

        while (resaults.Count < returnNum)
        {
            int num = Random.Range(0, Range);
            resaults.Add(num);
        }

        return resaults.ToList();
    }

    [ContextMenu("ActivateTestOneWindow")]
    public void Test()
    {
        ActivateRandomBoxes(1);
    }

    private RespawnBox ChoseRandomEmptyBox(bool mustMulti)
    {
        if (activeEmptyBoxes.Count == 0)
            return null;

        if (mustMulti == false)
        {
            int randomIndex = Random.Range(0, activeEmptyBoxes.Count);
            return activeEmptyBoxes[randomIndex];
        }
        else if (mustMulti == true)
        {

            List<RespawnBox> EmptyBoxesMulty = GetMultiEmptyList();
            int randomIndex = Random.Range(0, EmptyBoxesMulty.Count);
            return EmptyBoxesMulty[randomIndex];
        }

        return null;


    }
    private List<RespawnBox> GetMultiEmptyList()
    {
        List<RespawnBox> EmptyBoxesMulty = new List<RespawnBox>();
        foreach (var box in activeEmptyBoxes)
        {
            if (box.isSingleSummon == false)
                EmptyBoxesMulty.Add(box);
        }

        return EmptyBoxesMulty;
    }


    private RespawnData GetRandomPrefab()
    {

        float random = Random.Range(0f, 1f);

        float currentChance = 0;


        foreach (var possibleRespawn in respawns)
        {
            currentChance += possibleRespawn.respawnProb;

            if (random < currentChance)
            {
                if (possibleRespawn.respawnType == RespawnType.Hostage && wasLastRespawnHostage)
                {
                    wasLastRespawnHostage = false;
                    return null;
                }

                wasLastRespawnHostage = possibleRespawn.respawnType == RespawnType.Hostage;
                

            
                return possibleRespawn;
            }           
        }

        return null;
    }


    public void OnRespawnLeftTheBox(RespawnBox respawnBox, Target target)
    {
        StartCoroutine(onRepawnLefCo(respawnBox, target));

    }

    private IEnumerator onRepawnLefCo(RespawnBox resBox, Target target)
    {
        yield return new WaitForSeconds(1f);
        
        if (onEventMode)
        {
            eventTracker.Remove(target);

            if (eventTracker.Count == 0)
                isEventClear = true;

            if (isEventClear && isEventDataExecuted)
            {
                onEventMode = false;
                onEventEnded?.Invoke();
            }
        }

        activeEmptyBoxes.Add(resBox);
    }


}


