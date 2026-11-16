using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class SliderCountUpHelperUi : MonoBehaviour
{
    private float currentvalue = 0;
    private float maxvalue = 1;
    public Slider mySlider;

    private void Start()
    {
        mySlider = GetComponent<Slider>();
        if (mySlider == null)
            Debug.LogWarning("Slider Is Missing");

        mySlider.value = currentvalue;

    }

    public void StartSliderCount(float maxValue)
    {
        this.maxvalue = maxValue;
        mySlider.maxValue = maxValue;

        InvokeRepeating(nameof(IncreaseValue), 0, 0.2f);
    }

    private void IncreaseValue()
    {
        currentvalue += .2f;
        mySlider.value = currentvalue;

        if (currentvalue > maxvalue)
        {
            gameObject.SetActive(false);
        }
    }
}
