using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceBar : MonoBehaviour
{

    public Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
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
}
