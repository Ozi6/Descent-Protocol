using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MovementComponent : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _tileSize = 16f;

    private Rigidbody2D _rb;
    private Vector3 _targetPosition;
    private bool _isMoving;

    public bool IsMoving => _isMoving;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    public void MoveInDirection(Vector2 direction)
    {
        _targetPosition = transform.position + new Vector3(direction.x, direction.y, 0) * (_tileSize / 10f);
        _isMoving = true;
    }

    private void FixedUpdate()
    {
        if(_isMoving)
        {
            Vector3 newPos = Vector3.MoveTowards(transform.position, _targetPosition, Time.deltaTime * _moveSpeed);
            _rb.MovePosition(newPos);

            if(Vector3.Distance(transform.position, _targetPosition) < 0.01f)
            {
                _rb.linearVelocity = Vector2.zero;
                _isMoving = false;
            }
        }
    }
}