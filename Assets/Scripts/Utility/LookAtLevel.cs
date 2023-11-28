using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LookAtLevel : MonoBehaviour
{
    GameObject playerObject;
    Player playerScript;

    [SerializeField] List<Sprite> imagesToPull;
    Image UIimage;

    void Awake()
    {
        //Find Player
        playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            playerScript = playerObject.GetComponent<Player>();
        }

        UIimage = gameObject.GetComponent<Image>();
    }

    void Update()
    {
        int levelIndex = playerScript.GetPlayerLevel() - 1;
        UIimage.sprite = imagesToPull[levelIndex];
    }
}
