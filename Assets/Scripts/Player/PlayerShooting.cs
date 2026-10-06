using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField] PlayerCamera playerCam;
    [SerializeField] PlayerMovement movement;
    [SerializeField] float damage = 25f;
    [SerializeField] float range = 100f;
    [SerializeField] float fireRate = 0.2f;
    [SerializeField] LayerMask hitMask = ~0;

    float nextFire;
    bool aiming;

    void Update()
    {
        var mouse = Mouse.current;
        aiming = mouse.rightButton.isPressed;
        playerCam.Aiming = aiming;
        movement.Aiming = aiming;

        if (aiming && mouse.leftButton.wasPressedThisFrame && Time.time >= nextFire)
        {
            nextFire = Time.time + fireRate;
            Shoot();
        }
    }

    void Shoot()
    {
        Transform cam = playerCam.transform;
        if (Physics.Raycast(cam.position, cam.forward, out var hit, range, hitMask))
        {
            Debug.DrawLine(cam.position, hit.point, Color.red, 1f);
            var enemy = hit.collider.GetComponentInParent<EnemyHealth>();
            if (enemy != null) enemy.TakeDamage(damage);
        }
    }

    void OnGUI() // Crosshair temporal
    {
        if (!aiming) return;
        float s = 6f;
        Vector2 c = new Vector2(Screen.width / 2f, Screen.height / 2f);
        GUI.DrawTexture(new Rect(c.x - s, c.y - 1, s * 2, 2), Texture2D.whiteTexture);
        GUI.DrawTexture(new Rect(c.x - 1, c.y - s, 2, s * 2), Texture2D.whiteTexture);
    }
}