using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Sector : MonoBehaviour {

    // Designer
    private Tile[] _tiles = new Tile[0];
    private Door[] _doors = new Door[0];
    public RoomLink[] _roomLinks = new RoomLink[0];

    [System.Serializable]
    public class RoomLink
    {
        public Door door;
        public Tile tile1;
        public Tile tile2;
    }

    void Start () {
        _tiles = GetComponentsInChildren<Tile>();
        _doors = GetComponentsInChildren<Door>();
    }

    void OnDrawGizmos()
    {
        foreach (var link in _roomLinks)
        {
            if (link.tile1 != null && link.tile2 == null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(link.tile2.transform.position, 0.3f);
            }
            if (link.tile1 == null && link.tile2 != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(link.tile2.transform.position, 0.3f);
            }
            if (link.tile1 != null && link.tile2 != null && link.door == null)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(link.tile1.transform.position, link.tile2.transform.position);
            }
            if (link.tile1 != null && link.tile2 != null && link.door != null)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(link.tile1.transform.position, link.door.transform.position);
                Gizmos.DrawLine(link.door.transform.position, link.tile2.transform.position);
                Gizmos.DrawSphere(link.door.transform.position, 0.4f);
            }
        }
    }
}
