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
        Vector2 currentInput = Vector2.zero;
        if (Input.GetKey(KeyCode.W)) currentInput.y += 1;
        if (Input.GetKey(KeyCode.S)) currentInput.y -= 1;
        if (Input.GetKey(KeyCode.A)) currentInput.x -= 1;
        if (Input.GetKey(KeyCode.D)) currentInput.x += 1;

        if (currentInput != Vector2.zero)
            input = currentInput; 

        if (!isMoving && input != Vector2.zero)
        {
            Vector3 targetPos = transform.position + new Vector3(input.x, input.y, 0);
            StartCoroutine(Move(targetPos));
            input = Vector2.zero; 
        }
    }

IEnumerator Move(Vector3 targetPos)
    {
        isMoving = true;
        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;
        isMoving = false;
    }
}
