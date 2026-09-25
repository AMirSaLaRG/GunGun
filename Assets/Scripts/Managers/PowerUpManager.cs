using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PowerUpManager : MonoBehaviour
{
    public WaweManager currentWaveManager {  get; private set; }
    BuildingManager buildingManager;
    PlayerController player;

    [SerializeField] private PowerUpData[] basicData;
    [SerializeField] private GameObject dronePrefab;
    [SerializeField] private float StartAfter = 15f;


    private List<PowerUpData> powerUpsData;
    private Vector2 powerUpRespawnsInterval;


    private float timeForRespawn;

    private void Awake()
    {
        buildingManager = FindFirstObjectByType<BuildingManager>();
        player = FindFirstObjectByType<PlayerController>();

    }

    private void Start()
    {
        timeForRespawn = StartAfter;
    }

    private void Update()
    {
        if (currentWaveManager == null)
            return;

        timeForRespawn -= Time.deltaTime;
        if (timeForRespawn < 0)
            CheckAndRespawnPowerUp();
    }

    private void CheckAndRespawnPowerUp()
    {
        timeForRespawn = Random.Range(powerUpRespawnsInterval[0], powerUpRespawnsInterval[1]);

        ESkillType powerUpType = CheckPowerUp();

        Debug.Log("Respawning" + powerUpType);

        RespawnPowerUp(powerUpType);

    }

    private void RespawnPowerUp(ESkillType eSkillType)
    {
        SummonDrone(eSkillType);

    }

    private void SummonDrone(ESkillType skill)
    {
        if (skill == ESkillType.None) return;

        Transform[] wayPoints = GetWayPoints();

        if (wayPoints == null || wayPoints.Length == 0)
        {
            Debug.LogWarning("Could Not Get waypoints");
            return;
        }

        Vector3 startPoint = wayPoints[0].position;
                

        PowerUpData currentBasicData = null;

        foreach (var data in basicData)
           if (data.powerUpSkill == skill)
            {
                currentBasicData = data;
                break;
            }


        Drone_PowerUp drone = Instantiate(dronePrefab, startPoint, Quaternion.identity, transform).GetComponent<Drone_PowerUp>();

        drone.SetUp(player, skill, wayPoints, currentBasicData.myColor);

    }

    private Transform[] GetWayPoints()
    {
        PowerUpWayPointsSet wayPointSet = buildingManager.GetCurrentWayPointSet();

        Transform[] resault = new Transform[3];

        Transform[] startAndEnd =  wayPointSet.GetStartAndEndOfSet();
        Transform[] stopPoints = wayPointSet.GetStopPointsOfSet();

        if (startAndEnd.Length <= 1)
        {
            Debug.Log("start and end less than 1");
            return null;
        }
        if (stopPoints.Length <= 0)
        {
            Debug.Log("No Stop point");
            return null;
        }

        int randomindexStartStop = Random.Range(0, 1);

        resault[0] = startAndEnd[randomindexStartStop];
        resault[2] = startAndEnd[randomindexStartStop == 0 ? 1 : 0];

        int randomStopPointIndex = Random.Range(0, stopPoints.Length);
        resault[1] = stopPoints[randomStopPointIndex];

        return resault;
            

    }

    private ESkillType CheckPowerUp()
    {
        float chance = Random.Range(0f, 1f);

        ESkillType powerUpType = GetPowerUpTypeByChance(chance);
        return powerUpType;
    }

    private ESkillType GetPowerUpTypeByChance(float chance)
    {
        float currentItemChance = 0f;

        foreach (var powerUp in powerUpsData)
        {
            currentItemChance += powerUp.powerUpChance;
            if (currentItemChance > chance)
                return powerUp.powerUpSkill;
        }

        return ESkillType.None;
    }

    internal void SetWaveManager(WaweManager waveManager)
    {
        currentWaveManager = waveManager;
        powerUpsData = waveManager.GetPowerUpData();
        powerUpRespawnsInterval = waveManager.GetPowerUpRespawnInterval();
        ClearChances();
    }

    private void ClearChances()
    {
        float totalChance = 0f;
        float normalizer = 1f;

        foreach (var chance in powerUpsData)
        {
            totalChance += chance.powerUpChance;
        }

        if (totalChance < 1)
            powerUpsData.Add(new PowerUpData { powerUpChance = 1 - totalChance, powerUpSkill = ESkillType.None });

        if (totalChance > 1f)
            normalizer = 1 / totalChance;

        foreach (var chance in powerUpsData)
        {
            chance.powerUpChance *= normalizer;
        }
    }

    internal void WaveStoped()
    {
        currentWaveManager = null;
        timeForRespawn = StartAfter;
    }
}
