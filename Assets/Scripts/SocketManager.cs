using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;

public class SocketManager : MonoBehaviour
{
    [SerializeField] private int _port;

    [SerializeField] private Image _imageTCP;
    [SerializeField] private Image _imageUDP;


    private Socket _listenerTCP;
    private Socket _listenerUDP;

    private Socket _clientSocketTcp;
    private Socket _clientSocketUDP;
    private void Update()
    {
        void Update()
        {
            if (_listenerTCP == null)
            {
                _imageTCP.color = Color.red;
                OpenConnection();
            }
            else
            {
                _imageTCP.color = Color.yellow;
                if (_clientSocketTcp == null)
                {
                    _imageTCP.color = Color.yellow;

                    try
                    {
                        _clientSocketTcp = _listenerTCP.Accept();
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
                }
            }

            if (_listenerUDP == null)
            {
                _imageUDP.color = Color.red;
                OpenConnection();
            }
            else
            {
                _imageUDP.color = Color.yellow;
                if (_clientSocketUDP == null)
                {
                    _imageUDP.color = Color.yellow;
                    try
                    {
                        _clientSocketUDP = _listenerUDP.Accept();
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
                    _imageUDP.color = Color.green;
                }
            }
        }

    }

    private void OpenConnection()
    {
        try
        {
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, _port);


            _listenerTCP = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _listenerUDP = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Udp);

            _listenerTCP.Bind(localEndPoint);
            _listenerUDP.Bind(localEndPoint);

            _listenerTCP.Listen(1);
            _listenerUDP.Listen(1);

            _listenerTCP.Blocking = false;
            _listenerUDP.Blocking = false;
        }
        catch(SocketException e)
        {
            Debug.Log(e);
        }
    }
}
