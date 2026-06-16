using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Windows;

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

    private bool _overrideInput;

    [Header("Fly-By-Wire Systems")]
    public bool IsGForceLimited;
    public bool IsStallLimited;
    public bool IsInputSmoothened;

    [Header("G-Force Limiter")]
    public float MaxComfortGForce;
    public float MinComfortGForce;
    public float MaxOverrideGForce;
    public float MinOverrideGForce;
    public float MaxLimiterStrength;
    public float MinLimiterStrength;

    [Header("Stall Protection")]
    public float MaxStallPercentage;
    public float MaxAngleOfAttack;

    [Header("Input Smoother")]
    public float UpPitchSmoothingStrength;
    public float DownPitchSmoothingStrength;
    public float RollSmoothingStrength;
    public float YawSmoothingStrength;
    public float FlapSmoothingStrength;

    protected override void FixedUpdate()
    {
        ConvertInputs();
        base.FixedUpdate();
    }

    /// <summary>
    /// Limits the input based on prefered G-force
    /// </summary>
    /// <param name="input">The input to be limited</param>
    /// <returns>The input after being limited by G-force</returns>
    private float LimitGForce(float input)
    {
        if (!IsGForceLimited)
        {
            return input;
        }

        float maxGForce = _overrideInput ? MaxOverrideGForce : MaxComfortGForce;
        float minGForce = _overrideInput ? MinOverrideGForce : MinComfortGForce;

        float modifier = 1f;
        if (input < 0f && GForce > 0f)
        {
            modifier = 1f - Mathf.Clamp01(Mathf.Log10((GForce / maxGForce) + 1f) * MaxLimiterStrength);
        }
        else if (input > 0f && GForce < 0f)
        {
            modifier = 1f - Mathf.Clamp01(Mathf.Log10((GForce / minGForce) + 1f) * MinLimiterStrength);
        }

        return input * modifier;
    }

    /// <summary>
    /// Limits the input based on stall percentage
    /// </summary>
    /// <param name="input">The input to be limited</param>
    /// <param name="axis">The axis of the wing surface</param>
    /// <returns>The input after being limited by stall percentage</returns>
    private float LimitStall(float input, WingAxis axis)
    {
        if (!IsStallLimited)
        {
            return input;
        }

        float highestStall = 0f;
        float highestAngleOfAttack = 0f;
        float lowestAngleOfAttack = 0f;
        if (axis == WingAxis.Horizontal)
        {
            highestStall = Mathf.Max(_leftAileronParent.StallData, _rightAileronParent.StallData, _leftFlapParent.StallData, _rightFlapParent.StallData, _leftElevatorParent.StallData, _rightElevatorParent.StallData);
            highestAngleOfAttack = Mathf.Max(_leftAileronParent.AoAData, _rightAileronParent.AoAData, _leftFlapParent.AoAData, _rightFlapParent.AoAData, _leftElevatorParent.AoAData, _rightElevatorParent.AoAData);
            lowestAngleOfAttack = Mathf.Min(_leftAileronParent.AoAData, _rightAileronParent.AoAData, _leftFlapParent.AoAData, _rightFlapParent.AoAData, _leftElevatorParent.AoAData, _rightElevatorParent.AoAData);
        }
        else
        {
            highestStall = _rudderParent.StallData;
            highestAngleOfAttack = _rudderParent.AoAData;
            lowestAngleOfAttack = _rudderParent.AoAData;
        }

        float highestAbsoluteAngle = Mathf.Max(Mathf.Abs(highestAngleOfAttack), Mathf.Abs(lowestAngleOfAttack));
   
        float stallModifier = 1f - Mathf.Clamp01(highestStall / MaxStallPercentage);
        float angleOfAttackModifier = 1f - Mathf.Clamp01(highestAbsoluteAngle / MaxAngleOfAttack);
        float modifier = (stallModifier + angleOfAttackModifier) / 2f;

        int sign = (Mathf.Abs(highestAngleOfAttack) >= Mathf.Abs(lowestAngleOfAttack)) ? (int)Mathf.Sign(highestAngleOfAttack) : (int)Mathf.Sign(lowestAngleOfAttack);
        if (Mathf.Sign(input) == sign)
        {
            return input * modifier;
        }

        return input;
    }

    /// <summary>
    /// Smoothens the input to avoid oscillations and sharp movement
    /// </summary>
    /// <param name="targetInput">The currently requested input</param>
    /// <param name="currentInput">The current input, also refered to as the previously commanded input</param>
    /// <param name="strength">The rate at which the currentInput moves towards the targetInput</param>
    /// <returns>The input after being smoothened</returns>
    private float Smoother(float targetInput, float currentInput, float strength)
    {
        if (!IsInputSmoothened)
        {
            return targetInput;
        }

        return Mathf.MoveTowards(currentInput, targetInput, strength * Time.fixedDeltaTime);
    }

    /// <summary>
    /// Get the roll, pitch, yaw, flap and thrust input from the input action system
    /// </summary>
    protected override void GetInput()
    {
        _initialRollInput = _inputActions.FlyByWire.Roll.ReadValue<float>();
        _initialPitchInput = _inputActions.FlyByWire.Pitch.ReadValue<float>();
        _initialYawInput = _inputActions.FlyByWire.Yaw.ReadValue<float>();
        _initialFlapInput = _inputActions.FlyByWire.Flap.ReadValue<float>();

        _overrideInput = _inputActions.FlyByWire.Override.ReadValue<float>() == 1 ? true : false;
        _throttleInput = _inputActions.FlyByWire.Thrust.ReadValue<float>();
        WheelBrakeInput = _inputActions.FlyByWire.WheelBrake.ReadValue<float>();
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
        float smoothingStrength = _pitchInput <= 0f ? UpPitchSmoothingStrength : DownPitchSmoothingStrength;

        float gLimitedInput = LimitGForce(_initialPitchInput);
        float stallLimitedInput = -LimitStall(-gLimitedInput, WingAxis.Horizontal); //Input values are reversed in comparison to Flap, that is why there is minuses.
        float smoothedInput = Smoother(stallLimitedInput, _previousPitchInput, smoothingStrength);

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
        float stallLimitedInput = LimitStall(_initialYawInput, WingAxis.Vertical);
        float smoothedInput = Smoother(stallLimitedInput, _previousYawInput, YawSmoothingStrength);

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
        float stallLimitedInput = LimitStall(gLimitedInput, WingAxis.Horizontal);
        float smoothedInput = Smoother(stallLimitedInput, _previousFlapInput, FlapSmoothingStrength);

        _previousFlapInput = smoothedInput;

        return smoothedInput;
    }
}