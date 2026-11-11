using System;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Analytics.IAnalytic;

public class ScoreManager : MonoBehaviour
{
    private RewardManager rewardManager;

    [Header("Ui Elements")]
    [SerializeField] private PlayerCanvas playerCanvas;
    [SerializeField] private UiInGame inGameUi;


    [Header("Score Settings")]
    [SerializeField] private float comboIntervalCooldown = 2f;
    [SerializeField] private float onMoveHitPointBonus = 1.2f;
    [SerializeField] private float khalasHitPointBonus = 1.1f;
    [SerializeField] private float comboToPointMultiplier = 0.25f;
    [SerializeField] private int hostageKillAllowed = 3;

    [Header("Sfx")]
    [SerializeField] private AudioSource tenComboSfx;
    [SerializeField] private AudioSource twentyFiveComboSfx;
    [SerializeField] private AudioSource fiftyComboSfx;
    [SerializeField] private AudioSource oneStar;
    [SerializeField] private AudioSource twoStar;
    [SerializeField] private AudioSource threeStar;



    // ===== PROPERTIES =====
    public float points { get; private set; }
    public float currentStars { get; private set; }
    public int currentKills { get; private set; }
    public int currentCombo { get; private set; }
    public int currentHostageKilled { get; private set; }
    public float currentCashPoints { get; private set; }
    public bool isOnCombo { get; private set; }

    // ===== KHALAS =====
    public int khalasShotCount { get; private set; }
    private Enemy lastKill;
    private float lastComboTime;
    private float pointMultiPlyer = 1;

    private void Start()
    {
        rewardManager = FindFirstObjectByType<RewardManager>();
    }

    void Update()
    {
        CheckComboTime();
    }

    // ==========================================
    // ===== MAIN METHOD - Process Hit =====
    // ==========================================
    public void OnHostageKilled() => ProcessHostageHit(null);

    public void ProcessHit(HitData hitData)
    {
        if (hitData == null) return;

        switch (hitData.hitTYpe)
        {
            case EHitType.Enemy:
                ProcessEnemyHit(hitData);
                break;
            case EHitType.Target:
                ProcessTargetHit(hitData);
                break;

            case EHitType.MovingEnemy:
                ProcessMovingEnemyHit(hitData);
                break;

            case EHitType.EnemyProjectile:
                ProcessEnemyProjectileHit(hitData);
                break;

            case EHitType.Shield:
                ProcessShieldHit(hitData);
                break;

            case EHitType.Hostage:
                ProcessHostageHit(hitData);
                break;

            case EHitType.Khalas:
                ProcessKhalasHit(hitData);
                break;

            case EHitType.Missed:
                ProcessMissHit();
                break;
            default:
                break;
        }
    }

    // ==========================================
    // ===== PROCESS SPECIFIC HITS =====
    // ==========================================
    private void ProcessEnemyHit(HitData hitData)
    {
        Enemy enemy = hitData.hitedTarget as Enemy;
        if (enemy == null) return;

        float basePoints = enemy.GetTargetPoints();

        // Reset khalas
        ResetKhalas();

        // Add points with combo bonus
        AddPoints(basePoints);

        // Add kill
        currentKills++;

        // Add combo
        AddCombo(enemy.GetComboValue());
    }
    private void ProcessTargetHit(HitData hitData)
    {
        Target target = hitData.hitedTarget as Enemy;
        if (target == null) return;

        float basePoints = target.GetTargetPoints();

        // Reset khalas
        ResetKhalas();

        // Add points with combo bonus
        AddPoints(basePoints);

        // Add combo
        AddCombo(target.GetComboValue());
    }

    private void ProcessMovingEnemyHit(HitData hitData)
    {
        Enemy enemy = hitData.hitedTarget as Enemy;
        if (enemy == null) return;

        float basePoints = enemy.GetTargetPoints() * onMoveHitPointBonus;

        ResetKhalas();
        AddPoints(basePoints);
        currentKills++;
        AddCombo(enemy.GetComboValue());
    }
    private void ProcessEnemyProjectileHit(HitData hitData)
    {
        TargetableProjectile tProjectile = hitData.hitedTarget as TargetableProjectile;

        if (tProjectile == null) return;

        float basePoints = tProjectile.GetTargetPoints();

        AddPoints(basePoints);

        AddCombo(tProjectile.GetComboValue());


    }
    private void ProcessShieldHit(HitData hitData)
    {
        EnemyShield shield = hitData.hitedTarget as EnemyShield;
        if (shield == null) return;

        float basePoints = shield.GetTargetPoints();

        AddPoints(basePoints);
        AddCombo(shield.GetComboValue());
    }

