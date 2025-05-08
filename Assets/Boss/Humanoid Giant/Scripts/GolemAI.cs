using UnityEngine;

public class GolemAI : MonoBehaviour
{
    public Transform player;
    public float detectionRange = 5f;
    public float moveSpeed = 2f;

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!player) return;

        float distance = Vector3.Distance(transform.position, player.position);
        bool isNear = distance < detectionRange;

        animator.SetBool("IsPlayerNear", isNear); // must match Animator parameter *exactly*

        if (isNear)
        {
            // Rotate toward player
            Vector3 direction = (player.position - transform.position).normalized;
            direction.y = 0; // Ignore vertical rotation
            if (direction != Vector3.zero)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 5f);

            // Move toward player
            transform.position += direction * moveSpeed * Time.deltaTime;
        }
    }
}
