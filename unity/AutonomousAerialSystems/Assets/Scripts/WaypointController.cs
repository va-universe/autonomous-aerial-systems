using UnityEngine;

/// <summary>
/// The controller of the waypoint
/// </summary>
public class WaypointController : MonoBehaviour
{
    public bool IsReached;

    void Start()
    {
        IsReached = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.transform.CompareTag("Aircraft"))
        {
            IsReached = true;
        }
    }
}
