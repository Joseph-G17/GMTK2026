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

    [Header("Footsteps")]
    [SerializeField] private float footstepInterval = 0.5f; 
    [SerializeField] private float sprintFootstepInterval = 0.3f; 
    private float footstepTimer;

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

        //bool = true if we click shift and our input is moving
        isRunning = (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && input.sqrMagnitude > 0f;
        //if isRunning is true currentSpeed is run, else currentSpeed is walk
        currentSpeed = isRunning ? moveSpeed * sprintMultiplier : moveSpeed;

        if (Input.GetKeyDown(KeyCode.E))
            gadget.OnCrankInput();

        HandleFacing();
        HandleAnimation();
        HandleFootsteps();
    }

    private void FixedUpdate()
    {
        if (canMove == false) return;
        rb.MovePosition(rb.position + moveInput * currentSpeed * Time.fixedDeltaTime);
    }
    private void HandleFootsteps()
    {
        bool isMoving = input.sqrMagnitude > 0f;

        if (!isMoving)
        {
            footstepTimer = 0f; 
            return;
        }

        footstepTimer -= Time.deltaTime;
        if (footstepTimer <= 0f)
        {
            SoundManager.PlaySound(SoundManager.Library.player.footsteps, playerAudio, 1f);

            float noiseRadius = isRunning ? 8f : 3f;
            SoundManager.EmitSound(rb.position, noiseRadius);

            footstepTimer = isRunning ? sprintFootstepInterval : footstepInterval;
        }
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


