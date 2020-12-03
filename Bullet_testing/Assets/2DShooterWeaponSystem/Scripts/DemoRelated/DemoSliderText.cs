using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class DemoSliderText : MonoBehaviour
{
    private Slider slider;
    public Text text;
    public string prefix;
    public string unit;
    public byte decimals = 2;

    void OnEnable()
    {
        slider = GetComponent<Slider>();
        slider.onValueChanged.AddListener(ChangeValue);
        ChangeValue(slider.value);
    }

    void OnDisable()
    {
        slider.onValueChanged.RemoveAllListeners();
    }

    void ChangeValue(float value)
    {
        text.text = prefix + " " + value.ToString("n" + decimals) + " " + unit;
    }
}