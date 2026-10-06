using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] Transform target;
    [SerializeField] float height = 1.6f;

    [Header("Explorar")]
    [SerializeField] float shoulderX = 0.5f;
    [SerializeField] float distance = 2f;
    [SerializeField] float fov = 60f;

    [Header("Apuntar")]
    [SerializeField] float aimShoulderX = 0.7f;
    [SerializeField] float aimDistance = 1.2f;
    [SerializeField] float aimFov = 45f;
    [SerializeField] float blendSpeed = 10f;

    [Header("Look")]
    [SerializeField] float sens = 2f;
    [SerializeField] float minPitch = -40f;
    [SerializeField] float maxPitch = 70f;

    [Header("Collision")]
    [SerializeField] float radius = 0.2f;
    [SerializeField] LayerMask mask = ~0;

    public bool Aiming { get; set; }

    Camera cam;
    float yaw, pitch, curShoulder, curDistance;
    Vector2 lookInput;

    void Start()
    {
        cam = GetComponent<Camera>();
        yaw = target.eulerAngles.y;
        curShoulder = shoulderX;
        curDistance = distance;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        lookInput = Mouse.current.delta.ReadValue() * 0.1f;
    }

    void LateUpdate()
    {
        // Transición explorar <-> apuntar (independiente del framerate)
        float t = 1f - Mathf.Exp(-blendSpeed * Time.deltaTime);
        curShoulder = Mathf.Lerp(curShoulder, Aiming ? aimShoulderX : shoulderX, t);
        curDistance = Mathf.Lerp(curDistance, Aiming ? aimDistance : distance, t);
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, Aiming ? aimFov : fov, t);

        yaw  += lookInput.x * sens;
        pitch = Mathf.Clamp(pitch - lookInput.y * sens, minPitch, maxPitch);
        Quaternion rot = Quaternion.Euler(pitch, yaw, 0);

        Vector3 pivot = target.position + Vector3.up * height;
        Vector3 desired = pivot + rot * new Vector3(curShoulder, 0, -curDistance);

        Vector3 dir = desired - pivot;
        if (Physics.SphereCast(pivot, radius, dir.normalized, out var hit, dir.magnitude, mask))
            desired = pivot + dir.normalized * hit.distance;

        transform.SetPositionAndRotation(desired, rot);
    }
}