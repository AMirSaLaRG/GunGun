using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class RespawnManager : MonoBehaviour
{
    [Header("Setup")]
    [Header("RandomRespawnSetup")]
    [SerializeField] private Vector2 RespawnTime = new Vector2(1, 3);
    [SerializeField] [Range(0f, 1f)] float hostageRespawnChance;
    [SerializeField] [Range(0f, 1f)] float DoubleChanceEnemy;
    [SerializeField] [Range(0f, 1f)] float DoubleChanceHostage;
    [SerializeField] private int startingActiveBoxes;
    [Header("NumberOfRespawns")]
    [SerializeField] private Vector2Int minMaxAvailableRespawns = new Vector2Int(1, 1);
    [SerializeField] private List<RespawnData> Respawns = new List<RespawnData>();

    [Header("Prefabs")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject hostagePrefab;

    private RespawnBox[] myRespawnBoxes;
    private List<RespawnBox> activeEmptyBoxes = new List<RespawnBox>();
    private List<RespawnBox> boxes = new List<RespawnBox>();
    private List<RespawnBox> deActiveBoxes = new List<RespawnBox>();

    private List<RespawnBox> doubleSpawnBoxes = new List<RespawnBox>();

    private float nextTimeToRespawn = 0;
    private bool isOnRandomRespawn = true;
    private bool wasLastRespawnHostage = false;

    PlayerController player;

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

        ActivateRandomBoxes(startingActiveBoxes);
    }

    private void Update()
    {

        if (isOnRandomRespawn == false)
            return;

        RespawnRandomInterval();

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

    private void ActivateRandomBoxes(int num)
    {
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

    [ContextMenu("ActivateTestOneWindow")]
    public void Test()
    {
        ActivateRandomBoxes(1);
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




    private bool RandomRespawn()
    {
        int targetRespawnNum = Random.Range(minMaxAvailableRespawns.x, minMaxAvailableRespawns.y);
        for (int i = 0; i < targetRespawnNum; i++)
        {
            bool isRespawned = RespawnOn();
            if (isRespawned == false)
                return false;
        }
        return true;
    }

    private bool RespawnOn()
    {


        GameObject prefabToRespaw = GetRandomPrefab();
        if (prefabToRespaw == null)
            return false;

        if (prefabToRespaw == hostagePrefab)
        {
            if (RollForDouble(DoubleChanceHostage))
                RespawnDoubleEnemyAndHostage();
            else
                RespawnSingleHostage();

        }
        else
        {
            if (RollForDouble(DoubleChanceEnemy))
                RespawnDoubleEnemy();
            else
                RespawnSingleEnemy();
        }

        return true;
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


    private bool RollForDouble(float chance) => Random.Range(0f, 1f) < chance;


    private void RespawnSingleEnemy()
    {
        RespawnBox box = ChoseRandomEmptyBox(false);

        if (activeEmptyBoxes.Contains(box) == false)
            return;

        wasLastRespawnHostage = false;

        Target newTarget = box.RespawnRandomSide(enemyPrefab);
        Enemy newTargetData = newTarget.GetComponent<Enemy>();
        newTargetData.SetMyBox(box);
        newTargetData.SetMyRespawnManager(this);
        newTargetData.SetDanceMode(true);
        newTarget.atEndAction += OnRespawnLeftTheBox;

        activeEmptyBoxes.Remove(box);

    }
    private void RespawnSingleHostage()
    {
        RespawnBox box = ChoseRandomEmptyBox(false);

        if (activeEmptyBoxes.Contains(box) == false)
            return;

        wasLastRespawnHostage = true;

        Target newTarget = box.RespawnRandomSide(hostagePrefab);
        newTarget.SetMyBox(box);
        newTarget.SetMyRespawnManager(this);
        newTarget.atEndAction += OnRespawnLeftTheBox;

        activeEmptyBoxes.Remove(box);

    }

    private void RespawnDoubleEnemyAndHostage()
    {
        RespawnBox box = ChoseRandomEmptyBox(true);

        if (activeEmptyBoxes.Contains(box) == false)
            return;

        wasLastRespawnHostage = true;


        Target[] newTargets = box.DoubleRespawnRandomSide(hostagePrefab, enemyPrefab);
        foreach (var target in newTargets)
        {
            target.SetMyBox(box);
            target.SetMyRespawnManager(this);
            target.atEndAction += OnRespawnLeftTheBox;
        }

        doubleSpawnBoxes.Add(box);
        activeEmptyBoxes.Remove(box);

    }
    private void RespawnDoubleEnemy()
    {
        RespawnBox box = ChoseRandomEmptyBox(true);

        if (activeEmptyBoxes.Contains(box) == false)
            return;

        wasLastRespawnHostage = false;


        Target[] newTargets = box.DoubleRespawnRandomSide(enemyPrefab, enemyPrefab);
        foreach (var target in newTargets)
        {
            target.SetMyBox(box);
            target.SetMyRespawnManager(this);
            target.atEndAction += OnRespawnLeftTheBox;
        }

        doubleSpawnBoxes.Add(box);
        activeEmptyBoxes.Remove(box);

    }

    private GameObject GetRandomPrefab()
    {
        if (wasLastRespawnHostage)
            return enemyPrefab;

        float random = Random.Range(0f, 1f);


        if (random < hostageRespawnChance)
        {
            if (hostagePrefab != null)
                return hostagePrefab;
        }

        if (enemyPrefab != null)
            return enemyPrefab;

        return null;
    }

    public void OnRespawnLeftTheBox(RespawnBox respawnBox)
    {
        StartCoroutine(onRepawnLefCo(respawnBox));

    }

    private IEnumerator onRepawnLefCo(RespawnBox resBox)
    {
        yield return new WaitForSeconds(1f);
        if (doubleSpawnBoxes.Contains(resBox))
            doubleSpawnBoxes.Remove(resBox);
        else
            activeEmptyBoxes.Add(resBox);
    }


}


