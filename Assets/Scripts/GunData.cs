using UnityEngine;

[CreateAssetMenu(fileName = "GunData", menuName = "ScriptableObjects/GunData", order = 1)]
public class GunData : ScriptableObject
{
    public int          totalAmmunition;
    public int          magAmmunition;
    public float        fireRatio;
    public float        reloadTime;
    public float        bulletDistance;
    public LayerMask    layerMask;
    public int          gunDamage;

    
}
