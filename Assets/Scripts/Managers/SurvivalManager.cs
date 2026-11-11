using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SurvivalManager : MonoBehaviour
{

    [Header("Setup")]
    [SerializeField] private Vector2 minMaxWaveTime = new Vector2(20, 40);

    [Header("WaveVariation")]
    [SerializeField, Range(0f, 0.25f)] private float waveVariation = 0.1f;

    [SerializeField, Range(0f, 1f)] private float maximumWaveDifficulty = 0.85f;

    [Space]
    [Header("EnemyRespawnSetup")]

    [Header("MinMaxSummonCountByDifficulty")]
    [SerializeField] private Vector2 numAt0Difficulty;
    [SerializeField] private Vector2 numAt1Difficulty;
    [SerializeField] private AnimationCurve numCurve;

    [Header("MinMaxSummonIntervalByDifficulty")]
    [SerializeField] private Vector2 intervalAt0Difficulty;
    [SerializeField] private Vector2 intervalAt1Difficulty;
    [SerializeField] private AnimationCurve intervalCurve;


    [Header("basicEnemy")]
    [SerializeField] private Vector2 basicEnemyDif;

    [Header("Hostage")]
    [SerializeField] private Vector2 hostageDif;

    [Header("Taker")]
    [SerializeField] private Vector2 takerDif;

    [Header("Shielded")]
    [SerializeField] private Vector2 shieldedDif;

    [Header("Luncher")]
    [SerializeField] private Vector2 luncherDif;



    private BuildingManager buildingManager;
    private LevelManager levelManager;

    public int numOfWaves;
    public WaweManager generatedWaveManager;

    private int maxdifficulty = 10;
    private int availableBoxes = 0;
    public TransformBoxHolder boxHolder;

    private int consecutiveHighDifficultyWaves;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        buildingManager = FindFirstObjectByType<BuildingManager>();
        levelManager = FindFirstObjectByType<LevelManager>();
        generatedWaveManager = GetComponent<WaweManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [ContextMenu("Test")]
    public void Test()
    {
        GenerateNewWaveManager(10);
    }

    public void InitiateSurvival()
    {
        GenerateNewWaveManager(numOfWaves);
        generatedWaveManager.StartWaves(levelManager);
        //levelManager.SetNewWaveManager(generatedWaveManager);
    }

    private void GenerateNewWaveManager(int numOfWaves)
    {
        List<WaveData> waveData = new List<WaveData>();
        Building currenBuilding = buildingManager.GetCurrentBuilding();


        int currentDif = currenBuilding.survivalDifficulty;

        TransformBoxHolder currentBoxes = currenBuilding.boxHolder;
        boxHolder = currentBoxes;
        availableBoxes = currentBoxes.transform.childCount;

        if (availableBoxes <= 0)
        {
            Debug.Log("NoAvailableBoxes");
            return;
        }

        float waveTime = Mathf.Lerp(minMaxWaveTime.y, minMaxWaveTime.x, (float)currentDif / (float)maxdifficulty);

        for (int i = 0; i < numOfWaves; i++)
        {
            float progress = numOfWaves == 1
            ? 0f
            : (float)i / (numOfWaves - 1);

            float baseDifficulty = progress * maximumWaveDifficulty;
            float variation = Random.Range(-waveVariation, waveVariation);

            float countDifficulty = Mathf.Clamp01(baseDifficulty + variation);
            float intervalDifficulty = Mathf.Clamp01(baseDifficulty - variation);

            bool isRestWave = i > 0 && i % 4 == 0;

            if (isRestWave)
            {
                countDifficulty = Mathf.Max(0f, baseDifficulty - 0.1f);
                intervalDifficulty = Mathf.Max(0f, baseDifficulty - 0.1f);
            }

            Debug.Log(
                $"Wave {i}: " +
                $"base difficulty {baseDifficulty:F2}, " +
                $"count difficulty {countDifficulty:F2}, " +
                $"interval difficulty {intervalDifficulty:F2}");

            waveData.Add(
                GenerateNewWaveData(
                    countDifficulty,
                    intervalDifficulty,
                    waveTime,
                    i));

            if (countDifficulty > 0.75f && intervalDifficulty > 0.75f)
            {
                consecutiveHighDifficultyWaves++;
            }
            else
            {
                consecutiveHighDifficultyWaves = 0;
            }
        }
       
        generatedWaveManager.SetWaveData(waveData);

    }

    

    private WaveData GenerateNewWaveData(
        float countDifficulty,
        float intervalDifficulty,
        float waveTime,
        int waveIndex)
    {
        WaveData waveData = new WaveData();

        waveData.startingDelay = waveIndex == 0 ? 3.5f : 0;

        waveData.Type = WaveType.TimeBase;
        waveData.waveText = "Get Ready";

        waveData.respawns = GenerateRespawnsDataByDif(countDifficulty);

        waveData.minMaxNumSummon =
        GenerateRespawnCountByDif(countDifficulty, out int maxNum);

        waveData.minMaxRandomIntervalSummon =
        GenerateRespawnIntervalByDif(intervalDifficulty);

        waveData.durationIfTimeBase = waveTime;
        waveData.ActiveBoxes =
        GenerateNumActiveBoxesByDif(countDifficulty, maxNum);

        return waveData;
    }

    private List<RespawnData> GenerateRespawnsDataByDif(float waveDif)
    {
        List<RespawnData> newRespawnData = new List<RespawnData>();

        //basic Enemy
        RespawnData basicEnemyR = new RespawnData();
        basicEnemyR.respawnType = ERespawnType.BaseEnemy;
        basicEnemyR.respawnProb = 1;
        newRespawnData.Add(basicEnemyR);

        //Hostage
        RespawnData hostageR = new RespawnData();
        hostageR.respawnType = ERespawnType.Hostage;

        if (ProbHandlerHelper(hostageR, hostageDif, waveDif))
            newRespawnData.Add(hostageR);
        //Taker
        RespawnData TakerR = new RespawnData();
        TakerR.respawnType = ERespawnType.TakerWithHostage;

        if (ProbHandlerHelper(TakerR, takerDif, waveDif))
            newRespawnData.Add(TakerR);
        //Shielded
        RespawnData shieldedR = new RespawnData();
        shieldedR.respawnType = ERespawnType.ShieldedEnemy;

        if (ProbHandlerHelper(shieldedR, shieldedDif, waveDif))
            newRespawnData.Add(shieldedR);
        //Luncher
        RespawnData luncherR = new RespawnData();
        luncherR.respawnType = ERespawnType.LuncherEnemy;

        if (ProbHandlerHelper(luncherR, luncherDif, waveDif))
            newRespawnData.Add(luncherR);

        return newRespawnData;
    }

    private bool ProbHandlerHelper(RespawnData respawnData, Vector2 respawnDif, float waveDif)
    {


        float prob = Mathf.Clamp01(Mathf.InverseLerp(respawnDif.x, respawnDif.y, waveDif));
        respawnData.respawnProb = prob;

        return prob > 0;
    }

    private Vector2 GenerateRespawnIntervalByDif(float waveDif)
    {
        Vector2 newInterval = new Vector2();

        float intervalDif = intervalCurve.Evaluate(waveDif);

        newInterval.x = Mathf.Lerp(intervalAt0Difficulty.x, intervalAt1Difficulty.x, intervalDif);
        newInterval.y = Mathf.Lerp(intervalAt0Difficulty.y, intervalAt1Difficulty.y, intervalDif);

       return newInterval;
    }

    private Vector2Int GenerateRespawnCountByDif(float waveDif, out int maxNum)
    {
        Vector2Int newCount = new Vector2Int();

        float numDIf = numCurve.Evaluate(waveDif);

        newCount.x = Mathf.FloorToInt(Mathf.Lerp(numAt0Difficulty.x, numAt1Difficulty.x, numDIf));
        newCount.y = Mathf.FloorToInt(Mathf.Lerp(numAt0Difficulty.y, numAt1Difficulty.y, numDIf));

        maxNum = newCount.y;
        return newCount;
    }


    private int GenerateNumActiveBoxesByDif(float waveDif, int maxNumOfSummon)
    {
        if (availableBoxes > maxNumOfSummon)
        {
            return maxNumOfSummon * 2;
        }

        return availableBoxes;
    }
}
