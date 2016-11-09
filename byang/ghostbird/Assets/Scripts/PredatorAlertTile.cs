using UnityEngine;
using System.Collections;

public class PredatorAlertTile : MonoBehaviour
{
    private Sector _sector = null;
    private bool _triggered = false;

    public int _radius = 10;

    void Start()
    {
        _sector = GameObject.FindObjectOfType<Sector>();
        _triggered = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!_triggered && string.Equals(other.tag, Player.PLAYER_TAG))
        {
            _triggered = true;
            var triggerPos = transform.position;
            foreach (var t in _sector._tiles)
            {
                Vector3 tp = t.transform.position;
                float distance =
                    Mathf.Abs(tp.x - triggerPos.x) / Sector.TILE_SIZE +
                    Mathf.Abs(tp.z - triggerPos.z) / Sector.TILE_SIZE;
                t._predatorOccupancy += Mathf.Min(0, ((int)Mathf.Floor(distance) - _radius) / 2);
            }

            var predators = GameObject.FindObjectsOfType<Predator>();
            foreach (var p in predators)
            {
                p.ForgetPath();
            }
        }
    }

    void OnDrawGizmos()
    {
        if (!_triggered)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position, 0.4f);
        }
    }
}
