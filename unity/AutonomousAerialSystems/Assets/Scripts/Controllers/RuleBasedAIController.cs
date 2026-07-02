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

    #endregion

    [Header("Rule-Based AI")]
    public AircraftState State;

    [Header("Takeoff")]
    public float EndTakeoffAltitude;
    public float PitchDownInput;
    public float PitchTransitionRate;
    public float FlapTransitionRate;
    private float _pitchTransitionInput;
    private float _flapTransitionInput;

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
        base.FixedUpdate();

        RunSensors();
        HandleState();
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
                    State = AircraftState.Cruise;
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
            _overrideInput = false;
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
    /// Gets the requested roll input from the rule-based AI
    /// </summary>
    /// <returns>The requested roll input</returns>
    private float GetAIRequestedRoll()
    {
        return 0f;
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
        }
    }
}
