using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]

public class PlayerController : MonoBehaviour
{
    private Vector2 input;

    private Vector3 direction;

    private CharacterController characterController;

    private float horizontalVelocity;
    private float gravity = -9.81f;

    private float velocity;

    //Player Parameters
    [SerializeField] private float rotationTime = 0.05f;
    [SerializeField] private float speed;
    [SerializeField] private float gravityMultiplier = 3.0f;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        ApplyGravity();
        ApplyRotation();
        ApplyMovement();
    }

    private void ApplyGravity()
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
        //if the player is not inputting anything, do not update player's rotation
        if (input.sqrMagnitude == 0) return;

        //this chunk of code rotates the direction the player is facing
        var targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        var angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref horizontalVelocity, rotationTime); ;
        transform.rotation = Quaternion.Euler(0.0f, angle, 0.0f);
    }

    private void ApplyMovement()
    {
        //this line moves the player
        characterController.Move(direction * speed * Time.deltaTime);
    }

    /*This function is called by the "PlayerInput" Object to move the player
    based on Unity's input system*/
    public void Move(InputAction.CallbackContext context)
    {
        input = context.ReadValue<Vector2>();
        direction = new Vector3(input.x, 0.0f, input.y);
    }
}
