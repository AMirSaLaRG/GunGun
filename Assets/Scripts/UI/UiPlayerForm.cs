using UnityEngine;
using TMPro;

public class UiPlayerForm : MonoBehaviour
{
    [SerializeField] private TMP_InputField _input;


    public void NewNameInput()
    {
        string newName = _input.text.Trim();
        GameManager.instance.playerData.name = newName;
    }

    public void SubmitNameChange()
    {
        GameManager.instance.playerData.name = _input.text.Trim();
        if (GameManager.instance.currentLeaderBoardRank != 0)
        {
            GameManager.instance.UpdateNameChangeLeaderBoard();
        }
    }

    private void OnEnable()
    {
        if (GameManager.instance.playerData != null)
        {
            _input.text = GameManager.instance.playerData.name;
        }
    }
}
