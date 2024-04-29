
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float      _accelerationForwardForce;
    [SerializeField] private float      _accelerationStrafeForce;
    [SerializeField] private float      _accelerationJumpForce;

    [SerializeField] private float      _maxForwardVelocity;
    [SerializeField] private float      _maxBackwardVelocity;
    [SerializeField] private float      _maxStrafeVelocity;

    [SerializeField] private Transform  _floorDetectorPos;
    [SerializeField] private float      _floorDectecorRadius;
    [SerializeField] private LayerMask  _floorDetectionLayerMask;
    [SerializeField] private float      _gravityForce;
 
    public int HP { get; set; }

    private Vector2 _input;
    [SerializeField] private Vector3 _acceleration;
    [SerializeField] private Vector3 _velocity;
    private bool    _isOnAir;
    private CharacterController _characterController;

    private float _sinPI4;
    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _sinPI4 = Mathf.Sin(Mathf.PI / 4);

    }

    private void FixedUpdate()
    {
        Ray ray = new Ray(_floorDetectorPos.position, Vector3.down);
        _isOnAir = Physics.SphereCast(ray, _floorDectecorRadius, _floorDetectionLayerMask);

        GetInput();
        UpdateAcceleration();
        UpdateVelocity();
        UpdatePosition();
    }
    private void GetInput()
    {
        _input.y = Input.GetAxis("Forward");
        _input.x = Input.GetAxis("Strafe");
        if (Input.GetButton("Jump") && !_isOnAir)
        {
                Jump();
        }


    }

    private void UpdateAcceleration()
    {
        _acceleration.z = _input.y * _accelerationForwardForce;
        _acceleration.x = _input.x * _accelerationStrafeForce;
        _acceleration.y = _gravityForce;
        

    }

    private void Jump()
    {
        _velocity.y += _accelerationJumpForce;
    }
    private void UpdateVelocity()
    {
        _velocity += _acceleration * Time.fixedDeltaTime;

        if (_acceleration.z == 0f || (_acceleration.z * _velocity.z < 0f))
            _velocity.z = 0f;
        else if (_acceleration.x == 0f)
            _velocity.z = Mathf.Clamp(_velocity.z, _maxBackwardVelocity, _maxForwardVelocity);
        else
            _velocity.z = Mathf.Clamp(_velocity.z, _maxBackwardVelocity * _sinPI4, _maxForwardVelocity * _sinPI4);

        if (_acceleration.x == 0f || (_acceleration.x * _velocity.x < 0f))
            _velocity.x = 0f;
        else if (_acceleration.z == 0f)
            _velocity.x = Mathf.Clamp(_velocity.x, -_maxStrafeVelocity, _maxStrafeVelocity);
        else
            _velocity.x = Mathf.Clamp(_velocity.x, -_maxStrafeVelocity * _sinPI4, _maxStrafeVelocity * _sinPI4);
    }

    private void UpdatePosition()
    {
        Vector3 movement = _velocity * Time.fixedDeltaTime;
        _characterController.Move(movement);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(_floorDetectorPos.position, _floorDectecorRadius);
    }
}
