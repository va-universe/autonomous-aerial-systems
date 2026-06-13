using TMPro;
using UnityEngine;

/// <summary>
/// The controller of the second aircraft prototype
/// </summary>
public class SecondAircraftController : Controller
{
    [Header("UI Display")]

    public TextMeshProUGUI SpeedText;
    public TextMeshProUGUI AltitudeText;
    public TextMeshProUGUI StallingText;
    public TextMeshProUGUI GForceText;

    public TextMeshProUGUI LeftAileronText;
    public TextMeshProUGUI RightAileronText;
    public TextMeshProUGUI LeftFlapText;
    public TextMeshProUGUI RightFlapText;
    public TextMeshProUGUI LeftElevatorText;
    public TextMeshProUGUI RightElevatorText;
    public TextMeshProUGUI RudderText;

    protected override void Start()
    {
        base.Start();

        _rb.centerOfMass = _com.position;
    }

    /// <summary>
    /// Initializes all texts directly
    /// </summary>
    protected override void InitializeTextDisplays()
    {
        _speedText = SpeedText;
        _altitudeText = AltitudeText;
        _stallingText = StallingText;
        _gForceText = GForceText;

        _leftAileronText = LeftAileronText;
        _rightAileronText = RightAileronText;
        _leftFlapText = LeftFlapText;
        _rightFlapText = RightFlapText;
        _leftElevatorText = LeftElevatorText;
        _rightElevatorText = RightElevatorText;
        _rudderText = RudderText;
    }

    /// <summary>
    /// Apply thrust force at the center of mass
    /// </summary>
    protected override void ApplyThrust()
    {
        Vector3 centerOfMass = _com.position;
        Vector3 thrustForce = transform.forward * Thrust * _throttleInput;
        _rb.AddForceAtPosition(thrustForce, centerOfMass, ForceMode.Force);
    }
}
