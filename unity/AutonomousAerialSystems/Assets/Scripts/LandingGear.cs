using UnityEngine;

/// <summary>
/// Landing gear for the Wing Camber Prototype
/// </summary>
public class LandingGear : MonoBehaviour
{
    [Header("Colliders")]
    public WheelCollider LeftWheelCollider;
    public WheelCollider RightWheelCollider;
    public WheelCollider BackWheelCollider;

    [Header("Visuals")]
    public Transform LeftWheelVisual;
    public Transform RightWheelVisual;
    public Transform BackWheelVisual;

    [Header("Parameters")]
    public float Spring;
    public float Damper;
    public float TargetPosition;
    public float SuspensionDistance;

    public float ForwardStiffness;
    public float SidewaysStiffness;

    void Start()
    {
        InitializeWheel(LeftWheelCollider);
        InitializeWheel(RightWheelCollider);
        InitializeWheel(BackWheelCollider);
    }

    void Update()
    {
        UpdateWheelVisual(LeftWheelCollider, LeftWheelVisual);
        UpdateWheelVisual(RightWheelCollider, RightWheelVisual);
        UpdateWheelVisual(BackWheelCollider, BackWheelVisual);
    }

    /// <summary>
    /// Initialize wheel collider values
    /// </summary>
    /// <param name="wheelCollider">The wheel collider</param>
    private void InitializeWheel(WheelCollider wheelCollider)
    {
        JointSpring tempSuspension = wheelCollider.suspensionSpring;

        tempSuspension.spring = Spring;
        tempSuspension.damper = Damper;
        tempSuspension.targetPosition = TargetPosition;

        wheelCollider.suspensionSpring = tempSuspension;
        wheelCollider.suspensionDistance = SuspensionDistance;

        WheelFrictionCurve tempForwardFriction = wheelCollider.forwardFriction;
        tempForwardFriction.stiffness = ForwardStiffness;
        wheelCollider.forwardFriction = tempForwardFriction;

        WheelFrictionCurve tempSidewaysFriction = wheelCollider.sidewaysFriction;
        tempSidewaysFriction.stiffness = SidewaysStiffness;
        wheelCollider.sidewaysFriction = tempSidewaysFriction;
    }

    /// <summary>
    /// Update the wheel visuals
    /// </summary>
    /// <param name="wheelCollider">The wheel collider</param>
    /// <param name="wheelTransform">The transform of the wheel visual</param>
    private void UpdateWheelVisual(WheelCollider wheelCollider, Transform wheelTransform)
    {
        wheelCollider.GetWorldPose(out Vector3 position, out Quaternion rotation);

        Quaternion initialRotation = Quaternion.Euler(0f, 0f, -90f);

        wheelTransform.position = position;
        wheelTransform.rotation = rotation * initialRotation;
    }
}
