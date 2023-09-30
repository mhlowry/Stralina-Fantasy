using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(Player))]

public class PlayerCombat : MonoBehaviour
{

    //REMEMBERR TO DRAG GFX INTO ANIM IN UNITY EDITOR
    [SerializeField] private Animator anim;

    [SerializeField] private float uniAttackDelay = 1f;
    private float attackDuration = 0f;
    private float lastAttackTime = 0f;
    private bool playerIsLocked = false;

    static float attackImpact = 0;

    private Vector3 attackDirection;

    static int comboIndex = 0;
    static int storedAttackIndex = 0;

    private CharacterController characterController;
    private PlayerMovement playerMovement;
    private Player player;

    [SerializeField] private LayerMask enemyLayers;

    [SerializeField] private List<PlayerAttack> lightAttacks;
    [SerializeField] private List<PlayerAttack> heavyAttacks;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerMovement = GetComponent<PlayerMovement>();
        player = GetComponent<Player>();
    }

    private void Update()
    {
        //end the combo if too much time has passed since the last button press
        if (Time.time - lastAttackTime > uniAttackDelay + attackDuration)
            EndCombo();


        //If an attack is stored, attack at the earliest possible moment
        if (Time.time - lastAttackTime > attackDuration && storedAttackIndex > 0)
        {
            storedAttackIndex--;
            LightAttack();
        }

        //this scoots the player a little bit, giving attacks a degree of "oomph"
        if (Time.time - lastAttackTime < attackDuration)
        {
            //uses the direction the player is imputting in playerMovement
            characterController.Move(attackDirection * attackImpact * Time.deltaTime);
        }
    }

    //This gets called by the unity input manager.  Cool!
    public void CallLightAttack(InputAction.CallbackContext context)
    {
        //ensures that lightattack is only called once per button, and only if the player is actionable
        if (!context.started | playerIsLocked | player.IsStunned())
            return;

        //When the player is still in an attacking animation but 
        if (Time.time - lastAttackTime < attackDuration)
        {
            storedAttackIndex++;
            return;
        }

        LightAttack();
    }

    //You'll never guess what calls this function!  (The Unity Input Manager)
    public void CallHeavyAttack(InputAction.CallbackContext context)
    {
        //ensures that heavyattack is only called once per button, and only if the player is actionable
        if (!context.started | playerIsLocked | player.IsStunned())
            return;

        HeavyAttack();
    }

    private void LightAttack()
    {
        lastAttackTime = Time.time;
        comboIndex++;

        try
        {
            //performs an attack based on how many light attacks the player has performed
            switch (comboIndex)
            {
                case 1:
                    anim.SetBool("comboOver", false);
                    PerformAttack(lightAttacks[0]);
                    break;

                case 2:
                    PerformAttack(lightAttacks[1]);
                    break;

                case 3:
                    PerformAttack(lightAttacks[2]);
                    playerIsLocked = true;
                    break;

                default:
                    Debug.Log("LightAttack: Invalid Combo Index");
                    break;
            }
        }
        catch (ArgumentOutOfRangeException aRef)
        {
            Debug.Log(aRef);
            Debug.Log("PlayerAttack does not exist in the array: LightAttacks");
        }
    }

    private void HeavyAttack()
    {
        lastAttackTime = Time.time;

        try
        {
            //performs a heavy attack based on the comboIndex, and then locks  the player out of attacking until it's done.
            switch (comboIndex)
            {
                case 0:
                    //Raw Heavy Attack
                    anim.SetBool("comboOver", false);
                    PerformAttack(heavyAttacks[0]);
                    break;

                case 2:
                    PerformAttack(heavyAttacks[1]);
                    break;

                default:
                    Debug.Log("No Heavy Attack for this combo");
                    break;
            }

            playerIsLocked = true;
        }
        catch (ArgumentOutOfRangeException aRef)
        {
            Debug.Log(aRef);
            Debug.Log("PlayerAttack does not exist in the array: HeavyAttacks");
        }
    }
    public void EndCombo()
    {
        //reset all the important combo values to the necessary state
        comboIndex = 0;
        storedAttackIndex = 0;
        playerIsLocked = false;

        //End animation
        anim.SetBool("comboOver", true);
    }

    private void PerformAttack(PlayerAttack currentAttack)
    {
        //animate attack
        anim.SetTrigger(currentAttack.getAnim());
        
        //set the attack's direction to the player's direction (notably used for VFX)
        attackDirection = playerMovement.direction;

        //Get duration of the attack (endlag) and the impact (momentum)
        attackDuration = currentAttack.getDuration();
        attackImpact = currentAttack.getImpact();

        if (currentAttack.getVfxObj() == null)
        {
            Debug.Log("Attack's vfxObj is null");
            return;
        }

        //play vfx
        DisableAttackVFX(currentAttack.getVfxObj());
        StartCoroutine(PlayAttackVFX(currentAttack.getVfxObj(), currentAttack.getDelay()));
        
        if (currentAttack.getHitBoxes() == null)
        {
            Debug.Log("Attack's hitBox transform is null");
            return;
        }

        //hurt enemies
        StartCoroutine(DealDamage(currentAttack.getHitBoxes(), currentAttack.getDelay()));
    }

    private IEnumerator DealDamage(List<HitBox> hitBoxes, float delay)
    {
        yield return new WaitForSeconds(delay);

        List<Collider[]> hitEnemies = new List<Collider[]>();

        foreach (HitBox hitBox in hitBoxes)
        {
            hitEnemies.Add(Physics.OverlapSphere(hitBox.getPosition(), hitBox.getSize(), enemyLayers));
        }

        //This is to prevent enemies from getting hit twice if they're in range of 2 hitboxes
        HashSet<Collider> loggedEnemies = new HashSet<Collider>();

        foreach (Collider[] enemyList in hitEnemies)
        {
            foreach (Collider enemy in enemyList)
            {
                if (!loggedEnemies.Contains(enemy))
                {
                    enemy.GetComponent<Enemy>().TakeDamage(1);

                    loggedEnemies.Add(enemy);
                }
            }
        }
    }

    //This function 
    private IEnumerator PlayAttackVFX(GameObject vfxObj, float delay)
    {
        //Wait some time before spawning vfx
        yield return new WaitForSeconds(delay);

        //make sure the vfx is rotated in the right direction
        vfxObj.transform.rotation = transform.rotation;
        vfxObj.SetActive(true);

        //wait some time before diabling the vfx (needs fix)
        yield return new WaitForSeconds(0.8f);
        DisableAttackVFX(vfxObj);
    }

    private void DisableAttackVFX(GameObject vfxObj)
    {
        vfxObj.SetActive(false);
    }

    //This lets us see the hitboxes in the editor
    private void OnDrawGizmosSelected()
    {
        //Look at the hitboxes for light attacks
        foreach (PlayerAttack lightAttack in lightAttacks)
        {
            foreach (HitBox hitBox in lightAttack.getHitBoxes())
            {
                if (hitBox.getTransform() == null)
                    continue;
                Gizmos.DrawWireSphere(hitBox.getPosition(), hitBox.getSize());
            }
        }

        //Look at the hitboxes for heavy attacks
        foreach (PlayerAttack heavyAttack in heavyAttacks)
        {
            foreach (HitBox hitBox in heavyAttack.getHitBoxes())
            {
                if (hitBox.getTransform() == null)
                    continue;
                Gizmos.DrawWireSphere(hitBox.getPosition(), hitBox.getSize());
            }
        }
    }
}
