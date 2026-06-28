using UnityEngine;

/// <summary>
/// The controller of the rule-based AI prototype
/// </summary>
public class RuleBasedAIController : FlyByWireController
{
    /// <summary>
    /// Gets the flight controls, thrust, braking, override and systems toggle inputs from the rule-based AI
    /// </summary>
    protected override void GetInput()
    {
        _initialRollInput = GetAIRequestedRoll();
        _initialPitchInput = GetAIRequestedPitch();
        _initialYawInput = GetAIRequestedYaw();
        _initialFlapInput = GetAIRequestedFlap();

        _overrideInput = false;
        _throttleInput = GetAIRequestedThrottle();
        WheelBrakeInput = 0f;

        ToggleSystemInputs();
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
        return 0f;
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
        return 0f;
    }

    /// <summary>
    /// Gets the requested throttle/thrust input from the rule-based AI
    /// </summary>
    /// <returns>The requested throttle/thrust input</returns>
    private float GetAIRequestedThrottle()
    {
        return 0f;
    }
}
