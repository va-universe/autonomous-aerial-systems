using UnityEngine;

/// <summary>
/// The unified base for controllers using the wing-camber-based system
/// </summary>
public class Controller : MonoBehaviour
{
    protected const float GRAVITY = 9.81f;

    protected Rigidbody _rb;
    protected Transform _com; //Center of mass
    protected AircraftInputActions _inputActions;

    #region Control Surfaces
    protected ControlSurface _leftAileron;
    protected ControlSurface _rightAileron;
    protected ControlSurface _leftFlap;
    protected ControlSurface _rightFlap;
    protected ControlSurface _leftElevator;
    protected ControlSurface _rightElevator;
    protected ControlSurface _rudder;
    #endregion

    #region Wing Surfaces
    protected WingSurface _leftAileronParent;
    protected WingSurface _rightAileronParent;
    protected WingSurface _leftFlapParent;
    protected WingSurface _rightFlapParent;
    protected WingSurface _leftElevatorParent;
    protected WingSurface _rightElevatorParent;
    protected WingSurface _rudderParent;
    #endregion

    [Header("General Parameters")]
    public float WheelBrakeInput;
    public float AirDensityAtSeaLevel;
    public float Thrust;

    [Header("Stall Text")]
    public float StallTextThreshold;
    public float StallTextRedness;
    protected float _totalStall;

    [Header("G-Force")]
    public float GForce;
    protected Vector3 _previousVelocity;

    protected void Awake()
    {
        _inputActions = new AircraftInputActions();
    }

    protected virtual void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _com = transform.Find("Aerodynamics").Find("CenterOfMass").transform;

        _previousVelocity = _rb.linearVelocity;
        _rb.centerOfMass = _com.localPosition;

        InitializeControlSurfaces();
        InitializeWingSurfaces();
    }

    protected void OnEnable()
    {
        _inputActions.Enable();
    }

    /// <summary>
    /// Apply thrust force at the center of mass
    /// </summary>
    protected virtual void ApplyThrust(float throttleInput)
    {
        Vector3 centerOfMass = transform.position + _rb.centerOfMass;
        Vector3 thrustForce = transform.forward * Thrust * throttleInput;
        _rb.AddForceAtPosition(thrustForce, centerOfMass, ForceMode.Force);
    }

    /// <summary>
    /// Calculate the current G Force 
    /// </summary>
    protected void CalculateGForce()
    {
        Vector3 acceleration = (_rb.linearVelocity - _previousVelocity) / Time.fixedDeltaTime;
        Vector3 accelerationWithoutGravity = acceleration - Physics.gravity;

        GForce = Vector3.Dot(accelerationWithoutGravity, transform.up) / GRAVITY;
        _previousVelocity = _rb.linearVelocity;
    }

    /// <summary>
    /// Gets all control surfaces from prefab
    /// </summary>
    protected void InitializeControlSurfaces()
    {
        Transform pivots = transform.Find("Visual Components").Find("Pivots").transform;

        _leftAileron = pivots.Find("Left Aileron Pivot").GetComponent<ControlSurface>();
        _rightAileron = pivots.Find("Right Aileron Pivot").GetComponent<ControlSurface>();
        _leftFlap = pivots.Find("Left Flap Pivot").GetComponent<ControlSurface>();
        _rightFlap = pivots.Find("Right Flap Pivot").GetComponent<ControlSurface>();
        _leftElevator = pivots.Find("Left Elevator Pivot").GetComponent<ControlSurface>();
        _rightElevator = pivots.Find("Right Elevator Pivot").GetComponent<ControlSurface>();
        _rudder = pivots.Find("Rudder Pivot").GetComponent<ControlSurface>();
    }

    /// <summary>
    /// Gets all wing surfaces from prefab
    /// </summary>
    protected void InitializeWingSurfaces()
    {
        Transform wingSurfaces = transform.Find("Aerodynamics").Find("Wing Surfaces").transform;

        _leftAileronParent = wingSurfaces.Find("Left Aileron Parent").GetComponent<WingSurface>();
        _rightAileronParent = wingSurfaces.Find("Right Aileron Parent").GetComponent<WingSurface>();
        _leftFlapParent = wingSurfaces.Find("Left Flap Parent").GetComponent<WingSurface>();
        _rightFlapParent = wingSurfaces.Find("Right Flap Parent").GetComponent<WingSurface>();
        _leftElevatorParent = wingSurfaces.Find("Left Elevator Parent").GetComponent<WingSurface>();
        _rightElevatorParent = wingSurfaces.Find("Right Elevator Parent").GetComponent<WingSurface>();
        _rudderParent = wingSurfaces.Find("Rudder Parent").GetComponent<WingSurface>();
    }
}
