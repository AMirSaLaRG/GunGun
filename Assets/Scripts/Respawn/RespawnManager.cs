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
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject hostagePrefab;

    private RespawnBox[] myRespawnBoxes;
    private List<RespawnBox> EmptyBoxes = new List<RespawnBox>();
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

        EmptyBoxes = myRespawnBoxes.ToList();
    }


    private void Update()
    {

        if (isOnRandomRespawn == false)
            return;
        if (Time.time > nextTimeToRespawn)
        {
            bool isRespawned = RandomRespawn();
            
            if (isRespawned)
            {
                float cooldown = Random.Range(RespawnTime.x, RespawnTime.y);
                nextTimeToRespawn = Time.time + cooldown;
            }
            else
                nextTimeToRespawn = Time.time + (RespawnTime.x == 0? 1 : RespawnTime.x);
            
        }

    }


    private bool RandomRespawn()
    {

        
        return RespawnOn();
    }
    private RespawnBox ChoseRandomEmptyBox(bool mustMulti)
    {
        if (EmptyBoxes.Count == 0)
            return null;

        if (mustMulti == false)
        {
            int randomIndex = Random.Range(0, EmptyBoxes.Count);
            return EmptyBoxes[randomIndex];
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
        foreach (var box in EmptyBoxes)
        {
            if (box.isSingleSummon == false)
                EmptyBoxesMulty.Add(box);
        }

        return EmptyBoxesMulty;
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

    private bool RollForDouble(float chance) => Random.Range(0f, 1f) < chance;


    private void RespawnSingleEnemy()
    {
        RespawnBox box = ChoseRandomEmptyBox(false);

        if (EmptyBoxes.Contains(box) == false)
            return;

        wasLastRespawnHostage = false;

        Target newTarget = box.RespawnRandomSide(enemyPrefab);
        Enemy newTargetData = newTarget.GetComponent<Enemy>();
        newTargetData.SetMyBox(box);
        newTargetData.SetMyRespawnManager(this);
        newTargetData.SetDanceMode(true);
        newTarget.atEndAction += OnRespawnLeftTheBox;

        EmptyBoxes.Remove(box);

    }
    private void RespawnSingleHostage()
    {
        RespawnBox box = ChoseRandomEmptyBox(false);

        if (EmptyBoxes.Contains(box) == false)
            return;

        wasLastRespawnHostage = true;

        Target newTarget = box.RespawnRandomSide(hostagePrefab);
        newTarget.SetMyBox(box);
        newTarget.SetMyRespawnManager(this);
        newTarget.atEndAction += OnRespawnLeftTheBox;

        EmptyBoxes.Remove(box);

    }

    private void RespawnDoubleEnemyAndHostage()
    {
        RespawnBox box = ChoseRandomEmptyBox(true);

        if (EmptyBoxes.Contains(box) == false)
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
        EmptyBoxes.Remove(box);

    }
    private void RespawnDoubleEnemy()
    {
        RespawnBox box = ChoseRandomEmptyBox(true);

        if (EmptyBoxes.Contains(box) == false)
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
        EmptyBoxes.Remove(box);

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
            EmptyBoxes.Add(resBox);
    }


}


