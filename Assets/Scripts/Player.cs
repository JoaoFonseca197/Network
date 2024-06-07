using UnityEngine;

public class Player : MonoBehaviour , IPlayer
{
    public int ID {  get;  set; }

    public string Name { get; set; }

    public Player (int iD, string name)
    {
        ID = iD;
        Name = name;
    }
}
