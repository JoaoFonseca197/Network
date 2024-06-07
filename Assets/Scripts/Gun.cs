using Unity.Netcode;
using UnityEngine;

public class Gun : NetworkBehaviour , IGun
{
    [SerializeField] GunData _gunData;

    private int     _totalAmmunition;
    private int     _currentAmmunition;

    public int CurrentAmmunition => _currentAmmunition;
    public int TotalAmmunition => _totalAmmunition;

    private void Awake()
    {
        _totalAmmunition = _gunData.totalAmmunition;
        _currentAmmunition = _gunData.magAmmunition;
    }

    public  void Shoot(Vector3 origin,Vector3 direction)
    {
        //Physics.Raycast(origin, direction,out RaycastHit hit, _gunData.bulletDistance,_gunData.layerMask);
        
        //if(_gunData.magAmmunition <= 0)
        //    Reload();
        //else
        //{
        //    if (hit.collider != null)
        //    {
        //        hit.transform.GetComponent<ICharacter>().ReceiveDamage(_gunData.gunDamage);
        //    }
        //    _currentAmmunition--;
        //}
        ShootServerRpc(origin, direction);
    }


    [ServerRpc]
    public void ShootServerRpc(Vector3 origin, Vector3 direction)
    {
        Physics.Raycast(origin, direction, out RaycastHit hit, _gunData.bulletDistance, _gunData.layerMask);

        if (_gunData.magAmmunition <= 0)
            Reload();
        else
        {
            if (hit.collider != null)
            {
                hit.transform.GetComponent<ICharacter>().ReceiveDamage(_gunData.gunDamage);
            }
            _currentAmmunition--;
        }
    }

    private void Reload()
    {

    }
    

}
