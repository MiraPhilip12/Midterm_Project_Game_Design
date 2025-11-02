using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class PoliceAI_SafeNav : MonoBehaviour
{
    NavMeshAgent agent;
    public Transform player;
    public float chaseRange = 30f;
    public float catchRange = 1.2f;
    public int catchPenalty = 15;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        // If agent isn't on a NavMesh (common if placed slightly off or NavMesh rebuilt),
        // try to snap it to the nearest NavMesh position.
        if (!agent.isOnNavMesh)
        {
            NavMeshHit hit;
            // search radius 5 units - increase if your agent is far from the baked navmesh
            if (NavMesh.SamplePosition(transform.position, out hit, 5.0f, NavMesh.AllAreas))
            {
                transform.position = hit.position;
                // it's OK if agent.enabled is already true; but ensure agent can work
                agent.enabled = true;
            }
            else
            {
                Debug.LogWarning("[PoliceAI_SafeNav] No NavMesh near police; make sure NavMesh is baked and this object is near walkable geometry.");
            }
        }
    }

    void Update()
    {
        if (player == null) return;

        // don't call NavMeshAgent properties unless agent.isOnNavMesh is true
        if (!agent.isOnNavMesh)
        {
            // fallback: rotate towards player but don't call NavMesh properties
            Vector3 toPlayer = player.position - transform.position;
            toPlayer.y = 0f;
            if (toPlayer.sqrMagnitude > 0.01f)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(toPlayer), Time.deltaTime * 4f);
            return;
        }

        float d = Vector3.Distance(transform.position, player.position);
        if (d <= chaseRange)
        {
            agent.SetDestination(player.position);
        }
        else
        {
            if (!agent.hasPath || agent.remainingDistance < 0.5f)
                agent.ResetPath();
        }

        if (d <= catchRange)
        {
            OnCatchPlayer();
        }
    }

    void OnCatchPlayer()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(-catchPenalty);
            GameManager.Instance.ShowFeedback($"-{catchPenalty} Caught by Police");
        }

        // move police slightly away so it doesn't instantly recatch
        Vector3 back = (transform.position - player.position).normalized * 3f;
        transform.position = transform.position + back;
        agent.ResetPath();
    }
}
