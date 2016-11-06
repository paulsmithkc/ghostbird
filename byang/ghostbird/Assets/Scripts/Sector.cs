using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Sector : MonoBehaviour {

    public const float TILE_SIZE = 2.0f;

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

    public Tile GetClosestTile(Vector3 pos)
    {
        float closestDistance = float.MaxValue;
        Tile closestTile = null;

        if (_tiles != null)
        {
            foreach (var t in _tiles)
            {
                var tp = t.transform.position;
                float d = Vector3.Distance(tp, pos);
                if (d < closestDistance)
                {
                    closestDistance = d;
                    closestTile = t;
                }
            }
        }
        return closestTile;
    }

    public IEnumerable<Tile> GetAdjacentTiles(Tile origin)
    {
        if (origin != null && _tiles != null)
        {
            var originPos = origin.transform.position;
            foreach (var t in _tiles)
            {
                if (t != origin)
                {
                    var tp = t.transform.position;
                    if (Mathf.Abs(originPos.x - tp.x) < 3.0f && Mathf.Abs(originPos.z - tp.z) < 1.0f)
                    {
                        // Horizontal adjacent
                        yield return t;
                    }
                    if (Mathf.Abs(originPos.x - tp.x) < 1.0f && Mathf.Abs(originPos.z - tp.z) < 3.0f)
                    {
                        // Vertical adjacent
                        yield return t;
                    }
                }
            }
        }
    }

    public class PathNode : System.IComparable<PathNode>, IComparer<PathNode>
    {
        public int steps;
        public float distance;
        public Vector3 pos;
        public Tile tile;
        public PathNode previous;

        public int Compare(PathNode x, PathNode y)
        {
            if (x.distance < y.distance) { return -1; }
            else if (x.distance > y.distance) { return 1; }
            else { return 0; }
        }

        public int CompareTo(PathNode other)
        {
            return Compare(this, other);
        }
    }

    public List<Tile> FindShortestPath(Tile origin, Tile target, int maxIterations)
    {
        if (origin != null && target != null && _tiles != null)
        {
            var heap = new List<PathNode>();
            var visitedTiles = new List<Tile>();
            var originPos = origin.transform.position;
            var targetPos = target.transform.position;
            {
                float d = Vector3.Distance(originPos, targetPos);
                heap.Add(new PathNode { steps = 0, distance = d, pos = originPos, tile = origin, previous = null });
            }

            for (int i = 0; i <= maxIterations && heap.Count > 0; ++i)
            {
                PathNode current = heap[0];
                Vector3 currentPos = current.pos;
                Tile currentTile = current.tile;
                heap.RemoveAt(0);
                visitedTiles.Add(currentTile);
                foreach (var t in GetAdjacentTiles(currentTile))
                {
                    if (t == target) {
                        var path = new List<Tile>();
                        path.Add(t);
                        while (current != null)
                        {
                            path.Add(current.tile);
                            current = current.previous;
                        }
                        return path;
                    }
                    else if (!visitedTiles.Contains(t))
                    {
                        Vector3 tp = t.transform.position;
                        int steps = current.steps + 1;
                        float d = steps * TILE_SIZE + Vector3.Distance(tp, targetPos);
                        heap.Add(new PathNode { steps = steps, distance = d, pos = tp, tile = t, previous = current });
                    }
                }
                heap.Sort();
            }
        }
        return null;
    }

    void OnDrawGizmos()
    {
        if (_roomLinks != null)
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
}
