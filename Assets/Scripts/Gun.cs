using UnityEngine;

public class Gun : MonoBehaviour , IGun
{
    [SerializeField] GunData _gunData;

    private int     _totalAmmunition;
    private int     _magAmmunition;
    private int     _currentBulletNbr;

    private void Awake()
    {
        _totalAmmunition = _gunData.totalAmmunition;
        _currentBulletNbr = _gunData.magAmmunition;
        _magAmmunition = _gunData.magAmmunition;
    }

    public  void Shoot(Vector3 origin,Vector3 direction)
    {
        Physics.Raycast(origin, direction,out RaycastHit hit, _gunData.bulletDistance,_gunData.layerMask);
        if(_gunData.magAmmunition <= 0)
            Reload();
        else
        {
            if (hit.collider != null)
            {
                hit.transform.GetComponent<IPlayer>().ReceiveDamage(_gunData.gunDamage);
                _currentBulletNbr--;
            }

        }
        print("Shoot");
    }

    private void Reload()
    {

    }
    

}
