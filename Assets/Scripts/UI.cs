using TMPro;
using Unity.Netcode;
using UnityEngine;

public class UI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _textHP;
    [SerializeField] TextMeshProUGUI _textAmmunition;


    [ClientRpc]
    public void UpdateHPClientRpc(int currentHP)
    {
        _textHP.text = $"{currentHP}/100";
        if (NetworkManager.Singleton.IsServer)
            _textHP.color = Color.yellow;
    }
    [ClientRpc]
    public void UpdateAmmunitionClientRpc(int currentAmmunition, int totalAmmunition)
    {
        _textAmmunition.text = $"{currentAmmunition}/{totalAmmunition}";
    }
}
