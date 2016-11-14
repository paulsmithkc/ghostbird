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
    public const string BABY_TAG = "Baby";
    public const string PREDATOR_TAG = "Predator";

    // Physicis Tuning
    public const float _moveSpeed = 2.0f;
    public const float _tiredMax = 24.0f;
    public const int _foodMax = 10;
    public const float _eatInterval = 1.0f;
    public const float _hungerInterval = 12.0f;

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
    public List<Tile> _path = null;
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
        IDLE, WALKING, SLEEPING, EATING, HIDING, DEAD
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
        _foodCurrent = _foodMax;
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
            Time.timeScale = 1.0f;
            OnRestart();
            return;
        }

        if (_state == PlayerState.DEAD)
        {
            return;
        }
        if (Input.GetButtonDown(PAUSE_INPUT))
        {
            Time.timeScale = (Time.timeScale == 0.0f ? 1.0f : 0.0f);
            
            var controller = GameObject.FindObjectOfType<UI_Controller>();
            if (Time.timeScale == 0.0f)
            {
                controller.PauseGame();
            }
            else
            {
                controller.ResumeGame();
            }
        }

        float deltaTime = Time.deltaTime;

        if (Input.GetButtonDown("Fire2"))
        {
            _state = (_state == PlayerState.SLEEPING) ? PlayerState.IDLE : PlayerState.SLEEPING;
            _path = null;
            _originTile = null;
            _targetTile = null;
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
            if (_foodCurrent <= 0)
            {
                _state = PlayerState.DEAD;
                _path = null;
                _originTile = null;
                _targetTile = null;
            }
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
        if (_state == PlayerState.DEAD)
        {
            return;
        }

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
                        //Debug.Log("Player moving backwards");
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
                _state = PlayerState.WALKING;

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

            if (_path.Count <= 0)
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
        if (_state == PlayerState.DEAD)
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
            for (int i = _path.Count - 1; i >= 0; --i)
            {
                Vector3 nextPos = _path[i].transform.position;
                Gizmos.DrawLine(currentPos, nextPos);
                currentPos = nextPos;
            }
        }
    }

    public void OnRestart()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1.0f;
    }

    //void OnTriggerEnter(Collider other)
    //{
    //    switch (other.tag)
    //    {
    //        case Player.PREDATOR_TAG:
    //            _state = PlayerState.DEAD;
    //            break;
    //    }
    //}
}
