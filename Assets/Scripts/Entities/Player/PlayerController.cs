using System.Collections;
using Unity.VisualScripting;
using UnityEngine;


public class PlayerController : Player
{

    [Header("Components")]
    public float moveSpeed;
    public bool canMove;
    private Vector2 input;
    private bool isWalking;
    private bool isRunning;
    private Vector2 moveInput;
    public float sprintMultiplier = 1.5f;
    private float currentSpeed;

    protected override void Update()
    {
        if (canMove == false)
        {
            moveInput = Vector2.zero;
            return;
        }

        input = Vector2.zero;
        if (Input.GetKey(KeyCode.W)) input.y += 1;
        if (Input.GetKey(KeyCode.S)) input.y -= 1;
        if (Input.GetKey(KeyCode.A)) input.x -= 1;
        if (Input.GetKey(KeyCode.D)) input.x += 1;
        input = input.normalized;
        moveInput = input;

        isRunning = (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && input.sqrMagnitude > 0f;
        currentSpeed = isRunning ? moveSpeed * sprintMultiplier : moveSpeed;

        if (Input.GetKeyDown(KeyCode.E))
            gadget.OnCrankInput();

        HandleFacing();
        HandleAnimation();
    }

    private void FixedUpdate()
    {
        if (canMove == false) return;
        rb.MovePosition(rb.position + moveInput * currentSpeed * Time.fixedDeltaTime);
    }
    private void HandleFacing()
    {
        if (Input.GetKey(KeyCode.A))
        {
            rig.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            rig.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
    }

    private void HandleAnimation()
    {
        isWalking = input.sqrMagnitude > 0f && !isRunning;
        

        animator.SetBool("is_walking", isWalking);
        animator.SetBool("is_running", isRunning);
    }
}


