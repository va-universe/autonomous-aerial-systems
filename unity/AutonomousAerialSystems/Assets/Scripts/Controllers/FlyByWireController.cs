using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Windows;

/// <summary>
/// The controller of the fly-by-wire prototype
/// </summary>
public class FlyByWireController : Controller
{
    #region Inputs

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
    #endregion

    #region UI Display Text
    private TextMeshProUGUI _overrideText;
    private TextMeshProUGUI _brakingText;
    #endregion

    [Header("Max Deflection Overrides")]
    public float AileronDeflectionOverrideModifier;
    public float FlapDeflectionOverrideModifier;
    public float ElevatorDeflectionOverrideModifier;
    public float RudderDeflectionOverrideModifier;

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
    public float MaxComfortStallPercentage;
    public float MaxComfortAngleOfAttack;
    public float MaxOverrideStallPercentage;
    public float MaxOverrideAngleOfAttack;

    [Header("Input Smoother")]
    public float UpPitchSmoothingStrength;
    public float DownPitchSmoothingStrength;
    public float RollSmoothingStrength;
    public float YawSmoothingStrength;
    public float FlapSmoothingStrength;

    [Header("Yaw")]
    public float YawDampingStrength;
    public float InputDampingModifier;

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
   
        float stallModifier = 1f - Mathf.Clamp01(highestStall / MaxComfortStallPercentage);
        float angleOfAttackModifier = 1f - Mathf.Clamp01(highestAbsoluteAngle / MaxComfortAngleOfAttack);
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
        float yawDampedInput = Mathf.Clamp(_initialYawInput + GetYawDamping(), -1f, 1f);
        float stallLimitedInput = LimitStall(yawDampedInput, WingAxis.Vertical);
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

    /// <summary>
    /// Update the deflection angles in the wing surfaces, with deflection modifiers
    /// </summary>
    protected override void UpdateWingSurfaceData()
    {
        float maxAileronDeflection = GetDeflection(MaxAileronDeflection, AileronDeflectionOverrideModifier);
        _leftAileronParent.ControlSurfaceDeflection = maxAileronDeflection * _rollInput;
        _rightAileronParent.ControlSurfaceDeflection = maxAileronDeflection * -_rollInput;

        float maxFlapDeflection = GetDeflection(MaxFlapDeflection, FlapDeflectionOverrideModifier);
        _leftFlapParent.ControlSurfaceDeflection = maxFlapDeflection * _flapInput;
        _rightFlapParent.ControlSurfaceDeflection = maxFlapDeflection * _flapInput;

        float maxElevatorDeflection = GetDeflection(MaxElevatorDeflection, ElevatorDeflectionOverrideModifier);
        _leftElevatorParent.ControlSurfaceDeflection = maxElevatorDeflection * _pitchInput;
        _rightElevatorParent.ControlSurfaceDeflection = maxElevatorDeflection * _pitchInput;

        float maxRudderDeflection = GetDeflection(MaxRudderDeflection, RudderDeflectionOverrideModifier);
        _rudderParent.ControlSurfaceDeflection = maxRudderDeflection * _yawInput;
    }

    /// <summary>
    /// Visualize control surface deflection, with deflection modifiers
    /// </summary>
    protected override void VisualizeControlSurfaces()
    {
        float maxAileronDeflection = GetDeflection(MaxAileronDeflection, AileronDeflectionOverrideModifier);
        _leftAileron.DeflectSurface(_rollInput, maxAileronDeflection);
        _rightAileron.DeflectSurface(-_rollInput, maxAileronDeflection);

        float maxFlapDeflection = GetDeflection(MaxFlapDeflection, FlapDeflectionOverrideModifier);
        _leftFlap.DeflectSurface(_flapInput, maxFlapDeflection);
        _rightFlap.DeflectSurface(_flapInput, maxFlapDeflection);

        float maxElevatorDeflection = GetDeflection(MaxElevatorDeflection, ElevatorDeflectionOverrideModifier);
        _leftElevator.DeflectSurface(_pitchInput, maxElevatorDeflection);
        _rightElevator.DeflectSurface(_pitchInput, maxElevatorDeflection);

        float maxRudderDeflection = GetDeflection(MaxRudderDeflection, RudderDeflectionOverrideModifier);
        _rudder.DeflectSurface(_yawInput, maxRudderDeflection);
    }

    /// <summary>
    /// Get the max deflection
    /// </summary>
    /// <param name="maxDeflection">The max deflection without override effect</param>
    /// <param name="modifier">The override modifier</param>
    /// <returns>The new max deflection</returns>
    private float GetDeflection(float maxDeflection, float modifier)
    {
        if (_overrideInput)
        {
            return maxDeflection * modifier;
        }
        return maxDeflection;
    }

    /// <summary>
    /// Update UI display text, including Override and Braking text
    /// </summary>
    protected override void UpdateDisplay()
    {
        base.UpdateDisplay();

        if (_overrideText != null)
        {
            if (_overrideInput)
            {
                _overrideText.color = Color.red;
            }
            else
            {
                _overrideText.color = Color.gray;
            }
        }
        if (_brakingText != null)
        {
            if (WheelBrakeInput != 0)
            {
                _brakingText.color = Color.red;
            }
            else
            {
                _brakingText.color = Color.gray;
            }
        }
    }

    /// <summary>
    /// Gets all texts from canvas, including Override and Braking text
    /// </summary>
    protected override void InitializeTextDisplays()
    {
        base.InitializeTextDisplays();

        if (UICanvas != null)
        {
            Transform aircraftPanel = UICanvas.transform.Find("Aircraft Panel").transform;
            _overrideText = aircraftPanel.transform.Find("OverrideText").GetComponent<TextMeshProUGUI>();
            _brakingText = aircraftPanel.transform.Find("BrakingText").GetComponent<TextMeshProUGUI>();
        }
    }

    /// <summary>
    /// Gets the yaw damping input.
    /// </summary>
    /// <returns>The yaw damping input</returns>
    public float GetYawDamping()
    {
        float yawRate = Vector3.Dot(_rb.angularVelocity, transform.up);
        float inputModifier = 1f - (Mathf.Abs(_initialYawInput) * InputDampingModifier);
        float yawDamping = yawRate * YawDampingStrength * inputModifier;

        return yawDamping;
    }
}