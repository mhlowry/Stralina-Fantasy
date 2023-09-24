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

    static int comboIndex = 0;
    static int storedAttackIndex = 0;

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

        switch (comboIndex)
        {
            case 1:
                anim.SetBool("comboOver", false);
                anim.SetTrigger("slash1Trigger");
                SetAttackParameters(0.1f);

                //Debug.Log("attack 1");
                break;

            case 2:
                anim.SetTrigger("slash2Trigger");
                SetAttackParameters(0.1f);

                //Debug.Log("attack 2");
                break;

            case 3:
                anim.SetTrigger("stab1Trigger");
                SetAttackParameters(0.2f);

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

        switch (comboIndex)
        {
            case 0:
                anim.SetBool("comboOver", false);
                anim.SetTrigger("stab1Trigger");
                SetAttackParameters(0.4f);

                //Debug.Log("Heavy Attack: Raw");
                break;

            case 2:
                anim.SetTrigger("rapidStabTrigger");
                SetAttackParameters(0.8f);

                //Debug.Log("Heavy Attack: Combo finisher 1");
                break;

            default:
                Debug.Log("No Heavy Attack for this combo");
                break;
        }

        playerIsLocked = true;
    }

    private void SetAttackParameters(float duration)
    {
        attackDuration = duration;
    }

    private void EndCombo()
    {        
        comboIndex = 0;
        storedAttackIndex = 0;
        playerIsLocked = false;
        anim.SetBool("comboOver", true);
    }
}
