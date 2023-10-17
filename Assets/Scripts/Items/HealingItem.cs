using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingItem : Item
{
    [SerializeField] private int healAmount = 2;

    protected override void CollectItem(GameObject playerObj)
    {
        Player player = playerObj.GetComponent<Player>();
        player.HealPlayer(healAmount);

        base.CollectItem(playerObj);
    }
}
