using UnityEngine;

/// <summary>
/// Control surface deflection handler
/// </summary>
public class ControlSurface : MonoBehaviour
{
    [Header("Parameters")]
    public ControlSurfaceType surfaceType;
    public Vector3 RotationAxis;
    public float MaxDeflection;

    [Header("Input")]
    public float Input;
    private Quaternion _initialLocalRotation;

    void Awake()
    {
        _initialLocalRotation = transform.localRotation;
    }

    /// <summary>
    /// Rotates the control surface to the desired deflection, based on input and max deflection
    /// </summary>
    /// <param name="input">The deflection activation input (between -1 and 1)</param>
    public void DeflectSurface(float input)
    {
        float normalizedInput = Mathf.Clamp(input, -1f, 1f);
        float angle = input * MaxDeflection;

        transform.localRotation = _initialLocalRotation * Quaternion.AngleAxis(angle, RotationAxis);
    }

    /// <summary>
    /// Rotates the control surface to the desired deflection, based on input and max deflection
    /// </summary>
    /// <param name="input">The deflection activation input (between -1 and 1)</param>
    /// <param name="maxDeflection">The maximum deflection</param>
    public void DeflectSurface(float input, float maxDeflection)
    {
        float normalizedInput = Mathf.Clamp(input, -1f, 1f);
        float angle = input * maxDeflection;

        transform.localRotation = _initialLocalRotation * Quaternion.AngleAxis(angle, RotationAxis);
    }
}
