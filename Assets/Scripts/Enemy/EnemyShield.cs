using System.Collections;
using UnityEngine;

public class EnemyShield : Target
{
    [Header("ShieldSetup")]
    [SerializeField] protected float pointMultiplyerOfHolder = .4f;
    [SerializeField] protected Enemy myHolder;
    [SerializeField] private LayerMask whatIsUntargetable;
    [SerializeField] private float shieldDragMultiplier = 0.5f;

    private BoxCollider boxCollider;
    private Material myMaterial;
    private float originalDrag;
    private float originalAngularDrag;

    protected override void Start()
    {
        base.Start();
        points = pointMultiplyerOfHolder * myHolder.GetTargetPoints();
        boxCollider = GetComponent<BoxCollider>();
        myMaterial = GetComponent<Renderer>().material;
        ApplyShieldEffect();

    }

    public override void TakeDamage(int damage, Vector3 worldSpaceOfDamageTaken)
    {
        base.TakeDamage(damage, worldSpaceOfDamageTaken);

        boxCollider.isTrigger = false;
        gameObject.layer = GetLayerIndexFromMask(whatIsUntargetable);

        transform.parent = null;

        rb.AddForce(transform.up * 200, ForceMode.Impulse);
        rb.AddForce(transform.forward * 200, ForceMode.Impulse);
        rb.AddForce(transform.right * 200, ForceMode.Impulse);

        StartCoroutine(FadeToColor(bodyDispearAfterDeath /2));
    }

    private int GetLayerIndexFromMask(LayerMask layerMask)
    {
        // LayerMask is a bitmask, we need to find the first set bit
        int layerNumber = 0;
        int layerMaskValue = layerMask.value;

        while (layerMaskValue > 0)
        {
            layerMaskValue = layerMaskValue >> 1;
            layerNumber++;
        }

        return layerNumber - 1;
    }
    private IEnumerator FadeToColor(float duration = 1f)
    {
        Color originalColor = myMaterial.color;
        Color disableColor = Color.red;
        disableColor.a = 0.25f;

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            myMaterial.color = Color.Lerp(originalColor, disableColor, t);

            yield return null;
        }

        myMaterial.color = disableColor;
    }

    private void ApplyShieldEffect()
    {
        myHolder.rb.mass *= 5;
    }
    private void RemoveShiedlEffect()
    {
        myHolder.rb.mass /= 5;
    }

    private void OnDestroy()
    {
        if (myHolder != null)
            RemoveShiedlEffect();
    }


}
