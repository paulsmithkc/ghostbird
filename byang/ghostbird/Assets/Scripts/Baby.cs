using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Baby : MonoBehaviour {

    // Physicis Tuning
    public const float _moveSpeed = 2.0f;
    public const int _foodMax = 5;
    public const float _hungerInterval = 3.0f;

    private Sector _sector = null;
    private Player _player = null;
    private Rigidbody _rigidbody;
    private Animator _animator;

    public BabyState _state = BabyState.IDLE;
    public int _foodCurrent = 0;
    private float _hungerElapsed = 0.0f;
    private Stack<Tile> _path = null;
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
        
        if (_path != null && _path.Count > 0)
        {
            Vector3 currentPos = transform.position;
            Vector3 nextPos = currentPos;
            float moveDistance = 0.0f;
            do
            {
                Tile nextTile = _path.Peek();
                nextPos = nextTile.transform.position;
                moveDistance = Vector3.Distance(currentPos, nextPos);
                if (moveDistance <= 0.2f)
                {
                    _path.Pop();
                }
                else
                {
                    break;
                }
            } while (_path.Count > 0);

            if (moveDistance > 0.2f)
            {
                if (_path.Count <= 1 &&
                    _targetTile.GetComponentInChildren<HidingBush>() != null)
                {
                    nextPos += Vector3.forward * 0.1f;
                    moveDistance = Vector3.Distance(currentPos, nextPos);
                }

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

            if (_path.Count == 0)
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
