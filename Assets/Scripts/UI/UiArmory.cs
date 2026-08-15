using DG.Tweening;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UiArmory : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private Transform content;
    [SerializeField] private GameObject gunItemPrefab;
    [SerializeField] private GunManager gunManager;
    [SerializeField] private GameObject purchPage;
    [SerializeField] private UiGunItem purchContent;
    [SerializeField] private Button BuyBtn;
    [SerializeField] private TextMeshProUGUI popupPriceText;
    [SerializeField] private TextMeshProUGUI popupWarningText;

    private GunSo lookingGun;

    private Color popupWarningTextColor;

    private List<GunSo> gunsData = new List<GunSo>();

    private void Start()
    {
        popupWarningTextColor = popupWarningText.color;
    }

    private void PapulateGunItems()
    {
        if (gunManager == null)
            return;

        if (gunItemPrefab == null)
            return;

        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        gunsData.Clear();

        gunsData.AddRange(gunManager.allGuns);

        foreach (GunSo gunSo in gunsData)
        {
            UiGunItem newGunItem = Instantiate(gunItemPrefab, content.transform).GetComponent<UiGunItem>();
            newGunItem.Setup(gunSo);

            Button btn = newGunItem.GetComponent<Button>();
            if (btn == null)
                btn = newGunItem.AddComponent<Button>();

            List<int> unlockedGunsId = gunManager.GetUnlockedGunsId();
            if (unlockedGunsId.Count == 0)
                return;

            if (unlockedGunsId.Contains(gunSo.gunID))
            {
                newGunItem.SetUnlock();
                if (gunManager.currentGun == null)
                    if (gunSo.gunID == 0)
                        newGunItem.SetEquiped();
                    else { }
                else
                    if (gunManager.currentGun.gunID == gunSo.gunID)
                    newGunItem.SetEquiped();
            }
            

            btn.onClick.AddListener(() => OnGunSelected(gunSo.gunID));


        }

    }


    private void OnGunSelected(int gunId)
    {
        

        if (gunManager.unlockedGunsId.Contains(gunId))
        {
            if (gunManager.currentGun.gunID == gunId)
                return;
            EqupGun(gunId);
        }
        else
            PurchaseRequest(gunId);

        PapulateGunItems();
    }

    private void EqupGun(int gunId)
    {
        gunManager.SetCurrentGun(gunId);
    }
    private void PurchaseRequest(int gunId)
    {

        purchPage.SetActive(true);
        PopUpBtnFadeEffect(false);
        lookingGun = gunManager.gunDictionary[gunId];
        purchContent.Setup(lookingGun);
        popupPriceText.text = $"$ {lookingGun.WeaponData.gunPrice}";

    }

    public void DeActivePurchasePage()
    {
        popupWarningText.text = "";
        popupWarningText.color = popupWarningTextColor;

        lookingGun = null;
        purchPage.SetActive(false);
    }

    public void Buy()
    {
        bool bought = GameManager.instance.GunPurche(lookingGun);
        if (bought)
        {
            popupWarningText.text = "SUCCESSFUL\nYou bought the Gun \n Congratulation";
            PopUpBtnFadeEffect();

            PapulateGunItems();
        }
        else
        {
            popupWarningText.color = Color.red;
            popupWarningText.text = "FAILED\nYou dont have Requirement to purchase this Gun";

            BuyBtn.transform.DOShakePosition(.5f);
            PopUpBtnFadeEffect();

        }

    }

    private void PopUpBtnFadeEffect(bool isFading = true)
    {
        float targetValue = isFading ? 0 : 1;

        Image[] images = BuyBtn.GetComponentsInChildren<Image>();
        foreach (Image image in images)
            image.DOFade(targetValue, .5f);

        TextMeshProUGUI[] texts = BuyBtn.GetComponentsInChildren<TextMeshProUGUI>();
        foreach (TextMeshProUGUI text in texts)
            text.DOFade(targetValue, .5f);
    }

    private void OnEnable()
    {
        PapulateGunItems();
    }

}
