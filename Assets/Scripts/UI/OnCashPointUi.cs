using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OnCashPointUi : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private Image image;

    [SerializeField] private TextMeshProUGUI myText;


    private void Start()
    {
        transform.localScale = Vector3.zero;
    }

    public void SetUp(int points, float scale, float duration)
    {
        if (points <= 0)
            return;

        myText.text = $"$ {points}";
        myText.DOFade(1, 0);
        image.DOFade(1, 0);

        pulsScaleEffect(transform, scale, duration);
    }

    private void pulsScaleEffect(Transform transform, float scale, float duration)
    {
        transform.localScale = Vector3.zero;

        myText.DOFade(0, duration);
        image.DOFade(0, duration);

        transform.DOScale(scale, duration * .7f).OnComplete(() =>
        {
            transform.DOScale(0, duration * .3f);
        });
    }
}
