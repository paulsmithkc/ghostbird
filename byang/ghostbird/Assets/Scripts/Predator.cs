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

        if (_targetTile == null || _targetTile == _originTile)
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
            }
        }
    }

    void FixedUpdate()
    {
        float deltaTime = Time.fixedDeltaTime;
        
        if (_targetTile != null)
        {
            Vector3 currentPos = transform.position;
            Vector3 nextPos = _targetTile.transform.position;
            float moveDistance = Vector3.Distance(currentPos, nextPos);

            if (moveDistance > 0.4f)
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
            else
            {
                _targetTile = null;
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
            }
            else if (other.tag == Player.PLAYER_TAG)
            {
                var player = other.gameObject.GetComponent<Player>();
                player._state = Player.PlayerState.DEAD;
            }
            else if (other.tag == Player.BABY_TAG)
            {
                var baby = other.gameObject.GetComponent<Baby>();
                baby._state = Baby.BabyState.DEAD;
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        
        if (_targetTile != null)
        {
            Vector3 currentPos = transform.position;
            Vector3 nextPos = _targetTile.transform.position;

            Gizmos.DrawLine(currentPos, nextPos);
            Gizmos.DrawSphere(nextPos, 0.2f);
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
}
