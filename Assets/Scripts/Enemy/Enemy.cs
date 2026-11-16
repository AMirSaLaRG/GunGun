using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class Enemy : Target
{
    [Header("EnemySetup")]
    [SerializeField] protected float attackTime = 3f;
    [SerializeField] protected int damage = 1;
    [SerializeField] protected GameObject projectal;
    [SerializeField] protected float projectalSpeed = 100;

    [Header("Sfx Setup")]
    [SerializeField] protected AudioSource shotSfx;
    [SerializeField] private ParticleSystem shotVfx;


    [Header("Shooting Warning Sign Setup")]
    [SerializeField] private ParticleSystem ShootingWarningSignVfx;
    [SerializeField] private Animator uiSignsCanvas;
    [SerializeField] private AudioSource ShootingWarningSfx;
    [SerializeField] private float warningSecondsBeforShoot;
    [SerializeField] private SliderCountUpHelperUi aimingSlider;
    private Coroutine shootingWarningSignCo;

    private Vector3 startingRotation;
    private Vector3 startingPos;

    public bool ShouldDance = false;
    private bool isDancing = false;
    private Sequence danceSq;

    private Coroutine shootCo;

    protected string triggerAnimAttackKeyWord = "Shoot";

    protected Hostage myHostage;

    protected override void Start()
    {
        base.Start();
        startingRotation = transform.eulerAngles;
        startingPos = transform.position;


    }

    protected override void CheckAndSetMyType()
    {
        base.CheckAndSetMyType();

        switch (myType)
        {
            case ERespawnType.BaseEnemy:
                ActivateShields(0);
                break;
            case ERespawnType.ShieldedEnemy:
                ActivateShields(1);
                break;
        }
    }

    private void ActivateShields(int num)
    {
        foreach (var visualShield in visualShields)
            visualShield.SetActive(false);

        for (int i = 0; i < num; i++)
        {
            if (visualShields[i] != null)
                visualShields[i].SetActive(true);
        }
    }

    protected override void AtDieAction()
    {
        base.AtDieAction();

        if (myHostage != null)
            myHostage.TriggerAtEndOfDuration();

        StopWarningSign();
    }

    protected override void AtEndOfDurationAction()
    {
        base.AtEndOfDurationAction();
    }

    protected override void AtFinalPositionAction()
    {
        base.AtFinalPositionAction();

        startingRotation = transform.eulerAngles;
        startingPos = transform.position;

        AtFinalPositionEnemyAction();
    }

    protected virtual void AtFinalPositionEnemyAction()
    {
        
        shootCo = StartCoroutine(AitmAndShootCo());

    }

    private IEnumerator AitmAndShootCo()
    {
        Aiming();
        if (isDead) yield break;

        yield return new WaitForSeconds(attackTime);

        if (isDead) yield break;

        Shoot();
    }

    protected virtual void Shoot()
    {
        anim.SetTrigger(triggerAnimAttackKeyWord);
        GameObject newBullet = Instantiate(projectal, transform.position, GetDirectionTowardLookPosition(facingPosition));
        AudioManager.instance.PlaySfx(shotSfx, true);
        newBullet.GetComponent<EnemyProjectal>().SetUP(projectalSpeed, damage, player);
        if (shotVfx != null)
            shotVfx.gameObject.SetActive(true);

        StopWarningSign();
        uiSignsCanvas.Play("shoted");

    }

    protected virtual void Aiming()
    {
        GiveShootingWarningSign(attackTime);
    }

    public void SetDanceMode(bool enable) => ShouldDance = enable;
 
    protected void GiveShootingWarningSign(float NextShotTimeFromNow)
    {
        aimingSlider.StartSliderCount(NextShotTimeFromNow);

        float timeToGiveSign = NextShotTimeFromNow - warningSecondsBeforShoot;

        if (timeToGiveSign <= 0)
            StartWarningSign();
        else
            shootingWarningSignCo = StartCoroutine(GiveShootingWarningSignCo(timeToGiveSign));
    }

    private IEnumerator GiveShootingWarningSignCo(float timeToGiveSign)
    {
        yield return new WaitForSeconds(timeToGiveSign);
        if (isDead) yield break;

        StartWarningSign();
    }

    private void StartWarningSign() // THIS SHOULD GET ON TARGET NOT ONLY ENEMY
    {
        AudioManager.instance.PlaySfx(ShootingWarningSfx);
        if (ShootingWarningSignVfx == null)
            ShootingWarningSignVfx.Play();

        if (uiSignsCanvas != null)
            uiSignsCanvas.Play("warningSignStarts");
    }

    private void StopWarningSign()
    {
        aimingSlider.StartSliderCount(0);
        if (ShootingWarningSignVfx != null)
        {
            ShootingWarningSignVfx.gameObject.SetActive(false);
            ShootingWarningSignVfx.Stop();
        }

        if (uiSignsCanvas != null)
            uiSignsCanvas.Play("stopWarningSign");

        if (shootCo != null)
            StopCoroutine(shootCo);

        if (shootingWarningSignCo != null)
            StopCoroutine(shootingWarningSignCo);
    }

    public void SetupMyHostage (Hostage newHostage) => myHostage = newHostage;
    
}
