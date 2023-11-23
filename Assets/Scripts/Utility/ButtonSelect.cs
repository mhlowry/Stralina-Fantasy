using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSelect : MonoBehaviour
{
    [SerializeField] private Button selectButton;

    // Start is called before the first frame update
    private void OnEnable()
    {
        selectButton.Select();
    }
}
