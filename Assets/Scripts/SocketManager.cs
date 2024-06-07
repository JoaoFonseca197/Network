using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class SocketManager : MonoBehaviour
{
    [SerializeField] private int _port;

    [SerializeField] private Image _imageTCP;
    [SerializeField] private Image _imageUDP;
    [SerializeField] private Server _server;


    private Socket _listenerTCP;
    private Socket _listenerUDP;

    private Socket _clientSocketTcp;

    private byte[] _buffer;

    private IPEndPoint  _ipEndPoint;
    private EndPoint    _endPoint;

    private List<Socket> _clientSockets;

    private void Awake()
    {
        _buffer = new byte[256];
        _ipEndPoint = new IPEndPoint(IPAddress.Any, _port);
        _endPoint = new IPEndPoint(IPAddress.Any,_port);
    }

    private void Start()
    {
        
    }
    private void Update()
    {
        if (_listenerTCP == null)
        {
            _imageTCP.color = Color.red;
            OpenTCPConnection();
        }
        if (_listenerUDP == null)
        {
            _imageUDP.color = Color.red;
            OpenUDPConnection();
        }
        else
        {
            _imageTCP.color = Color.yellow;
            if (_clientSocketTcp == null)
            {
                _imageTCP.color = Color.yellow;

                try
                {
                    byte[] buffer = new byte[4];

                    Socket connectedSocket = _listenerTCP.Accept();
                    _clientSockets.Add( connectedSocket);
                    if(connectedSocket.ReceiveFrom(buffer, ref _endPoint)>4)
                    {
                        byte[] len = new byte[4]; 
                    }
                    connectedSocket.SendTo(buffer, connectedSocket.RemoteEndPoint);
                }
                catch (SocketException e)
                {
                    if (e.SocketErrorCode == SocketError.WouldBlock)
                    {
                        // The error was that this operation would block
                        // That's to be expected in our case while we don't have a a connection
                        return;
                    }
                    else
                    {
                        Debug.LogError(e);
                    }
                }
            }
            else
            {
                _imageTCP.color = Color.green;
                ReceiveCommands();
            }
        }
        
        try
        {
            if (_listenerUDP != null)
                _imageUDP.color = Color.yellow;

            if (_listenerUDP.ReceiveFrom(_buffer, ref _endPoint) > 4)
            {
                Debug.Log(_buffer);
                _imageUDP.color = Color.green;
            }
        }
        catch (SocketException e)
        {
            if(e.ErrorCode == 10035)
            {
                //NOthing
            }
            else
            {
                throw e;
            }
        }
    }

    private void ReceiveCommands()
    {

        if (_clientSocketTcp.Connected)
        {

            var lenBytes = new byte[4];
            int nBytes;
            foreach(Socket socket in _clientSockets)
            {
                nBytes = Receive(lenBytes, true);

                if (nBytes == 4)
                {
                    // Convert lenBytes from 4 bytes to a Uint32
                    UInt32 commandLen = BitConverter.ToUInt32(lenBytes);

                    var commandBytes = new byte[commandLen];
                    nBytes = Receive(commandBytes, false);

                    if (nBytes == commandLen)
                    {
                        string command = Encoding.ASCII.GetString(commandBytes);
                        if (command == "up")
                        {
                            transform.position += Vector3.up * 0.25f;
                        }
                        else if (command == "down")
                        {
                            transform.position += Vector3.down * 0.25f;
                        }
                        else if (command == "right")
                        {
                            transform.position += Vector3.right * 0.25f;
                        }
                        else if (command == "left")
                        {
                            transform.position += Vector3.left * 0.25f;
                        }
                        else
                        {
                            Debug.LogError($"Unknown command {command}!");
                        }
                    }
                }
                else
                {
                    try
                    {
                        if (_clientSocketTcp.Poll(1, SelectMode.SelectRead))
                        {
                        }
                    }
                    catch (SocketException e)
                    {
                        Debug.LogError(e);

                        // Close the socket if it's not connected anymore
                        _clientSocketTcp.Close();
                        _clientSocketTcp = null;
                    }
                }
            }

           
        }
        else
        {
            // Close the socket if it's not connected anymore
            _clientSocketTcp.Close();
            _clientSocketTcp = null;
        }
    }

    private int Receive(byte[] data, bool accountForLittleEndian = true)
    {
        try
        {
            // Normal path - received something

            if (accountForLittleEndian && (!BitConverter.IsLittleEndian))
                Array.Reverse(data);

        }
        catch (SocketException e)
        {
            if (e.SocketErrorCode == SocketError.WouldBlock)
            {
                // Didn't receive any data, just return 0
                return 0;
            }
            else
            {
                // Error => log it
                Debug.LogError(e);
            }
        }
        // Return -1 if there's an error
        return -1;
    }

    private void OpenTCPConnection()
    {
        try
        {
            _listenerTCP = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            _listenerTCP.Bind(_ipEndPoint);

            _listenerTCP.Listen(2);

            _listenerTCP.Blocking = false;
        }
        catch(SocketException e)
        {
            Debug.Log(e);
        }
    }

    private void OpenUDPConnection()
    {
        try
        {
            _listenerUDP = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            _listenerUDP.Bind(_ipEndPoint);

            _listenerUDP.Blocking = false;
        }
        catch (SocketException e)
        {
            Debug.Log(e);
        }
    }

    private void SetBytes(UInt32 value, byte[] bytes, int offset)
    {
        byte[] lenBytes = BitConverter.GetBytes(value);

        // Ensure little-endian byte order
        if (!BitConverter.IsLittleEndian)
            Array.Reverse(lenBytes);

        // Copy the length bytes into the first 4 bytes of the 'bytes' array
        Array.Copy(lenBytes, 0, bytes, offset, lenBytes.Length);
    }
}
