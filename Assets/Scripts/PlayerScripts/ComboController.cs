using System.Collections;
using System;
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

    [SerializeField] private LayerMask enemyLayers;

    [SerializeField] private List<PlayerAttack> lightAttacks;
    [SerializeField] private List<PlayerAttack> heavyAttacks;

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
        if (Time.time - lastAttackTime > attackDuration && storedAttackIndex > 0)
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
                    //Heavy Attack Rapid Stab
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
    private void EndCombo()
    {
        comboIndex = 0;
        storedAttackIndex = 0;
        playerIsLocked = false;
        anim.SetBool("comboOver", true);
    }

    /* duration is how long the attack will last
     * impact is the momentum the attack carries
     * vfxIndex corresponds to the VFX animation that will get called by VFX controller
     */
    private void PerformAttack(PlayerAttack currentAttack)
    {
        anim.SetTrigger(currentAttack.getAnim());

        attackDirection = playerController.direction;

        attackDuration = currentAttack.getDuration();
        attackImpact = currentAttack.getImpact();

        DisableAttackVFX(currentAttack.getVfxObj());
        StartCoroutine(playAttackVFX(currentAttack.getVfxObj(), currentAttack.getDelay()));

        List<Collider[]> hitEnemies = new List<Collider[]>();

        foreach (HitBox hitBox in currentAttack.getHitBoxes())
        {
            hitEnemies.Add(Physics.OverlapSphere(hitBox.getPosition(), hitBox.getSize(), enemyLayers));
        }

        foreach (Collider[] hitBoxes in hitEnemies)
        {
            foreach (Collider enemy in hitBoxes)
            {
                Debug.Log("We hit " + enemy.name);
            }
        }
    }
    private IEnumerator playAttackVFX(GameObject vfxObj, float delay)
    {
        yield return new WaitForSeconds(delay);
        vfxObj.transform.rotation = transform.rotation;
        vfxObj.SetActive(true);

        yield return new WaitForSeconds(0.8f);
        DisableAttackVFX(vfxObj);
    }

    private void DisableAttackVFX(GameObject vfxObj)
    {
        vfxObj.SetActive(false);
    }

    //This lets us see selected hitboxes in the editor
    private void OnDrawGizmosSelected()
    {
        foreach (PlayerAttack lightAttack in lightAttacks)
        {
            foreach (HitBox hitBox in lightAttack.getHitBoxes())
            {
                if (hitBox == null)
                    continue;
                Gizmos.DrawWireSphere(hitBox.getPosition(), hitBox.getSize());
            }
        }

        foreach (PlayerAttack heavyAttack in heavyAttacks)
        {
            foreach (HitBox hitBox in heavyAttack.getHitBoxes())
            {
                if (hitBox == null)
                    continue;
                Gizmos.DrawWireSphere(hitBox.getPosition(), hitBox.getSize());
            }
        }
    }
}
