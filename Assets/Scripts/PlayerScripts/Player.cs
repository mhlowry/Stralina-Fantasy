using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerCombat))]
public class Player : MonoBehaviour
{
    //STATS
    [SerializeField] int maxHealth = 100;
    [SerializeField] int curHealth;

    const int maxLevel = 7;
    [SerializeField] int playerLevel = 1;
    [SerializeField] int[] expToLevelUp = { 100, 500, 1000, 2000, 3000, 5000, 10000 };
    int curExp = 0;

    float maxAbilityMeter = 100f;
    float curAbilityMeter = 0f;

    //Stats that are adjustable, mostly through gear
    float attackScale = 1f;
    float defenseScale = 1f;
    float moveSpeedScale = 1f;
    
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

    //REMEMBERR TO DRAG GFX INTO ANIM
    [SerializeField] private GameObject gfxObj;
    [SerializeField] private ResourceBar healthBar;
    [SerializeField] private ResourceBar meterBar;

    private SpriteRenderer spriteRendererGFX;
    private Animator animGFX;
    private PlayerCombat playerCombat;

    private void Awake()
    {
        curHealth = maxHealth;
        playerCombat = GetComponent<PlayerCombat>();

        spriteRendererGFX = gfxObj.GetComponent<SpriteRenderer>();
        animGFX = gfxObj.GetComponent<Animator>();

        meterBar.SetMaxResource((int)maxAbilityMeter);
        healthBar.SetMaxResource(maxHealth);
    }

    private void Update()
    {
        //either un-stun the player or 
        if (Time.time - timeofHit > hitStunTime)
        {
            //take out of hitstun state
            animGFX.SetBool("inPain", false);
            isStunned = false;
        }
        if (Time.time - timeofHit > hitInvulTime)
        {
            isInvul = false;
        }
    }

    //Health-related functions
    public int GetMaxHealth() { return maxHealth; }
    public int GetCurHealth() {  return curHealth; }
    public bool IsStunned() {  return isStunned; }

    public void TakeDamage(int damage)
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

    void GameOver()
    {
        Debug.Log("You died lol");
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
            curExp = expToLevelUp[maxHealth - 1];
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
        healthBar.SetResource((int)curAbilityMeter);
        curAbilityMeter = Mathf.Clamp(curAbilityMeter + meterGained, 0, maxAbilityMeter);
    }
}
