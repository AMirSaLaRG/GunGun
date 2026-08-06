using DG.Tweening;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EntityActivator : MonoBehaviour, IBoxActivator
{
    [SerializeField] protected Transform myActivateItem;
    [SerializeField] protected Collider myActivateItemCollider;
    [SerializeField] protected Vector3 scaleDirection;
    [SerializeField] protected float openScale = 78f;
    [SerializeField] protected float activateTransitionTIme = 1.5f;
    [SerializeField] protected Ease ActivateEase;
    protected Vector3 closeScale;

    private List<RespawnBox> myRespawnBoxes = new List<RespawnBox>();
    public bool isActive { protected set; get; } = false;
    private int NumberOfCloseRequests;

    protected virtual void Awake()
    {
        if (myActivateItem == null)
        {
            myActivateItem = transform;
        }

        closeScale = myActivateItem.localScale;

        myActivateItemCollider = myActivateItem.GetComponent<Collider>();
    }
    public virtual void SetActive(out float activateTime)
    {

        activateTime = 0;

        if (isActive)
            return;
        myActivateItemCollider.enabled = false;
        Vector3 openScaleV3 = new Vector3(scaleDirection.x == 0 ? closeScale.x : openScale,
            scaleDirection.y == 0 ? closeScale.y : openScale,
            scaleDirection.z == 0 ? closeScale.z : openScale);
        myActivateItem.DOScale(openScaleV3, activateTransitionTIme).SetEase(ActivateEase).OnComplete(() =>
        {
            myActivateItemCollider.enabled = true;

        });

        isActive = true;
        activateTime = activateTransitionTIme;
    }

    public virtual void SetDeActive()
    {
        if (isActive == false) return;

        myActivateItem.DOScale(closeScale, activateTransitionTIme);

        isActive = false;
    }

    public void CheckCloseRequsts()
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
