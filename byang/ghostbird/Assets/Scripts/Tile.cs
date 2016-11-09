using UnityEngine;
using System.Linq;

public class Tile : MonoBehaviour {

    // Game State
    public int _predatorOccupancy = 0;
    public bool _passable = true;

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

    public void OnMouseEnter()
    {
        foreach (var c in GetComponentsInChildren<TileObject>())
        {
            c.OnMouseEnter();
        }
    }

    public void OnMouseExit()
    {
        foreach (var c in GetComponentsInChildren<TileObject>())
        {
            c.OnMouseExit();
        }
    }

    public void OnMouseDown()
    {
        var sector = GameObject.FindObjectOfType<Sector>();
        var player = GameObject.FindObjectOfType<Player>();

        player._originTile = sector.GetClosestTile(player.transform.position);
        player._targetTile = this;

        if (player._originTile != player._targetTile)
        {
            player._path = sector.FindShortestPath(player._originTile, this, 100);
            player._state = Player.PlayerState.WALKING;
        }
        else if (player._targetTile != null)
        {
            player.StartEating();
            player.StartHiding();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case Player.PREDATOR_TAG:
                ++_predatorOccupancy;
                break;
            case Player.PLAYER_TAG:
                //--_predatorOccupancy;
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
            new Vector3(0.0f, 0.0f, -0.5f * _predatorOccupancy),
            new Vector3(_width, _height, 0.5f)
        );
    }
}
