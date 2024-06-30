
# BitBullet

BitBullet is an FPS 1v1 online made for an university discipline.
The objective of the game is to kill the other player, the player
with the most kills wins.

## Implementation

### Client and Server
For the implementation of the game I started by creating the `NetworkSetup`.
In this script we take care of the connections and initializations of the server and client.
The following code Initializes as Client or server depending on the arguments given on the console,
this code is only in the editor.

```
IEnumerator Start()
    {
        // Parse command line arguments
        string[] args = System.Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "--server")
            {
                // --server found, this should be a server application
                _isServer = true;
            }
        }
        if (_isServer)
            yield return StartAsServerCR();
        else
            yield return StartAsClientCR();
    }
```

For the builds is the following.

- Client:

```
private void Start()
{
    StartAsServerCR();
}
```

```
IEnumerator StartAsClientCR()
    {
        SetWindowTitle("BitBullet - Client");
        var networkManager = GetComponent<NetworkManager>();
        networkManager.enabled = true;
        var transport = GetComponent<UnityTransport>();
        transport.enabled = true;
        // Wait a frame for setups to be done
        yield return null;
        if (networkManager.StartClient())
        {
            Debug.Log($"Connecting on port {transport.ConnectionData.Port}...");
        }
        else
        {
            Debug.LogError($"Failed to connect on port {transport.ConnectionData.Port}...");
        }
    }
```

- Server

```
private void Start()
{

   StartAsServerCR();

}
```

```
IEnumerator StartAsServerCR()
{
    SetWindowTitle("BitBullet - Server");
    var networkManager = GetComponent<NetworkManager>();
    networkManager.enabled = true;
    var transport = GetComponent<UnityTransport>();
    transport.enabled = true;
    // Wait a frame for setups to be done
    yield return null;


    NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;

    if (networkManager.StartServer())
    {
        Debug.Log($"Serving on port {transport.ConnectionData.Port}...");
    }
    else
    {
        Debug.LogError($"Failed to serve on port {transport.ConnectionData.Port}...");
    }
}
```

The `OnClientConnected` method is called when a player connects to the server.
When a player connects to the server a prefab Player is instantiated and given
the authority to the player.

```
private void OnClientConnected(ulong clientId)
{
    var spawnedPlayer = Instantiate(_character[_playerCount], _levelController.GetSpawnPoint(clientId), Quaternion.identity);


        
    NetworkObject nto = spawnedPlayer.GetComponent<NetworkObject>();
    nto.SpawnAsPlayerObject(clientId);


    _textMeshProUGUI.text = "Spawned Character";
    _playerCount = (_playerCount+ 1) % _character.Length;
}
```

### Camera

To give each player its own camera we need to see if the client is the owner
of the camera and if it is we enable it.
```
if (IsOwner)
        {
            _camera.enabled = true;
            _camera.depth = 1;
        }
        else
        {
            _camera.depth = 0;
        }
```

The reason that we don't use the `NetworkTransform` to update the rotation
is because it causes conflict with the `NetworkTransform` of the parent so 
when you rotate the character wouldn't rotate the camera.

### Character 

When it comes to the character, as said before, the player as authority over the
prefab but the server is the owner. This way only the server can make the prefab move and
the player gives the inputs, thus avoiding cheating from the player side.

```

private void Update()
{
    if (_networkObject.IsLocalPlayer)
    {
        UpdateLocalRotation();
        UpdateLocalHead();
        GetlocalInput();
    }
}
```

After we check if the input is different from the before to avoid sending to much information,
we call the method `(Method name)ServerRpc`.
This method calls a method that only runs in the server.
As the example from the code shows.

```
private void GetlocalInput()
{
    Vector2 oldInput = _input;
    if(_networkObject.IsLocalPlayer)
    {
        _input.x = Input.GetAxis("Strafe");
        _input.y = Input.GetAxis("Forward");
    }

    if (oldInput != _input)
        UpdateInputServerRpc(_input);

    if (Input.GetButton("Jump") && !_isOnAir)
    {
        Jump();
    }


    if (Input.GetButtonUp("Fire1"))
    {
        _weapon.Shoot(transform.position, transform.forward);
        _UI.UpdateAmmunitionClientParams(_weapon.CurrentAmmunition, _weapon.TotalAmmunition,_networkObject.OwnerClientId);
    }

}
```

```
[ServerRpc]
private void UpdateInputServerRpc(Vector2 input)
{
    if(NetworkManager.Singleton.IsServer)
        _networkInput.Value = input;
}
```
In the `FixedUpdate` method we check if is the server running the code.
If is, then we update the velocity and position of the character.
The `NetworkTranform` component will automatically update the transform for the 
client.
```
private void FixedUpdate()
{
    if (NetworkManager.Singleton.IsServer)
    {
        Collider[] collider = Physics.OverlapSphere(_floorDetectorPos.position, _floorDectecorRadius, _floorDetectionLayerMask);
        if (collider.Length == 0)
            _isOnAir = true;
        else
            _isOnAir = false;
        UpdateVelocity();
        UpdatePosition();
    }

}
```
# UI
The UI runs mostly in the server and when its time to update in the client we
call the method `UpdateUpdateHPClientRpc` this method uses the ClientRPC that,
makes the server call a method from the clients. Normally any ClientRPC method
is called in all client but in this one we have the parameter that defines which
client to note.
We can update the UI in the client side because even if the player cheats only
doesn't affect the gameplay since it all runs in the server.

```
[ClientRpc]
private void UpdateUpdateHPClientRpc(int currentHP, ClientRpcParams clientParams)
{
    UpdateLocalHp(currentHP);
}

```

## Network architecture diagram

This is the diagram of the network architecture

![Network](ReportImages/Network.png)

## Webgraphy

The information used was from the class "Sistemas de Redes para Jogos" that was 
lectured by professor **Diogo Andrade** and some video tutorials from Flarvain channel.

- [Video Classes](https://www.youtube.com/watch?v=3LdnSbCcrd0&list=PLheBz0T_uVP1ha7-M1p6ToCzSZEogulxJ)
- [Flarvain - Multiplayer [2022]](https://www.youtube.com/watch?v=6TRguZE1gGY&list=PL8K0QjCk8Zmi56rAL7KDjB8aZotCGn4T2&index=1)


