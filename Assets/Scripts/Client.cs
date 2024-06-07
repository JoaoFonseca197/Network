using System;
using System.Net.Sockets;
using System.Net;
using UnityEngine;
using System.Text;

public class Client : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] string _server;
    [SerializeField] string _name;
    private Socket _socketTCP;
    private Socket _socketUDP;

    private IPlayer _playerInfo;

    private void Awake()
    {
        _playerInfo = new Player(0, "Banana");
    }
    public void ConnectClient()
    {
        try
        {
            IPHostEntry ipHost = Dns.GetHostEntry(_server);
            IPAddress ipAddress = null;
            for (int i = 0; i < ipHost.AddressList.Length; i++)
            {
                if (ipHost.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                {
                    ipAddress = ipHost.AddressList[i];
                }
            }
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, 1234);

            // Create and connect the socket to the remote endpoint (TCP)
            _socketTCP = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _socketTCP.Connect(remoteEP);
            // Create and connect the socket to the remote endpoint (UDP)
            _socketUDP = new Socket(ipAddress.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
            _socketUDP.Connect(remoteEP);
        }
        catch (ArgumentNullException e)
        {
            Debug.Log($"ArgumentNullException: {e}");
        }
        catch (SocketException e)
        {
            Debug.Log($"SocketException: {e}");
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            ConnectClient();
            string command = _playerInfo.ID + " " + _playerInfo.Name;
            SendTCPMessage(command);
        }

        if (Input.GetKeyDown(KeyCode.S))
            SendTCPMessage("Banana");

        if (Input.GetKey(KeyCode.D))
            SendUDPMessage("WOW");
        if(Input.GetKeyDown(KeyCode.E))
        {
            _socketTCP.Shutdown(SocketShutdown.Both);
            _socketTCP.Close();
            
            _socketUDP.Shutdown(SocketShutdown.Both);
            _socketUDP.Close();
        }
        try
        {
            if(_socketTCP.Connected)
            {
                if(_playerInfo.ID == 0)
                {
                    _playerInfo.ID = ReceiveMessage();
                }
            }
        }
        catch(SocketException e)
        {
            Debug.Log(e);
        }

    }

    private void SendTCPMessage(string command)
    {
        UInt32 len = (UInt32)command.Length;

        var bytes = new byte[256];
        SetBytes(len, bytes, 0);
        SetBytes(command, bytes, 4);

        _socketTCP.Send(bytes, command.Length + 4, SocketFlags.None);
    }

    private int ReceiveMessage()
    {
       int value = _socketTCP.Receive(new byte[4]);

        return value;
    }

    private void SendUDPMessage(string command)
    {
        UInt32 len = (UInt32)command.Length;

        var bytes = new byte[256];
        SetBytes(len, bytes, 0);
        SetBytes(command, bytes, 4);

        _socketUDP.Send(bytes, command.Length + 4, SocketFlags.None);
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

    private void SetBytes(string value, byte[] bytes, int offset)
    {
        byte[] strBytes = Encoding.ASCII.GetBytes(value);

        // Copy the length bytes into the first 4 bytes of the 'bytes' array
        Array.Copy(strBytes, 0, bytes, offset, strBytes.Length);
    }
}

