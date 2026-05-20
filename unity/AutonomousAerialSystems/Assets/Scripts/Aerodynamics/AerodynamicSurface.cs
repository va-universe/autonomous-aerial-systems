using UnityEditor.Search;
using UnityEngine;

public class AerodynamicSurface : MonoBehaviour
{
    private Rigidbody _rb;
    private FirstAircraftController _controller;

    [Header("Lift")]
    public WingAxis LiftAxis;
    public float LiftCoefficient = 5.3f; //temp
    public float LiftSurfaceArea;

    [Header("Drag")]
    public float DragCoefficient = 0.02f; //temp
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

    private void ApplyForces()
    {
        Vector3 velocity = _rb.GetPointVelocity(transform.position);
        Vector3 airflowDirection = velocity.normalized * -1f;
        float speed = velocity.magnitude;

        float dynamicPressure = GetDynamicPressure(speed);
        float angleOfAttack = GetAngleOfAttack(airflowDirection);

        float liftCoefficient = Mathf.Clamp(LiftCoefficient * angleOfAttack * Mathf.Deg2Rad, -1.6f, 1.6f);

        Vector3 liftForce = GetLift(dynamicPressure, angleOfAttack, airflowDirection, liftCoefficient);
        Vector3 dragForce = GetDrag(dynamicPressure, angleOfAttack, airflowDirection, liftCoefficient);

        _rb.AddForceAtPosition(liftForce + dragForce, transform.position);
    }

    private float GetDynamicPressure(float speed)
    {
        float airDensity = GetAirDensity();
        float dynamicPressure = (airDensity * speed * speed) / 2f;

        return dynamicPressure;
    }
    private float GetAirDensity()
    {
        float altitude = Mathf.Max(0, transform.position.y);
        float turningPoint = 9000f; //Derived from testing in Geogebra

        float airDensity = _controller.AirDensityAtSeaLevel * Mathf.Exp(-altitude / turningPoint);
        return airDensity;
    }

    private float GetAngleOfAttack(Vector3 airflowDirection)
    {
        float angleOfAttack = 0f;
        if (LiftAxis == WingAxis.Horizontal)
        {
            angleOfAttack = Vector3.SignedAngle(transform.forward, airflowDirection, transform.right);
        }
        else if (LiftAxis == WingAxis.Vertical)
        {
            angleOfAttack = Vector3.SignedAngle(transform.forward, airflowDirection, transform.up);
        }

        return Mathf.Clamp(angleOfAttack, -20f, 20f); //temp
    }

    private Vector3 GetLift(float dynamicPressure, float angleOfAttack, Vector3 airflowDirection, float liftCoefficient)
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
    private Vector3 GetDrag(float dynamicPressure, float angleOfAttack, Vector3 airflowDirection, float liftCoefficient)
    {
        float inducedDrag = liftCoefficient * liftCoefficient * 0.1f;
        float drag = dynamicPressure * DragSurfaceArea * (DragCoefficient + inducedDrag);
        Vector3 dragForce = drag * airflowDirection;

        return dragForce;
    }
}
