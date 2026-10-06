using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{

    public bool Aiming { get; set; }

    [SerializeField] Transform cam;
    [SerializeField] float speed = 4f;
    [SerializeField] float turnSpeed = 720f; // grados/segundo
    [SerializeField] float gravity = -20f;
    [SerializeField] float runSpeed = 4f;
    [SerializeField] Animator animator;

    CharacterController cc;
    float verticalVel;

    void Awake() => cc = GetComponent<CharacterController>();

    void Update()
    {
        // Input WASD
        var kb = Keyboard.current;
        Vector2 input = new Vector2(
            (kb.dKey.isPressed ? 1 : 0) - (kb.aKey.isPressed ? 1 : 0),
            (kb.wKey.isPressed ? 1 : 0) - (kb.sKey.isPressed ? 1 : 0));
        input = Vector2.ClampMagnitude(input, 1f);

        // Dirección relativa a la cámara, aplanada al suelo
        Vector3 fwd   = Vector3.ProjectOnPlane(cam.forward, Vector3.up).normalized;
        Vector3 right = Vector3.ProjectOnPlane(cam.right,   Vector3.up).normalized;
        Vector3 move  = fwd * input.y + right * input.x;

        Vector3 faceDir = Aiming ? fwd : move;
        if (faceDir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(faceDir);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, targetRot, turnSpeed * Time.deltaTime);
                
            animator.SetBool("IsWalking", true);
            animator.SetBool("IsRunning", false);
        }
        else 
        {
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsRunning", false);
        }

        // Gravedad
        if (cc.isGrounded && verticalVel < 0) verticalVel = -2f;
        verticalVel += gravity * Time.deltaTime;

        float currentSpeed = 0;
        
        if(kb.leftShiftKey.isPressed)
        {
            currentSpeed = runSpeed;
            animator.SetBool("IsRunning", true);
        } 
        else
        {
            currentSpeed = speed;
            animator.SetBool("IsRunning", false);
        }
        cc.Move((move * currentSpeed + Vector3.up * verticalVel) * Time.deltaTime);
    }
}