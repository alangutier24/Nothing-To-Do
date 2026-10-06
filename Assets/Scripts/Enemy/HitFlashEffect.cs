using System.Collections;
using UnityEngine;

/// <summary>
/// This component creates a "flash" effect on the object's renderer when
/// the TriggerHitFlash() method is called.
/// </summary>
[RequireComponent(typeof(Renderer))]
public class HitFlashEffect : MonoBehaviour
{
    [Header("Flash Effect")] // Added header for clarity
    [Tooltip("The color to flash when hit. Use the 'Intensity' slider in the color picker for HDR glow.")]
    [ColorUsage(true, true)] // This attribute enables the HDR color picker
    public Color flashColor = Color.red;

    [Tooltip("How long the flash effect should last in seconds.")]
    public float flashDuration = 0.15f;

    [Tooltip("The shader property name to change. For glowing, use _EmissionColor.")]
    public string colorPropertyName = "_EmissionColor"; // Set default to _EmissionColor

    [Tooltip("Multiplies the flash color's intensity. Useful if not using the HDR color picker.")]
    public float emissionIntensity = 2.0f; // Added intensity multiplier
    [Header("Flinch Effect")]
    [Tooltip("Should the enemy 'flinch' (tilt back) when hit?")]
    public bool enableFlinch = true;

    [Tooltip("How many degrees the enemy will tilt backward.")]
    public float flinchAngle = 170.0f;

    [Tooltip("How long the entire tilt-and-return animation should take.")]
    public float flinchDuration = 0.3f;

    // The Renderer component for this object (works for MeshRenderer and SkinnedMeshRenderer)
    private Renderer _renderer;
    
    // The MaterialPropertyBlock used to override material properties
    private MaterialPropertyBlock _propBlock;
    
    // A reference to the currently running coroutine, if any
    private Coroutine _flashCoroutine;
    
    // A reference to the new flinch coroutine
    private Coroutine _flinchCoroutine;

    // We will cache the shader property ID from the string for performance.
    private int _colorPropertyID;

    private Quaternion _startRotation; // Store the "pre-flinch" rotation

    void Awake()
    {
        // Get the components we need
        _renderer = GetComponent<Renderer>();
        _propBlock = new MaterialPropertyBlock();
        
        // Convert the string property name to a Shader Property ID
        // This is much faster than using a string lookup every time.
        _colorPropertyID = Shader.PropertyToID(colorPropertyName);
        
        // A small check to make sure we found the renderer
        if (_renderer == null)
        {
            Debug.LogError("HitFlashEffect: No Renderer component found on this object!", this);
        }
    }

    /// <summary>
    /// Call this method from your projectile script (or any other script)
    /// to make the enemy flash.
    /// </summary>
    public void TriggerHitFlash()
    {
        // Don't flash if the component isn't set up correctly
        if (_renderer == null) return; 

        // If a flash is already running, stop it to restart
        if (_flashCoroutine != null)
        {
            StopCoroutine(_flashCoroutine);
        }
        
        // Start the flash coroutine
        _flashCoroutine = StartCoroutine(FlashCoroutine());

        // --- NEW FLINCH LOGIC ---
        if (enableFlinch)
        {
            // If a flinch is already running, stop it
            if (_flinchCoroutine != null)
            {
                StopCoroutine(_flinchCoroutine);
            }
            // Start the new flinch coroutine
            _startRotation = transform.rotation;
        }
        // --- END NEW FLINCH LOGIC ---
    }

    // This is the function your bullet/weapon script will call
    public void ApplyFlinch(Vector3 hitDirection)
    {
        // If we are already flinching, stop it and reset rotation
        if (_flinchCoroutine != null)
        {
            StopCoroutine(_flinchCoroutine);
            transform.rotation = _startRotation; 
        }
        
        // Start the new flinch
        _flinchCoroutine = StartCoroutine(FlinchCoroutine(hitDirection));
    }

    /// <summary>
    /// The coroutine that handles the flash logic.
    /// </summary>
    public IEnumerator FlashCoroutine()
    {
        _renderer.GetPropertyBlock(_propBlock);
        
        _propBlock.SetColor(_colorPropertyID, flashColor * emissionIntensity);
        
        _renderer.SetPropertyBlock(_propBlock);

        yield return new WaitForSeconds(flashDuration);

        _renderer.SetPropertyBlock(null);

        // Mark the coroutine as finished
        _flashCoroutine = null;
    }

    private IEnumerator FlinchCoroutine(Vector3 hitDirection)
    {
        _startRotation = transform.rotation; 

        Vector3 localHitDirection = transform.InverseTransformDirection(-hitDirection);

        Vector3 localRotationAxis = Vector3.Cross(localHitDirection, Vector3.up);

        Quaternion localFlinch = Quaternion.AngleAxis(flinchAngle, localRotationAxis.normalized);

        Quaternion flinchRotation = _startRotation * localFlinch;

        float elapsedTime = 0f;
        while (elapsedTime < flinchDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / flinchDuration; // 0 to 1
            float animProgress = Mathf.Sin(progress * Mathf.PI); 

            transform.rotation = Quaternion.Lerp(_startRotation, flinchRotation, animProgress);
            
            yield return null;
        }

        transform.rotation = _startRotation;
        _flinchCoroutine = null;
    }
}

