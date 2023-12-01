using System;
using System.Collections;
using Cinemachine;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerCombat))]
[RequireComponent(typeof(BoostManager))]
[RequireComponent(typeof(CinemachineImpulseSource))]
public class Player : MonoBehaviour, IDataPersistence
{
    //STATS
    bool disableInput = false;

    const int maxLevel = 7;
    [SerializeField, Range(1, maxLevel)]  int playerLevel = 1;
    int[] expToLevelUp = { 100, 500, 1000, 2000, 3000, 5000, 6000 };
    [SerializeField] int curExp = 0;
    [SerializeField] int curGold = 0;
    [SerializeField] private GoldDisplay goldDisplay;

    int maxHealth = 15;
    [SerializeField] int curHealth;

    const float maxAbilityMeter = 100f;
    [SerializeField, Range(0, maxAbilityMeter)] float curAbilityMeter = 0f;

    //STATS SHIT
    BoostManager boostManager;
    
    float attackScale = 1f;
    float defenseScale = 1f;

    protected float moveSpeedScale = 1f;

    float lightDmgScale = 1f;
    float heavyDmgScale = 1f;
    float atkSpeedScale = 1f;
    float atkKnockBScale = 1f;
    
    //fields for when the player takes damage
    float hitInvulTime = 1.2f;
    float hitStunTime = 0.6f;
    float timeofHit;
    bool isStunned = false;
    bool isInvul = false;
    bool invulOverride = false;

    //fields for knockback
    [SerializeField] private float decelerationRate = 1.0f;
    protected float takenKnockback;
    protected Vector3 currentKnockbackDir;

    //REMEMBER TO DRAG GFX INTO ANIM
    [SerializeField] private GameObject gfxObj;
    private ResourceBar healthBar;
    private ResourceBar meterBar;
    private ResourceBar expBar;

    [HideInInspector] public bool isTeleporting;

    private CharacterController characterController;
    private SpriteRenderer spriteRendererGFX;
    private CinemachineImpulseSource impulseSource;
    protected Animator animGFX;
    protected PlayerCombat playerCombat;
    protected PlayerMovement playerMovement;

    private GameOverMenu gameOverMenu;

    // dialogue stuff
    private DialogueUI dialogueUI;
    public DialogueUI DialogueUI => dialogueUI;
    public IInteractable Interactable { get; set; }

    //Doing certain things in awake and some things in start is really important apparently
    private void Awake()
    {
        playerCombat = GetComponent<PlayerCombat>();
        playerMovement = GetComponent<PlayerMovement>();

        spriteRendererGFX = gfxObj.GetComponent<SpriteRenderer>();
        animGFX = gfxObj.GetComponent<Animator>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
        characterController = GetComponent<CharacterController>();
        boostManager = GetComponent<BoostManager>();

        // should be in awake!
        try
        {
            playerLevel = GameManager.instance.GetPlayerLevel();
            curExp = GameManager.instance.GetPlayerExp();
        }
        catch { }

        gameOverMenu = FindObjectOfType<GameOverMenu>();

        try
        {
            healthBar = GameObject.Find("HealthBar").GetComponent<ResourceBar>();
            meterBar = GameObject.Find("MeterBar").GetComponent<ResourceBar>();
            expBar = GameObject.Find("ExpBar").GetComponent<ResourceBar>();
            dialogueUI = GameObject.Find("Canvas (base)").GetComponent<DialogueUI>();
        }
        catch { }
    }

    private void Start()
    {
        meterBar.SetMaxResource((int)maxAbilityMeter);
        meterBar.SetResource((int)curAbilityMeter);

        maxHealth += boostManager.healthPointBoosts * 2;
        healthBar.SetMaxResource(maxHealth);
        curHealth = maxHealth;

        expBar.SetMaxResource(expToLevelUp[playerLevel - 1]);
        expBar.SetResource(curExp);
    }

