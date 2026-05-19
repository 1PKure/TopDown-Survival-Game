using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float gravity = -20f;

    [Header("Aim")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask aimLayerMask;
    [SerializeField] private float rotationSpeed = 20f;

    private CharacterController characterController;
    private Vector3 verticalVelocity;

    private float currentSpeedMultiplier = 1f;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    private void Update()
    {
        HandleMovement();
        HandleAimRotation();
        ApplyGravity();
    }

    private void HandleMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 moveDirection = new Vector3(horizontal, 0f, vertical).normalized;

        float finalMoveSpeed = moveSpeed * currentSpeedMultiplier;

        characterController.Move(moveDirection * finalMoveSpeed * Time.deltaTime);
    }

    private void HandleAimRotation()
    {
        if (mainCamera == null)
        {
            return;
        }

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, 500f, aimLayerMask))
        {
            Vector3 lookDirection = hitInfo.point - transform.position;
            lookDirection.y = 0f;

            if (lookDirection.sqrMagnitude < 0.01f)
            {
                return;
            }

            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    private void ApplyGravity()
    {
        if (characterController.isGrounded && verticalVelocity.y < 0f)
        {
            verticalVelocity.y = -2f;
        }

        verticalVelocity.y += gravity * Time.deltaTime;
        characterController.Move(verticalVelocity * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out SlowZone slowZone))
        {
            currentSpeedMultiplier = slowZone.SpeedMultiplier;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out SlowZone slowZone))
        {
            currentSpeedMultiplier = 1f;
        }
    }
}