using UnityEngine;

public class KillCollider : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
            Player.player.Death();
    }
}
