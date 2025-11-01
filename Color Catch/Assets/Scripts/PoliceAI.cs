using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class PoliceAI : MonoBehaviour
{
    NavMeshAgent agent;
    public Transform player;
    public float chaseRange = 30f;
    public float catchRange = 1.2f;
    public int catchPenalty = 15; // points deducted on catch

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (player == null) return;

        float d = Vector3.Distance(transform.position, player.position);
        if (d <= chaseRange)
        {
            agent.SetDestination(player.position);
        }
        else
        {
            // idle or patrol: stay nearby
            if (agent.remainingDistance < 0.5f)
            {
                agent.ResetPath();
            }
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
        // Give a small teleport away to avoid repeated catches
        transform.position += transform.forward * 3f;
        agent.ResetPath();
    }
}
