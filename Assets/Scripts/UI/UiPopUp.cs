using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiPopUp : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI popUpText;
    [SerializeField] private Button popUpBtn;

    public void Deactive()
    {
        gameObject.SetActive(false);
    }


    private void OnEnable()
    {
        popUpText.text = "Spend 1 medal of honor to revive";
        popUpBtn.gameObject.SetActive(true);
    }

    
}
