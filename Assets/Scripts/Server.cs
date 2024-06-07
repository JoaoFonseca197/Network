using System.Collections.Generic;
using UnityEngine;

public class Server : MonoBehaviour
{
    [SerializeField] private SocketManager _socketManager;
    
    private List<IPlayer> _players = new List<IPlayer>();

    private int _newPlayerID;

    private void Awake()
    {
        _newPlayerID = 1;
    }
    public int ConnectPlayer(int playerID,string playerName)
    {
        if(playerID == 0)
        {
            _players.Add(new Player(_newPlayerID, playerName));
            _newPlayerID++;
            return _newPlayerID - 1;
        }
        else
        {
            return 0;
        }
    }
}
