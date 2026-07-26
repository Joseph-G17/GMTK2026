using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public enum Modes { Roaming, Following, Chasing, Looking, Stopped} //my goal: if loud enough noise made it will chase in sightmode
                                                                       //stay looking after each looking returns to blind
    [SerializeField] public Modes currentMode = Modes.Roaming;
    private Modes previousMode;

    [Header("Components")]
    [SerializeField] public Transform target;
    [SerializeField] public NavMeshAgent agent;
    [SerializeField] private Animator animator;
    [SerializeField] private CircleCollider2D detectRadius;
    [SerializeField] private Transform rig;

    [Header("Roaming Settings")]
    [SerializeField] private float roamRadius = 10f;
    [SerializeField] private float roamStopTime = 5f;
    private Vector2 roamOrigin;
    private float roamTimer;

    [Header("Sight Detection")]
    [SerializeField] private bool canSeePlayer;
    private bool inSightMode;
    private bool wasSeeingPlayer;
    private float startRadius = 2.5f;
    private Vector2 lastKnownPosition;

    [Header("Audio Detection")]
    public AudioSource enemyAudio;
    [SerializeField] private bool canHearPlayer;
    private bool inBlindMode;

    [Header("Stopped/Search")]
    [SerializeField] private float searchDuration = 4f;
    private float searchTimer;

    [Header("Hearing Thresholds")]
    [SerializeField] private float loudSoundThreshold = 6f; 
    [SerializeField] private float chaseAlertDuration = 5f;
    private float chaseAlertTimer;

    [Header("Looking/Search Settings")]
    [SerializeField] private float searchRadius = 4f; 
    [SerializeField] private float searchMoveWaitTime = 0.5f; 
    private float searchMoveTimer;

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
        detectRadius.enabled = false;
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
        if (detectRadius.enabled == true)
            detectRadius.enabled = false;

        roamTimer -= Time.deltaTime;
        bool reachedPoint = !agent.pathPending && agent.remainingDistance < 0.5f;

        if (reachedPoint && roamTimer <= 0f)
        {
            PickNewRoamPoint();
            roamTimer = roamStopTime;
        }
    }
    private void Search()
    {
        searchMoveTimer -= Time.deltaTime;
        bool reachedPoint = !agent.pathPending && agent.remainingDistance < 0.5f;

        if (reachedPoint && searchMoveTimer <= 0f)
        {
            PickNewSearchPoint();
            searchMoveTimer = searchMoveWaitTime;
        }
    }
    private void PickNewSearchPoint()
    {
        Vector2 randomPoint = lastKnownPosition + Random.insideUnitCircle * searchRadius;
        if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, searchRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }
    private void UpdateMode()
    {
        if (chaseAlertTimer > 0f && !canSeePlayer)
        {
            chaseAlertTimer -= Time.deltaTime;
            currentMode = Modes.Chasing;
            return;
        }
        if (canSeePlayer) 
        {
            currentMode = Modes.Chasing;
            lastKnownPosition = target.position;
            searchTimer = searchDuration; //reset timer per sight
        }
        else if (currentMode == Modes.Chasing)
        {
            //investigate last known spot
            currentMode = Modes.Looking;
            searchTimer = searchDuration;
        }
        else if (canHearPlayer && currentMode != Modes.Looking) //hear + cannot see
        {
            currentMode = Modes.Following;
            lastKnownPosition = target.position; 
        }
        else if (currentMode == Modes.Looking) //investigate location can be in sight mode or blind mode
        {
            searchTimer -= Time.deltaTime;
            if (searchTimer <= 0f)
                currentMode = Modes.Roaming;
        }
        else if (currentMode == Modes.Following && agent.remainingDistance < 0.5f && !agent.pathPending) //hear + investigate
        {
            currentMode = Modes.Looking; //reached location
            searchTimer = searchDuration;
        }
    }
    private void MovementPattern()
    {
        switch (currentMode) {
            case Modes.Roaming:
                Debug.Log("roaming");
                Roam();
                break;
            case Modes.Following:
                agent.SetDestination(lastKnownPosition);
                break;
            case Modes.Chasing:
                agent.SetDestination(canSeePlayer ? target.position : lastKnownPosition); 
                break;
            case Modes.Looking:
                Search();
                break;
            case Modes.Stopped:
                agent.isStopped = true;
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
        {
            lastKnownPosition = soundPosition;

            if (soundRadius >= loudSoundThreshold)
            {
                currentMode = Modes.Chasing;
                chaseAlertTimer = chaseAlertDuration;
                Debug.Log("alerted!");
                detectRadius.enabled = true;
                //SoundManager.PlaySound(SoundManager.Library.spider.spiderWarning, enemyAudio);
            }
            else
            {
                Debug.Log("investigating!");
            }
        }
    }

    private void UpdateSightState()//if you are in DetectRadius you are in sightMode, if not are in blindMode
    {
        inSightMode = currentMode == Modes.Chasing;
        inBlindMode = currentMode != Modes.Chasing;
        if (canSeePlayer != wasSeeingPlayer) //anumations expensive so we limit by tracking if 'was'
        {
            
            wasSeeingPlayer = canSeePlayer;
        }
    }

    private void UpdateActionState()
    {
        bool isChasing = currentMode == Modes.Chasing; 
        bool isLooking = currentMode == Modes.Looking;
        bool isStopped = currentMode == Modes.Stopped;

        animator.SetBool("is_chasing", isChasing);
        animator.SetBool("is_looking", isLooking);
        animator.SetBool("is_stopped", isStopped);

        //only fire sounds on the frame we in a new mode
        if (currentMode != previousMode)
        {
            switch (currentMode)
            {
                case Modes.Roaming:
                    animator.SetTrigger("roaming");
                    SoundManager.PlaySound(SoundManager.Library.spider.spiderRoam, enemyAudio);
                    break;
                case Modes.Looking:
                    SoundManager.PlaySound(SoundManager.Library.spider.spiderLooking, enemyAudio);
                    break;
                case Modes.Stopped:
                    SoundManager.PlaySound(SoundManager.Library.spider.spiderStopping, enemyAudio);
                    break;
                case Modes.Chasing:
                    SoundManager.PlaySound(SoundManager.Library.spider.spiderChasing, enemyAudio);
                    break;
            }
        }

        previousMode = currentMode;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(Application.isPlaying ? (Vector3)roamOrigin : transform.position, roamRadius);
    }
}