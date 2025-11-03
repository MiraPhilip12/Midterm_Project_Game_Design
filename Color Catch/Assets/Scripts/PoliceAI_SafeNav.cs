using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class PoliceAI_SafeNav : MonoBehaviour
{
    NavMeshAgent agent;
    public Transform player;
    public float chaseSpeed = 15f;
    public float minDistance = 5f; // Minimum distance to maintain
    public float maxDistance = 20f; // Maximum distance before chasing

    private Vector3 lastPlayerPosition;
    private bool isChasing = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = chaseSpeed;
        agent.angularSpeed = 120f;
        agent.acceleration = 12f;
        agent.stoppingDistance = minDistance;

        // Find player automatically
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
                Debug.Log("Found player: " + player.name);
            }
            else
            {
                Debug.LogError("Cannot find player with tag 'Player'");
            }
        }

        // Ensure NavMesh setup
        if (!agent.isOnNavMesh)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(transform.position, out hit, 5.0f, NavMesh.AllAreas))
            {
                transform.position = hit.position;
                agent.enabled = true;
            }
        }

        lastPlayerPosition = player.position;
    }

    void Update()
    {
        if (player == null) return;
        if (GameManager.Instance != null && GameManager.Instance.gameOver)
        {
            agent.isStopped = true;
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Always chase player but maintain distance
        if (distanceToPlayer > minDistance && agent.isOnNavMesh)
        {
            agent.isStopped = false;

            // Predict player's position for better chasing
            Vector3 playerDirection = (player.position - lastPlayerPosition).normalized;
            Vector3 predictedPosition = player.position + playerDirection * 2f;

            agent.SetDestination(predictedPosition);
            isChasing = true;
        }
        else
        {
            // Stop when close to player
            agent.isStopped = true;
            isChasing = false;

            // Face the player
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            directionToPlayer.y = 0;
            if (directionToPlayer != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            }
        }

        lastPlayerPosition = player.position;

        // Debug info
        if (isChasing)
        {
            Debug.Log($"Police chasing player. Distance: {distanceToPlayer}");
        }
    }

    // Police collision handled in PlayerCarController for instant game over
}