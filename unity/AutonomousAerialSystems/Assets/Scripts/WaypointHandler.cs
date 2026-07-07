using UnityEngine;

/// <summary>
/// Handler of the waypoint spawning for the Rule-Based AI Prototype testing
/// </summary>
public class WaypointHandler : MonoBehaviour
{
    [Header("Prefab")]
    public GameObject WaypointPrefab;

    [Header("Spawning Parameters")]
    public float CircleRadius;
    public float MinHeight;
    public float MaxHeight;

    [Header("Waypoint")]
    public WaypointController CurrentWaypoint;

    void Start()
    {
        CurrentWaypoint = SpawnWaypoint();
    }

    void FixedUpdate()
    {
        UpdateWaypoint();
    }

    /// <summary>
    /// Spawn new waypoints if previous waypoint was reached
    /// </summary>
    private void UpdateWaypoint()
    {
        if (CurrentWaypoint.IsReached)
        {
            WaypointController newWaypoint = SpawnWaypoint();

            Destroy(CurrentWaypoint.gameObject);
            CurrentWaypoint = newWaypoint;
        }
    }

    /// <summary>
    /// Spawn a new waypoint
    /// </summary>
    private WaypointController SpawnWaypoint()
    {
        float height = Random.Range(MinHeight, MaxHeight);
        Vector2 circle = Random.insideUnitCircle * CircleRadius;
        Vector3 position = new Vector3(circle.x, height, circle.y);

        GameObject waypointObj = Instantiate(WaypointPrefab, position, Quaternion.identity);
        WaypointController waypoint = waypointObj.GetComponent<WaypointController>();

        return waypoint;
    }
}
