using UnityEngine;

/// <summary>
/// Decentralized physics model where each surface apply their own force
/// </summary>
public class AerodynamicSurface : MonoBehaviour
{
    private Rigidbody _rb;
    private FirstAircraftController _controller;

    [Header("Lift")]
    public WingAxis LiftAxis;
    public float LiftSurfaceArea;

    [Header("Drag")]
    public float DragCoefficient;
    public float DragSurfaceArea;

    void Start()
    {
        _rb = GetComponentInParent<Rigidbody>();
        _controller = GetComponentInParent<FirstAircraftController>();
    }

    void FixedUpdate()
    {
        ApplyForces();
    }

    /// <summary>
    /// Apply lift and drag to this surface position
    /// </summary>
    private void ApplyForces()
    {
        Vector3 velocity = _rb.GetPointVelocity(transform.position);
        float speed = velocity.magnitude;

        if (speed > 1f)
        {
            Vector3 airflowDirection = velocity.normalized * -1f;

            float dynamicPressure = GetDynamicPressure(speed);
            float angleOfAttack = GetAngleOfAttack(velocity);
            float liftCoefficient = GetLiftCoefficient(angleOfAttack);

            Vector3 liftForce = GetLift(dynamicPressure, airflowDirection, liftCoefficient);
            Vector3 dragForce = GetDrag(dynamicPressure, airflowDirection);

            _rb.AddForceAtPosition(liftForce + dragForce, transform.position);
        }
    }

    /// <summary>
    /// Calculates the dynamic pressure
    /// </summary>
    /// <param name="speed">The current speed of this surface</param>
    /// <returns>The dynamic pressure</returns>
    private float GetDynamicPressure(float speed)
    {
        float airDensity = GetAirDensity();
        float dynamicPressure = (airDensity * speed * speed) / 2f;

        return dynamicPressure;
    }

    /// <summary>
    /// Calculates an approximation of the air density based on altitude
    /// </summary>
    /// <returns>The air density at the surface position</returns>
    private float GetAirDensity()
    {
        float altitude = Mathf.Max(0, transform.position.y);
        float turningPoint = 9000f;

        float airDensity = _controller.AirDensityAtSeaLevel * Mathf.Exp(-altitude / turningPoint);
        return airDensity;
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

        //Debug.Log(angleOfAttck);
        return angleOfAttck;
    }

    /// <summary>
    /// Calculates an approximation of the lift coefficient based on angle of attack
    /// </summary>
    /// <param name="angleOfAttack">The angle of attack of this surface</param>
    /// <returns>The lift coefficient</returns>
    private float GetLiftCoefficient(float angleOfAttack)
    {
        float radians = Mathf.Clamp(angleOfAttack, -25f, 25f) * Mathf.Deg2Rad;
        float liftCoefficient = radians;

        return liftCoefficient;
    }

    /// <summary>
    /// Calculates the lift force for this surface
    /// </summary>
    /// <param name="dynamicPressure">The dynamic pressure</param>
    /// <param name="airflowDirection">The direction of the airflow</param>
    /// <param name="liftCoefficient">The lift coefficient</param>
    /// <returns>The lift force</returns>
    private Vector3 GetLift(float dynamicPressure, Vector3 airflowDirection, float liftCoefficient)
    {
        float lift = dynamicPressure * LiftSurfaceArea * liftCoefficient;

        Vector3 liftDirection = Vector3.zero;
        if (LiftAxis == WingAxis.Horizontal)
        {
            liftDirection = Vector3.ProjectOnPlane(transform.up, airflowDirection).normalized;
        }
        else if (LiftAxis == WingAxis.Vertical)
        {
            liftDirection = Vector3.ProjectOnPlane(transform.right, airflowDirection).normalized;
        }

        Vector3 liftForce = lift * liftDirection;

        return liftForce;
    }

    /// <summary>
    /// Calculates the parasitic drag force for this surface
    /// </summary>
    /// <param name="dynamicPressure">The dynamic pressure</param>
    /// <param name="airflowDirection">The direction of the airflow</param>
    /// <returns>The parasitic drag force</returns>
    private Vector3 GetDrag(float dynamicPressure, Vector3 airflowDirection)
    {
        float drag = dynamicPressure * DragSurfaceArea * DragCoefficient;
        Vector3 dragForce = drag * airflowDirection;

        return dragForce;
    }
}
