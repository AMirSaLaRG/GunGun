using System;
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
    [SerializeField] private AudioSource SecondaryUnlockedSfx;

    [Header("ReloadReward")]
    [SerializeField] private GunRewardOnComboData ReloadSecondary;

    [SerializeField] private int reloadComboRepeat = 15;




    private UiRewardItem gunReward;

    private int lastComboRewardGiven = 0;

    private bool isFirstTimeSecondaryWeaponUnlocked = true;

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
            {
                if (gunManager.IsGunUnlocked(gunReward.gunId))
                {
                    GiveGunReward(gunReward);
                    lastComboRewardGiven = currentCombo;
                } else
                {
                    playerController.gunManager.Reload(true);
                }

            }
            
        }

        CheckReloadReward(currentCombo);


    }

    private void CheckReloadReward(int currentCombo)
    {
        if (currentCombo > gunComboRewards[gunComboRewards.Length - 1].combo)
        {
            if ((currentCombo - gunComboRewards[gunComboRewards.Length - 1].combo) % reloadComboRepeat == 0)
            {
                if (gunManager.IsGunUnlocked(ReloadSecondary.gunId))
                    GiveGunReward(ReloadSecondary);
            }
        }
    }

    public void GiveGunReward(GunRewardOnComboData reward)
    {
        if (reward.isInstant == true)
        {
            playerController.EquipNewWeapon(reward.gunId, true);
            AudioManager.instance.PlaySfx(gunRewardSfx);
        }
        else
        {
            if (isFirstTimeSecondaryWeaponUnlocked)
                AudioManager.instance.PlaySfx(SecondaryUnlockedSfx);
            else
                AudioManager.instance.PlaySfx(gunRewardSfx);

            isFirstTimeSecondaryWeaponUnlocked = false;

            playerController.SetSecondaryWeapon(reward.gunId);
        }

        Instantiate(GunRewardVfx, vfxPos.position, Quaternion.identity);
    }

    public void ResetRewards()
    {
        if (lastComboRewardGiven > 1)
            AudioManager.instance.PlaySfx(gunPenaltySfx);

        lastComboRewardGiven = 0;


        if (gunReward != null)
            Destroy(gunReward.gameObject);
        gunReward = null;

        isFirstTimeSecondaryWeaponUnlocked = true;

    }

    public void Penalty()
    {
        

        playerController.ResetRewards();
        ResetRewards();
        

    }

 
}
