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

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    /// <summary>
    /// Spawn a new waypoint
    /// </summary>
    private void SpawnWaypoint()
    {
        float height = Random.Range(MinHeight, MaxHeight);
        Vector2 circle = Random.insideUnitCircle * CircleRadius;
        Vector3 position = new Vector3(circle.x, height, circle.y);

        Instantiate(WaypointPrefab, position, Quaternion.identity);
    }
}