    private void ProcessKhalasHit(HitData hitData)
    {
        Enemy enemy = hitData.hitedTarget as Enemy;
        if (enemy == null) return;

        float basePoints = enemy.GetTargetPoints() * khalasHitPointBonus;

        khalasShotCount++;
        lastKill = enemy;

        AddPoints(basePoints);

        // Khalas combo
        if (khalasShotCount >= 5)
            AddCombo(2);
        else if (khalasShotCount >= 3)
            AddCombo(1);
    }

    private void ProcessHostageHit(HitData hitData)
    {

        currentHostageKilled++;

        ResetCombo();

        if (currentHostageKilled >= hostageKillAllowed)
        {
            GameManager.instance?.OnLastChance();
        }
    }
    internal void ClearHostageKill()
    {
        currentHostageKilled = 0;
        ResetCombo();
    }
    private void ProcessMissHit()
    {
        ResetCombo();

    }

   

    // ==========================================
    // ===== POINTS =====
    // ==========================================
    private void AddPoints(float basePoints)
    {
        float bonus = basePoints * currentCombo * comboToPointMultiplier;
        float total = basePoints + bonus;

        total *= pointMultiPlyer;

        points += total;
        currentCashPoints += total;


        UpdateUi();

    }

    // ==========================================
    // ===== COMBO =====
    // ==========================================
    private void AddCombo(int value)
    {
        currentCombo += value;
        isOnCombo = true;
        lastComboTime = Time.time;
        UpdateUi();

        rewardManager.CheckForComboReward(currentCombo);

        if (currentCombo == 10)
            AudioManager.instance.PlaySfx(tenComboSfx);
        else if (currentCombo == 25)
            AudioManager.instance.PlaySfx(twentyFiveComboSfx);
        else if (currentCombo == 50)
            AudioManager.instance.PlaySfx(fiftyComboSfx);
    }

    public void ExtandComboTime(float time)
    {
        if (isOnCombo)
        {
            lastComboTime += Time.time + time + comboIntervalCooldown;
            UpdateUi(false , time + comboIntervalCooldown);
        }
    }

    public void ResetCombo()
    {
        if (GameManager.instance.gameState != EGameState.InGame) return;

        isOnCombo = false;
        currentCombo = 0;
        rewardManager.Penalty();

        inGameUi.CashPoints(currentCashPoints);

        currentCashPoints = 0;
        UpdateUi();

   

    }

    private void CheckComboTime()
    {
        if (currentCombo == 0) return;
        if (Time.time > lastComboTime + comboIntervalCooldown)
            ResetCombo();
    }

    private void ResetKhalas()
    {
        khalasShotCount = 0;
        lastKill = null;


    }

    // ==========================================
    // ===== RESET =====
    // ==========================================
    public void ResetAll()
    {
        points = 0;
        currentKills = 0;
        currentCombo = 0;
        currentHostageKilled = 0;
        currentCashPoints = 0;
        isOnCombo = false;
        khalasShotCount = 0;
        lastKill = null;
        lastComboTime = 0;
        currentStars = 0;

        UpdateUi();
    }
    // ==========================================
    // ===== UI UPDATE =====
    // ==========================================
    private void UpdateUi(bool comboChanged = true, float time = -1)
    {
        inGameUi.ComboChange(currentCombo);
        inGameUi.KillChange(currentKills);
        inGameUi.PointChange(points);
        inGameUi.UiOnHostageKill(currentHostageKilled);

        if (comboChanged)
        {
            if (currentCombo == 0)
            {
                inGameUi.SetComboTimer(0);
            }
            else
                inGameUi.SetComboTimer(comboIntervalCooldown);
        } else
        {
            if (time > 0)
                inGameUi.SetComboTimer(time);
        }

        if (LevelManager.instance.isSurvival)
            return;

        if (points >= LevelManager.instance.currentLevelStarPoints[0] && currentStars < 1)
        {
            currentStars = 1;
            AudioManager.instance.PlaySfx(oneStar);
        }
        if (points >= LevelManager.instance.currentLevelStarPoints[1] && currentStars < 2)
        {
            currentStars = 2;
            AudioManager.instance.PlaySfx(twoStar);

        }
        if (points >= LevelManager.instance.currentLevelStarPoints[2] && currentStars < 3)
        {
            currentStars = 3;
            AudioManager.instance.PlaySfx(threeStar);

        }
    }

    public void AddBonusePointMuiltiplier(float additionalMultiplier)
    {
        pointMultiPlyer += additionalMultiplier;
    }
}