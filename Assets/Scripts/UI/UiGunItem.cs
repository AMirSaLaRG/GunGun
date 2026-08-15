using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiGunItem : MonoBehaviour
{
    [Header("Setting")]
    [SerializeField] private Image gunImage;
    [SerializeField] private TextMeshProUGUI gunNameText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private TextMeshProUGUI gunModeText;
    [SerializeField] private TextMeshProUGUI gunBulletsText;

    [SerializeField] private RectTransform price;
    [SerializeField] private RectTransform unlock;
    [SerializeField] private RectTransform equiped;

    public bool isUnlocked = false;
    public bool isEquiped = false;

    public void Setup(GunSo gunSo)
    {
        WeaponData data = gunSo.WeaponData;

        gunImage.sprite = gunSo.gunIcon;
        gunNameText.text = data.gunName;
        priceText.text = data.gunPierce.ToString();
        gunModeText.text = data.gunShotType.ToString();
        gunBulletsText.text = data.magazineAmoCap.ToString();

        SetStatus();
    }

    private void SetStatus()
    {
        price.gameObject.SetActive(!isUnlocked);
        unlock.gameObject.SetActive(isUnlocked && !isEquiped);
        equiped.gameObject.SetActive(isEquiped);
    }

    public void SetUnlock()
    {
        isUnlocked = true;
        SetStatus();
    }

    public void SetEquiped()
    {
        isEquiped = true;
        SetStatus();
    }


}
