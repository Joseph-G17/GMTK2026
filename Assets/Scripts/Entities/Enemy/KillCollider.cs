using UnityEngine;

public class KillCollider : MonoBehaviour
{
    [SerializeField] private CircleCollider2D collider; 
    private void Awake()
    {
        if (collider == null)
            collider = GetComponent<CircleCollider2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Player.player.Death();
    }
}
