using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
    private PlayerController playerController;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerController = GetComponent<PlayerController>();
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

        //performs an attack based on how many light attacks the player has performed
        switch (comboIndex)
        {
            case 1:
                anim.SetBool("comboOver", false);
                anim.SetTrigger("slash1Trigger");
                PerformAttack(0.1f, 0.5f);

                //Debug.Log("attack 1");
                break;

            case 2:
                anim.SetTrigger("slash2Trigger");
                PerformAttack(0.1f, 0.5f);

                //Debug.Log("attack 2");
                break;

            case 3:
                anim.SetTrigger("stab1Trigger");
                PerformAttack(0.2f, 1f);

                playerIsLocked = true;
                //Debug.Log("attack 3");
                break;

            default:
                Debug.Log("LightAttack: Invalid Combo Index");
                break;
        }
    }

    private void HeavyAttack()
    {
        lastAttackTime = Time.time;

        //performs a heavy attack based on the comboIndex, and then locks  the player out of attacking until it's done.
        switch (comboIndex)
        {
            case 0:
                anim.SetBool("comboOver", false);
                anim.SetTrigger("stab1Trigger");
                PerformAttack(0.4f, 2f);

                //Debug.Log("Heavy Attack: Raw");
                break;

            case 2:
                anim.SetTrigger("rapidStabTrigger");
                PerformAttack(0.8f, 2f);

                //Debug.Log("Heavy Attack: Combo finisher 1");
                break;

            default:
                Debug.Log("No Heavy Attack for this combo");
                break;
        }

        playerIsLocked = true;
    }

    private void PerformAttack(float duration, float impact)
    {
        attackDirection = playerController.direction;

        attackDuration = duration;
        attackImpact = impact;
    }

    private void EndCombo()
    {        
        comboIndex = 0;
        storedAttackIndex = 0;
        playerIsLocked = false;
        anim.SetBool("comboOver", true);
    }
}
