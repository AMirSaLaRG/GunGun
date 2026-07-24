using DG.Tweening;
using System;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class Target : MonoBehaviour, IDamagable
{
    protected PlayerController player;

    public Rigidbody rb { get; protected set; }
    public Animator anim { get; protected set; }
    public CapsuleCollider mycollider { protected set; get; }

    [Header("Visuals")]
    [SerializeField] private GameObject visuals;

    [Header("TargetSetup")]
    [SerializeField] protected float moveSpeed = 5;
    [SerializeField] protected float rotationSpeed = 5;
    [SerializeField] protected float duration = 5f;

    [SerializeField] protected int healthPoint = 1;
    [SerializeField] protected float bodyDispearAfterDeath = 4f;
    [Header("TargetPoints")]
    [SerializeField] protected float points;
    [SerializeField] protected float bounusMovingPoint = .1f;
    [SerializeField] protected int comboValue = 1;

    [SerializeField] protected Vector3 targetPos;

    private RespawnBox myRespawnBox;
    private RespawnManager myRespawnManager;

    public Action<RespawnBox, Target> atEndAction;
    public bool isDead { get { return healthPoint <= 0; } }
    protected bool canTakeDamage = true;
    public bool isMoving = true;
    private bool isFacingCamera = false;

    private float DurationEndTime = 0;
    private bool isDurationEnded = false;
    protected bool isAtFinalPosition;

    private bool isScaling;
    private Vector3 StartingScale;
    private float scaleTime = 1f;


    protected string boolAnimRunKeyWord = "IsRunning";
    protected string boolAnimDieKeyWord = "IsDead";
    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        anim = visuals.GetComponent<Animator>();

        mycollider = GetComponent<CapsuleCollider>();
        DurationEndTime = Mathf.Infinity;
    }

    protected virtual void Start()
    {
        StartingScale = transform.localScale;

        if (isScaling)
            ScaleToOrginal();
    }


    protected virtual void Update()
    {

        if (isMoving)
            MoveToTarget();
        if (anim  != null) 
            anim?.SetBool(boolAnimRunKeyWord, isMoving);


        if (isFacingCamera)
            LookAtCamera();

        if (isMoving == false && isFacingCamera == false)
            if (isDurationEnded == false)
                CheckDurationEnded();

    }

    public void SetUpTarget(PlayerController player, bool isScaling)
    {
        this.player = player;

        this.isScaling = isScaling;
 

    }

    private void ScaleToOrginal()
    {
        transform.localScale = Vector3.zero;

        transform.DOScale(StartingScale, scaleTime);
    }



    public virtual void TakeDamage(int damage, Vector3 worldSpaceOfDamageTaken)
    {
        if (isDurationEnded)
            return;

        if (canTakeDamage == false)
            return;

        if (isDead)
            return;

        healthPoint -= (int)damage;


        if (healthPoint <= 0)
        {
            Die();
        }
    }

    public virtual void RespawnTheTarget(Vector3 newtargetPos)
    {
        targetPos = newtargetPos;


        Vector3 facingDirection = (targetPos - transform.position).normalized;
        facingDirection.y = 0f;

        transform.forward = facingDirection;
    }

    protected virtual void MoveToTarget()
    {
        transform.position += transform.forward * moveSpeed * Time.deltaTime;
    }

    protected virtual void LookAtCamera()
    {
        Quaternion targetRotation = GetDirectionTowardCamera();

        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

        float angle = Quaternion.Angle(transform.rotation, targetRotation);

        if (angle < .5f)
        {
            isFacingCamera = false;
            TargetAtFinalPosition();
        }
    }

    protected Quaternion GetDirectionTowardCamera()
    {
        Vector3 direction = Camera.main.transform.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        return targetRotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isMoving == false)
            return;
        if (other.gameObject.TryGetComponent(out ViewTracker tracked))
        {

            isMoving = false;
            isFacingCamera = true;
        }

    }

    private void TargetAtFinalPosition()
    {
        StartDurationTime();
        AtFinalPositionAction();

    }

    private void StartDurationTime()
    {
        DurationEndTime = Time.time + duration;
    }

    protected virtual void AtFinalPositionAction()
    {
        isAtFinalPosition = true;
    }

    private void CheckDurationEnded()
    {
        if (isDead)
            return;
     

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
        if (myRespawnManager != null)
            atEndAction?.Invoke(myRespawnBox, this);

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
        if (myRespawnManager != null)
            atEndAction?.Invoke(myRespawnBox, this);
        if (anim != null)
            anim?.SetBool(boolAnimDieKeyWord, true);

        rb.constraints = RigidbodyConstraints.None;

        isMoving = false;
        isFacingCamera = false;

        transform.DOKill();

    }


    public float GetTargetPoints()
    {
        if (isMoving)
            return points + (points * bounusMovingPoint);
        else
            return points;

    }

    public int GetComboValue() => comboValue;

    public void SetMyRespawnManager(RespawnManager respawnManager) => myRespawnManager = respawnManager;
    public void SetMyBox(RespawnBox box) => myRespawnBox = box;

    public void SetMyDuration(float duration) => this.duration = duration;
}
