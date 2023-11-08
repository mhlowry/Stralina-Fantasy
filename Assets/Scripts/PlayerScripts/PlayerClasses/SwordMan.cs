using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;

public class SwordMan : Player
{
    bool isBlocking = false;
    bool tryingToBlock = false;
    bool didUlt = false;

    float timeOfUlt;
    [SerializeField] float ultCooldown = 0.75f;

    [SerializeField] private LayerMask enemyLayers;

    [SerializeField] ProjectileAttack superAttack;

    protected override void Update()
    {
        if(tryingToBlock && CanBlock())
        {
            isBlocking = true;
        }

        if(!CanBlock())
        {
            isBlocking = false;
        }

        if (Time.time - timeOfUlt >= ultCooldown && didUlt)
        {
            SetInvulFalse();
            EnableInput();
            didUlt = false;
            animGFX.SetBool("ultFinish", true);
        }

        animGFX.SetBool("isBlocking", isBlocking);

        base.Update();
    }

    public override void TakeDamage(int damage, float knockBack, UnityEngine.Vector3 enemyPosition)
    {
        if (isBlocking)
        {
            //animate the blocking
            animGFX.SetTrigger("blockHit");
            //Play blocking sounds
            AudioManager.instance.PlayAll(new string[] { "block_thud_2", "block_thud_4" });
            //drain the meter
            UseMeter(15f);
        }
        else
        {
            base.TakeDamage(damage, knockBack, enemyPosition);
        }
    }

    public void CallBlocking(InputAction.CallbackContext context)
    {
        //if the player is 
        if (context.canceled)
        {
            isBlocking = false;
            tryingToBlock = false;
            return;
        }
        
        tryingToBlock = true;
    }

    public void CallSpecial(InputAction.CallbackContext context)
    {
        if (!context.started || !CanSpecial())
            return;

        //Use all the meter
        UseMeter(GetMaxMeter());

        didUlt = true;
        timeOfUlt = Time.time;
        animGFX.SetBool("ultFinish", false);
        SetInvulTrue();
        DisableInput();
        animGFX.SetTrigger(superAttack.GetAnim());
        StartCoroutine(superAttack.ActivateAttack(this, 1f, 0f, enemyLayers, transform.forward));
    }

    private bool CanSpecial()
    {
        return
               CanInput()            //can input
            && !animGFX.GetBool("isRolling")   //is not currently rolling
            && !IsStunned()          //is not stunned
            && !PauseMenu.isPaused         //is not paused
            && GetCurMeter() >= GetMaxMeter(); //has full meter
    }

    private bool CanBlock()
    {
        return 
            animGFX.GetBool("comboOver")     //is not mid combo
            && !IsStunned()                  //is not stunned
            && !animGFX.GetBool("isRolling") //is not rolling
            && GetCurMeter() >= 15f;         //has enough meter
    }
}
