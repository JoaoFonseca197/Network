using TMPro;
using Unity.Netcode;
using UnityEngine;

public class UI : NetworkBehaviour
{
    
    [SerializeField] TextMeshProUGUI _textHP;
    [SerializeField] TextMeshProUGUI _textAmmunition;

    //private Character _character;
    //private Gun _gun;
    //private void Awake()
    //{
    //    if(NetworkManager.Singleton.IsClient)
    //    {
    //        _character = FindFirstObjectByType<Character>();
    //        _gun = _character.GetComponent<Gun>();
    //        Debug.Log("This is player name = " + _character.gameObject.name);
    //        Debug.Log("This is gun player name = " + _gun.gameObject.name);
    //    }

        
    //}
    private void OnEnable()
    {
        //if( _character != null )
        //{
        //    _character.UpdateHp += UpdateHPClientRpc;
        //    _gun.UpdateAmmunition += UpdateAmmunitionClientRpc;
        //}
        //NetworkManager.Singleton.OnClientConnectedCallback += GetPlayerComponents;
    }
    //private void GetPlayerComponents(ulong clientid)
    //{
    //    NetworkObject nto = NetworkManager.Singleton.ConnectedClientsList[];
    //    _gun = clientid.GetComponent<Gun>();
    //    _character = clientid.GetComponent<Character>();

    //}
    public void UpdateLocalHp(int currentHP)
    {
        UpdateHPClientRpc(currentHP);

    }
    [ClientRpc]
    public void UpdateHPClientRpc(int currentHP)
    {
        _textHP.text = $"{currentHP}/100";
        if (NetworkManager.Singleton.IsServer)
            _textHP.color = Color.yellow;
    }

    public void UpdateAmmunition(int currentAmmunition, int totalAmmunition)
    {
        _textAmmunition.text = $"{currentAmmunition}/{totalAmmunition}";
    }
    [ClientRpc]
    public void UpdateAmmunitionClientRpc(int currentAmmunition, int totalAmmunition)
    {
        _textAmmunition.text = $"{currentAmmunition}/{totalAmmunition}";
    }
}
