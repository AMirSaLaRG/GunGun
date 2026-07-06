using System;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class Target : MonoBehaviour, IDamagable
{
    protected PlayerController player;

    public Rigidbody rb { get; private set; }
    public CapsuleCollider mycollider { private set; get; }

    [Header("TargetSetup")]
    [SerializeField] protected float moveSpeed = 5;
    [SerializeField] protected float rotationSpeed = 5;
    [SerializeField] protected float duration = 5f;

    [SerializeField] protected int healthPoint = 1;
    [SerializeField] protected float bodyDispearAfterDeath = 4f;
    [Header("TargetPoints")]
    [SerializeField] protected float points;
    [SerializeField] protected int comboValue = 1;

    [SerializeField] protected Vector3 targetPos;

    private RespawnBox myRespawnBox;
    private RespawnManager myRespawnManager;

    public Action<RespawnBox> atEndAction;
    public bool isDead { get { return healthPoint <= 0; } }
    protected bool canTakeDamage = true;
    private bool isMoving = true;
    private bool isFacingCamera = false;

    private float DurationEndTime = 0;
    private bool isDurationEnded = false;
    protected bool isAtFinalPosition;

    protected virtual void Awake()
    {
        player = FindFirstObjectByType<PlayerController>();

        rb = GetComponent<Rigidbody>();

        mycollider = GetComponent<CapsuleCollider>();
        DurationEndTime = Mathf.Infinity;
    }

    protected virtual void Start()
    {
    }

    protected virtual void Update()
    {

        if (isMoving)
            MoveToTarget();


        if (isFacingCamera)
            LookAtCamera();

        if (isMoving == false && isFacingCamera == false)
            if (isDurationEnded == false)
                CheckDurationEnded();

    }
    public virtual void TakeDamage(int damage)
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
        Vector3 direction = Camera.main.transform.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

        float angle = Quaternion.Angle(transform.rotation, targetRotation);

        if (angle < 5f)
        {
            isFacingCamera = false;
            TargetAtFinalPosition();
        }
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
    private void AtEndOfDuration()
    {

        AtEndOfDurationAction();
        Destroy(gameObject);
    }

    protected virtual void AtEndOfDurationAction()
    {
        if (myRespawnManager != null)
            atEndAction?.Invoke(myRespawnBox);
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
            atEndAction?.Invoke(myRespawnBox);


        rb.constraints = RigidbodyConstraints.None;
    }


    public float GetTargetPoints() => points;

    public int GetComboValue() => comboValue;

    public void SetMyRespawnManager(RespawnManager respawnManager) => myRespawnManager = respawnManager;
    public void SetMyBox(RespawnBox box) => myRespawnBox = box;
}
