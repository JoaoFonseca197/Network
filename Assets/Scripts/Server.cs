using System.Collections.Generic;
using UnityEngine;

public class Server : MonoBehaviour
{
    [SerializeField] private SocketManager _socketManager;
    
    private List<IPlayer> _players = new List<IPlayer>();

    public void ConnectPlayer()
    {

    }
}
