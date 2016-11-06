using UnityEngine;
using System.Linq;

public class Tile : MonoBehaviour {

    // Game State
    public int _predatorOccupancy = 0;

    // Designer
    public float _width = 1.0f;
    public float _height = 1.0f;
    public AdjacenctRoom[] _adjacentRooms = new AdjacenctRoom[0];

    [System.Serializable]
    public class AdjacenctRoom {
        public GameObject door;
        public GameObject room;
    }
    
    void Start () {
        _predatorOccupancy = 0;
    }

    void OnMouseDown()
    {
        var sector = GameObject.FindObjectOfType<Sector>();
        var player = GameObject.FindObjectOfType<Player>();

        player._originTile = this;
        player._targetTile = sector.GetClosestTile(player.transform.position);
        player._path = sector.FindShortestPath(this, player._targetTile, 100);

        Debug.Log("down");
    }

    void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case Player.PREDATOR_TAG:
                ++_predatorOccupancy;
                break;
            case Player.PLAYER_TAG:
                --_predatorOccupancy;
                break;
        }
    }

    void OnDrawGizmos()
    {
        var roomCenter = transform.position;
        Gizmos.matrix = Matrix4x4.TRS(
            roomCenter,
            transform.localRotation,
            Vector3.one
        );

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(
            Vector3.zero, 
            new Vector3(_width, _height, 0.5f + 0.5f * _predatorOccupancy)
        );
        //Gizmos.DrawSphere(Vector3.zero, 0.2f);

        /*
        Gizmos.color = Color.blue;
        foreach (var adj in _adjacentRooms)
        {
            if (adj.room != null)
            {
                Gizmos.DrawLine(Vector3.zero, adj.room.transform.position - roomCenter);
            }
        }
        */
    }
}
