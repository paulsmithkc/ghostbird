using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Baby : MonoBehaviour {

    // Physicis Tuning
    public const float _moveSpeed = 1.8f;
    public const int _foodMax = 10;
    public const float _hungerInterval = 24.0f;

    private Sector _sector = null;
    private Player _player = null;
    private Rigidbody _rigidbody;
    private Animator _animator;

    public BabyState _state = BabyState.IDLE;
    public int _foodCurrent = 0;
    private float _hungerElapsed = 0.0f;
    private List<Tile> _path = null;
    private Tile _originTile = null;
    private Tile _targetTile = null;
    
    public enum BabyState
    {
        IDLE, WALKING, SLEEPING, EATING, HIDING, DEAD
    }

    void Start () {
        _sector = GameObject.FindObjectOfType<Sector>();
        _player = GameObject.FindObjectOfType<Player>();

        if (!_rigidbody)
        {
            _rigidbody = GetComponent<Rigidbody>();
        }
        if (!_animator)
        {
            _animator = GetComponent<Animator>();
        }
        
        _state = BabyState.IDLE;
        _foodCurrent = _foodMax;
        _hungerElapsed = 0.0f;
        _path = null;
        _originTile = null;
        _targetTile = null;
    }

    void Update()
    {
        if (_state == BabyState.DEAD)
        {
            return;
        }

        float deltaTime = Time.deltaTime;

        Tile originTile = _sector.GetClosestTile(this.transform.position);
        Tile targetTile = _sector.GetClosestTile(_player.transform.position);

        if (targetTile != _targetTile)
        {
            _originTile = originTile;
            _targetTile = targetTile;
            _path = _sector.FindShortestPath(_originTile, _targetTile, 100);
        }

        _hungerElapsed += deltaTime;
        if (_hungerElapsed >= _hungerInterval)
        {
            _hungerElapsed -= _hungerInterval;
            _foodCurrent = Mathf.Clamp(_foodCurrent - 1, 0, _foodMax);
            if (_foodCurrent <= 0)
            {
                _state = BabyState.DEAD;
            }
        }
    }

    void FixedUpdate()
    {
        if (_state == BabyState.DEAD)
        {
            return;
        }

        float deltaTime = Time.fixedDeltaTime;

        int minTilesAway;
        switch (_player._state)
        {
            case Player.PlayerState.SLEEPING:
            case Player.PlayerState.HIDING:
                minTilesAway = 0;
                break;
            default:
                minTilesAway = 1;
                break;
        }


        if (_path != null && _path.Count > minTilesAway)
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
                        //Debug.Log("Baby moving backwards");
                        _path.RemoveAt(_path.Count - 1);
                        continue;
                    }
                }

                // Hide Behind bushes
                if (_path.Count <= 1 &&
                    _targetTile.GetComponentInChildren<HidingBush>() != null)
                {
                    nextPos += Vector3.forward * 0.1f;
                    moveDistance = Vector3.Distance(currentPos, nextPos);
                    moveTolerance = 0.2f;
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
                _state = BabyState.WALKING;

                if (moveVector.x > 0.0f)
                {
                    transform.localScale = Vector3.one;
                }
                else if (moveVector.x < 0.0f)
                {
                    transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
                }
            }

            if (_path.Count <= minTilesAway)
            {
                switch (_player._state)
                {
                    case Player.PlayerState.SLEEPING:
                        _state = BabyState.SLEEPING;
                        break;
                    case Player.PlayerState.HIDING:
                        _state = BabyState.HIDING;
                        break;
                    default:
                        _state = BabyState.IDLE;
                        break;
                }
            }
        }

        _animator.SetBool("walking", _state == BabyState.WALKING);
        _animator.SetBool("sleeping", _state == BabyState.SLEEPING || _state == BabyState.HIDING);
    }

    void OnDrawGizmos()
    {
        if (_state == BabyState.DEAD)
        {
            return;
        }

        Gizmos.color = Color.yellow;

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
            for (int i = _path.Count - 1; i >= 0; --i)
            {
                Vector3 nextPos = _path[i].transform.position;
                Gizmos.DrawLine(currentPos, nextPos);
                currentPos = nextPos;
            }
        }
    }

    //void OnTriggerEnter(Collider other)
    //{
    //    switch (other.tag)
    //    {
    //        case Player.PREDATOR_TAG:
    //            _state = BabyState.DEAD;
    //            break;
    //    }
    //}
}
