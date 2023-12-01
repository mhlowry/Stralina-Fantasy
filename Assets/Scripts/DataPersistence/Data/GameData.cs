using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    //public int deathCount;
    //public Vector3 playerPosition;
    // public bool disableInput;
    public int playerLevel;
    public int curExp;
    public int curGold = 0;
    public bool hasSpokenToShopkeeper;
    // public int[] expToLevelUp;
    // public int maxHealth;
    // public int curHealth;
    // public float maxAbilityMeter;
    // public float curAbilityMeter;
    // public float attackScale;
    // public float defenseScale;
    // public float moveSpeedScale;
    // public float lightDmgScale;
    // public float heavyDmgScale;
    // public float atkSpeedScale;
    // public float atkKnockBScale;
    // public bool isStunned;
    // public bool isInvul;
    // public bool invulOverride;
    public bool [] levelsCompleted;

    // The values defined in this constructor will be the default values
    // the game starts when there's no data to load
    public GameData()
    {
        // disableInput = false;
        //deathCount = 0;
        playerLevel = 1;
        curExp = 0;
        curGold = 0;
        // expToLevelUp = new int[] 
        //                 { 100, 500, 1000, 2000, 3000, 
        //                 5000, 10000 };
        // maxHealth = 10;
        // curHealth = maxHealth;
        // maxAbilityMeter = 100f;
        // curAbilityMeter = 0f;
        // attackScale = 1f;
        // defenseScale = 1f;
        // moveSpeedScale = 1f;
        // lightDmgScale = 1f;
        // heavyDmgScale = 1f;
        // atkSpeedScale = 1f;
        // atkKnockBScale = 1f;
        // isStunned = false;
        // isInvul = false;
        // invulOverride = false;
        //playerPosition = new Vector3(-1.02f, 1.56f, -2.13f);
        levelsCompleted = new bool[] 
                            { false, false, false, false, false,
                            false, false, false, false, false };
        hasSpokenToShopkeeper = false;
        
    }
}
