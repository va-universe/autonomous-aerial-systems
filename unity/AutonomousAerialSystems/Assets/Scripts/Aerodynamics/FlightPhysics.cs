using UnityEngine;

/// <summary>
/// Centralized physics model
/// </summary>
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
    /// Applies a global and simplified lift for the entire aircraft
    /// </summary>
    /// <param name="airDensity">The air density at the aircraft position</param>
    private void ApplyLift(float airDensity)
    {
        float speed = _rb.linearVelocity.magnitude;
        float lift = 0.5f * speed * speed * airDensity * LiftSurfaceArea * LiftCoefficient;

        _rb.AddForce(transform.up * lift);
    }

    /// <summary>
    /// Applies a global and simplified parasitic drag for the entire aircraft
    /// </summary>
    /// <param name="airDensity">The air density at the aircraft position</param>
    private void ApplyDrag(float airDensity)
    {
        Vector3 velocity = _rb.linearVelocity;
        float speed = velocity.magnitude;

        Vector3 drag = 0.5f * speed * speed * airDensity * DragSurfaceArea * DragCoefficient * -velocity.normalized;
        _rb.AddForce(drag);
    }

    /// <summary>
    /// Calculates and returns an approximation of the air density based on altitude
    /// </summary>
    /// <returns>The air density at the surface position</returns>
    private float GetAirDensity()
    {
        float altitude = Mathf.Max(0, transform.position.y);
        float turningPoint = 9000f;

        float airDensity = SeaLevelDensity * Mathf.Exp(-altitude / turningPoint);
        return airDensity;
    }
}
