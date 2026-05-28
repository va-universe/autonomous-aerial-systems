using UnityEngine;

/// <summary>
/// Decentralized physics model where each parent surface apply their own force
/// </summary>
public class WingSurface : MonoBehaviour
{
    private Rigidbody _rb;
    private SecondAircraftController _controller;

    [Header("Airfoil")]
    public WingAxis LiftAxis;
    public float InitialZeroLiftAngle;

    [Header("Control Surface")]
    public float DeflectionCoefficient;
    public float ControlSurfaceDeflection;
    private float _totalSurfaceArea;

    void Start()
    {
        _rb = GetComponentInParent<Rigidbody>();
        _controller = GetComponentInParent<SecondAircraftController>();

        SetSurfaceArea();
    }

    void Update()
    {
        
    }

    void FixedUpdate()
    {
        ApplyForces();
    }

    private void ApplyForces()
    {
        Vector3 velocity = _rb.GetPointVelocity(transform.position);
        float speed = velocity.magnitude;

        if (speed > 1f)
        {
            float angleOfAttack = GetAngleOfAttack(velocity);
            float zeroLiftAngle = GetZeroLiftAngle();
            float liftCoefficient = GetLiftCoefficient(angleOfAttack, zeroLiftAngle);

            Vector3 liftForce = Vector3.zero;
            Vector3 dragForce = Vector3.zero;

            _rb.AddForceAtPosition(liftForce + dragForce, transform.position);
        }
    }

    /// <summary>
    /// Calculates the angle of attack
    /// </summary>
    /// <param name="velocity">The velocity at the surface position</param>
    /// <returns>The angle of attack</returns>
    private float GetAngleOfAttack(Vector3 velocity)
    {
        Vector3 chordLine = transform.forward;
        Vector3 projectedVelocity = Vector3.zero;

        float angleOfAttck = 0f;
        if (LiftAxis == WingAxis.Horizontal)
        {
            projectedVelocity = Vector3.ProjectOnPlane(velocity.normalized, transform.right);
            angleOfAttck = Vector3.SignedAngle(chordLine, projectedVelocity, transform.right);
        }
        else if (LiftAxis == WingAxis.Vertical)
        {
            projectedVelocity = Vector3.ProjectOnPlane(velocity.normalized, transform.up);
            angleOfAttck = Vector3.SignedAngle(chordLine, projectedVelocity, transform.up);
        }

        return angleOfAttck * Mathf.Deg2Rad;
    }

    /// <summary>
    /// Calculates the angle of attack where the lift is zero
    /// </summary>
    /// <returns></returns>
    private float GetZeroLiftAngle()
    {
        float deflection = (ControlSurfaceDeflection * Mathf.Deg2Rad) * DeflectionCoefficient;
        float zeroLiftAngle = InitialZeroLiftAngle + deflection;

        return zeroLiftAngle;
    }

    /// <summary>
    /// Calculates the lift coefficient
    /// </summary>
    /// <param name="angleOfAttack">The angle of attack at this surface</param>
    /// <param name="zeroLiftAngle">The zero lift angle</param>
    /// <returns>The lift coefficient</returns>
    private float GetLiftCoefficient(float angleOfAttack, float zeroLiftAngle)
    {
        float liftCoefficient = 2f * Mathf.PI * (angleOfAttack - zeroLiftAngle);

        return liftCoefficient;
    }

    /// <summary>
    /// Calculates the total surface area of the wing and control surface combined
    /// </summary>
    private void SetSurfaceArea()
    {
        Vector3 scale = transform.localScale;
        _totalSurfaceArea = scale.x * scale.y * scale.z;
    }
}
