using UnityEngine;
using UnityEngine.InputSystem; // Import the new Input System!

/// <summary>
/// Attach this script to your Player or Main Camera.
/// This version uses the new Unity Input System package.
/// </summary>
public class GrenadeLauncher: MonoBehaviour
{
    [Header("References")]
    [Tooltip("The grenade prefab to be launched.")]
    public GameObject grenadePrefab;

    [Tooltip("The point from which the grenade will be fired. Create an empty child object on your camera/player and assign it here.")]
    public Transform firePoint;

    [Header("Input")]
    [Tooltip("The Input Action for firing (e.g., a 'FireGrenade' action bound to the right mouse button).")]
    public InputActionProperty fireAction;

    [Header("Settings")]
    [Tooltip("The forward force applied to the grenade on launch.")]
    public float launchForce = 20f;

    [Tooltip("Additional upward force for a higher arc.")]
    public float upwardLaunchForce = 10f; // <-- NEW VARIABLE

    private void OnEnable()
    {
        // Subscribe to the "performed" event for the fire action
        fireAction.action.Enable();
        fireAction.action.performed += OnFire;
    }

    private void OnDisable()
    {
        // Unsubscribe when the component is disabled
        fireAction.action.performed -= OnFire;
        fireAction.action.Disable();
    }

    /// <summary>
    /// This method is called by the Input System when the fireAction is performed.
    /// </summary>
    private void OnFire(InputAction.CallbackContext context)
    {
        LaunchGrenade();
    }

    void LaunchGrenade()
    {
        // Safety checks
        if (grenadePrefab == null || firePoint == null)
        {
            Debug.LogError("GrenadeLauncher: Prefab or FirePoint not set!", this);
            return;
        }

        // 1. Create the grenade instance
        GameObject grenadeInstance = Instantiate(grenadePrefab, firePoint.position, firePoint.rotation);

        // 2. Get its Rigidbody
        Rigidbody rb = grenadeInstance.GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Grenade prefab is missing a Rigidbody component!", grenadeInstance);
            return;
        }

        // 3. Apply the force
        // Combine forward force and upward force to create an arc
        Vector3 launchDirection = (firePoint.forward * launchForce) + (firePoint.up * upwardLaunchForce);
        rb.AddForce(launchDirection, ForceMode.Impulse);
    }
}

