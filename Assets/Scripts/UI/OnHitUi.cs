using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OnHitUi : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private Image[] images;
    [SerializeField] private TextMeshProUGUI onHitText;

    private float myScale;
    private float myOnHitDuration;

    public void Setup(Vector3 screenPoint, string text, Color color, float scale, Vector2 onHitOffset, float onHitDuration)
    {
        transform.position = new Vector3(screenPoint.x + onHitOffset.x, screenPoint.y + onHitOffset.y, transform.position.z);

        onHitText.color = color;
        onHitText.text = text;

        myScale = scale;
        myOnHitDuration = onHitDuration;
        pulsScaleEffect(transform, scale, onHitDuration);


        foreach (var image in images)
        {
            image.color = color;
            image.transform.localScale = Vector3.zero;
            image.transform.DOScale(1, onHitDuration).SetEase(Ease.OutElastic);
            PulsFadeEffectAndFade(image, onHitDuration);
        }
    }

    public void SetUpKhalas(int combo)
    {
        onHitText.text = combo.ToString();
        
        DOTween.Kill(this.gameObject);
        transform.localScale = Vector3.one * myScale;
        float newScale = myScale * combo * 1.2f;
        pulsScaleEffect(transform, newScale, myOnHitDuration);


    }
    private void PulsFadeEffectAndFade(Image targetImage, float duration)
    {

        targetImage.DOFade(.4f, duration / 2).OnComplete(() =>
        {
            targetImage.DOFade(0, duration / 2);
        });
    }

    private void pulsScaleEffect(Transform transform, float scale, float duration)
    {
        transform.localScale = Vector3.zero;

        transform.DOScale(scale, duration * .7f).OnComplete(() =>
        {
            transform.DOScale(0, duration * .3f);
        });
    }
}
