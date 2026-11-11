using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Target : MonoBehaviour, IDamagable
{
    protected PlayerController player;

    public Rigidbody rb { get; protected set; }
    public Animator anim { get; protected set; }
    public CapsuleCollider mycollider { protected set; get; }

    [Header("Target Sfx")]
    [SerializeField] protected AudioSource onTakeDamageSfx;
    [SerializeField] protected AudioSource onDeathSfx;
    [SerializeField] protected AudioSource onDeadTargetGetShotSfx;

    [Header("Target Type")]
    [SerializeField] protected bool isStatic = false;


    [Header("Visuals")]
    [SerializeField] protected GameObject visuals;
    [SerializeField] protected List<GameObject> visualShields = new List<GameObject>();
    [SerializeField] protected GameObject takeShotVfx;



    [Header("TargetSetup")]
    [SerializeField] protected float moveSpeed = 5;
    [SerializeField] protected float rotationSpeed = 5;
    [SerializeField] protected float duration = 5f;
    protected float BaseMoveSpeed;

    [SerializeField] protected int healthPoint = 1;
    [SerializeField] protected float bodyDispearAfterDeath = 4f;
    public float GetBodyDis() => bodyDispearAfterDeath;
    [Header("TargetPoints")]
    [SerializeField] protected float points;
    [SerializeField] protected float bounusMovingPoint = .1f;
    [SerializeField] protected int comboValue = 1;

    [SerializeField] protected Vector3 targetPos;

    [Header("Leaving Warning Sign Setup")]
    [SerializeField] private ParticleSystem leavingWarningSign;
    [SerializeField] private float timeToGiveLeavingWarningSignBefore;
    private bool isLeavingWarningSignGiven = false;

    [Header("At Position Warning Sign Setup")]
    [SerializeField] private ParticleSystem atPositionWarningSign;
    private bool isatPositionWarningSignGiven = false;

    protected ERespawnType myType;

    protected RespawnBox myBox;
    protected RespawnManager myRespawnManager;
    protected DG.Tweening.Sequence moveSq;

    public bool isDead { get { return healthPoint <= 0; } }
    protected bool canTakeDamage = true;
    public bool isMoving { protected set; get; } = true;
    protected Vector3 facingPosition;

    private float DurationEndTime = 0;
    private bool isDurationEnded = false;
    protected bool isAtFinalPosition;

    private bool isScaling;
    private Vector3 StartingScale;
    private float scaleTime = 1f;

    protected Transform[] myWayPoints;


    protected string boolAnimRunKeyWord = "IsRunning";
    protected string boolAnimDieKeyWord = "IsDead";

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        anim = visuals.GetComponent<Animator>();

        mycollider = GetComponent<CapsuleCollider>();
        DurationEndTime = Mathf.Infinity;

        StartingScale = transform.localScale;




    }

    protected virtual void Start()
    {
        if (isStatic)
            return;

        BaseMoveSpeed = moveSpeed;


        if (isScaling)
            ScaleToOrginal();

        if (myWayPoints != null && myWayPoints.Length > 0)
            MoveByWayPoints();

    }


    protected virtual void Update()
    {
        bool flowControl = IsTargetAtFinalState();
        if (!flowControl)
        {
            return;
        }


        if (isDurationEnded == false)
            CheckDurationEnded();

    }

   

    private bool IsTargetAtFinalState()
    {
        if (isStatic)
            return false;

        if (isMoving)
            return false;

        if (isDead)
            return false;

        return true;
    }

    private void TargetAtFinalPosition()
    {
        DurationEndTime = Time.time + duration;

        AtFinalPositionAction();
    }

    protected virtual void CheckAndSetMyType()
    {

    }



    public void SetUpTarget(PlayerController player, RespawnManager respawnManager, RespawnBox myNewBox, ERespawnType myNewType)
    {
        this.player = player;
        this.myRespawnManager = respawnManager;
        this.myBox = myNewBox;

        myType = myNewType;
        CheckAndSetMyType();
    }

    public void SetScalingAtRespawn(bool enable) => isScaling = enable;

    private void ScaleToOrginal()
    {
        transform.localScale = Vector3.zero;
        transform.DOScale(StartingScale, scaleTime);
    }



    public virtual void TakeDamage(int damage, HitData hitData)
    {
        if (isDurationEnded)
            return;

        if (canTakeDamage == false)
            return;

        TakeShotForce(hitData);
        TakeShotVfx(hitData);

        if (isDead)
        {
            if (onDeadTargetGetShotSfx != null)
                AudioManager.instance.PlaySfx(onDeadTargetGetShotSfx, true);
        } else
        {
            healthPoint -= (int)damage;

            if (onTakeDamageSfx != null)
                AudioManager.instance.PlaySfx(onTakeDamageSfx, true);


            if (healthPoint <= 0)
            {
                if (onDeathSfx != null)
                    AudioManager.instance.PlaySfx(onDeathSfx, true, true);

                Die();
            }
        }
    }



    public Vector3 GetRandomPointOnCollider()
    {
        if (mycollider == null) return transform.position;

        Bounds bounds = mycollider.bounds;
        return new Vector3(
            UnityEngine.Random.Range(bounds.min.x, bounds.max.x),
            UnityEngine.Random.Range(bounds.min.y, bounds.max.y),
            UnityEngine.Random.Range(bounds.min.z, bounds.max.z)
        );
    }
    private void TakeShotForce(HitData hitData)
    {
        if (rb == null) return;

        rb.isKinematic = false;

        rb.AddForceAtPosition(-hitData.hitForce * hitData.hitNorm, hitData.hitPos, ForceMode.Impulse);
    }

    private void TakeShotVfx(HitData hitData)
    {
        Vector3 placePos = hitData.hitPos + (hitData.hitNorm * 0.002f);
        Quaternion rotation = Quaternion.LookRotation(hitData.hitNorm);

        GameObject bloodVfx = Instantiate(takeShotVfx, placePos, rotation, transform);


    }

    public virtual void SetTargetWayPoints(Transform[] newWayPoints)
    {
        myWayPoints = newWayPoints;
        if (myWayPoints == null || myWayPoints.Length < 1) return;
        MoveByWayPoints();
    }

    protected virtual void MoveByWayPoints()
    {
        if (anim != null)
            anim?.SetBool(boolAnimRunKeyWord, true);

        if (myWayPoints == null || myWayPoints.Length < 2) return;

        if (moveSq != null)
            moveSq.Kill();

        moveSq = DOTween.Sequence();

        // Store current Y
        float yPos = transform.position.y;

        // Start at first waypoint (only X and Z)
        transform.position = new Vector3(myWayPoints[0].position.x, yPos, myWayPoints[0].position.z);

        Vector3 previousPos = new Vector3(
        myWayPoints[0].position.x,
        yPos,
        myWayPoints[0].position.z
    );

        for (int i = 1; i < myWayPoints.Length; i++)
        {
            int index = i;
            float yOffset = 0;
            if (isScaling && i == 0)
                yOffset = 3;// this magic number tries to fix eye look on run for fix it should put a empty on eye of target and get differents of it by center and put it as eye height or height

            // Target with locked Y
            Vector3 target = new Vector3(
                myWayPoints[index].position.x,
                yPos + yOffset, 
                myWayPoints[index].position.z
            );

            // Calculate duration based on distance
            Vector3 previousPosFlat = new Vector3(previousPos.x, 0, previousPos.z);
            Vector3 targetPosFlat = new Vector3(target.x, 0, target.z);
            float distance = Vector3.Distance(previousPosFlat, targetPosFlat);
            float duration = distance / moveSpeed;

            // Move X and Z
            moveSq.Append(
                transform.DOMoveX(myWayPoints[index].position.x, duration)
                    .SetEase(Ease.Linear)
            );

            moveSq.Join(
                transform.DOMoveZ(myWayPoints[index].position.z, duration)
                    .SetEase(Ease.Linear)
            );

            // Rotate while moving (JOIN with the movement)
            moveSq.Join(
                transform.DOLookAt(target, 0.1f)
                    .SetEase(Ease.OutQuad)
            );

            previousPos = target;
        }

        moveSq.OnComplete(() =>
        {
            isMoving = false;
            anim?.SetBool(boolAnimRunKeyWord, false);
            TargetAtFinalPosition();
        });
    }

    protected virtual void SpeedUpTarget(float startSpeed, float endSpeed, float duration)
    {
        StartCoroutine(SpeedUpTargetCo(startSpeed, endSpeed, duration));
    }
    private IEnumerator SpeedUpTargetCo(float startSpeed, float endSpeed, float duration)
    {
        float elapse = 0;
        while (elapse < duration)
        {
            moveSpeed = Mathf.Lerp(startSpeed, endSpeed, elapse / duration);
            yield return null;
            elapse += Time.deltaTime;
        }
        moveSpeed = endSpeed;
    }

    protected Quaternion GetDirectionTowardLookPosition(Vector3 lookPosition)
    {
        Vector3 direction = lookPosition - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        return targetRotation;
    }

    protected virtual void AtFinalPositionAction()
    {
        isAtFinalPosition = true;
        isMoving = false;

        if (facingPosition == null)
            facingPosition = player.mainCamera.transform.position;
         
        StartLookitAt(facingPosition);
    }

    private void StartLookitAt(Vector3 facingPosition)
    {
        float d = 1 / rotationSpeed;

        transform.DOLookAt(facingPosition, d)
                    .SetEase(Ease.OutQuad);
    }

  

    private void CheckDurationEnded()
    {


        GiveAtPosWarningSign();

        CheckAndGiveLeavingSign(DurationEndTime);

        if (Time.time > DurationEndTime)
        {
            isDurationEnded = true;
            AtEndOfDuration();
        }
    }
    protected virtual void AtEndOfDuration()
    {

        AtEndOfDurationAction();
        
        Destroy(gameObject);
    }

    protected virtual void AtEndOfDurationAction()
    {


        if (myBox != null)
            myBox.RespawnLeftTheBox(this);

        transform.DOKill();

    }



    private void Die()
    {
        if (isDurationEnded)
            return;

        AtDieAction();

        Destroy(gameObject, bodyDispearAfterDeath);

    }

    protected virtual void AtDieAction()
    {
        if (myBox != null)
            myBox.RespawnLeftTheBox(this);

        if (anim != null)
            anim?.SetBool(boolAnimDieKeyWord, true);

        if (moveSq != null)
            moveSq.Kill();

        if (anim != null)
            anim?.SetBool(boolAnimRunKeyWord, false);

        if (rb != null)
            rb.constraints = RigidbodyConstraints.None;

        isMoving = false;

        transform.DOKill();

    }


    public float GetTargetPoints()
    {
            return points;
    }

    protected void GiveAtPosWarningSign()
    {
        if (atPositionWarningSign == null)
            return;

        if (isatPositionWarningSignGiven == true)
            return;

        atPositionWarningSign.Play();
        isatPositionWarningSignGiven = true;
    }
    protected void CheckAndGiveLeavingSign(float timeToLeave)
    {
        if (leavingWarningSign == null)
            return;

        if (isLeavingWarningSignGiven)
            return;

        if (Time.time > timeToLeave - timeToGiveLeavingWarningSignBefore)
        {
            leavingWarningSign.Play();
            isLeavingWarningSignGiven = true;
        }

    }
    public int GetComboValue() => comboValue;

    public void SetMyDuration(float duration) => this.duration = duration;

    private void OnDestroy()
    {
        DOTween.Kill(gameObject);
    }


  

    
}
