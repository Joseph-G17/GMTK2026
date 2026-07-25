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
    private Vector2 moveInput; // cached for FixedUpdate

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

        moveInput = input; // store for physics step

        if (Input.GetKeyDown(KeyCode.E))
            gadget.OnCrankInput();

        HandleFacing();
        HandleAnimation();
    }

    private void FixedUpdate()
    {
        if (canMove == false) return;
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }
    private void HandleFacing()
    {
        // Flip to face left when pressing A, face right (default) when pressing D
        if (Input.GetKey(KeyCode.A))
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
        // W alone doesn't change facing here — add an else-if for KeyCode.W
        // if you want a distinct "facing up" flip/rotation too.
    }

    private void HandleAnimation()
    {
        isWalking = input.sqrMagnitude > 0f;
        // isRunning stays false for now — hook up a sprint key later

        animator.SetBool("is_walking", isWalking);
        animator.SetBool("is_running", isRunning);
    }
}


