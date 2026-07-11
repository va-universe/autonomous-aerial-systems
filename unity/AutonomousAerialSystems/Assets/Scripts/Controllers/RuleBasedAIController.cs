using System;
using TMPro;
using UnityEngine;

/// <summary>
/// The controller of the rule-based AI prototype
/// </summary>
public class RuleBasedAIController : FlyByWireController
{
    #region Inputs & UI display texts

    private bool _isAIActivated;
    private TextMeshProUGUI _altitudeGroundText;
    private TextMeshProUGUI _autonomousText;
    private TextMeshProUGUI _stateText;

    #endregion

    [Header("Bank Limiter")]
    public float BankLimiterStrength;
    public float BankCorrectionModifier;
    public float MaxCorrectionInput;

    public float MaxBankLimit;
    public float MinBankLimit;
    public float BankLimitAltitudeCap;


    [Header("Rule-Based AI")]
    public AircraftState State;

    [Header("Takeoff")]
    public float EndTakeoffAltitude;
    public float PitchDownInput;
    public float PitchTransitionRate;
    public float FlapTransitionRate;
    private float _pitchTransitionInput;
    private float _flapTransitionInput;

    [Header("Tracking")]
    public WaypointHandler SimulationHandler;
    public float TrackingOffset;
    public float PitchTrackingStrength;
    public float RollTrackingStregnth;
    public float FlapTrackingStrength;
    private GameObject _currentWaypoint;

    [Header("Sensor Systems")]
    public bool IsGrounded;
    public float GroundSensorRange;
    private float _altitudeAboveGround;

    protected override void Start()
    {
        base.Start();

        State = AircraftState.Grounded;
    }

    protected override void FixedUpdate()
    {
        UpdateWaypoint();

        base.FixedUpdate();

        RunSensors();
        HandleState();
    }

    private void UpdateWaypoint()
    {
        if (SimulationHandler != null)
        {
            _currentWaypoint = SimulationHandler.CurrentWaypoint.gameObject;
        }
    }

    /// <summary>
    /// The state handler for the rule-based AI aircraft
    /// </summary>
    private void HandleState()
    {
        if (_isAIActivated)
        {
            bool isTakeoffComplete = _altitudeAboveGround >= EndTakeoffAltitude;

            if (isTakeoffComplete)
            {
                if (_pitchTransitionInput >= 0 && _flapTransitionInput <= 0)
                {
                    if (_currentWaypoint != null)
                    {
                        State = AircraftState.Tracking;
                    }
                    else
                    {
                        State = AircraftState.Cruise;
                    }
                }
                else
                {
                    State = AircraftState.Transition;
                }
            }
            else
            {
                State = AircraftState.Takeoff;
            }
        }
        else if (IsGrounded)
        {
            State = AircraftState.Grounded;
        }
    }

    /// <summary>
    /// Run all sensor systems
    /// </summary>
    protected virtual void RunSensors()
    {
        _altitudeAboveGround = GetAltitudeAboveGround();
        IsGrounded = _altitudeAboveGround < 1.5f;
    }

    /// <summary>
    /// Gets the distance between the aircraft's center and the ground surface
    /// </summary>
    /// <returns>The altitude above ground</returns>
    private float GetAltitudeAboveGround()
    {
        float altitudeAboveGround = Mathf.Infinity;

        RaycastHit[] hits = Physics.RaycastAll(transform.position, Vector3.down, GroundSensorRange);
        foreach (RaycastHit hit in hits)
        {
            if (hit.transform.CompareTag("Ground") && hit.distance < altitudeAboveGround)
            {
                altitudeAboveGround = hit.distance;
            }
        }

        return altitudeAboveGround;
    }

    /// <summary>
    /// Gets the flight controls, thrust, braking, override and systems toggle inputs from the rule-based AI
    /// </summary>
    protected override void GetInput()
    {
        GetControlInputs();
        ToggleSystemInputs();

        if (_inputActions.RuleBasedAI.ToggleAI.WasPressedThisFrame())
        {
            _isAIActivated = !_isAIActivated;
        }
    }

    /// <summary>
    /// Gets the flight controls, thrust, braking and override inputs from the rule-based AI
    /// </summary>
    private void GetControlInputs()
    {
        if (_isAIActivated)
        {
            _initialRollInput = GetAIRequestedRoll();
            _initialPitchInput = GetAIRequestedPitch();
            _initialYawInput = GetAIRequestedYaw();
            _initialFlapInput = GetAIRequestedFlap();

            _throttleInput = GetAIRequestedThrottle();
            _overrideInput = GetAIOverrideInput();
            WheelBrakeInput = GetAIRequestedWheelBrake();
        }
        else
        {
            _initialRollInput = 0f;
            _initialPitchInput = 0f;
            _initialYawInput = 0f;
            _initialFlapInput = 0f;

            _throttleInput = 0f;
            _overrideInput = false;
            WheelBrakeInput = 1f;
        }
    }

