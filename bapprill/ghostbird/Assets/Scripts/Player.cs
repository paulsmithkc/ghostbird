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
    private const float _tiredMax = 15.0f;
    private const int _foodMax = 5;
    private const float _sleepMultiplier = -2.0f;
    private const float _eatInterval = 1.0f;
    private const float _hungerInterval = 3.0f;

    // Game State
    public float _tiredCurrent = 0.0f;
    public int _foodCurrent = 0;

    [HideInInspector]
    public float _hungerElapsed = 0.0f;
    [HideInInspector]
    public bool _sleeping = false;
    [HideInInspector]
    public bool _eating = false;
    [HideInInspector]
    public float _eatingElapsed = 0.0f;
    [HideInInspector]
    public Stack<Tile> _path = null;
    [HideInInspector]
    public Tile _originTile = null;
    [HideInInspector]
    public Tile _targetTile = null;
    
    // Designer
    private Rigidbody _rigidbody;
    private Animator _animator;

    //Persistent data
    public PlayerData pData;

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
        
        _tiredCurrent = pData.sleepLevel;
        _foodCurrent = pData.playerFood;
        _hungerElapsed = 0.0f;
        _sleeping = false;
        _eating = false;
        _eatingElapsed = 0.0f;
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
        
        _sleeping = _sleeping ^ Input.GetButtonDown("Fire2");

        //if (Input.GetButtonDown("Fire3"))
        //{
        //    StartEating();
        //}

        if (_eating)
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
                    _eating = false;
                }
            }
        }

        _hungerElapsed += deltaTime;
        if (_hungerElapsed >= _hungerInterval)
        {
            _hungerElapsed -= _hungerInterval;
            _foodCurrent = Mathf.Clamp(_foodCurrent - 1, 0, _foodMax);
        }

        _tiredCurrent = Mathf.Clamp(
            _tiredCurrent + (_sleeping ? deltaTime * _sleepMultiplier : deltaTime),
            0.0f, _tiredMax
        );
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
                _sleeping = false;
                walking = true;

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
                StartEating();
            }
        }

        _animator.SetBool("walking", walking);
        _animator.SetBool("sleeping", _sleeping);
    }
    
    public void StartEating()
    {
        if (!_eating && _targetTile != null)
        {
            var tree = _targetTile.GetComponentInChildren<FruitTree>();
            Debug.Log(tree != null ? "tree found" : "tree not found");

            if (_foodCurrent < _foodMax && tree != null && tree.EatFruit())
            {
                _foodCurrent += 1;
                _animator.SetTrigger("eating");

                _sleeping = false;
                _eating = true;
                _eatingElapsed = 0.0f;
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
