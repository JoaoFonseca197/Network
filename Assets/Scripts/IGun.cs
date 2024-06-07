
using UnityEngine;

public interface IGun 
{
    public int CurrentAmmunition { get; }
    public int TotalAmmunition  { get; }
    void Shoot(Vector3 origin,Vector3 direction);

}
