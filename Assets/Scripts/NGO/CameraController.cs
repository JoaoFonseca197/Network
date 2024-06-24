using Unity.Netcode;
using UnityEngine;

public class CameraController : NetworkBehaviour
{
    [SerializeField] GameObject     _cameraGameObject;
    [SerializeField] Camera         _camera;
    // Start is called before the first frame update
    private NetworkVariable<float> _networkXRotation;
    float _xRotation; 
    void Awake()
    {


        _camera = GetComponentInChildren<Camera>();

        _xRotation = _camera.transform.rotation.x;

        _networkXRotation = new NetworkVariable<float>();
        
    }

    private void Start()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            _networkXRotation.Value = _xRotation;
        }


        if (IsOwner)
        {
            _camera.enabled = true;
            _camera.depth = 1;
        }
        else
        {
            _camera.depth = 0;
        }
    }
    [ServerRpc]
    public void RotateInXAxisServerRpc(float input)
    {

        _xRotation -= input;
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

        _networkXRotation.Value = _xRotation;
    }

    // Update is called once per frame
    void Update()
    {
        _camera.transform.rotation = Quaternion.Euler(_networkXRotation.Value, transform.rotation.eulerAngles.y, 0);
    }
}
