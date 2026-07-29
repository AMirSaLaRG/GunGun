using DG.Tweening;
using UnityEngine;

[System.Serializable]
public class EntityActivator : MonoBehaviour, IBoxActivator
{
    [SerializeField] protected Transform myActivateItem;
    [SerializeField] protected Vector3 scaleDirection;
    [SerializeField] protected float openScale = 78f;
    [SerializeField] protected float activateTransitionTIme = 1.5f;
    [SerializeField] protected Ease ActivateEase;
    protected Vector3 closeScale;
    private bool isActive = false;

    protected virtual void Awake()
    {
        if (myActivateItem == null)
        {
            myActivateItem = transform;
        }

        closeScale = myActivateItem.localScale;

    }
    public virtual void SetActive()
    {
        if (isActive)
            return;
     
        Vector3 openScaleV3 = new Vector3(scaleDirection.x == 0 ? closeScale.x : openScale,
            scaleDirection.y == 0 ? closeScale.y : openScale,
            scaleDirection.z == 0 ? closeScale.z : openScale);
        myActivateItem.DOScale(openScaleV3, activateTransitionTIme).SetEase(ActivateEase);

        isActive = true;
       
    }

    public virtual void SetDeActive()
    {
        if (isActive == false) return;
 
        myActivateItem.DOScale(closeScale, activateTransitionTIme);

        isActive = false;
        
    }
}
