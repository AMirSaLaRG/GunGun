using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OnComboUi : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private Image image;

    [SerializeField] private TextMeshProUGUI myText;


    public void Setup(Vector3 screenPoint, int number, float scale, Vector2 offset, float onHitDuration)
    {
        transform.position = new Vector3(screenPoint.x + offset.x, screenPoint.y + offset.y, transform.position.z);
        transform.localScale = Vector3.zero;

        DOTween.Kill(this.gameObject);

        string currentText= number.ToString();

        myText.text = $"X {currentText}";
 
        pulsScaleEffect(transform, scale, onHitDuration);
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
