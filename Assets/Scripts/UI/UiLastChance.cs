using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UiLastChance : MonoBehaviour
{
    UiManager uiManager;


    [SerializeField] private ShopManager shopManager;
    [SerializeField] private Button reviveRequestBtn;
    [SerializeField] private Button giveUpBtn;
    [SerializeField] private GameObject popUp;
    [SerializeField] private TextMeshProUGUI popUpText;
    [SerializeField] private Button ReviveBtn;


    private void Awake()
    {
        AsignBtns();
        popUp.SetActive(false);
    }

    private void AsignBtns()
    {
        reviveRequestBtn.onClick.AddListener(() =>
        {
            OnReviveRequestClicked();
        });

        giveUpBtn.onClick.AddListener(() =>
        {
            OnGiveUpClicked();
        });
        ReviveBtn.onClick.AddListener(() =>
        {
            OnReviveClicked();
        });
    }

    private void OnReviveRequestClicked()
    {
        popUp.SetActive(true);


    }

    private void OnReviveClicked()
    {
        if (shopManager.AnotherChanceRequest())
        {
            popUp.SetActive(false);
            GameManager.instance.ReviveThePlayer();
        }
        else
        {
            popUpText.text = "Sorry!\nNot Enough Medal of honor";
            ReviveBtn.gameObject.SetActive(false);
        }
    }


    private void OnGiveUpClicked()
    {
       GameManager.instance.GameOver();
    }

}
