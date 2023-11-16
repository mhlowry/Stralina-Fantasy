using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Player))]
[RequireComponent(typeof(PlayerCombat))]

public class PlayerMovement : MonoBehaviour
{
    private Vector2 input;

    [HideInInspector] public Vector3 direction;

    private CharacterController characterController;

    private float horizontalVelocity;
    private float gravity = -9.81f;

    private float velocity;

    //Things to pull from objects attached to main player
    //REMEMBER TO DRAG GFX INTO ANIM AND PLAYERVISUAL INTO playerVISUAL IN UNITY EDITOR
    [SerializeField] private Animator anim;
    [SerializeField] private GameObject playerVisual;

    //Player Parameters
    [SerializeField] private float rotationTime = 0.05f;
    [SerializeField] private float speed;
    [SerializeField] private float gravityMultiplier = 3.0f;

    [SerializeField] private int numOfRolls = 1;
    [SerializeField] private float rollSpeed = 15f;
    [SerializeField] private float rollCooldown = 2f;
    [SerializeField] private float rollDeceleration = 20f;
    [SerializeField] private float rollDuration = 0.5f;
    private float curRollSpeed;
    private int usedRolls = 0;
    private int storedRolls = 0;
    private float initialRollTime;
    private float lastRollTime;

    //Listen man, i'm new to C# okay?
    private bool isRolling;
    public bool IsRolling
    {
        get { return isRolling; }
        private set { isRolling = value; }
    }

    private bool canRoll = true;

    private Player player;
    private PlayerCombat playerCombat;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        player = GetComponent<Player>();
        playerCombat = GetComponent<PlayerCombat>();
    }

    private void Update()
    {

        ApplyGravity();

        ApplyRotation();
        FlipCheck();
        ApplyMovement();

        //STUFF TO DO WITH ROLLING VVVVVVVVVV I DONT KNOW WHY THERES SO MUCH BUT THERE IS
        //stop rolling if rolling
        anim.SetBool("isRolling", isRolling);

        if (Time.time - lastRollTime > rollDuration && isRolling == true)
        {
            isRolling = false;
            player.SetInvulFalse();
        }

        if (usedRolls + storedRolls >= numOfRolls)
            canRoll = false;

        if (Time.time - initialRollTime > rollCooldown)
        {
            canRoll = true;
            usedRolls = 0;
            storedRolls = 0;
        }

        //If a roll is stored, roll at the earliest possible moment
        if (Time.time - lastRollTime > rollDuration && storedRolls > 0)
        {
            storedRolls--;
            CharacterRoll();
        }

        //add rolling momentum
        if (isRolling)
        {
            //adds deceleration to the impact of certain attacks
            float deceleration = rollDeceleration * Time.deltaTime;
            curRollSpeed = Mathf.Max(0.0f, curRollSpeed - deceleration);

            //uses the direction the player is imputting in playerMovement
            characterController.Move(transform.forward * curRollSpeed * Time.deltaTime);
        }
    }

    public void CallRoll(InputAction.CallbackContext context)
    {
        //ensures that roll is only called once per button, and only if the player is actionable and can roll
        if (!context.started || !CanCallRoll()) // || !anim.GetBool("comboOver")
            return;

        //When the player is still rolling but the roll button was just pressed.
        if (isRolling)
        {
            storedRolls++;
            return;
        }

        CharacterRoll();
    }

    /*This function is called by the "PlayerInput" Object to move the player
    based on Unity's input system*/
    public void Move(InputAction.CallbackContext context)
    {
        input = context.ReadValue<Vector2>();
        //UnityEngine.Debug.Log("input: " + input);
        direction = new Vector3(input.x, 0.0f, input.y);
    }

    private void CharacterRoll()
    {
        //make player invulnerable
        player.SetInvulTrue();
        isRolling = true;

        playerCombat.attackDuration = 0.0f;

        anim.SetTrigger("startRoll");

        curRollSpeed = rollSpeed;

        if (usedRolls == 0)
            initialRollTime = Time.time;

        usedRolls++;
        lastRollTime = Time.time;
    }

    private void FlipCheck()
    {
        //if the player is not inputting anything, do not update player's orientation
        if (input.sqrMagnitude == 0 || direction.x == 0) return;

        bool facingLeft = direction.x < 0;
        float playerOrientation = playerVisual.transform.localScale.x;

        if (facingLeft & playerOrientation < 0 || !facingLeft & playerOrientation > 0)
        {
            playerVisual.transform.localScale = new Vector3(-1.0f * playerOrientation, 1.0f, 1.0f);
        }
    }

    public void ApplyGravity()
    {
        if (characterController.isGrounded && velocity < 0.0f)
        {
            velocity = -1.0f;
        }
        else
        {
            velocity += gravity * gravityMultiplier * Time.deltaTime;
        }

        direction.y = velocity;
    }

    private void ApplyRotation()
    {
        //if the player is not inputting anything (or weird hitstop interaction?), do not update player's rotation
        if (input.sqrMagnitude == 0 || Time.timeScale == 0) return;

        //this chunk of code rotates the direction the player is facing
        var targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        var angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref horizontalVelocity, rotationTime); ;
        transform.rotation = Quaternion.Euler(0.0f, angle, 0.0f);
    }

    private void ApplyMovement()
    {
        if (player.isTeleporting)
            return;

        if (CanCallMove())
        {
            //plays walking animation & moves player
            anim.SetFloat("direction", Mathf.Abs(direction.x) + Mathf.Abs(direction.z));
            characterController.Move(direction * (speed * player.GetMoveSpeedScale()) * Time.deltaTime);
        }
        else
            characterController.Move(new Vector3(0.0f, direction.y, 0.0f) * Time.deltaTime);
    }

    private bool CanCallMove()
    {
        return
            !player.IsStunned()             //is not stunned
            && !isRolling                   //is not rolling
            && anim.GetBool("comboOver")    //is not attacking
            && !anim.GetBool("isBlocking")  //is not blocking
            && player.CanInput();           //can input
    }

    private bool CanCallRoll()
    {
        return
            !player.IsStunned()     //is not stunned
            && canRoll              // can roll
            && player.CanInput();   //can input
    }

    //this function reduces the problem where enemies would just go like fucking flying
    //if you tapped them a little too hard.  Haven't found a complete solution yet
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        float pushPower = 1.5f;
        Rigidbody body = hit.collider.attachedRigidbody;

        // no rigidbody
        if (body == null || body.isKinematic)
            return;

        // We dont want to push objects below us
        if (hit.moveDirection.y < -0.1)
            return;

        // Calculate push direction from move direction,
        // we only push objects to the sides never up and down
        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);

        // Apply the push
        body.velocity = pushDir * pushPower;

    }
}
