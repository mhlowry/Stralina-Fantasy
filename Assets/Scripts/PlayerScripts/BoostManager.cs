using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostManager : MonoBehaviour
{
    [Range(0, 5)] public int lightAttackBoosts = 0;

    [Range(0, 5)] public int heavyAttackBoosts = 0;

    [Range(0, 5)] public int speedBoosts = 0;
        
    [Range(0, 5)] public int willpowerBoosts = 0;

    [Range(0, 5)] public int healthPointBoosts = 0;
}
