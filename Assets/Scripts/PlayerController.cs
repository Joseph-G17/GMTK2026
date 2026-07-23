using System.Collections;
using Unity.VisualScripting;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    [Header("Components")]
    public float moveSpeed;
    public bool isMoving;
    public Vector2 input;

    private void Update()
    {
        input = Vector2.zero;
        
        if (Input.GetKey(KeyCode.W)) input.y += 1;
        if (Input.GetKey(KeyCode.S)) input.y -= 1;
        if (Input.GetKey(KeyCode.A)) input.x -= 1;
        if (Input.GetKey(KeyCode.D)) input.x += 1;

        input = input.normalized; //prevents diagonal being faster

        transform.position += (Vector3)(input * moveSpeed * Time.deltaTime);
    }
}


