using DG.Tweening;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EntityActivator : MonoBehaviour, IBoxActivator
{
    [SerializeField] protected bool onlyEnable = false;
    [SerializeField] protected BoxCollider myActivateItemCollider;
    [SerializeField] protected float activateTransitionTime = 1.5f;
    [SerializeField] protected Ease ActivateEase;

    private Vector3 closePos;
    private Vector3 closeScale;
    private Quaternion closeQuaternion;

    [SerializeField] private Vector3 openPos;
    [SerializeField] private Vector3 openScale;
    [SerializeField] private Quaternion openQuaternion;

    protected List<RespawnBox> myRespawnBoxes = new List<RespawnBox>();
    public bool isActive { protected set; get; } = false;
    private int NumberOfCloseRequests;

    private Sequence openSequence;

    protected virtual void Awake()
    {
   

        closePos = transform.localPosition;
        closeScale = transform.localScale;
        closeQuaternion = transform.localRotation;

        if (myActivateItemCollider == null)
            myActivateItemCollider = transform.GetComponent<BoxCollider>();
    }

    [ContextMenu("Capture Open State")]
    private void CaptureOpenState()
    {
        openPos = transform.localPosition;
        openScale = transform.localScale;
        openQuaternion = transform.localRotation;
    }

    public virtual void SetActive(out float activateTime, float forceTime = 0)
    {

        float duration = forceTime == 0? activateTransitionTime : forceTime;

        activateTime = 0;

        if (isActive)
            return;

        if (onlyEnable)
        {
            Debug.Log("SetActive");

            gameObject.SetActive(true);
            isActive = true;
            return;
        }
        myActivateItemCollider.enabled = false;

        // Create a sequence to animate all transforms together
        openSequence = DOTween.Sequence();

        // Scale
        openSequence.Join(transform.DOScale(openScale, duration).SetEase(ActivateEase));

        // Rotation
        openSequence.Join(transform.DOLocalRotateQuaternion(openQuaternion, duration).SetEase(ActivateEase));

        // Position
        openSequence.Join(transform.DOLocalMove(openPos, duration).SetEase(ActivateEase));

        // When complete, re-enable collider
        openSequence.OnComplete(() =>
        {
            myActivateItemCollider.enabled = true;
        });

        isActive = true;
        activateTime = duration;
    }

    public virtual void SetDeActive(bool isReseting = false)
    {
        float duration = activateTransitionTime;


        if (isReseting)
        {
            transform.DOKill(true);
            if (openSequence != null)
                openSequence.Kill();

            transform.localScale = closeScale;
            transform.localPosition = closePos;
            transform.localRotation = closeQuaternion;

            if (onlyEnable)
                gameObject.SetActive(false);

            isActive = false;

            return;

        }
        else if (isActive == false) return;

        if (onlyEnable)
        {
            gameObject.SetActive(false);
            isActive = false;
            return;
        }

        Sequence closeSequence = DOTween.Sequence();

        // Scale
        closeSequence.Join(transform.DOScale(closeScale, duration).SetEase(ActivateEase));

        // Rotation
        closeSequence.Join(transform.DOLocalRotateQuaternion(closeQuaternion, duration).SetEase(ActivateEase));

        // Position
        closeSequence.Join(transform.DOLocalMove(closePos, duration).SetEase(ActivateEase));

        isActive = false;
    }

    public virtual void CheckCloseRequsts(bool isImediate = false)
    {
        if (isImediate)
        {
            StopAllCoroutines();
            SetDeActive(true);
            return;
        }

        foreach (var box in myRespawnBoxes)
        {
            if (box.isActive == true)
            {
                SetActive(out float t);
                return;
            }
        }
        SetDeActive();
    }

    public virtual void ResetBox()
    {
        SetDeActive(true);
    }

    public void AddBox(RespawnBox myBox)
    {
        myRespawnBoxes.Add(myBox);
    }
}
