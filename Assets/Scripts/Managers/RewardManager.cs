using UnityEngine;

public class RewardManager : MonoBehaviour
{
    private PlayerController playerController;
    private GunManager gunManager;

    [Header("InGameUi")]
    [SerializeField] private Transform RewardItemHolder;
    [SerializeField] private GameObject RewardItemPrefab;

    [Header("GunComboRewards")]
    [SerializeField] private GunRewardOnComboData[] gunComboRewards;

    private UiRewardItem gunReward;

    private int lastComboRewardGiven = 0;

    private void Start()
    {
        playerController = FindFirstObjectByType<PlayerController>();
        gunManager = playerController.gunManager;

        ResetRewards();
    }


    public void CheckForComboReward(int currentCombo)
    {
        CheckGunRewardOnCombo(currentCombo);
    }

    public void CheckGunRewardOnCombo(int currentCombo)
    {
        if (currentCombo <= lastComboRewardGiven)
            return;

        foreach (var gunReward in gunComboRewards)
        {
            if (currentCombo == gunReward.combo)
                if (gunManager.IsGunUnlocked(gunReward.gunId, out GunSo gunData))
                {
                    GiveGunReward(gunData);
                    lastComboRewardGiven = currentCombo;
                }
        }


    }
    public void GiveGunReward(GunSo gunData)
    {
        Debug.Log("Giving Gun Reward" + gunData.gunID);
        if (gunReward != null)
            Destroy(gunReward.gameObject);

        UiRewardItem newItem = Instantiate(RewardItemPrefab, RewardItemHolder).GetComponent<UiRewardItem>();

        Debug.Log("Created ui" + newItem.gameObject.name);


        RewardData rewardData = new RewardData
        {
            type = ERewardType.Gun,
            hasAutoClaimed = false,
        };

        newItem.Setup(rewardData, gunData.gunIcon, () =>
        {
            playerController.EquipNewWeapon(gunData.gunID);
        });

        gunReward = newItem;
    }

    public void ResetRewards()
    {
        lastComboRewardGiven = 0;
        if (gunReward != null)
            Destroy(gunReward.gameObject);
        gunReward = null;
    }

    public void Penalty()
    {
        playerController.EquipNewWeapon(0);
        ResetRewards();
    }
}
