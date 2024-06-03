

public interface IPlayer 
{
    int HP      {  get; }
    bool IsDead {  get; }   

    void ReceiveDamage(int damage);

}
