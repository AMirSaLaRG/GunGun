using TMPro;
using UnityEngine;

public class UiCountTo : MonoBehaviour
{
    private float numberToReach;
    private float currentNumber;

    [SerializeField] float timeToReach = 1.1f;

    private TextMeshProUGUI text;
    private void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        if (text == null)
            Debug.Log("This script only uses on component with text mesh pro " + gameObject.name);
    }


    public void StartSettingTheNumber(float number, float timeToReach = 0)
    {
        if(timeToReach > 0)
            this.timeToReach = timeToReach;

        numberToReach = number;
        InvokeRepeating(nameof(CountingTo), 0, timeToReach / numberToReach);
    }

    public void ResetNumber()
    {
        numberToReach = 0;
        currentNumber = 0;
        WriteToText(0);

    }
    private void CountingTo()
    {

        if (currentNumber < numberToReach)
        {
            WriteToTextHole(currentNumber);
            currentNumber++;
        } else
        {
            WriteToTextHole(numberToReach);
            CancelInvoke(nameof(CountingTo));
        }
    }

    private void WriteToText(float num)
    {
        
        text.text = num.ToString(".0");
    }
    private void WriteToTextHole(float num)
    {
        
        text.text = num.ToString(".");
    }
}
