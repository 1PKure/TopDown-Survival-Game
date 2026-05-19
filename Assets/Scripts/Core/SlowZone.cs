using UnityEngine;

public class SlowZone : MonoBehaviour
{
    [SerializeField] private float speedMultiplier = 0.5f;

    public float SpeedMultiplier => speedMultiplier;
}