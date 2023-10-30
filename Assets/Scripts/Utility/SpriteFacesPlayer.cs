using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class SpriteFacesPlayer : MonoBehaviour
{
    private GameObject playerObject;


    // Start is called before the first frame update
    void Awake()
    {
        //Find Player
        playerObject = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        // Call the FlipCheck method to handle sprite flipping
        FlipCheck();
    }

    private void FlipCheck()
    {
        // Get the direction from the enemy to the player
        Vector3 directionToPlayer = playerObject.transform.position - transform.position;

        // Check the direction and flip the sprite accordingly
        if (directionToPlayer.x < 0)
        {
            // The enemy is to the right of the player, so flip the sprite to face right
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (directionToPlayer.x > 0)
        {
            // The enemy is to the left of the player, so flip the sprite to face left
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }
}
