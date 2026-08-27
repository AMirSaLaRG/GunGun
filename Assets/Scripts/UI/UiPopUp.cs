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
        popUpText.text = "";
        popUpBtn.gameObject.SetActive(true);
    }

    
}
