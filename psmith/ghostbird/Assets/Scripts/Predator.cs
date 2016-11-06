using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Predator : MonoBehaviour
{
    private const float _moveSpeed = 0.5f;

    // Designer
    private Rigidbody _rigidbody;
    private Sector _sector = null;
    //private Player _player = null;

    // Game state
    private Stack<Tile> _path = null;
    private Tile _originTile = null;
    private Tile _targetTile = null;
    private Tile _previousTarget = null;

    void Start()
    {
        if (!_rigidbody)
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        _sector = GameObject.FindObjectOfType<Sector>();
        //_player = GameObject.FindObjectOfType<Player>();

        _path = null;
        _originTile = null;
        _targetTile = null;
        _previousTarget = null;
    }

    void Update()
    {
        if (_path == null || _path.Count <= 0)
        {
            _originTile = _sector.GetClosestTile(this.transform.position);
            Tile target = null;

            var adjacent = _sector.GetAdjacentTiles(_originTile)
                                  .OrderBy(x => x._predatorOccupancy);

            // Don't move into tiles with other predators
            var predators = GameObject.FindObjectsOfType<Predator>();
            foreach (var t in adjacent)
            {
                bool containsPredator = false;
                foreach (var p in predators)
                {
                    if (t == p._originTile || 
                        t == p._targetTile ||
                        t == _previousTarget)
                    {
                        containsPredator = true;
                        break;
                    }
                }
                if (!containsPredator)
                {
                    target = t;
                    break;
                }
            }
            if (target == null)
            {
                target = _previousTarget;
            }

            if (_originTile != null && target != null)
            {
                _previousTarget = _targetTile;
                _targetTile = target;
                _path = new Stack<Tile>();
                _path.Push(_targetTile);
            }
            
            //_targetTile = _sector.GetClosestTile(_player.transform.position);
            //_path = _sector.FindShortestPath(_originTile, _targetTile, 100);
        }
    }

    void FixedUpdate()
    {
        float deltaTime = Time.fixedDeltaTime;
        
        if (_path != null && _path.Count > 0)
        {
            Vector3 currentPos = transform.position;
            Vector3 nextPos = currentPos;
            float moveDistance = 0.0f;
            do
            {
                nextPos = _path.Peek().transform.position;
                moveDistance = Vector3.Distance(currentPos, nextPos);
                if (moveDistance <= 0.5f)
                {
                    _path.Pop();
                }
                else
                {
                    break;
                }
            } while (_path.Count > 0);

            if (moveDistance > 0.5f)
            {
                if (_moveSpeed < moveDistance)
                {
                    moveDistance = _moveSpeed;
                }
                Vector3 moveVector =
                    (_moveSpeed < moveDistance) ?
                    (nextPos - currentPos) :
                    (nextPos - currentPos).normalized * _moveSpeed;

                _rigidbody.MovePosition(currentPos + moveVector * deltaTime);
                _rigidbody.velocity = Vector3.zero;
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            case Player.PREDATOR_TAG:
            case Player.PLAYER_TAG:
                _path = null;
                _targetTile = null;
                _previousTarget = null;
                break;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;

        if (_originTile != null)
        {
            Gizmos.DrawSphere(_originTile.transform.position, 0.2f);
        }
        if (_targetTile != null)
        {
            Gizmos.DrawSphere(_targetTile.transform.position, 0.2f);
        }

        if (_path != null)
        {
            Vector3 currentPos = transform.position;
            foreach (var tile in _path)
            {
                Vector3 nextPos = tile.transform.position;
                Gizmos.DrawLine(currentPos, nextPos);
                currentPos = nextPos;
            }
        }
    }
}
