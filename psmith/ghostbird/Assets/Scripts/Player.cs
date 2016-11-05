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
    private Rigidbody _rigidbody;

    void Start()
    {
        if (!_rigidbody)
        {
            _rigidbody = GetComponent<Rigidbody>();
        }
    }

    void Update()
    {
    }

    void FixedUpdate()
    {
        float horizontal = Input.GetAxis(HORIZONTAL_INPUT);
        float vertical = Input.GetAxis(VERTICAL_INPUT);

        _rigidbody.velocity = new Vector3(
            horizontal * _moveSpeed,
            0.0f,
            vertical * _moveSpeed
        );
    }
}
