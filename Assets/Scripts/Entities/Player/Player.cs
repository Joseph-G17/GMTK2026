using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player player;
    public Rigidbody2D rb;

    [HideInInspector] public bool isDead;
    //[SerializeField] protected SpriteRenderer sr;
    [SerializeField] protected Animator animator;
    [SerializeField] protected PlayerController movement;
    [SerializeField] protected PlayerGadgets gadget;
    private Vector2 input;

    protected virtual void Start()
    {
        player = this;

        //if (sr == null) sr = GetComponent<SpriteRenderer>();
        if (animator == null)
            animator = GetComponent<Animator>();
        if (movement == null)
            movement = GetComponent<PlayerController>();
        if (gadget == null)
            gadget = GetComponent<PlayerGadgets>();

        isDead = false;
        movement.canMove = true;
    }

    protected virtual void Update()
    {
      
    }

    public void Death()
    {
        DeathscreenManager.instance.ShowDeathscreen();
        //animator.SetTrigger("isDead");
        isDead = true;
        movement.canMove = false;
        //sr.enabled = false;
        gadget.userLight.enabled = false;
    }
}
