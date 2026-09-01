using System;
using UnityEngine;
using UnityEngine.UI;

public class UiRewardItem : MonoBehaviour
{
    private Button claimBtn;
    private Image icon;

    private Action onClaim;
    private Sprite RewardImage;
    private RewardData data;
    public ERewardType rewardType;

  

    public void Setup(RewardData data, Sprite image, Action onClaimAction)
    {
        this.onClaim = onClaimAction;
        this.RewardImage = image;
        this.data = data;
        rewardType = data.type;

        claimBtn = GetComponent<Button>();
        icon = GetComponentInChildren<Image>();

        icon.sprite = image;

        AssignBtn();
    }

    private void AssignBtn()
    {
        claimBtn.onClick.RemoveAllListeners();
        claimBtn.onClick.AddListener(() => {
            onClaim?.Invoke();
            AfterClaim();
        });
    }

    private void AfterClaim()
    {
        Destroy(gameObject);
    }
}
