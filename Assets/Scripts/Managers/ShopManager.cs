using UnityEngine;

public class ShopManager : MonoBehaviour
{
    UiManager uiManager;
    PlayerController playerController;

    [Header("Setup")]
    [SerializeField] private int anotherChanceCoinCost = 1;

    private void Awake()
    {
        uiManager = FindFirstObjectByType<UiManager>();
        playerController = FindFirstObjectByType<PlayerController>();
    }

    private void ChangePlayersPoints(float points)
    {
        PlayerData playerData = GameManager.instance.playerData;


        playerData.totalPoints += points;
        SaveManager.instance.Save();

        uiManager.UpdateTotalPoints();
    }
    private void ChangePlayersSoldirCoin(int coins)
    {
        PlayerData playerData = GameManager.instance.playerData;


        playerData.totalPoints += coins;
        SaveManager.instance.Save();

        uiManager.UpdateTotalPoints();
    }

    public bool AnotherChanceRequest()
    {
        PlayerData playerData = GameManager.instance.playerData;

        if (playerData.soldirCoins < anotherChanceCoinCost)
            if (GameManager.instance.isTesting == false)
                return false;

        ChangePlayersSoldirCoin(anotherChanceCoinCost);
        return true;

    }

    public bool GunPurchase(GunSo gun)
    {
        PlayerData playerData = GameManager.instance.playerData;

        float gunPrice = gun.WeaponData.gunPrice;
        float playerPoints = playerData.totalPoints;

        if (GameManager.instance.isTesting)
            playerPoints = Mathf.Infinity;

        if (playerPoints < gunPrice)
            return false;

        ChangePlayersPoints(-gunPrice);

        int[] unlockedWeapons = new int[playerData.unlockedWeapons.Length + 1];
        playerData.unlockedWeapons.CopyTo(unlockedWeapons, 0);
        unlockedWeapons[unlockedWeapons.Length - 1] = gun.gunID;

        playerData.unlockedWeapons = unlockedWeapons;


        //playerController.EquipNewWeapon(gun.gunID);

        return true;
    }
}
