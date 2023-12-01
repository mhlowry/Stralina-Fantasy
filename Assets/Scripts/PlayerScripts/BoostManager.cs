using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostManager : MonoBehaviour
{
    [Range(0, 5)] public int lightAttackBoosts = 0;
    public int lightAttackSpecificBoosts = 0;  // Tracks specific boosts for light attack

    [Range(0, 5)] public int heavyAttackBoosts = 0;
    public int heavyAttackSpecificBoosts = 0;  // Tracks specific boosts for heavy attack

    [Range(0, 5)] public int speedBoosts = 0;
    public int speedSpecificBoosts = 0;        // Tracks specific boosts for speed

    [Range(0, 5)] public int willpowerBoosts = 0;
    public int willpowerSpecificBoosts = 0;    // Tracks specific boosts for willpower

    [Range(0, 5)] public int healthPointBoosts = 0;
    public int healthSpecificBoosts = 0;       // Tracks specific boosts for health points
}
