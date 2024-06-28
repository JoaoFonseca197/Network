using System;
using Unity.Netcode;
using UnityEngine;

public class Gun : NetworkBehaviour , IGun
{
    [SerializeField] GunData _gunData;

    private UI      _UI;
    private int     _totalAmmunition;
    private int     _currentAmmunition;
    private NetworkObject _networkObject;

    public int CurrentAmmunition => _currentAmmunition;
    public int TotalAmmunition => _totalAmmunition;

    public Action<int,int> UpdateAmmunition;

    private void Awake()
    {
        _networkObject = GetComponentInParent<NetworkObject>();
        _totalAmmunition = _gunData.totalAmmunition;
        _currentAmmunition = _gunData.magAmmunition;
        _UI = FindFirstObjectByType<UI>();
    }

    public  void Shoot(Vector3 origin,Vector3 direction)
    {
        ShootServerRpc(origin, direction);
    }


    [ServerRpc]
    public void ShootServerRpc(Vector3 origin, Vector3 direction)
    {
        Physics.Raycast(origin, direction, out RaycastHit hit, _gunData.bulletDistance, _gunData.layerMask);

        
        if (hit.collider != null)
        {
            hit.transform.GetComponent<ICharacter>().ReceiveDamage(_gunData.gunDamage);
        }
        _currentAmmunition--;
        if (_currentAmmunition == 0)
            Reload();


        _UI.UpdateAmmunitionClientParams(_currentAmmunition, _totalAmmunition, _networkObject.OwnerClientId);
    }

    private void Reload()
    {
        if (_totalAmmunition == 0)
            return;
        else if (_totalAmmunition > 0 && _totalAmmunition < _gunData.magAmmunition)
        {
            _currentAmmunition = _totalAmmunition;
            _totalAmmunition = 0;
        }
        else if(_totalAmmunition > _gunData.magAmmunition)
        {
            _currentAmmunition = _gunData.magAmmunition;
            _totalAmmunition -= _gunData.magAmmunition;
        }

    }

    //Simplified Respawn
    public void Respawn()
    {
        _totalAmmunition = _gunData.totalAmmunition;
        _currentAmmunition = _gunData.magAmmunition;
        _UI.UpdateAmmunitionClientParams(_currentAmmunition, _totalAmmunition, _networkObject.OwnerClientId);
    }
    

}
