using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public enum Modes { Roaming, Following, Chasing, Stopped}

    [SerializeField] public Modes currentMode = Modes.Roaming;
    private Modes previousMode;

    [Header("Components")]
    [SerializeField] public Transform target;
    [SerializeField] public NavMeshAgent agent;
    [SerializeField] private Animator animator;
    [SerializeField] private CircleCollider2D detectRadius;
    [SerializeField] private Transform rig;

    [Header("Roaming Settings")]
    [SerializeField] private float roamRadius = 8f;
    [SerializeField] private float roamStopTime;
    private Vector2 roamOrigin;
    private float roamTimer;

    [Header("Sight Detection")]
    [SerializeField] private bool canSeePlayer; 
    private bool wasSeeingPlayer;
    private float startRadius = 2.5f;
    private Vector2 lastKnownPosition;

    [Header("Audio Detection")]
    public AudioSource enemyAudio;
    [SerializeField] private bool canHearPlayer;

    [Header("Stopped/Search")]
    [SerializeField] private float searchDuration = 4f;
    private float searchTimer;

    //rigging direction
    private float facingThreshold = 0.05f;

    private void Awake()
    {
        if(agent == null)
            agent = GetComponent<NavMeshAgent>();

        agent.updateRotation = false;
        agent.updateUpAxis = false;

        if (animator == null)
            animator = GetComponentInChildren<Animator>();
        if (enemyAudio == null)
            enemyAudio = GetComponent<AudioSource>();
        if (detectRadius == null)
            detectRadius = GetComponent<CircleCollider2D>();
        if (rig == null)
            rig = GetComponent<Transform>();

    }

    void Start()
    {
        roamOrigin = transform.position;
        previousMode = currentMode;
        wasSeeingPlayer = canSeePlayer;
        PickNewRoamPoint();
        detectRadius.radius = startRadius;
    }

    void OnEnable()
    {
        SoundManager.OnSoundEmitted += HandleSoundEmitted;
    }
    private void OnDisable()
    {
        SoundManager.OnSoundEmitted -= HandleSoundEmitted;
    }

    void Update()
    {
        UpdateMode();
        MovementPattern(); //where we change currentMode;
        HandleFacing();
        UpdateSightState();
        UpdateActionState();
    }
    private void PickNewRoamPoint()
    {
        Vector2 randomPoint = roamOrigin + Random.insideUnitCircle * roamRadius;
        if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, roamRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }
    private void Roam()
    {
        roamStopTime = Random.Range(1f, 6f);

        roamTimer -= Time.deltaTime;
        bool reachedPoint = !agent.pathPending && agent.remainingDistance < 0.5f;

        if (reachedPoint && roamTimer <= 0f)
        {
            PickNewRoamPoint();
            roamTimer = roamStopTime;
        }
    }
    private void UpdateMode()
    {
        if (canSeePlayer)
        {
            currentMode = Modes.Chasing;
            lastKnownPosition = target.position;
            searchTimer = searchDuration; //reset timer per sight
        }
        else if (currentMode == Modes.Chasing)
        {
            //investigate last known spot
            currentMode = Modes.Stopped;
            searchTimer = searchDuration;
        }
        else if (canHearPlayer && currentMode != Modes.Stopped)
        {
            currentMode = Modes.Following;
            lastKnownPosition = target.position; 
        }
        else if (currentMode == Modes.Stopped)
        {
            searchTimer -= Time.deltaTime;
            if (searchTimer <= 0f)
                currentMode = Modes.Roaming;
        }
        else if (currentMode == Modes.Following && agent.remainingDistance < 0.5f && !agent.pathPending)
        {
            currentMode = Modes.Stopped; //reached location
            searchTimer = searchDuration;
        }
    }
    private void MovementPattern()
    {
        switch (currentMode) {
            case Modes.Roaming:
                Roam();
                break;
            case Modes.Following:
                agent.SetDestination(lastKnownPosition);
                break;
            case Modes.Chasing:
                agent.SetDestination(target.position);
                break;
            case Modes.Stopped:
                agent.SetDestination(lastKnownPosition);
                break;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            canSeePlayer = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            canSeePlayer = false;
        }
    }
    private void HandleFacing()
    {
        float xVel = agent.velocity.x;
        if (xVel < facingThreshold)
            rig.rotation = Quaternion.Euler(0f, 0f, 0f);
        else if (xVel > -facingThreshold)
            rig.rotation = Quaternion.Euler(0f, 180f, 0f);
    }
    private void HandleSoundEmitted(Vector2 soundPosition, float soundRadius)
    {
        float distance = Vector2.Distance(transform.position, soundPosition);
        canHearPlayer = distance <= soundRadius;
        if (canHearPlayer)
            lastKnownPosition = soundPosition;
    }

    private void UpdateSightState()
    {
        if (canSeePlayer != wasSeeingPlayer)
        {
            //if canSeePlayer is true enter sight_mode, else canSeePlayer is false
            animator.SetTrigger(canSeePlayer ? "sight_mode" : "blind_mode");
            wasSeeingPlayer = canSeePlayer;
        }
    }

    private void UpdateActionState()
    {
        bool isFollowing = currentMode == Modes.Following;
        bool isChasing = currentMode == Modes.Chasing; 
        bool isStopped = currentMode == Modes.Stopped;

        animator.SetBool("is_chasing", isChasing);
        animator.SetBool("is_stopped", isStopped);
        animator.SetBool("is_following", isFollowing);

        //"roaming" trigger only on the frame we transition INTO Roaming
        if (currentMode == Modes.Roaming && previousMode != Modes.Roaming)
        {
            animator.SetTrigger("roaming");
        }

        previousMode = currentMode;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(Application.isPlaying ? (Vector3)roamOrigin : transform.position, roamRadius);
    }
}