using DG.Tweening;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EntityActivator : MonoBehaviour, IBoxActivator
{
    [SerializeField] protected BoxCollider myActivateItemCollider;
    [SerializeField] protected float activateTransitionTIme = 1.5f;
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

    public virtual void SetActive(out float activateTime)
    {

        activateTime = 0;

        if (isActive)
            return;
        myActivateItemCollider.enabled = false;

        // Create a sequence to animate all transforms together
        Sequence openSequence = DOTween.Sequence();

        // Scale
        openSequence.Join(transform.DOScale(openScale, activateTransitionTIme).SetEase(ActivateEase));

        // Rotation
        openSequence.Join(transform.DOLocalRotateQuaternion(openQuaternion, activateTransitionTIme).SetEase(ActivateEase));

        // Position
        openSequence.Join(transform.DOLocalMove(openPos, activateTransitionTIme).SetEase(ActivateEase));

        // When complete, re-enable collider
        openSequence.OnComplete(() =>
        {
            myActivateItemCollider.enabled = true;
        });

        isActive = true;
        activateTime = activateTransitionTIme;
    }

    public virtual void SetDeActive()
    {
        if (isActive == false) return;

        Sequence closeSequence = DOTween.Sequence();

        // Scale
        closeSequence.Join(transform.DOScale(closeScale, activateTransitionTIme).SetEase(ActivateEase));

        // Rotation
        closeSequence.Join(transform.DOLocalRotateQuaternion(closeQuaternion, activateTransitionTIme).SetEase(ActivateEase));

        // Position
        closeSequence.Join(transform.DOLocalMove(closePos, activateTransitionTIme).SetEase(ActivateEase));

        isActive = false;
    }

    public virtual void CheckCloseRequsts()
    {
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

    public void AddBox(RespawnBox myBox)
    {
        myRespawnBoxes.Add(myBox);
    }
}