    //Set the spawnpoint of the player on scene load
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Transform spawnpoint = GameObject.Find("SPAWNPOINT").transform;
        try
        {
            transform.position = spawnpoint.position;
        }
        catch 
        {
            Debug.Log("SPAWNPOINT object not found");
        }
    }

    public void CallTogglePause(InputAction.CallbackContext context)
    {
        if (!context.started || isStunned || curHealth <= 0) // || !anim.GetBool("comboOver")
            return;

        Debug.Log("called pause");

        if (PauseMenu.isPaused)
        {
            PauseMenu.instance.ResumeGame();
        }
        else
        {
            PauseMenu.instance.PauseGame();
        }
    }

    protected virtual void Update()
    {
        //either un-stun the player or 
        if (Time.time - timeofHit > (hitStunTime - boostManager.willpowerBoosts * 0.1f))
        {
            //take out of hitstun state
            animGFX.SetBool("inPain", false);
            isStunned = false;
        }
        else
        {
            //adds deceleration to the knockback being taken
            float deceleration = decelerationRate * Time.deltaTime;
            takenKnockback = Mathf.Max(0.0f, takenKnockback - deceleration);

            //uses the direction the player is imputting in playerMovement
            characterController.Move(currentKnockbackDir * takenKnockback * Time.deltaTime);
            //makes sure that gravity is still being applied while attacking
            playerMovement.ApplyGravity();
        }

        if (Time.time - timeofHit > (hitInvulTime + boostManager.willpowerBoosts * 0.2f) && !invulOverride)
        {
            isInvul = false;
        }
    }

    public void CallDialogue(InputAction.CallbackContext context)
    {
        //ensures that is only called once per button
        if (!context.started)
            return;

        Interactable?.Interact(this);
    }

    public void LoadData(GameData data)
    {
        this.playerLevel = data.playerLevel;
        this.curExp = data.curExp;
        // Update any UI or game elements that depend on these values
        expBar.SetMaxResource(expToLevelUp[playerLevel - 1]);
        expBar.SetResource(curExp);
    }

    public void SaveData(ref GameData data)
    {
        data.playerLevel = this.playerLevel;
        data.curExp = this.curExp;
    }

    //Disable/Enable player's ability to do anything
    public bool CanInput() {  return !disableInput; }
    public void DisableInput() { disableInput = true; }
    public void EnableInput() { disableInput = false; }

    //Health-related functions
    public int GetMaxHealth() { return maxHealth; }
    public int GetCurHealth() {  return curHealth; }
    public bool IsStunned() {  return isStunned; }

    public virtual void TakeDamage(int damage, float knockBack, Vector3 enemyPosition)
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
        //shake the camera
        CameraShake.instance.ShakeCamera(impulseSource);
        //hitstop on small hits if less than hitstop on big hits
        if (damage < 4)
        {
            HitStop.instance.Stop(0.15f);
            AudioManager.instance.PlayAll(new string[] {"damage_2", "damage_3"});
        }
        else
        {
            HitStop.instance.Stop(0.4f);
            AudioManager.instance.PlayAll(new string[] {"heavydamage_1", "heavydamage_2", "heavydamage_3" });
        }

        //deal knockback to the player
        Vector3 playerPosition = transform.position;
        Vector3 relativePos = playerPosition - enemyPosition;
        Vector3 knockbackDir = relativePos.normalized;

        currentKnockbackDir = knockbackDir;
        takenKnockback = knockBack;
        //begin the blinking effect
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
            //do not set to false if timescale is zero
            if(Time.timeScale > 0)
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
        if (gameOverMenu != null)
        {
            gameOverMenu.SetGameOverText("YOU DIED");
        }

        if (PauseMenu.isPaused)
        {
            PauseMenu.instance.ResumeGame();
        }

        AudioManager.instance.Play("player_death");
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
        curGold += expGain;  // Gain gold equal to the experience gained
        expBar.SetResource(curExp);

        if (GameManager.instance != null)
        {
            GameManager.instance.SetPlayerExp(curExp);
            GameManager.instance.SetPlayerGold(curGold);
        }

        if (goldDisplay != null)
        {
            goldDisplay.UpdateGoldDisplay();  // Directly update the gold display
        }

        if (curExp >= expToLevelUp[playerLevel - 1])
        {
            LevelUp();
        }

        if (DataPersistenceManager.instance != null)
            DataPersistenceManager.instance.SaveGame();
    }

    void LevelUp()
    {
        curExp -= expToLevelUp[playerLevel - 1];
        playerLevel += 1;

        if (playerLevel == maxLevel)
            curExp = expToLevelUp[maxLevel - 1];

        expBar.SetMaxResource(expToLevelUp[playerLevel - 1]);
        expBar.SetResource(curExp);

        if (DataPersistenceManager.instance != null)
            DataPersistenceManager.instance.SaveGame();

        if (GameManager.instance != null)
        {
            GameManager.instance.SetPlayerExp(curExp);
            GameManager.instance.SetPlayerLevel(playerLevel);
        }
    }

    //Stats-related functions
    public float GetAttackScale() {  return attackScale; }
    public float GetMoveSpeedScale() { return moveSpeedScale + (boostManager.speedBoosts * 0.1f); }

    //Combo-related functions
    public float GetLightDmgScale() { return lightDmgScale + (boostManager.lightAttackBoosts * 0.5f); }
    public float GetHeavyDmgScale() { return heavyDmgScale + (boostManager.heavyAttackBoosts * 0.5f); }
    public float GetAtkSpeedScale() { return atkSpeedScale; }
    public float GetKnockBScale() { return atkKnockBScale; }

    //Meter-related functions
    public float GetMaxMeter() { return maxAbilityMeter; }
    public float GetCurMeter() { return curAbilityMeter; }

    public void GainMeter(float meterGained)
    {
        curAbilityMeter = Mathf.Clamp(curAbilityMeter + meterGained, 0, maxAbilityMeter);
        meterBar.SetResource((int)curAbilityMeter);
        ChangeMeterColor();
    }
    public void UseMeter(float meterUsed)
    {
        curAbilityMeter = Mathf.Clamp(curAbilityMeter - meterUsed, 0, maxAbilityMeter);
        meterBar.SetResource((int)curAbilityMeter);
        ChangeMeterColor();
    }

    void ChangeMeterColor()
    {
        Color colorMeter;
        if (curAbilityMeter >= 15)
            colorMeter = Color.blue;
        else
            colorMeter = new Color(1.0f, 0.64f, 0.0f); //Orange

        meterBar.SetColor(colorMeter);
    }

    void OnValidate()
    {
        // Check if the game is currently running, as we don't want to save during edit mode
        if (Application.isPlaying)
        {
            // Save the game whenever the level or EXP is changed in the Inspector
            if(DataPersistenceManager.instance != null)
                DataPersistenceManager.instance.SaveGame();
        }
    }

}
