using UnityEngine;
using System.Collections;

public class PredatorAlertTile : MonoBehaviour
{
    private Sector _sector = null;
    private bool _triggered = false;

    public int _radius = 10;
    public AudioSource _audioSource = null;
    public AudioClip _alertSound = null;
    public float _alertVolume = 0.5f;

    void Start()
    {
        _sector = GameObject.FindObjectOfType<Sector>();
        _triggered = false;

        if (!_audioSource)
        {
            _audioSource = GetComponent<AudioSource>();
        }
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

            var target = GetComponent<Tile>();
            var predators = GameObject.FindObjectsOfType<Predator>();
            foreach (var p in predators)
            {
                p.RushTowards(target);
            }

            if (_audioSource != null && _alertSound != null)
            {
                _audioSource.PlayOneShot(_alertSound, _alertVolume);
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
