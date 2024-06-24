using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Windows;

public class CameraController : NetworkBehaviour
{
    [SerializeField] Camera _camera;
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
    }
    [ServerRpc]
    public void RotateInXAxisServerRpc(float input)
    {

        Debug.Log("this is MouseY = " + input);


        _xRotation -= input;
        Debug.Log(_xRotation);
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

        _networkXRotation.Value = _xRotation;
    }

    // Update is called once per frame
    void Update()
    {
        _camera.transform.rotation = Quaternion.Euler(_networkXRotation.Value, transform.rotation.eulerAngles.y, 0);
    }
}
