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
    [SerializeField] private GameObject GunRewardVfx;
    [SerializeField] private Transform vfxPos;
    [SerializeField] private AudioSource gunRewardSfx;
    [SerializeField] private AudioSource gunPenaltySfx;

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
                if (gunManager.IsGunUnlocked(gunReward.gunId))
                {
                    GiveGunReward(gunReward);
                    lastComboRewardGiven = currentCombo;
                }
        }


    }
    public void GiveGunReward(GunRewardOnComboData reward)
    {
        if (reward.isInstant == true)
            playerController.EquipNewWeapon(reward.gunId, true);
        else
            playerController.SetSecondaryWeapon(reward.gunId);

        Instantiate(GunRewardVfx, vfxPos.position, Quaternion.identity);
        AudioManager.instance.PlaySfx(gunRewardSfx);
    }

    public void ResetRewards()
    {
        if (lastComboRewardGiven > 1)
            AudioManager.instance.PlaySfx(gunPenaltySfx);

        lastComboRewardGiven = 0;


        if (gunReward != null)
            Destroy(gunReward.gameObject);
        gunReward = null;
        
    }

    public void Penalty()
    {
        

        playerController.ResetRewards();
        ResetRewards();
        

    }
}
