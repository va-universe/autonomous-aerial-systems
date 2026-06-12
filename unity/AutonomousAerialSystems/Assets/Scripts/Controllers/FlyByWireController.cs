using System;
using TMPro;
using UnityEngine;

/// <summary>
/// The controller of the fly-by-wire prototype
/// </summary>
public class FlyByWireController : Controller
{
    #region Inputs
    private float _rollInput;
    private float _pitchInput;
    private float _yawInput;
    private float _flapInput;
    private float _throttleInput;
    #endregion

    [Header("UI Display")]
    public Canvas UICanvas;

    #region UI Display Text
    private TextMeshProUGUI _speedText;
    private TextMeshProUGUI _altitudeText;
    private TextMeshProUGUI _stallingText;
    private TextMeshProUGUI _gForceText;

    private TextMeshProUGUI _leftAileronText;
    private TextMeshProUGUI _rightAileronText;
    private TextMeshProUGUI _leftFlapText;
    private TextMeshProUGUI _rightFlapText;
    private TextMeshProUGUI _leftElevatorText;
    private TextMeshProUGUI _rightElevatorText;
    private TextMeshProUGUI _rudderText;

    private int _numWingText;
    #endregion

    protected override void Start()
    {
        base.Start();

        InitializeTextDisplays();
    }

    void Update()
    {
        GetInput();
        UpdateText();
    }

    void FixedUpdate()
    {
        DeflectControlSurfaces();
        ApplyThrust(_throttleInput);

        CalculateGForce();
    }

    /// <summary>
    /// Gets all texts from canvas
    /// </summary>
    private void InitializeTextDisplays()
    {
        Transform aircraftPanel = UICanvas.transform.Find("Aircraft Panel").transform;
        Transform wingPanel = UICanvas.transform.Find("Wing Panel").transform;

        _speedText = aircraftPanel.transform.Find("SpeedText").GetComponent<TextMeshProUGUI>();
        _altitudeText = aircraftPanel.transform.Find("AltitudeText").GetComponent<TextMeshProUGUI>();
        _stallingText = aircraftPanel.transform.Find("StallingText").GetComponent<TextMeshProUGUI>();
        _gForceText = aircraftPanel.transform.Find("GForceText").GetComponent<TextMeshProUGUI>();

        _leftAileronText = wingPanel.Find("LeftAileronText").GetComponent<TextMeshProUGUI>();
        _rightAileronText = wingPanel.Find("RightAileronText").GetComponent<TextMeshProUGUI>();
        _leftFlapText = wingPanel.Find("LeftFlapText").GetComponent<TextMeshProUGUI>();
        _rightFlapText = wingPanel.Find("RightFlapText").GetComponent<TextMeshProUGUI>();
        _leftElevatorText = wingPanel.Find("LeftElevatorText").GetComponent<TextMeshProUGUI>();
        _rightElevatorText = wingPanel.Find("RightElevatorText").GetComponent<TextMeshProUGUI>();
        _rudderText = wingPanel.Find("RudderText").GetComponent<TextMeshProUGUI>();
    }

    /// <summary>
    /// Get the roll, pitch, yaw, flap and thrust input from the input action system
    /// </summary>
    private void GetInput()
    {
        _rollInput = _inputActions.AircraftWithFlaps.Roll.ReadValue<float>();
        _pitchInput = _inputActions.AircraftWithFlaps.Pitch.ReadValue<float>();
        _yawInput = _inputActions.AircraftWithFlaps.Yaw.ReadValue<float>();

        _flapInput = _inputActions.AircraftWithFlaps.Flap.ReadValue<float>();
        _throttleInput = _inputActions.AircraftWithFlaps.Thrust.ReadValue<float>();

        WheelBrakeInput = _inputActions.AircraftWithFlaps.WheelBrake.ReadValue<float>();
    }

    /// <summary>
    /// Deflect all control surfaces visually and mathematically
    /// </summary>
    private void DeflectControlSurfaces()
    {
        UpdateWingSurfaceData();
        VisualizeControlSurfaces();
    }

