using UnityEngine;

/// <summary>
/// Decentralized physics model where each parent surface apply their own force
/// </summary>
public class WingSurface : MonoBehaviour
{
    private Rigidbody _rb;
    private SecondAircraftController _controller;

    public float _totalSurfaceArea;

    void Start()
    {
        _rb = GetComponentInParent<Rigidbody>();
        _controller = GetComponentInParent<SecondAircraftController>();

        SetSurfaceArea();
    }

    void Update()
    {
        
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
