using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Follow")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 12f, -2f);
    [SerializeField] private float followSpeed = 10f;

    [Header("Rotation")]
    [SerializeField] private bool useFixedRotation = true;
    [SerializeField] private Vector3 fixedRotation = new Vector3(75f, 0f, 0f);

    private void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        Vector3 desiredPosition = target.position + offset;

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            followSpeed * Time.deltaTime
        );

        if (useFixedRotation)
        {
            transform.rotation = Quaternion.Euler(fixedRotation);
        }
        else
        {
            transform.LookAt(target.position);
        }
    }
}