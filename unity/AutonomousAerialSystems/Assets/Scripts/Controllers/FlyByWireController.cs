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

    #region Previous Inputs
    private float _previousRollInput;
    private float _previousYawInput;
    private float _previousPitchInput;
    private float _previousFlapInput;
    #endregion

    [Header("G-Force Limiter")]
    public float MaxGForce;
    public float MinGForce;
    public float LimiterStrength;

    [Header("Input Smoother")]
    public float PitchSmoothingStrength;
    public float RollSmoothingStrength;
    public float YawSmoothingStrength;
    public float FlapSmoothingStrength;

    protected override void FixedUpdate()
    {
        ConvertInputs();
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

    private float Smoother(float targetInput, float currentInput, float strength)
    {
        return Mathf.MoveTowards(currentInput, targetInput, strength * Time.fixedDeltaTime);
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

    /// <summary>
    /// Converts raw user inputs into fly-by-wire-modified inputs
    /// </summary>
    private void ConvertInputs()
    {
        _pitchInput = GetPitchInput();
        _rollInput = GetRollInput();
        _yawInput = GetYawInput();
        _flapInput = GetFlapInput();
    }

    /// <summary>
    /// Get the fly-by-wire pitch input
    /// </summary>
    /// <returns>The pitch input</returns>
    private float GetPitchInput()
    {
        float gLimitedInput = LimitGForce(_initialPitchInput);
        float smoothedInput = Smoother(gLimitedInput, _previousPitchInput, PitchSmoothingStrength);

        _previousPitchInput = smoothedInput;

        return smoothedInput;
    }

    /// <summary>
    /// Get the fly-by-wire roll input
    /// </summary>
    /// <returns>The roll input</returns>
    private float GetRollInput()
    {
        float smoothedInput = Smoother(_initialRollInput, _previousRollInput, RollSmoothingStrength);

        _previousRollInput = smoothedInput;

        return smoothedInput;
    }

    /// <summary>
    /// Get the fly-by-wire yaw input
    /// </summary>
    /// <returns>The yaw input</returns>
    private float GetYawInput()
    {
        float smoothedInput = Smoother(_initialYawInput, _previousYawInput, YawSmoothingStrength);

        _previousYawInput = smoothedInput;

        return smoothedInput;
    }

    /// <summary>
    /// Get the fly-by-wire flap input
    /// </summary>
    /// <returns>The flap input</returns>
    private float GetFlapInput()
    {
        float gLimitedInput = -LimitGForce(-_initialFlapInput); //Input values are reversed in comparison to Pitch, that is why there is minuses.
        float smoothedInput = Smoother(gLimitedInput, _previousFlapInput, FlapSmoothingStrength);

        _previousFlapInput = smoothedInput;

        return smoothedInput;
    }
}