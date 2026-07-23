using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCanvas : MonoBehaviour
{
    [Header("TakingDamageUi")]
    [SerializeField] private Transform takingDamageElements;
    [SerializeField] private float takingDamageUiTime;

    private Image[] takingDamageImages;

    private void Start()
    {
        takingDamageImages = GetComponentsInChildren<Image>();
    }
    [ContextMenu("Test")]

    public void OnTakingDamage()
    {
        foreach (var image in takingDamageImages)
            PulsEffectAndFade(image);
    }
    public void PulsEffectAndFade(Image targetImage)
    {
        targetImage.DOFade(.4f, takingDamageUiTime / 2).OnComplete(() =>
        {
            targetImage.DOFade(0, takingDamageUiTime / 2);
        });
    }
}
