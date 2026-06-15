using TMPro;
using UnityEngine;

/// <summary>
/// The controller of the fly-by-wire prototype
/// </summary>
public class FlyByWireController : Controller
{
    #region Initial Inputs
    private float _initialRollInput;
    private float _initialYawInput;
    private float _initialPitchInput;
    private float _initialFlapInput;
    #endregion

    [Header("G-Force Limiter")]
    public float MaxGForce;
    public float MinGForce;
    public float LimiterStrength;

    protected override void FixedUpdate()
    {
        ConvertInput();
        base.FixedUpdate();
    }

    private float LimitGForce(float input)
    {
        float modifier = 1f;
        if (input < 0f && GForce > 0f)
        {
            modifier = 1f - Mathf.Clamp01(Mathf.Log10((GForce / MaxGForce) + 1f) * LimiterStrength);
        }
        else if (input > 0f && GForce < 0f)
        {
            modifier = 1f - Mathf.Clamp01(Mathf.Log10((GForce / MinGForce) + 1f) * LimiterStrength);
        }

        return input * modifier;
    }

    /// <summary>
    /// Get the roll, pitch, yaw, flap and thrust input from the input action system
    /// </summary>
    protected override void GetInput()
    {
        _initialRollInput = _inputActions.AircraftWithFlaps.Roll.ReadValue<float>();
        _initialPitchInput = _inputActions.AircraftWithFlaps.Pitch.ReadValue<float>();
        _initialYawInput = _inputActions.AircraftWithFlaps.Yaw.ReadValue<float>();
        _initialFlapInput = _inputActions.AircraftWithFlaps.Flap.ReadValue<float>();

        _throttleInput = _inputActions.AircraftWithFlaps.Thrust.ReadValue<float>();
        WheelBrakeInput = _inputActions.AircraftWithFlaps.WheelBrake.ReadValue<float>();
    }

    private void ConvertInput()
    {
        _pitchInput = LimitGForce(_initialPitchInput);
        _flapInput = -LimitGForce(-_initialFlapInput);

        _rollInput = _initialRollInput;
        _yawInput = _initialYawInput;
    }
}