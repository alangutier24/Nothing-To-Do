using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] float maxHealth = 100f;
    public float currentHealth;

    void Awake() => currentHealth = maxHealth;

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;

        

        if (currentHealth <= 0) Die();
    }

    void Die() => Destroy(gameObject);
}