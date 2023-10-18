using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerCombat))]
public class Player : MonoBehaviour
{
    //STATS
    bool disableInput = false;

    const int maxLevel = 7;
    [SerializeField, Range(1, maxLevel)] int playerLevel = 1;
    [SerializeField] int[] expToLevelUp = { 100, 500, 1000, 2000, 3000, 5000, 10000 };
    int curExp = 0;

    [SerializeField] int maxHealth = 10;
    [SerializeField] int curHealth;

    const float maxAbilityMeter = 100f;
    [SerializeField, Range(1, maxAbilityMeter)] float curAbilityMeter = 0f;

    //Stats that are adjustable, mostly through gear
    float attackScale = 1f;
    float defenseScale = 1f;
    protected float moveSpeedScale = 1f;
    
    float lightDmgScale = 1f;
    float heavyDmgScale = 1f;
    float atkSpeedScale = 1f;
    float atkKnockBScale = 1f;
    
    //fields for when the player takes damage
    [SerializeField] float hitInvulTime = 1.2f;
    [SerializeField] float hitStunTime = 0.6f;
    float timeofHit;
    bool isStunned = false;
    bool isInvul = false;
    bool invulOverride = false;

    //REMEMBERR TO DRAG GFX INTO ANIM
    [SerializeField] private GameObject gfxObj;
    [SerializeField] private ResourceBar healthBar;
    [SerializeField] private ResourceBar meterBar;

    private SpriteRenderer spriteRendererGFX;
    protected Animator animGFX;
    protected PlayerCombat playerCombat;

    //Doing certain things in awake and some things in start is really important apparently
    private void Awake()
    {
        playerCombat = GetComponent<PlayerCombat>();

        spriteRendererGFX = gfxObj.GetComponent<SpriteRenderer>();
        animGFX = gfxObj.GetComponent<Animator>();
    }

    private void Start()
    {
        meterBar.SetMaxResource((int)maxAbilityMeter);
        curAbilityMeter = 0.0f;
        meterBar.SetResource((int)curAbilityMeter);

        healthBar.SetMaxResource(maxHealth);
        curHealth = maxHealth;
    }

    protected virtual void Update()
    {
        //either un-stun the player or 
        if (Time.time - timeofHit > hitStunTime)
        {
            //take out of hitstun state
            animGFX.SetBool("inPain", false);
            isStunned = false;
        }

        if (Time.time - timeofHit > hitInvulTime && !invulOverride)
        {
            isInvul = false;
        }
    }

    //Disable/Enable player's ability to do anything
    public bool CanInput() {  return !disableInput; }
    public void DisableInput() { disableInput = true; }
    public void EnableInput() { disableInput = false; }

    //Health-related functions
    public int GetMaxHealth() { return maxHealth; }
    public int GetCurHealth() {  return curHealth; }
    public bool IsStunned() {  return isStunned; }

    public virtual void TakeDamage(int damage)
    {
        //do not take damage if invulnerable
        if (isInvul)
            return;

        //adjust numbers
        timeofHit = Time.time;
        curHealth -= (int)(damage * (2 - defenseScale));
        isStunned = true;
        isInvul = true;

        //play the take damage animation
        animGFX.SetBool("inPain", true);
        StartCoroutine(BlinkEffect());

        //update health bar
        healthBar.SetResource(curHealth);

        //end the player's combo
        playerCombat.EndCombo();

        if (curHealth <= 0)
            GameOver();
    }

    public void HealPlayer(int healthGain)
    {
        curHealth = Mathf.Clamp(curHealth + healthGain, 1, maxHealth);
        healthBar.SetResource(curHealth);
    }

    IEnumerator BlinkEffect()
    {
        while (isInvul)
        {
            spriteRendererGFX.enabled = false;

            yield return new WaitForSeconds(0.1f);

            spriteRendererGFX.enabled = true;

            yield return new WaitForSeconds(0.1f);
        }
    }

    //Invulnerability related functions
    public void ToggleInvul() { isInvul = !isInvul; }
    public void SetInvulTrue()
    {
        invulOverride = true;
        isInvul = true;
    }
    public void SetInvulFalse()
    {
        invulOverride = false;
        isInvul = false;
    }

    //function to know when player is dead
    public static event Action OnPlayerDeath;

    void GameOver()
    {
        Debug.Log("You died lol");
        OnPlayerDeath?.Invoke();
    }

    //Level-related functions
    public int GetPlayerLevel() { return playerLevel; }
    public int GetPlayerExp() { return curExp; }

    public void GainExp(int expGain)
    {
        if (playerLevel == maxLevel)
            return;

        curExp += expGain;

        if (curExp >= expToLevelUp[playerLevel - 1])
        {
            LevelUp();
        }
    }

    void LevelUp()
    {
        curExp -= expToLevelUp[playerLevel - 1];
        playerLevel += 1;

        if (playerLevel == maxLevel)
            curExp = expToLevelUp[maxLevel - 1];
    }

    //Stats-related functions
    public float GetAttackScale() {  return attackScale; }
    public float GetMoveSpeedScale() { return moveSpeedScale; }

    //Combo-related functions
    public float GetLightDmgScale() { return lightDmgScale; }
    public float GetHeavyDmgScale() { return heavyDmgScale; }
    public float GetAtkSpeedScale() { return atkSpeedScale; }
    public float GetKnockBScale() { return atkKnockBScale; }

    //Meter-related functions
    public float GetMaxMeter() { return maxAbilityMeter; }
    public float GetCurMeter() { return curAbilityMeter; }

    public void GainMeter(float meterGained)
    {
        curAbilityMeter = Mathf.Clamp(curAbilityMeter + meterGained, 0, maxAbilityMeter);
        meterBar.SetResource((int)curAbilityMeter);
    }
    public void UseMeter(float meterUsed)
    {
        curAbilityMeter = Mathf.Clamp(curAbilityMeter - meterUsed, 0, maxAbilityMeter);
        meterBar.SetResource((int)curAbilityMeter);
    }
}