    /// <summary>
    /// Gets the override input from the rule-based AI
    /// </summary>
    /// <returns>The override input</returns>
    private bool GetAIOverrideInput()
    {
        if (State == AircraftState.Tracking)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Gets the requested roll input from the rule-based AI
    /// </summary>
    /// <returns>The requested roll input</returns>
    private float GetAIRequestedRoll()
    {
        float requestedInput = 0f;

        if (State == AircraftState.Tracking)
        {
            Vector3 localWaypoint = transform.InverseTransformPoint(_currentWaypoint.transform.position + new Vector3(0f, TrackingOffset, 0f));
            Vector3 direction = localWaypoint.normalized;

            requestedInput = Mathf.Clamp(direction.x * RollTrackingStregnth, -1f, 1f);
        }

        return requestedInput;
    }

    /// <summary>
    /// Gets the requested pitch input from the rule-based AI
    /// </summary>
    /// <returns>The requested pitch input</returns>
    private float GetAIRequestedPitch()
    {
        float requestedInput = 0f;

        if (State == AircraftState.Takeoff)
        {
            _pitchTransitionInput = -1f;
            requestedInput = _pitchTransitionInput;
        }
        else if (State == AircraftState.Transition)
        {
            requestedInput = _pitchTransitionInput;
            _pitchTransitionInput = Mathf.Min(PitchDownInput, _pitchTransitionInput + PitchTransitionRate * Time.deltaTime);
        }
        else if (State == AircraftState.Tracking)
        {
            Vector3 localWaypoint = transform.InverseTransformPoint(_currentWaypoint.transform.position + new Vector3(0f, TrackingOffset, 0f));
            Vector3 direction = localWaypoint.normalized;

            requestedInput = Mathf.Clamp(direction.y * -PitchTrackingStrength, -1f, 1f);
        }

        return requestedInput;
    }

    /// <summary>
    /// Gets the requested yaw input from the rule-based AI
    /// </summary>
    /// <returns>The requested yaw input</returns>
    private float GetAIRequestedYaw()
    {
        return 0f;
    }

    /// <summary>
    /// Gets the requested flap input from the rule-based AI
    /// </summary>
    /// <returns>The requested flap input</returns>
    private float GetAIRequestedFlap()
    {
        float requestedInput = 0f;

        if (State == AircraftState.Takeoff)
        {
            _flapTransitionInput = 1f;
            requestedInput = _flapTransitionInput;
        }
        else if (State == AircraftState.Transition)
        {
            requestedInput = _flapTransitionInput;
            _flapTransitionInput = Mathf.Max(0, _flapTransitionInput - FlapTransitionRate * Time.deltaTime);
        }
        else if (State == AircraftState.Tracking)
        {
            Vector3 localWaypoint = transform.InverseTransformPoint(_currentWaypoint.transform.position + new Vector3(0f, TrackingOffset, 0f));
            Vector3 direction = localWaypoint.normalized;

            requestedInput = Mathf.Clamp(direction.y * FlapTrackingStrength, -1f, 1f);
        }

        return requestedInput;
    }

    /// <summary>
    /// Gets the requested throttle/thrust input from the rule-based AI
    /// </summary>
    /// <returns>The requested throttle/thrust input</returns>
    private float GetAIRequestedThrottle()
    {
        float requestedInput = 1f;

        if (State == AircraftState.Grounded)
        {
            requestedInput = 0f;
        }

        return requestedInput;
    }

    /// <summary>
    /// Gets the requested wheel braking input from the rule-based AI
    /// </summary>
    /// <returns>The requested wheel braking input</returns>
    private float GetAIRequestedWheelBrake()
    {
        return 0f;
    }

    /// <summary>
    /// Update UI display text, including rule-based AI toggling indicator
    /// </summary>
    protected override void UpdateDisplay()
    {
        base.UpdateDisplay();

        if (_altitudeGroundText != null)
        {
            if (_altitudeAboveGround < Mathf.Infinity)
            {
                float altitude = (float)Math.Round(_altitudeAboveGround, 1);
                _altitudeGroundText.text = $"Altitude Above Ground: {altitude} m";
            }
            else
            {
                _altitudeGroundText.text = $"Altitude Above Ground: N/A";
            }
        }

        if (_autonomousText != null && _isAIActivated)
        {
            _autonomousText.color = Color.white;
        }
        else
        {
            _autonomousText.color = Color.gray;
        }

        if (_stateText != null)
        {
            _stateText.text = State.ToString();
        }
    }

    /// <summary>
    /// Gets all texts from canvas, including the autonomy text
    /// </summary>
    protected override void InitializeTextDisplays()
    {
        base.InitializeTextDisplays();

        if (UICanvas != null)
        {
            Transform aircraftPanel = UICanvas.transform.Find("Aircraft Panel").transform;
            _altitudeGroundText = aircraftPanel.transform.Find("AltitudeGroundText").GetComponent<TextMeshProUGUI>();
            _autonomousText = aircraftPanel.transform.Find("AutonomousText").GetComponent<TextMeshProUGUI>();
            _stateText = aircraftPanel.transform.Find("StateText").GetComponent<TextMeshProUGUI>();
        }
    }

    /// <summary>
    /// Gets the roll input for the fly-by-wire system, with the bank limiter/stabilizer
    /// </summary>
    /// <returns>The roll input with bank limiting</returns>
    protected override float GetRollInput()
    {
        float bankAngle = GetBankAngle();
        float maxBankAngle = GetMaxBankAngle();

        float bankLimitedInput = LimitBank(_initialRollInput, bankAngle, maxBankAngle);
        float bankCorrectedInput = Mathf.Clamp(bankLimitedInput + GetBankCorrection(bankAngle, maxBankAngle), -1f, 1f);
        float smoothedInput = Smoother(bankCorrectedInput, _previousRollInput, RollSmoothingStrength);

        _previousRollInput = smoothedInput;

        return smoothedInput;
    }

    /// <summary>
    /// Gets the maximum bank/roll angle, based on altitude above ground
    /// </summary>
    /// <returns>The max bank angle</returns>
    public float GetMaxBankAngle()
    {
        float strength = 1f - Mathf.Clamp01(_altitudeAboveGround / BankLimitAltitudeCap);
        float maxBankAngle = MaxBankLimit - strength * (MaxBankLimit - MinBankLimit);

        return maxBankAngle;
    }

    /// <summary>
    /// Limits the input, based on a desired maximum banking angle
    /// </summary>
    /// <param name="input">The roll input to be limited</param>
    /// <param name="bankAngle">The bank/roll angle</param>
    /// <param name="maxBankAngle">The maximum bank/roll angle</param>
    /// <returns>The roll input after being limited based on bank angle</returns>
    private float LimitBank(float input, float bankAngle, float maxBankAngle)
    {
        float modifier = 1f;

        if (input > 0f && bankAngle > 0f)
        {
            modifier = 1f - Mathf.Clamp01(Mathf.Log10(bankAngle / maxBankAngle + 1f) * BankLimiterStrength);
        }
        else if (input < 0f && bankAngle < 0f)
        {
            modifier = 1f - Mathf.Clamp01(Mathf.Log10(-bankAngle / maxBankAngle + 1f) * BankLimiterStrength);
        }

        return input * modifier;
    }

    /// <summary>
    /// Gets the bank correction input
    /// </summary>
    /// <param name="bankAngle">The bank/roll angle</param>
    /// <param name="maxBankAngle">The maximum bank/roll angle</param>
    /// <returns>The roll correction input addition</returns>
    private float GetBankCorrection(float bankAngle, float maxBankAngle)
    {
        float bankError = Mathf.Abs(bankAngle) - maxBankAngle;
        float bankCorrectionInput = 0f;

        if (bankError > 0f)
        {
            float correctionStrength = bankError / (180f - maxBankAngle);
            float bankCorrection = correctionStrength * BankCorrectionModifier;

            bankCorrectionInput = Mathf.Sign(bankAngle) * Mathf.Clamp(bankCorrection, 0f, MaxCorrectionInput);
        }

        Debug.Log($"Bank Angle: {Math.Round(bankAngle, 1)}° | Input Correction: {Math.Round(bankCorrectionInput, 3)} | Initial Input: {Math.Round(_initialRollInput, 3)} | Input: {Math.Round(_rollInput, 3)}");

        return bankCorrectionInput;
    }

    /// <summary>
    /// Gets the bank/roll angle of the aircraft
    /// </summary>
    /// <returns>The bank angle</returns>
    private float GetBankAngle()
    {
        float bankAngle = transform.eulerAngles.z;

        if (bankAngle > 180f)
        {
            bankAngle -= 360f;
        }

        return bankAngle;
    }
}
