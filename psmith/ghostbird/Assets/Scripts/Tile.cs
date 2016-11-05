using UnityEngine;
using System.Linq;

public class Tile : MonoBehaviour {

    // Game State
    private int _guardOccupancy = 0;
    //private bool _cameraOn = false;

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
        _guardOccupancy = 0;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        switch (other.tag)
        {
            case Player.GUARD_TAG:
                ++_guardOccupancy;
                break;
            case Player.PLAYER_TAG:
                --_guardOccupancy;
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
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(_width, _height, 0.5f));
        Gizmos.DrawSphere(Vector3.zero, 0.2f);

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
