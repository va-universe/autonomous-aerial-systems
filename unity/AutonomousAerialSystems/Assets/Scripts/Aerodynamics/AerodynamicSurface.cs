using UnityEditor.Search;
using UnityEngine;

public class AerodynamicSurface : MonoBehaviour
{
    private Rigidbody _rb;
    private FirstAircraftController _controller;

    [Header("Lift")]
    public WingAxis LiftAxis;
    public float LiftCoefficient;
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

    private void ApplyForces()
    {
        Vector3 velocity = _rb.GetPointVelocity(transform.position);
        Vector3 airflowDirection = velocity.normalized * -1f;
        float speed = velocity.magnitude;

        float dynamicPressure = GetDynamicPressure(speed);

        Vector3 liftForce = GetLift(dynamicPressure, airflowDirection);
        Vector3 dragForce = GetDrag(dynamicPressure, airflowDirection);

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
        float turningPoint = 9000f;

        float airDensity = _controller.AirDensityAtSeaLevel * Mathf.Exp(-altitude / turningPoint);
        return airDensity;
    }

    private Vector3 GetLift(float dynamicPressure, Vector3 airflowDirection)
    {
        float lift = dynamicPressure * LiftSurfaceArea * LiftCoefficient;

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
    private Vector3 GetDrag(float dynamicPressure, Vector3 airflowDirection)
    {
        float drag = dynamicPressure * DragSurfaceArea * DragCoefficient;
        Vector3 dragForce = drag * airflowDirection;

        return dragForce;
    }
}
