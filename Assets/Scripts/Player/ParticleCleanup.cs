using UnityEngine;

public class ParticleCleanup : MonoBehaviour
{
    // Public variable to hold the reference to the ParticleSystem component
    private ParticleSystem ps;

    void Start()
    {
        // Get the Particle System component on this GameObject
        ps = GetComponent<ParticleSystem>();

        if (ps == null)
        {
            Debug.LogError("ParticleCleanup requires a ParticleSystem component on the same GameObject!");
            // If component is missing, destroy after a short delay to prevent leaks
            Destroy(gameObject, 1.0f);
            return;
        }

        // Destroy the GameObject after the particle system has finished playing.
        // We use the system's duration plus its longest start lifetime.
        float duration = ps.main.duration + ps.main.startLifetime.constantMax;
        
        Destroy(gameObject, duration);
    }
}
