using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Sector : MonoBehaviour {

    // Designer
    private Room[] _rooms = new Room[0];
    private Door[] _doors = new Door[0];
    public RoomLink[] _roomLinks = new RoomLink[0];

    [System.Serializable]
    public class RoomLink
    {
        public Door door;
        public Room room1;
        public Room room2;
    }

    void Start () {
        _rooms = GetComponentsInChildren<Room>();
        _doors = GetComponentsInChildren<Door>();
    }

    void OnDrawGizmos()
    {
        foreach (var link in _roomLinks)
        {
            if (link.room1 != null && link.room2 == null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(link.room1.transform.position, 0.3f);
            }
            if (link.room1 == null && link.room2 != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(link.room2.transform.position, 0.3f);
            }
            if (link.room1 != null && link.room2 != null && link.door == null)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(link.room1.transform.position, link.room2.transform.position);
            }
            if (link.room1 != null && link.room2 != null && link.door != null)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(link.room1.transform.position, link.door.transform.position);
                Gizmos.DrawLine(link.door.transform.position, link.room2.transform.position);
                Gizmos.DrawSphere(link.door.transform.position, 0.4f);
            }
        }
    }
}
