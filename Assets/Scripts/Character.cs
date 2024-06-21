using UnityEngine;
using TMPro;
using Unity.Netcode;


public class Character : NetworkBehaviour, ICharacter
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
    [SerializeField] private Transform                  _camera;

    [SerializeField] private int                        _maxHealth;
    [SerializeField] private TextMeshPro                _hpTextMeshPro;
                             
    private Animator                                    _animator;

    private Vector2 _input;
    [SerializeField] private Vector3 _velocity;

    private bool                    _isOnAir;
    private bool                    _isDead;
    private bool                    _startJump;
    private IGun                    _weapon;
    private NetworkObject           _networkObject;
    private UI                      _UI;
    private NetworkVariable<int>    _hp;
    private NetworkVariable<Transform> _networkCamera;
    private NetworkVariable<Vector2>    _networkInput;

    private CharacterController _characterController;

    private float _xRotation;

    public int HP { get; set; }

    public bool IsDead => _isDead;


    private void Awake()
    {
        _networkInput = new NetworkVariable<Vector2>();
        _networkCamera = new NetworkVariable<Transform>();
        _hp = new NetworkVariable<int>();
        if (NetworkManager.Singleton.IsServer)
        {
            _hp.Value = _maxHealth;
            _networkCamera.Value = _camera;
            _networkInput.Value = _input;
        }





        _hpTextMeshPro.text = _hp.Value.ToString();
        _xRotation = 0;
        _characterController = GetComponent<CharacterController>();
        _weapon = GetComponentInChildren<IGun>();
        Cursor.lockState = CursorLockMode.Locked;
        _UI = FindFirstObjectByType<UI>();
        _networkObject = GetComponent<NetworkObject>();

    }

    private void Start()
    {
        //_UI.UpdateHPClientRpc(_hp.Value);
        //_UI.UpdateAmmunitionClientRpc(_weapon.CurrentAmmunition, _weapon.TotalAmmunition);
    }



    private void Update()
    {
        //if(_networkObject.IsLocalPlayer)
        //{
        //    UpdateLocalHead();
        //    GetlocalInput();
        //}

        //if (NetworkManager.Singleton.IsServer)
        //    UpdateServer();
            

    }

    private void UpdateServer()
    {
        
        UpdateRotation();
    }

    private void FixedUpdate()
    {
        //if( NetworkManager.Singleton.IsServer) 
        //{
        //    Collider[] collider = Physics.OverlapSphere(_floorDetectorPos.position, _floorDectecorRadius, _floorDetectionLayerMask);
        //    if (collider.Length == 0)
        //        _isOnAir = true;
        //    else
        //        _isOnAir = false;
        //    UpdateVelocity();
        //    UpdatePosition();
        //}
            
    }

    private void UpdateRotation()
    {
        float rotation = Input.GetAxis("Mouse X") * _rotationVelocityFactor;
        

        transform.Rotate(0f, rotation, 0f);
    }
    private void UpdateLocalHead()
    {
        float input;

        input = Input.GetAxis("Mouse Y") * _rotationVelocityFactor;

        //UpdateHeadServerRpc(input);

    }

    //[ServerRpc]
    //private void UpdateHeadServerRpc(float input)
    //{

    //     _xRotation -= input;

    //    _xRotation = Mathf.Clamp(_xRotation, -90f,90f);

    //    _networkCamera.Value.localRotation = Quaternion.Euler(_xRotation,0,0); 
    //}

    

    private void GetlocalInput()
    {
        Vector2 oldInput = _input;
        _input.y = Input.GetAxis("Forward");
        _input.x = Input.GetAxis("Strafe");


            //UpdateInputServerRpc(_input);

        if (Input.GetButton("Jump") && !_isOnAir)
        {
            Jump();
        }


        if (Input.GetButtonUp("Fire1") && _networkObject.IsLocalPlayer)
        {
            _weapon.Shoot(transform.position,transform.forward);
            _UI.UpdateAmmunitionClientRpc(_weapon.CurrentAmmunition, _weapon.TotalAmmunition);
        }

    }
    //[ServerRpc]
    //private void UpdateInputServerRpc(Vector2 input)
    //{
    //    _networkInput.Value = input;
    //}
    
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
        _velocity =  _velocity + _networkInput.Value.y * _accelerationForwardForce * transform.forward + _networkInput.Value.x * _accelerationStrafeForce * transform.right + ForceJump * Vector3.up;
        if(!_isOnAir)
            _velocity = _velocity * (1 - Time.fixedDeltaTime * _dragForce);

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
        if (_hp.Value > 0)
        {
            _hp.Value -= damage;
            //_animator.Play("Hit");
            CheckDeath();
            //_animator.SetBool("Hit", false);
            _hpTextMeshPro.text = _hp.Value.ToString();
            _UI.UpdateHPClientRpc(_hp.Value);
        }
    }

    //[ServerRpc]
    //private void ReceiveDamageServerRpc(int damage)
    //{
    //    if(_hp.Value > 0)
    //    {
    //        _hp.Value -= damage;
    //        //_animator.Play("Hit");
    //        CheckDeath();
    //        //_animator.SetBool("Hit", false);
    //        _hpTextMeshPro.text = _hp.Value.ToString();
    //        _UI.UpdateHPClientRpc(_hp.Value);
    //    }
    //}
    private void CheckDeath()
    {
        if (_hp.Value <= 0)
        {
            _isDead = true;
            _hpTextMeshPro.text = "Dead";
        }
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