    /// <summary>
    /// Update the deflection angles in the wing surfaces
    /// </summary>
    private void UpdateWingSurfaceData()
    {
        _leftAileronParent.ControlSurfaceDeflection = _leftAileron.MaxDeflection * _rollInput;
        _rightAileronParent.ControlSurfaceDeflection = _rightAileron.MaxDeflection * -_rollInput;

        _leftFlapParent.ControlSurfaceDeflection = _leftFlap.MaxDeflection * _flapInput;
        _rightFlapParent.ControlSurfaceDeflection = _rightFlap.MaxDeflection * _flapInput;

        _leftElevatorParent.ControlSurfaceDeflection = _leftElevator.MaxDeflection * _pitchInput;
        _rightElevatorParent.ControlSurfaceDeflection = _rightElevator.MaxDeflection * _pitchInput;

        _rudderParent.ControlSurfaceDeflection = _rudder.MaxDeflection * _yawInput;
    }

    /// <summary>
    /// Visualize control surface deflection
    /// </summary>
    private void VisualizeControlSurfaces()
    {
        _leftAileron.DeflectSurface(_rollInput);
        _rightAileron.DeflectSurface(-_rollInput);

        _leftFlap.DeflectSurface(_flapInput);
        _rightFlap.DeflectSurface(_flapInput);

        _leftElevator.DeflectSurface(_pitchInput);
        _rightElevator.DeflectSurface(_pitchInput);

        _rudder.DeflectSurface(_yawInput);
    }

