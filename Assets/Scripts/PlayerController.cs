using UnityEngine;
using Cinemachine;
using System;


public class PlayerController : MonoBehaviour, IPlayer
{
    [SerializeField] private float                      _accelerationForwardForce;
    [SerializeField] private float                      _accelerationStrafeForce;
    [SerializeField] private float                      _accelerationJumpForce;

    [SerializeField] private float                      _maxForwardVelocity;
    [SerializeField] private float                      _maxBackwardVelocity;
    [SerializeField] private float                      _maxStrafeVelocity;

    [SerializeField] private Transform                  _floorDetectorPos;
    [SerializeField] private float                      _floorDectecorRadius;
    [SerializeField] private LayerMask                  _floorDetectionLayerMask;

    [SerializeField] private float                      _rotationVelocityFactor;
    [SerializeField] private float                      _minHeadDownAngle;
    [SerializeField] private float                      _maxHeadUpAngle;
    [SerializeField] private float                      _gravityForce;
    [SerializeField] private float                      _dragForce;
    [SerializeField] private IGun                       _weapon;
    [SerializeField] private Transform                     _camera;

    private Animator                                    _animator;

    private Vector2 _input;
    private Vector3 _acceleration;
    [SerializeField] private Vector3 _velocity;

    private bool    _isOnAir;
    private bool    _isDead;
    private bool    _startJump;

    private CharacterController _characterController;

    private float _sinPI4;
    private float _xRotation;

    public int HP { get; set; }

    public bool IsDead => _isDead;


    private void Awake()
    {
        _xRotation = 0;
        _characterController = GetComponent<CharacterController>();
        _sinPI4 = Mathf.Sin(Mathf.PI / 4);
        _weapon = GetComponentInChildren<IGun>();
        Cursor.lockState = CursorLockMode.Locked;

    }

    private void Update()
    {
        GetInput();
        UpdateHead();
        UpdateRotation();
    }

    private void FixedUpdate()
    {
        Collider[] collider = Physics.OverlapSphere(_floorDetectorPos.position, _floorDectecorRadius, _floorDetectionLayerMask);
        if(collider.Length == 0)
            _isOnAir = true;
        else
            _isOnAir = false;
        UpdateVelocity();
        UpdatePosition();
    }

    private void UpdateRotation()
    {
        float rotation = Input.GetAxis("Mouse X") * _rotationVelocityFactor;

        transform.Rotate(0f, rotation, 0f);
    }

    private void UpdateHead()
    {
        //Vector3 headRotation = _camera.localEulerAngles;
        float input;

        input = Input.GetAxis("Mouse Y") * _rotationVelocityFactor;

         _xRotation -= input;

        _xRotation = Mathf.Clamp(_xRotation, -90f,90f);

        _camera.localRotation = Quaternion.Euler(_xRotation,0,0); ;
    }
    private void GetInput()
    {
        _input.y = Input.GetAxis("Forward");
        _input.x = Input.GetAxis("Strafe");
        if (Input.GetButton("Jump") && !_isOnAir)
        {
            Jump();
        }
        if (Input.GetButtonUp("Fire1"))
        {
            _weapon.Shoot(transform.position,transform.forward);
        }

    }

   

    private void Jump()
    {
        //_velocity.y += _accelerationJumpForce
        _startJump = true;
    }
    private void UpdateVelocity()
    {
        float ForceJump =0;
        if (_startJump)
        {
            ForceJump += _accelerationJumpForce;
            _startJump = false;
        }
        else
            ForceJump = 0;
        _velocity =  _velocity + _input.y * _accelerationForwardForce * transform.forward + _input.x * _accelerationStrafeForce * transform.right + ForceJump * Vector3.up;
        if(!_isOnAir)
            _velocity = _velocity * (1 - Time.fixedDeltaTime * _dragForce);



        //if ((_velocity.x > 0 || _velocity.z > 0) && !_isOnAir)
        //{
        //    float dragStrength = Math.Min(_velocity.magnitude, (1 - Mathf.Abs(Vector3.Dot(_velocity.normalized, transform.forward))) * _dragForce);
        //    _velocity += -_velocity.normalized * dragStrength;

        //}

        //_velocity += _acceleration * Time.fixedDeltaTime;








        //_velocity = _velocity.normalized * Mathf.Clamp(_velocity.magnitude, 0, _maxForwardVelocity);
        if (_isOnAir)
        {
            _velocity.y += _gravityForce * Time.deltaTime;

        }
        else
        {
            if (_velocity.y > 0)
                _velocity.y -= 0.1f;
        }


               
    }

    private void UpdatePosition()
    {
        Vector3 movement = _velocity * Time.fixedDeltaTime;
        _characterController.Move(movement);
    }

    public void ReceiveDamage(int damage)
    {
        _animator.Play("Hit");
        CheckDeath();
        _animator.SetBool("Hit", false);

    }

    private void CheckDeath()
    {
        if (HP <= 0)
            _isDead = true;
        else
            _isDead = false;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        //Gizmos.DrawSphere(_floorDetectorPos.position, _floorDectecorRadius);
        Gizmos.DrawRay(transform.position, transform.forward * 10);
        Gizmos.DrawSphere(_floorDetectorPos.position, _floorDectecorRadius);
    }
}
