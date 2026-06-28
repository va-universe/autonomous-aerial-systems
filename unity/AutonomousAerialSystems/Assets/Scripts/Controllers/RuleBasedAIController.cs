using TMPro;
using UnityEngine;

/// <summary>
/// The controller of the rule-based AI prototype
/// </summary>
public class RuleBasedAIController : FlyByWireController
{
    #region Inputs & UI display texts

    private bool _isAIActivated;
    private TextMeshProUGUI _autonomousText;

    #endregion

    [Header("Rule-Based AI Systems")]
    public bool IsTakingOff;

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

        if (IsTakingOff)
        {
            requestedInput = -1f;
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

        if (IsTakingOff)
        {
            requestedInput = 1f;
        }

        return requestedInput;
    }

    /// <summary>
    /// Gets the requested throttle/thrust input from the rule-based AI
    /// </summary>
    /// <returns>The requested throttle/thrust input</returns>
    private float GetAIRequestedThrottle()
    {
        return 1f;
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
            _autonomousText = aircraftPanel.transform.Find("AutonomousText").GetComponent<TextMeshProUGUI>();
        }
    }
}
