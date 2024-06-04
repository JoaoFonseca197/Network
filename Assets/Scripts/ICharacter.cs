

public interface ICharacter 
{
    int HP      {  get; }
    bool IsDead {  get; }   

    void ReceiveDamage(int damage);

}
