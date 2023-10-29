using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComboLookAtLevel : MonoBehaviour
{
    GameObject playerObject;
    Player playerScript;

    [SerializeField] List<Sprite> comboTreesSwdMan;


    void Awake()
    {
        //Find Player
        playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            playerScript = playerObject.GetComponent<Player>();
        }
    }

    void OnEnable ()
    {
        gameObject.GetComponent<Image>().sprite = comboTreesSwdMan[playerScript.GetPlayerLevel() - 1];
    }
}
