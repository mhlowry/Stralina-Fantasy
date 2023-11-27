using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceBar : MonoBehaviour
{
    public Slider slider;
    private Image filling;

    private void Awake()
    {
        slider = GetComponent<Slider>();
        filling = transform.GetChild(0).gameObject.GetComponent<Image>();
    }

    public void SetMaxResource(int value)
    {
        slider.maxValue = value;
        slider.value = value;
    }

    public void SetResource(int value)
    {
        slider.value = value;
    }

    public void SetColor(Color color)
    {
        filling.color = color;
    }
}
