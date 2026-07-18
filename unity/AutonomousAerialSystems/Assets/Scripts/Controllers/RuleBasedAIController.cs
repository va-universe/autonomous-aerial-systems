using System;
using TMPro;
using UnityEngine;

/// <summary>
/// The controller of the rule-based AI prototype
/// </summary>
public class RuleBasedAIController : FlyByWireController
{
    private bool _isAIActivated;

    #region UI display texts
    
    private TextMeshProUGUI _altitudeGroundText;
    private TextMeshProUGUI _upcomingAltitudeText;
    private TextMeshProUGUI _autonomousText;
    private TextMeshProUGUI _stateText;

    #endregion

    [Header("Bank Limiter")]
    public float BankLimiterStrength;
    public float BankCorrectionModifier;
    public float MaxBankCorrectionInput;

    public float MaxBankLimit;
    public float MinBankLimit;
    public float BankLimitEndAltitude;

    [Header("Pitch Limiter")]
    public float PitchLimiterStrength;
    public float PitchCorrectionModifier;
    public float MaxPitchCorrectionInput;

    public float MaxPitchLimit;
    public float MinPitchLimit;
    public float PitchLimitStartAltitude;
    public float PitchLimitEndAltitude;

    [Header("Flap Increase")]
    public float FlapIncreaseStartAltitude;
    public float FlapIncreaseEndAltitude;
    public float MaxFlapInputIncrease;

    [Header("Rule-Based AI")]
    public AircraftState State;

    [Header("Takeoff")]
    public float EndTakeoffAltitude;
    public float PitchDownInput;
    public float PitchTransitionRate;
    public float FlapTransitionRate;
    private float _pitchTransitionInput;
    private float _flapTransitionInput;
    private bool _isTakeoffComplete;

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
    private float _upcomingAltitude;

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
            if (!_isTakeoffComplete)
            {
                _isTakeoffComplete = _altitudeAboveGround >= EndTakeoffAltitude;
            }            

