using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    PlayerController player;

    [Header("Setup")]
    public List<RespawnData> respawnsInfo = new List<RespawnData>();
    private Vector2Int minMaxAvailableRespawns = new Vector2Int(1, 1);

    [Header("RandomRespawnSetup")]
    private Vector2 respawnTime = new Vector2(1, 3);

    [Header("basicSetup")]
    [SerializeField] private GameObject basicEnemyPrefab;
    [SerializeField] private GameObject enemyWithHostagePrefab;
    [SerializeField] private GameObject HostagePrefab;
    private List<RespawnData> respawnsBasicSetup = new List<RespawnData>();

    [Header("Prefabs")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject hostagePrefab;

    public System.Action jobDone;

    private RespawnBox[] myRespawnBoxes;
    private List<RespawnBox> activeEmptyBoxes = new List<RespawnBox>();
    private List<RespawnBox> deActiveBoxes = new List<RespawnBox>();
    private List<Target> targetTracker = new List<Target>();

    private bool isOnRandomRespawn = false;

    private int CountDown = 10;
    private bool shouldCountDown = true;
    public bool isSummonAll { private set; get; } = false;
    private RespawnType countDownType = RespawnType.BaseEnemy;

    private float nextTimeToRespawn = 0;
    private bool wasLastRespawnHostage = false;

    private bool onEventMode;
    public bool isSceenClear { private set; get; }
    private bool isEventEnded;
    public System.Action onEventEnded;
    
    private void Awake()
    {
        CreateBasicRepspawnSetUpByPrefabs();
        GetAndCheckElements();
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

    public void SetupRandom(List<RespawnData> newRespawns, int newActiveBoxes,
        Vector2 newMinMaxIntervalSummon,
        Vector2Int newMinMaxParallarRespawns,
        int newCountDown = 0, RespawnType newCountDownType = RespawnType.BaseEnemy)
    {
        ReplcaeRespawnInfo(newRespawns);

        SetNewRespawnTime(newMinMaxIntervalSummon);

        ActivateRandomBoxes(newActiveBoxes);

        SetNewRespawnNumber(newMinMaxParallarRespawns);

        CheckAndSetRespawnBase(newCountDown, newCountDownType);

        isOnRandomRespawn = true;
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

            bool isRespawned = REspawnRandomly();
            if (isRespawned == false)
                return false;
        }
        return true;
    }

    private bool REspawnRandomly()
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
        RespawnData basicEnemyData = respawnsBasicSetup.Find(x => x.respawnType == RespawnType.BaseEnemy);

        foreach (var possibleRespawn in respawnsInfo)
        {
            currentChance += possibleRespawn.respawnProb;

            if (random <= currentChance)
            {
                resault = possibleRespawn;

                if (resault.respawnType == RespawnType.Hostage && wasLastRespawnHostage)
                    resault = basicEnemyData;

                wasLastRespawnHostage = resault.respawnType == RespawnType.Hostage;

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
        RespawnBox myBox = GetBoxByName(data.respawnBoxName);

        if (myBox == null)
        {
            Debug.Log("This Name is Not on THis scene plz Change it : __" + data.respawnBoxName);

            return;
        }
        
        ActiveBox(myBox);

        Target newTarget = RespawnTargetOn(data.respawnData, myBox, data.duration);

        if (newTarget == null)
            Debug.Log("Could not respawn on box " +  newTarget.name + "box: " + myBox.gameObject.name);

    }

    public void BreakRespawn()
    {
        isOnRandomRespawn = false;
        activeEmptyBoxes.Clear();
    }

    private Target RespawnTargetOn(RespawnData targetData, RespawnBox respawnBox, float duration = 0)
    {
        GameObject myPrefab = targetData.prefab;

        if (myPrefab == null)
        {
            myPrefab = respawnsBasicSetup.Find(x => x.respawnType == targetData.respawnType).prefab;
        }


        Target myTarget = respawnBox.RespawnRandomSide(myPrefab);

        myTarget.SetMyBox(respawnBox);
        myTarget.SetMyRespawnManager(this);

        if (duration != 0)
            myTarget.SetMyDuration(duration);

        targetTracker.Add(myTarget);

        isSceenClear = false;

        myTarget.atEndAction += OnRespawnLeftTheBox;

        return myTarget;
    }

    private void RespawnTargetRandomly(RespawnData respawnData)
    {
        GameObject prefab = respawnData.prefab;
        
        if (prefab == null)
            prefab = respawnsBasicSetup.Find(x => x.respawnType == respawnData.respawnType).prefab;

        if (shouldCountDown)
            CheckForCountDown(respawnData.respawnType);

        RespawnBox box = ChoseRandomEmptyBox(false);

        if (activeEmptyBoxes.Contains(box) == false)
            return;

        Target newTarget = RespawnPrefabOn(prefab, box);

        if (respawnData.respawnType == RespawnType.Hostage)
        {
            Hostage newHostage = newTarget.GetComponent<Hostage>();
            targetTracker.Remove(newHostage);

            RespawnEnemyWithHostage(box, newHostage);

        }
    }

    private Target RespawnPrefabOn(GameObject prefab, RespawnBox box, bool usingLastSide = false)
    {


        Target newTarget = box.RespawnRandomSide(prefab, usingLastSide);

        targetTracker.Add(newTarget);
        isSceenClear = false;

        newTarget.SetMyBox(box);
        newTarget.SetMyRespawnManager(this);

        newTarget.atEndAction += OnRespawnLeftTheBox;

        if (activeEmptyBoxes.Contains(box))
            activeEmptyBoxes.Remove(box);

        return newTarget;
    }
    
    private void RespawnEnemyWithHostage(RespawnBox box, Hostage hostage)
    {
        Target newEnemyWithHostage = RespawnPrefabOn(enemyWithHostagePrefab, box, true);
        EnemyWithHostage enemyWithHostage = newEnemyWithHostage.GetComponent<EnemyWithHostage>();

        enemyWithHostage.Setup(hostage);
        hostage.Setup(enemyWithHostage);
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
    private RespawnBox GetBoxByName(string name)
    {
        RespawnBox myBox;

        myBox = myRespawnBoxes.FirstOrDefault(x => x.gameObject.name == name);

        return myBox;
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

    private void ActivateRandomBoxes(int newNum)
    {
        int currentNum = myRespawnBoxes.Length - deActiveBoxes.Count;
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

    public void DeActiveAllBoxes()
    {
        foreach (var box in myRespawnBoxes)
            box.DeActivateThisBox();

        activeEmptyBoxes.Clear();

        deActiveBoxes.Clear();
        deActiveBoxes.AddRange(myRespawnBoxes);

        targetTracker.Clear();


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

    public void OnRespawnLeftTheBox(RespawnBox respawnBox, Target target)
    {
        StartCoroutine(onRepawnLefCo(respawnBox, target));

    }

    private IEnumerator onRepawnLefCo(RespawnBox resBox, Target target)
    {
        yield return new WaitForSeconds(1f);

        targetTracker.Remove(target);


        if (targetTracker.Count == 0)
            isSceenClear = true;

        if (onEventMode)
        {
            if (isSceenClear && isEventEnded)
            {
                onEventMode = false;
                onEventEnded?.Invoke();
            }
        }

        if (activeEmptyBoxes.Contains(resBox) == false)
            activeEmptyBoxes.Add(resBox);
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

    private void SetNewRespawnNumber(Vector2Int newMinMaxParallarRespawns)
    {
        minMaxAvailableRespawns = newMinMaxParallarRespawns;
        if (minMaxAvailableRespawns == Vector2Int.zero)
            minMaxAvailableRespawns = new Vector2Int(1, 1);
    }

    private void SetNewRespawnTime(Vector2 newMinMaxIntervalSummon)
    {
        respawnTime = newMinMaxIntervalSummon;
        if (respawnTime == Vector2.zero)
            respawnTime = new Vector2(1, 1);
    }

    private void ReplcaeRespawnInfo(List<RespawnData> newRespawns)
    {
        respawnsInfo.Clear();
        respawnsInfo.AddRange(newRespawns);
    }

    private void GetAndCheckElements()
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

        deActiveBoxes = myRespawnBoxes.ToList();
    }

    private void CreateBasicRepspawnSetUpByPrefabs()
    {
        RespawnData basicEnemy = new RespawnData();
        basicEnemy.prefab = basicEnemyPrefab;
        basicEnemy.respawnType = RespawnType.BaseEnemy;
        respawnsBasicSetup.Add(basicEnemy);


        RespawnData Hostage = new RespawnData();
        Hostage.prefab = HostagePrefab;
        Hostage.respawnType = RespawnType.Hostage;
        respawnsBasicSetup.Add(Hostage);
    }
}


