using TMPro;
using Unity.Netcode;
using UnityEngine;

public class UI : NetworkBehaviour
{
    
    [SerializeField] TextMeshProUGUI _textHP;
    [SerializeField] TextMeshProUGUI _textAmmunition;

    private ClientRpcParams _clientRpcParams;



    private void Awake()
    {
        _clientRpcParams = new ClientRpcParams();
    }

    
    public void UpdateHPClientParams(int currentHP,ulong clientID)
    {
        _clientRpcParams.Send.TargetClientIds = new ulong[] { clientID };

        UpdateUpdateHPClientRpc(currentHP, _clientRpcParams);

    }
    [ClientRpc]
    private void UpdateUpdateHPClientRpc(int currentHP, ClientRpcParams clientParams)
    {
        UpdateLocalHp(currentHP);
    }
    public void UpdateLocalHp(int currentHP)
    {
        _textHP.text = $"{currentHP}/100";

    }
    

    public void UpdateAmmunitionClientParams(int currentAmmunition,int totalAmmunition, ulong clientID)
    {
        _clientRpcParams.Send.TargetClientIds = new ulong[] { clientID };

        UpdateAmmunitionClientRpc(currentAmmunition,totalAmmunition, _clientRpcParams);

    }
    [ClientRpc]
    public void UpdateAmmunitionClientRpc(int currentAmmunition, int totalAmmunition, ClientRpcParams clientParams)
    {
        UpdateAmmunition(currentAmmunition, totalAmmunition);
    }

    public void UpdateAmmunition(int currentAmmunition, int totalAmmunition)
    {
        _textAmmunition.text = $"{currentAmmunition}/{totalAmmunition}";
    }
}