            if (_isTakeoffComplete)
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
            _isTakeoffComplete = false;
            State = AircraftState.Grounded;
        }
    }

    /// <summary>
    /// Run all sensor systems
    /// </summary>
    protected virtual void RunSensors()
    {
        _altitudeAboveGround = GetAltitudeAboveGround();
        _upcomingAltitude = GetUpcomingAltitude();
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
    /// Gets the upcoming altitude above ground, seen from a 45 degree down and forwards angled sensor
    /// </summary>
    /// <returns>The upcoming altitude above ground</returns>
    private float GetUpcomingAltitude()
    {
        float distanceToGround = Mathf.Infinity;
        Vector3 direction = (Vector3.down + Vector3.forward).normalized; //45 degrees forward and down

        RaycastHit[] hits = Physics.RaycastAll(transform.position, direction, GroundSensorRange);
        foreach (RaycastHit hit in hits)
        {
            if (hit.transform.CompareTag("Ground") && hit.distance < distanceToGround)
            {
                distanceToGround = hit.distance;
            }
        }

        float cosine = 0.70710678118f; //cos(45)
        float altitudeAboveGround = cosine * distanceToGround;

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
            //Waypoint Tracking
            Vector3 localWaypoint = transform.InverseTransformPoint(_currentWaypoint.transform.position + new Vector3(0f, TrackingOffset, 0f));
            Vector3 direction = localWaypoint.normalized;
            float trackingInput = Mathf.Clamp(direction.x * RollTrackingStregnth, -1f, 1f);

            //Bank Limiting & Correction
            float bankAngle = GetAngle(AircraftAxis.Roll);
            float maxBankAngle = GetMaxBankAngle();
            float bankLimitedInput = LimitBank(trackingInput, bankAngle, maxBankAngle);
            float bankCorrectedInput = Mathf.Clamp(bankLimitedInput + GetBankCorrection(bankAngle, maxBankAngle), -1f, 1f);

            requestedInput = bankCorrectedInput;
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
            //Waypoint Tracking
            Vector3 localWaypoint = transform.InverseTransformPoint(_currentWaypoint.transform.position + new Vector3(0f, TrackingOffset, 0f));
            Vector3 direction = localWaypoint.normalized;
            float trackingInput = Mathf.Clamp(direction.y * -PitchTrackingStrength, -1f, 1f);

            //Pitch Limiting & Correction
            float pitchAngle = GetAngle(AircraftAxis.Pitch);
            float minPitchAngle = GetMinPitchAngle();
            float pitchLimitedInput = LimitPitchDown(trackingInput, pitchAngle, minPitchAngle);
            float pitchCorrectedInput = Mathf.Clamp(pitchLimitedInput + GetPitchCorrection(pitchAngle, minPitchAngle), -1f, 1f);

            requestedInput = pitchCorrectedInput;
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
            //Waypoint Tracking
            Vector3 localWaypoint = transform.InverseTransformPoint(_currentWaypoint.transform.position + new Vector3(0f, TrackingOffset, 0f));
            Vector3 direction = localWaypoint.normalized;
            float trackingInput = Mathf.Clamp(direction.y * FlapTrackingStrength, -1f, 1f);

            //Ground Evasion Flap
            requestedInput = Mathf.Clamp(trackingInput + GetGroundEvasionFlap(), -1f, 1f);
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
        if (_upcomingAltitudeText != null)
        {
            if (_upcomingAltitude < Mathf.Infinity)
            {
                float altitude = (float)Math.Round(_upcomingAltitude, 1);
                _upcomingAltitudeText.text = $"Upcoming Altitude (AAG): {altitude} m";
            }
            else
            {
                _upcomingAltitudeText.text = $"Upcoming Altitude (AAG): N/A";
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
            _upcomingAltitudeText = aircraftPanel.transform.Find("UpcomingAltitudeText").GetComponent<TextMeshProUGUI>();
            _autonomousText = aircraftPanel.transform.Find("AutonomousText").GetComponent<TextMeshProUGUI>();
            _stateText = aircraftPanel.transform.Find("StateText").GetComponent<TextMeshProUGUI>();
        }
    }

    /// <summary>
    /// Gets the flap increase from being close to the ground
    /// </summary>
    /// <returns>The flap increase input addition</returns>
    private float GetGroundEvasionFlap()
    {
        float altitude = Mathf.Min(_altitudeAboveGround, _upcomingAltitude);
        float strength = 1f - Mathf.Clamp01((altitude - FlapIncreaseStartAltitude) / (FlapIncreaseEndAltitude - FlapIncreaseStartAltitude));
        float flapIncrease = strength * MaxFlapInputIncrease;

        return flapIncrease;
    }

    /// <summary>
    /// Limits the pitch down input, based on a desired minimum pitch angle
    /// </summary>
    /// <param name="pitchInput">The pitch input to be limited</param>
    /// <param name="pitchAngle">The pitch angle</param>
    /// <param name="minPitchAngle">The minimum pitch angle</param>
    /// <returns>The pitch input after being limited based on the pitch angle</returns>
    private float LimitPitchDown(float pitchInput, float pitchAngle, float minPitchAngle)
    {
        float modifier = 1f;

        if (pitchInput > 0f && pitchAngle < 0f)
        {
            modifier = 1f - Mathf.Clamp01(Mathf.Log10(pitchAngle / minPitchAngle + 1f) * PitchLimiterStrength);
        }

        return pitchInput * modifier;
    }

    /// <summary>
    /// Gets the pitch up correction input
    /// </summary>
    /// <param name="pitchAngle">The pitch angle</param>
    /// <param name="minPitchAngle">The minimum pitch angle</param>
    /// <returns>The pitch up correction input addition</returns>
    private float GetPitchCorrection(float pitchAngle, float minPitchAngle)
    {
        float pitchCorrectionInput = 0f;

        if (pitchAngle <= minPitchAngle)
        {
            float pitchError = Mathf.Abs(pitchAngle - minPitchAngle);

            float correctionStrength = pitchError / (180.1f - minPitchAngle);
            float pitchCorrection = correctionStrength * PitchCorrectionModifier;

            pitchCorrectionInput = -Mathf.Clamp(pitchCorrection, 0f, MaxPitchCorrectionInput);
        }

        Debug.Log($"Pitch Angle: {Math.Round(pitchAngle, 1)}° | Min Pitch Angle: {Math.Round(minPitchAngle, 1)}° | Input Correction: {Math.Round(pitchCorrectionInput, 3)} | Initial Input: {Math.Round(_initialPitchInput, 3)} | Input: {Math.Round(_pitchInput, 3)}");

        return pitchCorrectionInput;
    }


    /// <summary>
    /// Gets the minimum pitch angle, based on altitude above ground
    /// </summary>
    /// <returns>The minimum pitch angle</returns>
    public float GetMinPitchAngle()
    {
        float altitude = Mathf.Min(_altitudeAboveGround, _upcomingAltitude);
        float strength = 1f - Mathf.Clamp01((altitude - PitchLimitStartAltitude) / (PitchLimitEndAltitude - PitchLimitStartAltitude));
        float minPitchAngle = MinPitchLimit - strength * (MinPitchLimit - MaxPitchLimit);

        return minPitchAngle;
    }

    /// <summary>
    /// Gets the maximum bank/roll angle, based on altitude above ground
    /// </summary>
    /// <returns>The maximum bank angle</returns>
    public float GetMaxBankAngle()
    {
        float altitude = Mathf.Min(_altitudeAboveGround, _upcomingAltitude);
        float strength = 1f - Mathf.Clamp01(altitude / BankLimitEndAltitude);
        float maxBankAngle = MaxBankLimit - strength * (MaxBankLimit - MinBankLimit);

        return maxBankAngle;
    }

    /// <summary>
    /// Limits the roll input, based on a desired maximum banking angle
    /// </summary>
    /// <param name="rollInput">The roll input to be limited</param>
    /// <param name="bankAngle">The bank angle</param>
    /// <param name="maxBankAngle">The maximum bank angle</param>
    /// <returns>The roll input after being limited based on the bank angle</returns>
    private float LimitBank(float rollInput, float bankAngle, float maxBankAngle)
    {
        float modifier = 1f;

        if (rollInput > 0f && bankAngle > 0f)
        {
            modifier = 1f - Mathf.Clamp01(Mathf.Log10(bankAngle / maxBankAngle + 1f) * BankLimiterStrength);
        }
        else if (rollInput < 0f && bankAngle < 0f)
        {
            modifier = 1f - Mathf.Clamp01(Mathf.Log10(-bankAngle / maxBankAngle + 1f) * BankLimiterStrength);
        }

        return rollInput * modifier;
    }

    /// <summary>
    /// Gets the bank correction input
    /// </summary>
    /// <param name="bankAngle">The bank angle</param>
    /// <param name="maxBankAngle">The maximum bank angle</param>
    /// <returns>The bank correction input addition</returns>
    private float GetBankCorrection(float bankAngle, float maxBankAngle)
    {
        float bankError = Mathf.Abs(bankAngle) - maxBankAngle;
        float bankCorrectionInput = 0f;

        if (bankError > 0f)
        {
            float correctionStrength = bankError / (180.1f - maxBankAngle);
            float bankCorrection = correctionStrength * BankCorrectionModifier;

            bankCorrectionInput = Mathf.Sign(bankAngle) * Mathf.Clamp(bankCorrection, 0f, MaxBankCorrectionInput);
        }

        //Debug.Log($"Bank Angle: {Math.Round(angle, 1)}° | Input Correction: {Math.Round(bankCorrectionInput, 3)} | Initial Input: {Math.Round(_initialRollInput, 3)} | Input: {Math.Round(_rollInput, 3)}");

        return bankCorrectionInput;
    }

    /// <summary>
    /// Gets the angle around a specific aircraft axis
    /// </summary>
    /// <param name="axis">The axis of the angle</param>
    /// <returns>The angle of the specific aircraft axis</returns>
    private float GetAngle(AircraftAxis axis)
    {
        float axisAngle = transform.eulerAngles.z; // Roll/Bank angle
        if (axis == AircraftAxis.Pitch)
        {
            axisAngle = transform.eulerAngles.x;
        }
        else if (axis == AircraftAxis.Yaw)
        {
            axisAngle = transform.eulerAngles.y;
        }

        if (axisAngle > 180f)
        {
            axisAngle -= 360f;
        }

        if (axis == AircraftAxis.Pitch)
        {
            axisAngle *= -1f;
        }

        return axisAngle;
    }
}
