using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Damage")]
    [Tooltip("Amount of damage this projectile deals.")]
    [SerializeField]
    private float damageAmount = 25f;

    [Header("Effects")]
    [Tooltip("The particle effect to spawn when hitting an enemy.")]
    public GameObject hitEnemyEffectPrefab;

    [Header("Effects")]
    [Tooltip("The particle effect to spawn when hitting environment")]
    public GameObject hitEnvironmentEffectPrefab;

    [Header("Decal")]
    [Tooltip("The prefab (like a URP Decal Projector) to spawn when hitting the environment.")]
    public GameObject decalPrefab;

    [Tooltip("Bullet Speed")]
    public float speed = 10.0f;

    private Rigidbody rb;

    private const string ENEMY_TAG = "Enemy";

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        rb.linearVelocity = transform.forward * speed;
    }

    void OnCollisionEnter(Collision collision)
    {

        GameObject hitObject = collision.gameObject;
        
        ContactPoint contact = collision.contacts[0];

        
        if (hitObject != null && hitObject.CompareTag(ENEMY_TAG))
        {

            Debug.Log("I hit: " + collision.gameObject.name);
            
            StoneTrollHealthScript enemyHealth = hitObject.GetComponent<StoneTrollHealthScript>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damageAmount);
            }

            if (hitEnemyEffectPrefab != null)
            {
                GameObject gameObject = Instantiate(hitEnemyEffectPrefab, contact.point, Quaternion.identity);
                Destroy(gameObject, 3.0f);         
            }
        }
        else // Hit environment/wall
        {
            Debug.Log("Hit Environment");
            if (decalPrefab != null)
            {
                Quaternion rotation = Quaternion.LookRotation(contact.normal);

                Vector3 spawnPosition = contact.point + contact.normal * 0.01f;

                Instantiate(decalPrefab, spawnPosition, rotation);
            }
            if (hitEnvironmentEffectPrefab != null)
            {
                GameObject gameObject = Instantiate(hitEnvironmentEffectPrefab, contact.point, Quaternion.identity);
                Destroy(gameObject, 3.0f);

            }
        }

        Destroy(gameObject);
    }
}
