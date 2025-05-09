using UnityEngine;

public class BossHealth : MonoBehaviour
{
    public int maxHealth = 300;
    private int currentHealth;

    private Animator animator;
    private bool triggered66 = false;
    private bool triggered33 = false;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        float percent = (float)currentHealth / maxHealth;

        if (!triggered66 && percent <= 0.66f)
        {
            triggered66 = true;
            TriggerStagger();
        }
        else if (!triggered33 && percent <= 0.33f)
        {
            triggered33 = true;
            TriggerStagger();
        }

        if (currentHealth <= 0)
        {
            Die(); // future upgrade
        }
    }

    private void TriggerStagger()
    {
        Debug.Log("Stagger triggered!");
        animator.SetTrigger("Damaged");
    }

    private void Die()
    {
        Debug.Log("Boss defeated!");
        // animator.SetTrigger("Die"); optional for later
        Destroy(gameObject);
    }
}
