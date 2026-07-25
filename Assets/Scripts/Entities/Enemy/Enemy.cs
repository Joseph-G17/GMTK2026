using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public enum Modes { Roaming, Following, Chasing, Trapped }

    [SerializeField] public Modes currentMode = Modes.Roaming;

    [Header("Components")]
    [SerializeField] public Transform target;
    [SerializeField] public NavMeshAgent agent;
    [SerializeField] private Animator animator;

    [Header("Detection")]
    [SerializeField] private bool canSeePlayer; 
    private bool wasSeeingPlayer;
    private Modes previousMode;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        previousMode = currentMode;
        wasSeeingPlayer = canSeePlayer;
    }

    void Update()
    {
        agent.SetDestination(target.position);

        UpdateSightState();
        UpdateActionState();
    }

    private void UpdateSightState()
    {
        // Only fire the trigger when the sight state actually changes,
        // so we don't spam triggers every frame.
        if (canSeePlayer != wasSeeingPlayer)
        {
            animator.SetTrigger(canSeePlayer ? "sight_mode" : "blind_mode");
            wasSeeingPlayer = canSeePlayer;
        }
    }

    private void UpdateActionState()
    {
        bool isChasing = currentMode == Modes.Chasing;
        bool isStopped = currentMode == Modes.Trapped;

        animator.SetBool("isChasing", isChasing);
        animator.SetBool("isStopped", isStopped);

        // Fire "roaming" trigger only on the frame we transition INTO Roaming
        if (currentMode == Modes.Roaming && previousMode != Modes.Roaming)
        {
            animator.SetTrigger("roaming");
        }

        previousMode = currentMode;
    }
}