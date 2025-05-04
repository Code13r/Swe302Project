using UnityEngine;
using UnityEngine.UI;

public class healthBar : MonoBehaviour
{
    public float maxHealth = 100f;
    public float health;

    public Slider healthSlider;         // assign the green bar
    public Slider easeHealthSlider;     // assign the red delay bar
    public GameObject healthBarUI;      // assign the entire canvas

    private float lerpSpeed = 0.05f;
    private bool isVisible = false;

    void Start()
    {
        health = maxHealth;

        if (healthSlider != null) healthSlider.maxValue = maxHealth;
        if (easeHealthSlider != null) easeHealthSlider.maxValue = maxHealth;

        if (healthBarUI != null)
        {
            healthBarUI.SetActive(false);  // ✅ Start hidden
        }
    }

    void Update()
    {
        if (!isVisible || healthSlider == null || easeHealthSlider == null)
            return;

        healthSlider.value = health;
        easeHealthSlider.value = Mathf.Lerp(easeHealthSlider.value, health, lerpSpeed);
    }

    public void TakeDamage(float damage)
    {
        if (!isVisible && healthBarUI != null)
        {
            Debug.Log("Health bar enabled");
            healthBarUI.SetActive(true);   // ✅ Show on first hit
            isVisible = true;
        }

        health -= damage;

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Enemy died.");
        Destroy(gameObject);  // ✅ Destroys the whole enemy (and the UI with it)
    }
}
