using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public enum Modes { Roaming, Following, Chasing, Trapped}
    [SerializeField]
    public Modes currentMode = Modes.Roaming;

    [Header("Components")]
    [SerializeField] public Transform target;
    [SerializeField] public NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    void Update()
    {
        agent.SetDestination(target.position);
    }
}
