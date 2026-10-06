using UnityEngine;

/// <summary>
/// Attach this script to your Grenade Prefab.
/// It requires a Rigidbody and a Collider.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class GrenadeProjectile : MonoBehaviour
{
    [Tooltip("How long the grenade exists before being destroyed (if it hits nothing).")]
    public float lifetime = 10f;

    // We can add explosion effects, damage, etc. here later.

    void Awake()
    {
        // Destroy the grenade after its lifetime expires
        Destroy(gameObject, lifetime);
    }

    // This is called when the grenade collides with another object
    void OnCollisionEnter(Collision collision)
    {
        // We hit something!

        // --- Check if we hit an enemy ---
        // Try to get the HitFlashEffect component from the object we hit
        HitFlashEffect enemyFlash = collision.gameObject.GetComponentInParent<HitFlashEffect>();

        // If the component exists (meaning we hit an enemy)...
        if (enemyFlash != null)
        {
            // ...trigger the flash!
            enemyFlash.TriggerHitFlash();
        }

        // --- Add explosion logic here later ---
        // For now, just print a message
        Debug.Log("Grenade hit: " + collision.gameObject.name);

        // --- Destroy the grenade on impact ---
        // We could also trigger an explosion particle effect here
        Destroy(gameObject);
    }
}
