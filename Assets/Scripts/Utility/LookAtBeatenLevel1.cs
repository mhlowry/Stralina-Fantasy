using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtBeatenLevel1 : MonoBehaviour
{
    [SerializeField] GameObject returnBasePause;
    [SerializeField] GameObject returnBaseGameOver;

    private void Start()
    {
        Debug.Log("Beaten Level 1: " + GameManager.instance.GetLevelsCompleted()[0]);
        if (!GameManager.instance.GetLevelsCompleted()[0])
        {
            returnBasePause.SetActive(false);
            returnBaseGameOver.SetActive(false);
        }
    }
}
