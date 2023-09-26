using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static VFXController;

public class ComboController : MonoBehaviour
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
    private VFXController vfxController;
    private PlayerController playerController;

    [SerializeField] private List<PlayerAttack> lightAttacks;
    [SerializeField] private List<PlayerAttack> heavyAttacks;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerController = GetComponent<PlayerController>();
        vfxController = GetComponent<VFXController>();
    }

    // Update is called once per frame
    private void Update()
    {
        //end the combo if too much time has passed since the last button press
        if (Time.time - lastAttackTime > uniAttackDelay + attackDuration)
            EndCombo();
        

        //If an attack is stored, attack at the earliest possible moment
        if(Time.time - lastAttackTime > attackDuration && storedAttackIndex > 0)
        {
            storedAttackIndex--;
            LightAttack();
        }

        //this scoots the player a little bit, giving attacks a degree of "oomph"
        if (Time.time - lastAttackTime < attackDuration)
        {
            //uses the direction the player is imputting in playerController
            characterController.Move(attackDirection * attackImpact * Time.deltaTime);
        }
    }

    //This gets called by the unity input manager.  Cool!
    public void CallLightAttack(InputAction.CallbackContext context)
    {
        //ensures that lightattack is only called once per button
        if (!context.started | playerIsLocked)
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
        //ensures that lightattack is only called once per button
        if (!context.started | playerIsLocked)
            return;

        HeavyAttack();
    }

    private void LightAttack()
    {
        lastAttackTime = Time.time;
        comboIndex++;

        vfxController.DisableAttackVFX();

        try
        {
            //performs an attack based on how many light attacks the player has performed
            switch (comboIndex)
            {
                case 1:
                    anim.SetBool("comboOver", false);
                    PerformAttack(lightAttacks[0]);

                    //Debug.Log("attack 1");
                    break;

                case 2:
                    PerformAttack(lightAttacks[1]);

                    //Debug.Log("attack 2");
                    break;

                case 3:
                    PerformAttack(lightAttacks[2]);

                    playerIsLocked = true;
                    //Debug.Log("attack 3");
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

        vfxController.DisableAttackVFX();

        try
        {
            //performs a heavy attack based on the comboIndex, and then locks  the player out of attacking until it's done.
            switch (comboIndex)
            {
                case 0:
                    anim.SetBool("comboOver", false);
                    PerformAttack(heavyAttacks[0]);

                    //Debug.Log("Heavy Attack: Raw");
                    break;

                case 2:
                    PerformAttack(heavyAttacks[1]);

                    //Debug.Log("Heavy Attack: Combo finisher 1");
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

    /* duration is how long the attack will last
     * impact is the momentum the attack carries
     * vfxIndex corresponds to the VFX animation that will get called by VFX controller
     */
    private void PerformAttack(PlayerAttack currentAttack)
    {
        anim.SetTrigger(currentAttack.animTrigger);

        attackDirection = playerController.direction;

        attackDuration = currentAttack.duration;
        attackImpact = currentAttack.impact;

        vfxController.playAttackVFX(currentAttack.vfxIndex);
    }

    private void EndCombo()
    {        
        comboIndex = 0;
        storedAttackIndex = 0;
        playerIsLocked = false;
        anim.SetBool("comboOver", true);    
        vfxController.DisableAttackVFX();
    }

    //This has the information a given attack needs
    [System.Serializable]
    public class PlayerAttack
    {
        public string animTrigger;

        public float duration;
        public float impact;
        public int vfxIndex;
    }
}
