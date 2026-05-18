using UnityEngine;

/// <summary>
/// This component is used on control surfaces in order to adjust deflection
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
    /// Rotates the control surface to the product of the input and max deflection
    /// </summary>
    /// <param name="input">Rotation input (between -1 and 1)</param>
    public void DeflectSurface(float input)
    {
        float normalizedInput = Mathf.Clamp(input, -1f, 1f);
        float angle = input * MaxDeflection;

        transform.localRotation = _initialLocalRotation * Quaternion.AngleAxis(angle, RotationAxis);
    }
}
