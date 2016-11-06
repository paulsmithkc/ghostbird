using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Player : MonoBehaviour
{
    // Inputs
    public const string HORIZONTAL_INPUT = "Horizontal";
    public const string VERTICAL_INPUT = "Vertical";

    // Tags
    public const string PLAYER_TAG = "Player";
    public const string PREDATOR_TAG = "Predator";

    // Physicis Tuning
    private const float _moveSpeed = 2.0f;

    // Game State
    [HideInInspector]
    public bool _sleeping = false;
    [HideInInspector]
    public List<Tile> _path = null;
    [HideInInspector]
    public Tile _originTile = null;
    [HideInInspector]
    public Tile _targetTile = null;

    // Designer
    private Rigidbody _rigidbody;
    private Animator _animator;

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

        _sleeping = false;
        _path = null;
        _originTile = null;
        _targetTile = null;
    }

    void Update()
    {
        _sleeping = _sleeping ^ Input.GetButtonDown("Fire2");

        if (Input.GetButtonDown("Fire3"))
        {
            _sleeping = false;
            _animator.SetTrigger("eating");
        }
    }

    void FixedUpdate()
    {
        float deltaTime = Time.fixedDeltaTime;

        //float horizontal = Input.GetAxis(HORIZONTAL_INPUT);
        //float vertical = Input.GetAxis(VERTICAL_INPUT);
        /*
        _rigidbody.velocity = new Vector3(
            horizontal * _moveSpeed,
            0.0f,
            vertical * _moveSpeed
        );
        */

        bool walking = false;
        if (_path != null && _path.Count > 0)
        {
            Vector3 currentPos = transform.position;
            Vector3 nextPos = currentPos;
            float moveDistance = 0.0f;
            do
            {
                nextPos = _path[0].transform.position;
                moveDistance = Vector3.Distance(currentPos, nextPos);
                if (moveDistance <= 0.5f)
                {
                    _path.RemoveAt(0);
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
                _sleeping = false;
                walking = true;
            }
        }

        _animator.SetBool("walking", walking);
        _animator.SetBool("sleeping", _sleeping);
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
