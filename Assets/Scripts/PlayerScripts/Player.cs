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
    /*
    const int maxLevel = 7;
    int playerLevel = 1;
    int[] expToLevelUp = { 100, 500, 1000, 2000, 3000, 5000, 10000 };
    int curExp = 0;

    float maxAbilityMeter = 100f;
    float curAbilityMeter = 0f;

    //Stats that are adjustable in the editor and by gear
    float attackScale = 1f;
    */
    float defenseScale = 1f;
    /*
    float moveSpeedScale = 1f;

    float lightDmgScale = 1f;
    float heavyDmgScale = 1f;
    float atkSpeedScale = 1f;
    float atkKnockBScale = 1f;
    */
    //fields for when the player takes damage
    [SerializeField] float hitInvulTime = 0.3f;
    [SerializeField] float hitStunTime = 0.2f;
    float timeofHit;
    bool isStunned = false;
    bool isInvul = false;

    //Movement Controller and Combo Controller Respectively
    private PlayerCombat playerCombat;

    //REMEMBERR TO DRAG GFX INTO ANIM
    [SerializeField] private GameObject gfxObj;

    private SpriteRenderer spriteRendererGFX;
    private Animator animGFX;
    private void Awake()
    {
        curHealth = maxHealth;
        playerCombat = GetComponent<PlayerCombat>();

        spriteRendererGFX = gfxObj.GetComponent<SpriteRenderer>();
        animGFX = gfxObj.GetComponent<Animator>();
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

        //end combo
        playerCombat.EndCombo();

        if (curHealth <= 0)
            GameOver();
    }

    public void HealPlayer(int healthGain)
    {
        curHealth = Mathf.Clamp(curHealth + healthGain, 1, maxHealth);
    }

    //Well, what's it sound like it does?
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
}
