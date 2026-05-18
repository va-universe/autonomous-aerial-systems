using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// The controller of the first aircraft prototype
/// </summary>
public class FirstAircraftController : MonoBehaviour
{
    private AircraftInputActions _inputActions;

    private float _rollInput;
    private float _pitchInput;
    private float _yawInput;

    [Header("Control Surfaces")]
    public ControlSurface LeftAileron;
    public ControlSurface RightAileron;
    public ControlSurface LeftElevator;
    public ControlSurface RightElevator;
    public ControlSurface Rudder;

    void Awake()
    {
        _inputActions = new AircraftInputActions();
    }

    void Update()
    {
        GetInput();
        DeflectControlSurfaces(_rollInput, _pitchInput, _yawInput);
    }

    void OnEnable()
    {
        _inputActions.Enable();
    }

    /// <summary>
    /// Get the roll, pitch and yaw input from the input action system
    /// </summary>
    private void GetInput()
    {
        _rollInput = _inputActions.Aircraft.Roll.ReadValue<float>();
        _pitchInput = _inputActions.Aircraft.Pitch.ReadValue<float>();
        _yawInput = _inputActions.Aircraft.Yaw.ReadValue<float>();

        //Debug.Log($"Roll: {_rollInput}, Pitch: {_pitchInput}, Yaw: {_yawInput}");
    }

    /// <summary>
    /// Deflect all control surfaces based on inputs
    /// </summary>
    /// <param name="roll">The deflection of the ailerons</param>
    /// <param name="pitch">The deflection of the elevators</param>
    /// <param name="yaw">The deflection of the rudder</param>
    private void DeflectControlSurfaces(float roll, float pitch, float yaw)
    {
        LeftAileron.DeflectSurface(roll);
        RightAileron.DeflectSurface(-roll);

        LeftElevator.DeflectSurface(pitch);
        RightElevator.DeflectSurface(pitch);

        Rudder.DeflectSurface(yaw);
    }
}
