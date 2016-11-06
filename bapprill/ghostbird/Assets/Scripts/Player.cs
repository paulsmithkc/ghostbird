using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    // Inputs
    public const string HORIZONTAL_INPUT = "Horizontal";
    public const string VERTICAL_INPUT = "Vertical";

    // Tags
    public const string PLAYER_TAG = "Player";
    public const string GUARD_TAG = "Guard";

    // Physicis Tuning
    private const float _moveSpeed = 8.0f;

    // Game State
    private int _loneliness = 0;

    // Designer
    private Rigidbody2D _rigidbody2D;

    void Start()
    {
        if (!_rigidbody2D)
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
        }
    }

    void Update()
    {
    }

    void FixedUpdate()
    {
        float horizontal = Input.GetAxis(HORIZONTAL_INPUT);
        float vertical = Input.GetAxis(VERTICAL_INPUT);

        _rigidbody2D.velocity = new Vector2(
            horizontal * _moveSpeed,
            vertical * _moveSpeed
        );
    }
}
