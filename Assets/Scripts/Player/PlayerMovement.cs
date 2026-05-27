using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float gravity = -20f;

    [Header("Dash")]
    [SerializeField] private KeyCode dashKey = KeyCode.LeftShift;
    [SerializeField] private float dashSpeed = 14f;
    [SerializeField] private float dashDuration = 0.18f;
    [SerializeField] private float dashCooldown = 1f;

    [Header("Aim")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask aimLayerMask;
    [SerializeField] private float rotationSpeed = 20f;

    private CharacterController characterController;
    private Vector3 verticalVelocity;

    private float currentSpeedMultiplier = 1f;
    private Vector3 lastMoveDirection = Vector3.forward;

    private bool isDashing;
    private float lastDashTime = -999f;

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
        HandleDashInput();

        if (!isDashing)
        {
            HandleMovement();
        }

        HandleAimRotation();
        ApplyGravity();
    }

    private void HandleMovement()
    {
        Vector3 moveDirection = GetInputDirection();

        if (moveDirection.sqrMagnitude > 0.01f)
        {
            lastMoveDirection = moveDirection;
        }

        float finalMoveSpeed = moveSpeed * currentSpeedMultiplier;

        characterController.Move(moveDirection * finalMoveSpeed * Time.deltaTime);
    }

    private void HandleDashInput()
    {
        if (isDashing)
        {
            return;
        }

        if (!Input.GetKeyDown(dashKey))
        {
            return;
        }

        if (Time.time < lastDashTime + dashCooldown)
        {
            return;
        }

        Vector3 dashDirection = GetInputDirection();

        if (dashDirection.sqrMagnitude < 0.01f)
        {
            dashDirection = lastMoveDirection;
        }

        StartCoroutine(DashRoutine(dashDirection.normalized));
    }

    private IEnumerator DashRoutine(Vector3 dashDirection)
    {
        isDashing = true;
        lastDashTime = Time.time;

        float timer = 0f;

        while (timer < dashDuration)
        {
            characterController.Move(dashDirection * dashSpeed * Time.deltaTime);

            timer += Time.deltaTime;
            yield return null;
        }

        isDashing = false;
    }

    private Vector3 GetInputDirection()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        return new Vector3(horizontal, 0f, vertical).normalized;
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