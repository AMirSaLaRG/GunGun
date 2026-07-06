using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;

public class Enemy : Target
{

    [Header("EnemySetup")]
    [SerializeField] private float attackTime = 3f;
    [SerializeField] private int damage = 1;

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

    }

    protected override void AtEndOfDurationAction()
    {
        base.AtEndOfDurationAction();
        DanceSequence(false, true);
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
        StartCoroutine(AimAtPlayerAndShootCo());

    }

    private IEnumerator AimAtPlayerAndShootCo()
    {
        Aiming();
        yield return new WaitForSeconds(attackTime);
        Shoot();
    }

    private void Shoot()
    {
        Debug.Log("Bang!!");
        player.TakeDamage(damage);

    }

    private void Aiming()
    {
        Debug.Log("Aiming!");

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
