using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UiFadePannel : MonoBehaviour
{
    private Image image;
    private Color color;
    void Start()
    {
        image = GetComponent<Image>();

        color = image.color;
        color.a = 1;

        image.color = color;
    }

    private void OnEnable()
    {
        if (color.a == 1)
        {
            image.DOFade(.2f, 1);
        }
    }

    private void OnDisable()
    {
        image.color = color;
    }

}