    /// <summary>
    /// Update UI display text
    /// </summary>
    private void UpdateText()
    {
        if (_speedText != null)
        {
            float speed = (float)Math.Round(_rb.linearVelocity.magnitude, 1);
            _speedText.text = $"Speed: {speed} m/s";
        }
        if (_altitudeText != null)
        {
            float altitude = Mathf.Round(transform.position.y);
            _altitudeText.text = $"Altitude: {altitude} m";
        }
        if (_stallingText != null)
        {
            float numTextModifier = _numWingText / 7f;
            if (_totalStall > StallTextThreshold * numTextModifier)
            {
                float stall = Mathf.Clamp01(_totalStall / (6f * numTextModifier * StallTextRedness));

                _stallingText.text = "STALLING";
                _stallingText.color = Color.Lerp(Color.white, Color.red, stall);
            }
            else
            {
                _stallingText.text = "";
            }
        }
        if (_gForceText != null)
        {
            float gForce = Mathf.Round(GForce);
            _gForceText.text = $"{gForce} G";

            float gForceStrength = 0f;
            if (GForce >= 0)
            {
                gForceStrength = Mathf.Clamp01((GForce - 1f) / 8f);
            }
            else
            {
                gForceStrength = Mathf.Clamp01(GForce / -4f);
            }

            _gForceText.color = Color.Lerp(Color.white, Color.red, gForceStrength);
        }

        _totalStall = 0f;
        _numWingText = 0;

        if (_leftAileronText != null)
        {
            WingSurface surface = _leftAileronParent;
            float lift = (float)Math.Round(surface.LiftData, 1);
            float drag = (float)Math.Round(surface.DragData, 1);
            float stall = Mathf.Round(surface.StallData);
            float angleOfAttack = (float)Math.Round(surface.AoAData, 1);

            _leftAileronText.text = $"Left Aileron | Lift: {lift} kN | Drag: {drag} kN | AoA: {angleOfAttack}° | Stall: {stall}%";
            _leftAileronText.color = Color.Lerp(Color.white, Color.red, stall / StallTextRedness);

            _totalStall += stall;
            _numWingText += 1;
        }
        if (_leftFlapText != null)
        {
            WingSurface surface = _leftFlapParent;
            float lift = (float)Math.Round(surface.LiftData, 1);
            float drag = (float)Math.Round(surface.DragData, 1);
            float stall = Mathf.Round(surface.StallData);
            float angleOfAttack = (float)Math.Round(surface.AoAData, 1);

            _leftFlapText.text = $"Left Flap | Lift: {lift} kN | Drag: {drag} kN | AoA: {angleOfAttack}° | Stall: {stall}%";
            _leftFlapText.color = Color.Lerp(Color.white, Color.red, stall / StallTextRedness);

            _totalStall += stall;
            _numWingText += 1;
        }
        if (_leftElevatorText != null)
        {
            WingSurface surface = _leftElevatorParent;
            float lift = (float)Math.Round(surface.LiftData, 1);
            float drag = (float)Math.Round(surface.DragData, 1);
            float stall = Mathf.Round(surface.StallData);
            float angleOfAttack = (float)Math.Round(surface.AoAData, 1);

            _leftElevatorText.text = $"Left Elevator | Lift: {lift} kN | Drag: {drag} kN | AoA: {angleOfAttack}° | Stall: {stall}%";
            _leftElevatorText.color = Color.Lerp(Color.white, Color.red, stall / StallTextRedness);

            _totalStall += stall;
            _numWingText += 1;
        }

        if (_rudderText != null)
        {
            WingSurface surface = _rudderParent;
            float lift = (float)Math.Round(surface.LiftData, 1);
            float drag = (float)Math.Round(surface.DragData, 1);
            float stall = Mathf.Round(surface.StallData);
            float angleOfAttack = (float)Math.Round(surface.AoAData, 1);

            _rudderText.text = $"Rudder | Lift: {lift} kN | Drag: {drag} kN | AoA: {angleOfAttack}° | Stall: {stall}%";
            _rudderText.color = Color.Lerp(Color.white, Color.red, stall / StallTextRedness);

            _totalStall += stall;
            _numWingText += 1;
        }

        if (_rightAileronText != null)
        {
            WingSurface surface = _rightAileronParent;
            float lift = (float)Math.Round(surface.LiftData, 1);
            float drag = (float)Math.Round(surface.DragData, 1);
            float stall = Mathf.Round(surface.StallData);
            float angleOfAttack = (float)Math.Round(surface.AoAData, 1);

            _rightAileronText.text = $"Stall: {stall}% | AoA: {angleOfAttack}° | Drag: {drag} kN | Lift: {lift} kN | Right Aileron";
            _rightAileronText.color = Color.Lerp(Color.white, Color.red, stall / StallTextRedness);

            _totalStall += stall;
            _numWingText += 1;
        }
        if (_rightFlapText != null)
        {
            WingSurface surface = _rightFlapParent;
            float lift = (float)Math.Round(surface.LiftData, 1);
            float drag = (float)Math.Round(surface.DragData, 1);
            float stall = Mathf.Round(surface.StallData);
            float angleOfAttack = (float)Math.Round(surface.AoAData, 1);

            _rightFlapText.text = $"Stall: {stall}% | AoA: {angleOfAttack}° | Drag: {drag} kN | Lift: {lift} kN | Right Flap";
            _rightFlapText.color = Color.Lerp(Color.white, Color.red, stall / StallTextRedness);

            _totalStall += stall;
            _numWingText += 1;
        }
        if (_rightElevatorText != null)
        {
            WingSurface surface = _rightElevatorParent;
            float lift = (float)Math.Round(surface.LiftData, 1);
            float drag = (float)Math.Round(surface.DragData, 1);
            float stall = Mathf.Round(surface.StallData);
            float angleOfAttack = (float)Math.Round(surface.AoAData, 1);

            _rightElevatorText.text = $"Stall: {stall}% | AoA: {angleOfAttack}° | Drag: {drag} kN | Lift: {lift} kN | Right Elevator";
            _rightElevatorText.color = Color.Lerp(Color.white, Color.red, stall / StallTextRedness);

            _totalStall += stall;
            _numWingText += 1;
        }
    }
}