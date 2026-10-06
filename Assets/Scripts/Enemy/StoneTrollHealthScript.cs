using UnityEngine;

public class StoneTrollHealthScript : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("Component References")]
    public Animator animator;

    [Header("Effects")]
    public GameObject deathParticlesPrefab;

    private bool isDead = false;

    void Start()
    {
        // Set health at the start
        currentHealth = maxHealth;

        // Try to automatically find the Animator if not assigned
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    // This is the main function to call from your player's attack script
    public void TakeDamage(float damageAmount)
    {
        if (isDead)
        {
            return;
        }

        currentHealth -= damageAmount;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
        else
        {
            animator.SetTrigger("Hit");
        }
    }

    private void Die()
    {
        isDead = true;

        animator.SetTrigger("Die");

        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }

        // Example: Disable any AI script
        // YourAIBehaviour ai = GetComponent<YourAIBehaviour>();
        // if (ai != null)
        // {
        //     ai.enabled = false;
        // }
    }

    public void SpawnDeathParticles()
    {
        Debug.Log("Spawn Death Particles");
        if (deathParticlesPrefab != null)
        {
            Instantiate(deathParticlesPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject, 5f);
    }
}
