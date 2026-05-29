using UnityEngine;

/// <summary>
/// Decentralized physics model where each parent surface apply their own force
/// </summary>
public class WingSurface : MonoBehaviour
{
    private Rigidbody _rb;
    private SecondAircraftController _controller;

    private Transform _orientation;

    [Header("Airfoil")]
    public WingAxis LiftAxis;
    public float InitialZeroLiftAngle;
    public float LiftModifier;

    [Header("Control Surface")]
    public float DeflectionCoefficient;
    public float ControlSurfaceDeflection;
    private float _totalSurfaceArea;

    [Header("Stalling")]
    public float StallAngle;
    public float MaxStallMultiplier;
    public float StallDragModifier;

    [Header("Drag")]
    public float DragCoefficient;
    public float InducedDragModifier;

    [Header("UI Display Data")]
    public float LiftData;
    public float DragData;
    public float StallData;
    public float AoAData;

    void Start()
    {
        _rb = GetComponentInParent<Rigidbody>();
        _controller = GetComponentInParent<SecondAircraftController>();
        _orientation = transform.Find("Orientation").transform;

        SetSurfaceArea();
    }

    void FixedUpdate()
    {
        ApplyForces();
    }

    /// <summary>
    /// Apply lift and drag at the surface position
    /// </summary>
    private void ApplyForces()
    {
        Vector3 velocity = _rb.GetPointVelocity(_orientation.transform.position);
        float speed = velocity.magnitude;

        if (speed > 1f)
        {
            Vector3 airflowDirection = velocity.normalized * -1f;

            float dynamicPressure = GetDynamicPressure(speed);
            float angleOfAttack = GetAngleOfAttack(velocity);
            float zeroLiftAngle = GetZeroLiftAngle();
            float stallEffect = GetStallEffect(angleOfAttack, speed);

            float liftCoefficient = GetLiftCoefficient(angleOfAttack, zeroLiftAngle, stallEffect);

            Vector3 liftForce = GetLift(dynamicPressure, airflowDirection, liftCoefficient);
            Vector3 dragForce = GetDrag(dynamicPressure, airflowDirection, liftCoefficient, stallEffect);

            Debug.DrawRay(_orientation.transform.position, liftForce / 100f, Color.green);
            Debug.DrawRay(_orientation.transform.position, dragForce / 100f, Color.red);
            _rb.AddForceAtPosition(liftForce + dragForce, _orientation.transform.position);

            LiftData = liftForce.magnitude / 1000f; //Lift in kN
            DragData = dragForce.magnitude / 1000f; //Drag in kN
            StallData = stallEffect * 100f; //Stall effect in %
            AoAData = angleOfAttack * Mathf.Rad2Deg; //Angle of attack in °
        }
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
        float lift = dynamicPressure * _totalSurfaceArea * liftCoefficient;

        Vector3 liftDirection = Vector3.zero;
        if (LiftAxis == WingAxis.Horizontal)
        {
            liftDirection = Vector3.ProjectOnPlane(_orientation.transform.up, airflowDirection).normalized;
        }
        else if (LiftAxis == WingAxis.Vertical)
        {
            liftDirection = Vector3.ProjectOnPlane(-_orientation.transform.right, airflowDirection).normalized;
        }

        Vector3 liftForce = lift * liftDirection;

        return liftForce;
    }

    /// <summary>
    /// Calculates the total drag force for this surface
    /// </summary>
    /// <param name="dynamicPressure">The dynamic pressure</param>
    /// <param name="airflowDirection">The direction of the airflow</param>
    /// <param name="liftCoefficient">The lift coefficient</param>
    /// <returns>The parasitic and induced drag</returns>
    private Vector3 GetDrag(float dynamicPressure, Vector3 airflowDirection, float liftCoefficient, float stallEffect)
    {
        Vector3 parasiticDrag = GetParasiticDrag(dynamicPressure, airflowDirection);
        Vector3 inducedDrag = GetInducedDrag(dynamicPressure, airflowDirection, liftCoefficient);
        Vector3 stallDrag = GetStallDrag(dynamicPressure, airflowDirection, stallEffect);

        Vector3 totalDrag = parasiticDrag + inducedDrag;

        return totalDrag;
    }

