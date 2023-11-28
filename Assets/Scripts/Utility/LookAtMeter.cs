using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LookAtMeter : MonoBehaviour
{
    GameObject playerObject;
    Player playerScript;

    [SerializeField] List<Sprite> meterGuageSwdMan;

    void Awake()
    {
        //Find Player
        playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            playerScript = playerObject.GetComponent<Player>();
        }
    }

    void LateUpdate()
    {
        if (playerScript.GetCurMeter() < 15)
        {
            gameObject.GetComponent<Image>().sprite = meterGuageSwdMan[0];
        }
        else if (playerScript.GetCurMeter() >= 100)
        {
            gameObject.GetComponent<Image>().sprite = meterGuageSwdMan[2];
        }
        else
        {
            gameObject.GetComponent<Image>().sprite = meterGuageSwdMan[1];
        }
    }
}
