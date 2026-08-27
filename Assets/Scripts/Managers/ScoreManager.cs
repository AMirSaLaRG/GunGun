using System;
using Unity.VisualScripting;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [Header("Ui Elements")]
    [SerializeField] private PlayerCanvas playerCanvas;
    [SerializeField] private UiInGame inGameUi;


    [Header("Score Settings")]
    [SerializeField] private float comboIntervalCooldown = 2f;
    [SerializeField] private float onMoveHitPointBonus = 1.2f;
    [SerializeField] private float khalasHitPointBonus = 1.1f;
    [SerializeField] private float comboToPointMultiplier = 0.25f;
    [SerializeField] private int hostageKillAllowed = 3;




    // ===== PROPERTIES =====
    public float points { get; private set; }
    public int currentKills { get; private set; }
    public int currentCombo { get; private set; }
    public int currentHostageKilled { get; private set; }
    public float currentCashPoints { get; private set; }
    public bool isOnCombo { get; private set; }

    // ===== KHALAS =====
    public int khalasShotCount { get; private set; }
    private Enemy lastKill;
    private float lastComboTime;

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

            case EHitType.MovingEnemy:
                ProcessMovingEnemyHit(hitData);
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

    }

    public void ResetCombo()
    {
        isOnCombo = false;
        currentCombo = 0;

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

        UpdateUi();
    }
    // ==========================================
    // ===== UI UPDATE =====
    // ==========================================
    private void UpdateUi(bool comboChanged = true)
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
        }
    }

   
}