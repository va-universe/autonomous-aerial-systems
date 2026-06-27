using UnityEngine;

/// <summary>
/// Landing gear handler for the wing-camber-based prototypes
/// </summary>
public class LandingGear : MonoBehaviour
{
    private Controller _controller;

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

    [Header("Braking")]
    public float BrakeTorque;
    public float BrakeRate;
    private float _currentBrakeInput;

    void Start()
    {
        _controller = GetComponent<Controller>();

        InitializeWheel(LeftWheelCollider);
        InitializeWheel(RightWheelCollider);
        InitializeWheel(BackWheelCollider);
    }

    void Update()
    {
        UpdateWheelVisuals(LeftWheelCollider, LeftWheelVisual);
        UpdateWheelVisuals(RightWheelCollider, RightWheelVisual);
        UpdateWheelVisuals(BackWheelCollider, BackWheelVisual);
    }

    private void FixedUpdate()
    {
        ApplyBrake();
    }

    /// <summary>
    /// Initialize wheel collider parameters
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
    /// Update the wheel position and rotation
    /// </summary>
    /// <param name="wheelCollider">The wheel collider</param>
    /// <param name="wheelTransform">The transform of the wheel visuals</param>
    private void UpdateWheelVisuals(WheelCollider wheelCollider, Transform wheelTransform)
    {
        wheelCollider.GetWorldPose(out Vector3 position, out Quaternion rotation);

        Quaternion initialRotation = Quaternion.Euler(0f, 0f, -90f);

        wheelTransform.position = position;
        wheelTransform.rotation = rotation * initialRotation;
    }

    /// <summary>
    /// Apply braking torque on main wheels, if braking
    /// </summary>
    private void ApplyBrake()
    {
        float targetBrakeInput = _controller.WheelBrakeInput;

        _currentBrakeInput = Mathf.MoveTowards(_currentBrakeInput, targetBrakeInput, BrakeRate * Time.fixedDeltaTime);

        float brakeTorque = _currentBrakeInput * BrakeTorque;

        LeftWheelCollider.brakeTorque = brakeTorque;
        RightWheelCollider.brakeTorque = brakeTorque;
        BackWheelCollider.brakeTorque = 0f;

        //Fixes a bug where the wheels get locked in place
        LeftWheelCollider.motorTorque = 0.1f;
        RightWheelCollider.motorTorque = 0.1f;
    }
}
