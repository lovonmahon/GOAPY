using UnityEngine;
using UnityEngine.AI;

// Add an animator if there isn't one
// [RequireComponent(typeof(NavMeshAgent))]
public class AILocomotionAnimator : MonoBehaviour
{
    NavMeshAgent agent;
    [SerializeField] Animator animator;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if(agent == null) return;
    }

    void Update()
    {
        Vector3 velocity = agent.velocity;

        if (velocity.sqrMagnitude < 0.0001f)
        {
            animator.SetFloat("MoveX", 0f);
            animator.SetFloat("MoveY", 0f);
            return;
        }

        // Convert world velocity to local space
        Vector3 local = transform.InverseTransformDirection(velocity.normalized);

        animator.SetFloat("MoveX", local.x);
        animator.SetFloat("MoveY", local.z);
    }
}
