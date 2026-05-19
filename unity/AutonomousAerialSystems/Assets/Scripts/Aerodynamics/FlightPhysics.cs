using UnityEngine;

public class FlightPhysics : MonoBehaviour
{
    private Rigidbody _rb;

    [Header("Lift")]
    public float LiftCoefficient;
    public float LiftSurfaceArea;

    [Header("Drag")]
    public float DragCoefficient;
    public float DragSurfaceArea;

    [Header("Air Density")]
    public float SeaLevelDensity;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        float airDensity = GetAirDensity();
        ApplyLift(airDensity);
        ApplyDrag(airDensity);
    }

    /// <summary>
    /// Compute a global and simplified lift for the aircraft
    /// </summary>
    /// <param name="airDensity">The current air density</param>
    private void ApplyLift(float airDensity)
    {
        float speed = _rb.linearVelocity.magnitude;
        float lift = 0.5f * speed * speed * airDensity * LiftSurfaceArea * LiftCoefficient;

        _rb.AddForce(transform.up * lift);
    }

    /// <summary>
    /// Compute a global and simplified drag for the aircraft
    /// </summary>
    /// <param name="airDensity">The current air density</param>
    private void ApplyDrag(float airDensity)
    {
        Vector3 velocity = _rb.linearVelocity;
        float speed = velocity.magnitude;

        Vector3 drag = 0.5f * speed * speed * airDensity * DragSurfaceArea * DragCoefficient * -velocity.normalized;
        _rb.AddForce(drag);
    }

    /// <summary>
    /// Extremly simplified air density calculator
    /// </summary>
    /// <returns>The air density at the current altitude</returns>
    private float GetAirDensity()
    {
        float altitude = Mathf.Max(0, transform.position.y);
        float turningPoint = 9000f; //Derived from testing in Geogebra

        float airDensity = SeaLevelDensity * Mathf.Exp(-altitude / turningPoint);
        return airDensity;
    }
}
