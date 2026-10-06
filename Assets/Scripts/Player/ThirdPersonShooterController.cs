using UnityEngine;
using Unity.Cinemachine;
using StarterAssets;
using UnityEngine.Animations.Rigging;

public class ThirdPersonShooterController : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Camera aimCamera;
    [SerializeField] private GameObject crosshair;
    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletSapwnPosition;
    [SerializeField] private Transform gunTarget;
    [SerializeField] private Rig aimRig;
    [SerializeField] private Rig pistolRig;

    private Vector3 hitPoint;
    private Camera activeCamera;
    private ThirdPersonController thirdPersonController;
    private StarterAssetsInputs startAssetsInput;

    private void Awake()
    {
        startAssetsInput = GetComponent<StarterAssetsInputs>();
        thirdPersonController = GetComponent<ThirdPersonController>();
    }

    private void Update()

    {
        activeCamera = mainCamera.isActiveAndEnabled ? mainCamera : aimCamera;

        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = activeCamera.ScreenPointToRay(screenCenterPoint);
        if(Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderLayerMask))
        {
            hitPoint = raycastHit.point;
            gunTarget.position = raycastHit.point;
        }

        if (startAssetsInput.aim)
        {
            mainCamera.gameObject.SetActive(false);
            crosshair.SetActive(true);
            thirdPersonController.SetRotateOnMove(false);
            thirdPersonController.SetIsAimingPistol(true);

            aimRig.weight = 0.7f;
            pistolRig.weight = 0.7f;

            Vector3 worldAimTarget = gunTarget.position;
            worldAimTarget.y  = hitPoint.y;
            Vector3 aimDirection = (worldAimTarget - transform.position).normalized;

            transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f); 
        }
        else
        {
            mainCamera.gameObject.SetActive(true);
            crosshair.SetActive(false);
            thirdPersonController.SetRotateOnMove(true);
            thirdPersonController.SetIsAimingPistol(false);
            aimRig.weight = 0.0f;
            pistolRig.weight = 0.0f;
        }

        if(startAssetsInput.shoot)
        {
            Vector3 aimDir = (gunTarget.position - bulletSapwnPosition.position).normalized;
            Instantiate(bulletPrefab, bulletSapwnPosition.position, Quaternion.LookRotation(aimDir));
            startAssetsInput.shoot = false;

        }

        if(startAssetsInput.breakDance)
        {
            thirdPersonController.SetBreakDancing(true);
            startAssetsInput.breakDance = false;
        }


    }

}
