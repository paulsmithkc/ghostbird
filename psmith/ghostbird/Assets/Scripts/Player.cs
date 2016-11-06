using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Player : MonoBehaviour
{
    // Inputs
    public const string QUIT_INPUT = "Quit";
    public const string RESTART_INPUT = "Restart";
    public const string PAUSE_INPUT = "Pause";
    public const string HORIZONTAL_INPUT = "Horizontal";
    public const string VERTICAL_INPUT = "Vertical";
    
    // Tags
    public const string PLAYER_TAG = "Player";
    public const string PREDATOR_TAG = "Predator";

    // Physicis Tuning
    private const float _moveSpeed = 2.0f;
    private const float _tiredMax = 10.0f;
    private const int _foodMax = 5;
    private const float _eatInterval = 1.0f;
    private const float _hungerInterval = 3.0f;

    // Game State
    public float _tiredCurrent = 0.0f;
    public int _foodCurrent = 0;
    
    private float _hungerElapsed = 0.0f;
    private float _eatingElapsed = 0.0f;
    public PlayerState _state = PlayerState.IDLE;

    //[HideInInspector]
    //public bool _sleeping = false;
    //[HideInInspector]
    //public bool _eating = false;
    //[HideInInspector]
    //public bool _hiding = false;
    [HideInInspector]
    public Stack<Tile> _path = null;
    [HideInInspector]
    public Tile _originTile = null;
    [HideInInspector]
    public Tile _targetTile = null;
    
    // Designer
    private Rigidbody _rigidbody;
    private Animator _animator;
    //private SpriteRenderer _spriteRenderer;

    public enum PlayerState
    {
        IDLE, WALKING, SLEEPING, EATING, HIDING
    }

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
        //if (!_spriteRenderer)
        //{
        //    _spriteRenderer = GetComponent<SpriteRenderer>();
        //}

        _tiredCurrent = 0.0f;
        _foodCurrent = 0;
        _hungerElapsed = 0.0f;
        _eatingElapsed = 0.0f;
        _state = PlayerState.IDLE;
        _path = null;
        _originTile = null;
        _targetTile = null;
    }

    void Update()
    {
        if (Input.GetButtonDown(QUIT_INPUT))
        {
            Time.timeScale = 0.0f;
            Application.Quit();
            return;
        }
        if (Input.GetButtonDown(RESTART_INPUT))
        {
            OnRestart();
            return;
        }
        if (Input.GetButtonDown(PAUSE_INPUT))
        {
            Time.timeScale = (Time.timeScale == 0.0f ? 1.0f : 0.0f);
        }

        float deltaTime = Time.deltaTime;

        if (Input.GetButtonDown("Fire2"))
        {
            _state = (_state == PlayerState.SLEEPING) ? PlayerState.IDLE : PlayerState.SLEEPING;
        }

        //if (Input.GetButtonDown("Fire3"))
        //{
        //    StartEating();
        //}

        if (_state == PlayerState.EATING)
        {
            _eatingElapsed += deltaTime;
            if (_eatingElapsed >= _eatInterval)
            {
                _eatingElapsed -= _eatInterval;

                var tree = _targetTile.GetComponentInChildren<FruitTree>();
                if (_foodCurrent < _foodMax && tree != null && tree.EatFruit())
                {
                    _foodCurrent += 1;
                    _animator.SetTrigger("eating");
                }
                else
                {
                    _state = PlayerState.IDLE;
                }
            }
        }

        _hungerElapsed += deltaTime;
        if (_hungerElapsed >= _hungerInterval)
        {
            _hungerElapsed -= _hungerInterval;
            _foodCurrent = Mathf.Clamp(_foodCurrent - 1, 0, _foodMax);
        }

        _tiredCurrent = Mathf.Max(
            0.0f, _tiredCurrent + deltaTime * GetSleepMultiplier()
        );
        if (_tiredCurrent >= _tiredMax)
        {
            _tiredCurrent = _tiredMax;
            _state = PlayerState.SLEEPING;
            _path = null;
            _originTile = null;
            _targetTile = null;
        }
    }

    private float GetSleepMultiplier()
    {
        switch (_state)
        {
            case PlayerState.SLEEPING:
            case PlayerState.HIDING:
                return -2.0f;
            case PlayerState.WALKING:
                return 1.0f;
            default:
                return 0.25f;
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
                //_state = PlayerState.WALKING;

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
                _state = PlayerState.IDLE;
            }

            if (_path.Count == 0)
            {
                StartEating();
                StartHiding();
            }
        }

        _animator.SetBool("walking", _state == PlayerState.WALKING);
        _animator.SetBool("sleeping", _state == PlayerState.SLEEPING || _state == PlayerState.HIDING);
    }
    
    public void StartEating()
    {
        if (_state != PlayerState.EATING && _targetTile != null)
        {
            var tree = _targetTile.GetComponentInChildren<FruitTree>();

            if (_foodCurrent < _foodMax && tree != null && tree.EatFruit())
            {
                _foodCurrent += 1;
                _animator.SetTrigger("eating");

                _state = PlayerState.EATING;
                _eatingElapsed = 0.0f;
            }
        }
    }

    public void StartHiding()
    {
        if (_state != PlayerState.HIDING && _targetTile != null)
        {
            var bush = _targetTile.GetComponentInChildren<HidingBush>();
            if (bush != null)
            {
                _state = PlayerState.HIDING;
            }
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

    public void OnRestart()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