    private Vector3 GetStallDrag(float dynamicPressure, Vector3 airflowDirection, float stallEffect)
    {
        float stallDragCoefficient = stallEffect * StallDragModifier;
        float stallDrag = dynamicPressure * _totalSurfaceArea * stallDragCoefficient;
        Vector3 stallDragForce = stallDrag * airflowDirection;

        return stallDragForce;
    }

    /// <summary>
    /// Calculates the parasitic drag force for this surface
    /// </summary>
    /// <param name="dynamicPressure">The dynamic pressure</param>
    /// <param name="airflowDirection">The direction of the airflow</param>
    /// <returns>The parasitic drag force</returns>
    private Vector3 GetParasiticDrag(float dynamicPressure, Vector3 airflowDirection)
    {
        float parasiticDrag = dynamicPressure * _totalSurfaceArea * DragCoefficient;
        Vector3 parasiticDragForce = parasiticDrag * airflowDirection;

        return parasiticDragForce;
    }

    /// <summary>
    /// Calculates the induced drag force for this surface
    /// </summary>
    /// <param name="dynamicPressure">The dynamic pressure</param>
    /// <param name="airflowDirection">The direction of the airflow</param>
    /// <param name="liftCoefficient">The lift coefficient</param>
    /// <returns>The induced drag force</returns>
    private Vector3 GetInducedDrag(float dynamicPressure, Vector3 airflowDirection, float liftCoefficient)
    {
        float inducedDragCoefficient = liftCoefficient * liftCoefficient * InducedDragModifier;
        float inducedDrag = dynamicPressure * _totalSurfaceArea * inducedDragCoefficient;
        Vector3 inducedDragForce = inducedDrag * airflowDirection;

        return inducedDragForce;
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
        float altitude = Mathf.Max(0, _orientation.transform.position.y);
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
        Vector3 chordLine = _orientation.transform.forward;
        Vector3 projectedVelocity = Vector3.zero;

        float angleOfAttck = 0f;
        if (LiftAxis == WingAxis.Horizontal)
        {
            projectedVelocity = Vector3.ProjectOnPlane(velocity.normalized, _orientation.transform.right);
            angleOfAttck = Vector3.SignedAngle(chordLine, projectedVelocity, _orientation.transform.right);
        }
        else if (LiftAxis == WingAxis.Vertical)
        {
            projectedVelocity = Vector3.ProjectOnPlane(velocity.normalized, _orientation.transform.up);
            angleOfAttck = Vector3.SignedAngle(chordLine, projectedVelocity, _orientation.transform.up);
        }

        return angleOfAttck * Mathf.Deg2Rad;
    }

    /// <summary>
    /// Calculates the angle of attack where the lift is zero
    /// </summary>
    /// <returns>The zero lift angle of attack</returns>
    private float GetZeroLiftAngle()
    {
        float deflection = (-ControlSurfaceDeflection * Mathf.Deg2Rad) * DeflectionCoefficient;
        float zeroLiftAngle = (InitialZeroLiftAngle * Mathf.Deg2Rad) + deflection;

        return zeroLiftAngle;
    }

    /// <summary>
    /// Calculates the lift coefficient
    /// </summary>
    /// <param name="angleOfAttack">The angle of attack at this surface</param>
    /// <param name="zeroLiftAngle">The zero lift angle</param>
    /// <returns>The lift coefficient</returns>
    private float GetLiftCoefficient(float angleOfAttack, float zeroLiftAngle, float stallEffect)
    {
        float liftCoefficient = LiftModifier * (angleOfAttack - zeroLiftAngle);
        float stallMultiplier = Mathf.Lerp(1f, MaxStallMultiplier, stallEffect);

        liftCoefficient *= stallMultiplier;

        return liftCoefficient;
    }

    private float GetStallEffect(float angleOfAttack, float speed)
    {
        float absoluteAngleOfAttack = Mathf.Abs(angleOfAttack) * Mathf.Rad2Deg;

        if (absoluteAngleOfAttack > StallAngle && speed > 5f)
        {
            float stallEffect = Mathf.Clamp01((absoluteAngleOfAttack - StallAngle) / 10f);

            return stallEffect;
        }

        return 0f;
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
