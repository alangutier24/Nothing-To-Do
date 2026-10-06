using UnityEngine;
// We MUST use the URP namespace to access the DecalProjector class
using UnityEngine.Rendering.Universal; 

public class DecalFader : MonoBehaviour
{
    [Tooltip("Time in seconds before the decal starts to fade.")]
    [SerializeField]
    private float delayBeforeFade = 0.5f; 

    [Tooltip("Duration in seconds over which the decal will fade out.")]
    [SerializeField]
    private float fadeDuration = 2.0f; 
    
    // The total time the decal should remain visible before destruction.
    // This value is still used for the final Destroy call.
    [Tooltip("Total time in seconds before the decal is destroyed.")]
    [SerializeField]
    private float totalLifetime = 3.0f; 

    private float timer;
    private bool isFading = false;
    
    // Get the DecalProjector component instance
    private DecalProjector decalProjectorComponent; 
    
    // We will directly manipulate the DecalProjector.fadeFactor property
    // No MaterialPropertyBlock or custom shader property name is required!
    
    void Start()
    {
        // 1. Get the Decal Projector component using the strong type
        decalProjectorComponent = GetComponent<DecalProjector>(); 

        if (decalProjectorComponent == null) 
        {
            // Fallback: If the component is missing (e.g., deleted), destroy the object safely.
            Destroy(gameObject, totalLifetime); 
            return;
        }
        
        // 2. Ensure the decal starts fully visible
        decalProjectorComponent.fadeFactor = 1.0f; 
        
        // 3. Start the fading process after the delay
        Invoke(nameof(StartFade), delayBeforeFade);

        // 4. Pre-emptively destroy the object after the entire process is complete
        Destroy(gameObject, delayBeforeFade + fadeDuration);
    }

    private void StartFade()
    {
        isFading = true;
        timer = 0f;
    }

    void Update()
    {
        if (!isFading) return;

        timer += Time.deltaTime;
        float normalizedTime = timer / fadeDuration;

        if (normalizedTime >= 1.0f)
        {
            isFading = false;
            // Ensure it's fully transparent on the last frame
            decalProjectorComponent.fadeFactor = 0.0f; 
            return;
        }

        // Calculate the new opacity value (fading from 1.0f to 0.0f)
        // This value is what the URP Decal Projector component uses for overall opacity.
        float newOpacity = Mathf.Lerp(1.0f, 0.0f, normalizedTime);
        
        // Set the built-in fadeFactor property of the DecalProjector
        decalProjectorComponent.fadeFactor = newOpacity;
    }
}
