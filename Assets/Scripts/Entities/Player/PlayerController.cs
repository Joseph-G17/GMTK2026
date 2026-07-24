using System.Collections;
using Unity.VisualScripting;
using UnityEngine;


public class PlayerController : Player
{

    [Header("Components")]
    public float moveSpeed;
    public bool canMove;
    private Vector2 input;
    
    protected override void Update()
    {
        if (canMove == false)
            return;

        input = Vector2.zero;
        
        if (Input.GetKey(KeyCode.W)) input.y += 1;
        if (Input.GetKey(KeyCode.S)) input.y -= 1;
        if (Input.GetKey(KeyCode.A)) input.x -= 1;
        if (Input.GetKey(KeyCode.D)) input.x += 1;

        input = input.normalized; //prevents diagonal being faster
        //= (Vector3)(input * moveSpeed * Time.deltaTime);
        
        rb.MovePosition(rb.position + input * moveSpeed * Time.fixedDeltaTime);

        if (Input.GetKey(KeyCode.E))
            StartCoroutine(gadget.CrankLight());
    }


}


