using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;
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

    private void Awake()
    {
        myRespawnBoxes = GetComponentsInChildren<RespawnBox>();

        if (myRespawnBoxes == null || myRespawnBoxes.Length == 0)
        {
            Debug.LogError("NO RESPAWN BOXES FOUND! Disabling RespawnManager.");
            enabled = false; // Stop the script from running
            return;
        }

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
        RespawnBox box = ChoseRandomEmptyBox();
        if (box == null)
            return false;
        
        return RespawnOn(box);
    }
    private RespawnBox ChoseRandomEmptyBox()
    {
        if (EmptyBoxes.Count == 0)
            return null;
        int randomIndex = Random.Range(0, EmptyBoxes.Count);
        return EmptyBoxes[randomIndex];
    }

    private bool RespawnOn(RespawnBox box)
    {
        if (EmptyBoxes.Contains(box) == false)
            return false;

        GameObject prefabToRespaw = GetRandomPrefab();
        if (prefabToRespaw == null)
            return false;

        
        if (prefabToRespaw == hostagePrefab)
        {
            if (RollForDouble(DoubleChanceHostage))
                RespawnDoubleEnemyAndHostage(box);
            else
                RespawnSingleHostage(box);

        }
        else
        {
            if (RollForDouble(DoubleChanceEnemy))
                RespawnDoubleEnemy(box);
            else
                RespawnSingleEnemy(box);
        }



        EmptyBoxes.Remove(box);

        return true;
    }

    private bool RollForDouble(float chance) => Random.Range(0f, 1f) < chance;


    private void RespawnSingleEnemy(RespawnBox box)
    {
        Target newTarget = box.RespawnRandomSide(enemyPrefab);
        Enemy newTargetData = newTarget.GetComponent<Enemy>();
        newTargetData.SetMyBox(box);
        newTargetData.SetMyRespawnManager(this);
        newTargetData.SetDanceMode(true);
        newTarget.atEndAction += OnRespawnLeftTheBox;
    }
    private void RespawnSingleHostage(RespawnBox box)
    {
        Target newTarget = box.RespawnRandomSide(hostagePrefab);
        newTarget.SetMyBox(box);
        newTarget.SetMyRespawnManager(this);
        newTarget.atEndAction += OnRespawnLeftTheBox;
    }

    private void RespawnDoubleEnemyAndHostage(RespawnBox box)
    {
        Target[] newTargets = box.DoubleRespawnRandomSide(hostagePrefab, enemyPrefab);
        foreach (var target in newTargets)
        {
            target.SetMyBox(box);
            target.SetMyRespawnManager(this);
            target.atEndAction += OnRespawnLeftTheBox;
        }
        doubleSpawnBoxes.Add(box);
    }
    private void RespawnDoubleEnemy(RespawnBox box)
    {
        Target[] newTargets = box.DoubleRespawnRandomSide(enemyPrefab, enemyPrefab);
        foreach (var target in newTargets)
        {
            target.SetMyBox(box);
            target.SetMyRespawnManager(this);
            target.atEndAction += OnRespawnLeftTheBox;
        }
        doubleSpawnBoxes.Add(box);
    }

    private GameObject GetRandomPrefab()
    {
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
        if (doubleSpawnBoxes.Contains(respawnBox))
            doubleSpawnBoxes.Remove(respawnBox);
        else
            EmptyBoxes.Add(respawnBox);
    }



}


