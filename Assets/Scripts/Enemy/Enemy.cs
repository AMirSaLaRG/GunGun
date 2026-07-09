using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;

public class Enemy : Target
{
 

    [Header("EnemySetup")]
    [SerializeField] protected float attackTime = 3f;
    [SerializeField] protected int damage = 1;
    [SerializeField] protected GameObject projectal;
    [SerializeField] protected float projectalSpeed = 100;

    [Header("DanceSetup")]
    [SerializeField] private float danceTime = 2f;
    [SerializeField] private float danceMoveRange = 1.5f;
    [SerializeField] private float danceRotationRange = 15f;
    private float danceCycleDelay = .4f;

    private Vector3 startingRotation;
    private Vector3 startingPos;

    public bool ShouldDance = false;
    private bool isDancing = false;
    private Sequence danceSq;

    private Coroutine shootCo;

    protected string triggerAnimAttackKeyWord = "Shoot";

    protected override void Start()
    {
        base.Start();
        startingRotation = transform.eulerAngles;
        startingPos = transform.position;


    }

    protected override void Update()
    {
        base.Update();

    }

    protected override void AtDieAction()
    {
        base.AtDieAction();
        DanceSequence(false, true);

        if (shootCo != null)
            StopCoroutine(shootCo);

        

    }

    protected override void AtEndOfDurationAction()
    {
        DanceSequence(false, true);

        base.AtEndOfDurationAction();
    }
    protected override void AtFinalPositionAction()
    {
        base.AtFinalPositionAction();

        startingRotation = transform.eulerAngles;
        startingPos = transform.position;

        if (ShouldDance)
            DanceSequence(true);

        ShootThePlayer();
    }


    private void ShootThePlayer()
    {
        AimAtPlayerAndShoot();
    }

    private void AimAtPlayerAndShoot()
    {
        shootCo = StartCoroutine(AimAtPlayerAndShootCo());

    }

    private IEnumerator AimAtPlayerAndShootCo()
    {
        Aiming();
        yield return new WaitForSeconds(attackTime);
        Shoot();
    }

    private void Shoot()
    {
        anim.SetTrigger(triggerAnimAttackKeyWord);
        GameObject newBullet = Instantiate(projectal, transform.position, GetDirectionTowardCamera());
        newBullet.GetComponent<EnemyProjectal>().SetUP(projectalSpeed, damage, player);

    }
    
    protected virtual void Aiming()
    {
        

    }

    public void SetDanceMode(bool enable) => ShouldDance = enable;
    [ContextMenu("DanceCancelTest")]
    public void CancelDance()
    {
        SetDanceMode(false);
        DanceSequence(false);
    }

    private void DanceSequence(bool enable,bool imediate = false, Action callback = null)
    {

        if (enable)
        {
            danceSq = DOTween.Sequence();


            danceSq.Append(transform.DORotate(new Vector3(startingRotation.x, startingRotation.y, -danceRotationRange), danceTime / 2))
                .Join(transform.DOMoveX(startingPos.x -danceMoveRange, danceTime / 2))
                .Append(transform.DORotate(new Vector3(startingRotation.x, startingRotation.y, 0), danceTime / 2))
                .Join(transform.DOMoveX(startingPos.x, danceTime / 2))
                .Append(transform.DORotate(new Vector3(startingRotation.x, startingRotation.y, danceRotationRange), danceTime / 2))
                .Join(transform.DOMoveX(startingPos.x + danceMoveRange, danceTime / 2))
                .Append(transform.DORotate(new Vector3(startingRotation.x, startingRotation.y, 0), danceTime / 2))
                .Join(transform.DOMoveX(startingPos.x, danceTime / 2))
                .AppendInterval(danceCycleDelay)
                .SetLoops(-1, LoopType.Restart)
                .SetLink(gameObject)
                .OnComplete(() => callback.Invoke());

            isDancing = true;
        } else
        {
            if (danceSq != null && danceSq.IsActive())
            {
                if (imediate == false)
                {
                    danceSq.Kill();
                    transform.DORotate(new Vector3(startingRotation.x, startingRotation.y, 0), danceTime / 2);
                    transform.DOMoveX(startingPos.x, danceTime / 2).OnComplete(() =>
                    {
                        danceSq = null;
                        isDancing = false;
                    });
                }
                
                if (imediate)
                {
                    danceSq.Kill();
                    danceSq = null;
                    isDancing = false;
                }

            
            }
        }
        


    }
}
