using System.Collections;
using TMPro;
using UnityEngine;

public class UiCountTo : MonoBehaviour
{
    private float numberToReach;

    [SerializeField] float timeToReach = 1.1f;

    private TextMeshProUGUI text;
    private void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        if (text == null)
            Debug.Log("This script only uses on component with text mesh pro " + gameObject.name);
    }


    public void StartSettingTheNumber(float number, float newTImeToReach = 0)
    {

        if (newTImeToReach > 0)
            timeToReach = newTImeToReach;



        numberToReach = number;

        Debug.Log($"passed number {number} this made numbertoreach to {numberToReach}");

        if (numberToReach < 0)
            return;

        StartCoroutine(CountToCo(timeToReach));

    }

    private IEnumerator CountToCo(float duration)
    {
        Debug.Log("StartCo" + duration);

        float elaps = 0;
        float currentNum = 0;
        while (elaps < duration)
        {

            currentNum = Mathf.Lerp(0, numberToReach, elaps / duration);

            WriteToTextHole(currentNum);


            yield return null;
            elaps += Time.deltaTime;
        }

        WriteToTextHole(numberToReach);

    }

    public void ResetNumber()
    {
        numberToReach = 0;
        WriteToTextHole(0);

    }

    private void WriteToTextHole(float num)
    {
        if (text == null)
            text = GetComponent<TextMeshProUGUI>();
        if (text == null)
        {
            Debug.Log("Could not find Text mesh pro");
            return; 
        }
        text.text = num.ToString(".");
    }
}
