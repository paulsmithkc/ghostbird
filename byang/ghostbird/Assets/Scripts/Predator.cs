using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Predator : MonoBehaviour
{
    private const float _moveSpeed = 0.5f;
    private const float _attackDelay = 5.0f;

    // Designer
    private Rigidbody _rigidbody;
    private Animator _animator;
    private Sector _sector = null;
    //private Player _player = null;

    // Game state
    private List<Tile> _path = null;
    private Tile _originTile = null;
    private Tile _targetTile = null;
    private Tile _previousTarget = null;
    private float _attackElapsed = 0.0f;
    private bool _attacking = false;

    void Start()
    {
        if (!_rigidbody)
        {
            _rigidbody = GetComponent<Rigidbody>();
        }
        if (!_animator)
        {
            _animator = GetComponent<Animator>();
        }

        _sector = GameObject.FindObjectOfType<Sector>();
        //_player = GameObject.FindObjectOfType<Player>();

        _path = null;
        _originTile = null;
        _targetTile = null;
        _previousTarget = null;
        _attackElapsed = Random.Range(0.0f, _attackDelay);
        _attacking = false;
    }

    void Update()
    {
        float deltaTime = Time.deltaTime;

        if (!_attacking)
        {
            _attackElapsed += deltaTime;
            if (_attackElapsed >= _attackDelay)
            {
                OnAttackStart();
            }
        }

        if (_targetTile == null || 
            _targetTile == _originTile || 
            _path == null ||
            _path.Count == 0)
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

            // If there are no more options, then just turn around
            if (target == null)
            {
                target = _previousTarget;
            }

            // If we found a target, then save it
            if (_originTile != null && target != null)
            {
                _previousTarget = _targetTile;
                _targetTile = target;
                _path = new List<Tile>(1);
                _path.Add(_targetTile);
            }
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
            float moveTolerance = 0.2f;
            do
            {
                Tile nextTile = _path[_path.Count - 1];
                nextPos = nextTile.transform.position;
                moveDistance = Vector3.Distance(currentPos, nextPos);

                // Avoid walking backwards
                if (_path.Count >= 2)
                {
                    Vector3 nextMove = _path[_path.Count - 2].transform.position - nextPos;
                    Vector3 moveVector = nextPos - currentPos;
                    if (Vector3.Dot(moveVector, nextMove) <= 0.0f)
                    {
                        //Debug.Log("Predator moving backwards");
                        _path.RemoveAt(_path.Count - 1);
                        continue;
                    }
                }

                // Close enough, move toward the next tile
                if (moveDistance <= moveTolerance)
                {
                    _path.RemoveAt(_path.Count - 1);
                    continue;
                }

                // Arrived at target
                break;
                
            } while (_path.Count > 0);

            if (moveDistance > moveTolerance)
            {
                float moveSpeed = Mathf.Min(deltaTime * _moveSpeed, moveDistance);
                Vector3 moveVector = (nextPos - currentPos).normalized * moveSpeed;

                _rigidbody.MovePosition(currentPos + moveVector);
                _rigidbody.velocity = Vector3.zero;

                if (moveVector.x > 0.0f)
                {
                    transform.localScale = Vector3.one;
                }
                else if (moveVector.x < 0.0f)
                {
                    transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
                }
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == Player.PLAYER_TAG || other.tag == Player.BABY_TAG)
        {
            if (!_attacking)
            {
                OnAttackStart();
                return;
            }

            if (other.tag == Player.PLAYER_TAG)
            {
                var player = other.gameObject.GetComponent<Player>();
                if (player._state != Player.PlayerState.HIDING)
                {
                    player._state = Player.PlayerState.DEAD;
                }
            }
            else if (other.tag == Player.BABY_TAG)
            {
                var baby = other.gameObject.GetComponent<Baby>();
                if (baby._state != Baby.BabyState.HIDING)
                {
                    baby._state = Baby.BabyState.DEAD;
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;

        //if (_originTile != null)
        //{
        //    Gizmos.DrawSphere(_originTile.transform.position, 0.2f);
        //}
        if (_targetTile != null)
        {
            Gizmos.DrawSphere(_targetTile.transform.position, 0.2f);
        }

        if (_path != null)
        {
            Vector3 currentPos = transform.position;
            for (int i = _path.Count - 1; i >= 0; --i)
            {
                Vector3 nextPos = _path[i].transform.position;
                Gizmos.DrawLine(currentPos, nextPos);
                currentPos = nextPos;
            }
        }
    }


    public void OnAttackStart()
    {
        _attackElapsed = 0.0f;
        _animator.SetTrigger("attack");
        _attacking = true;
    }

    public void OnAttackEnd()
    {
        _attacking = false;
    }

    public void RushTowards(Tile tile)
    {
        _originTile = _sector.GetClosestTile(this.transform.position);
        _targetTile = tile;
        _previousTarget = null;
        _path = _sector.FindShortestPath(_originTile, tile, 100);

        if (_path != null)
        {
            for (int i = 0; i < 2 && _path.Count > 0; ++i)
            {
                _path.RemoveAt(0);
            }
        }
    }
}
