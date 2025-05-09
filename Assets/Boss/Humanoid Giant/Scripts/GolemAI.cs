using UnityEngine;

public class GolemAI : MonoBehaviour
{
    public Transform player;
    public float detectionRange = 10f;
    public float attackRange = 2f;
    public float moveSpeed = 2f;

    private Animator animator;
    private bool isAttacking = false;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // Walk logic
        if (distance < detectionRange && distance > attackRange)
        {
            animator.SetBool("IsPlayerNear", true);
            isAttacking = false;

            // Move toward player
            Vector3 direction = (player.position - transform.position).normalized;
            direction.y = 0;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 5f);
            transform.position += direction * moveSpeed * Time.deltaTime;
        }
        // Attack logic
        else if (distance <= attackRange && !isAttacking)
        {
            animator.SetBool("IsPlayerNear", false);
            animator.SetTrigger("Attack");
            isAttacking = true;
        }
        else
        {
            animator.SetBool("IsPlayerNear", false);
        }
    }
}
