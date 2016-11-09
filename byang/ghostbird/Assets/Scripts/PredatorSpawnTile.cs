using UnityEngine;
using System.Collections;

public class PredatorSpawnTile : MonoBehaviour {

    private bool _triggered = false;
    public GameObject _predatorPrefab = null;
    public Tile[] _spawnTiles = new Tile[0];
    
    void Start () {
        _triggered = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!_triggered && string.Equals(other.tag, Player.PLAYER_TAG))
        {
            _triggered = true;
            foreach (var t in _spawnTiles)
            {
                GameObject.Instantiate(_predatorPrefab, t.transform.position, Quaternion.identity);
            }
        }
    }
}
