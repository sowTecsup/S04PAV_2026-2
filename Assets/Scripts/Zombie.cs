using UnityEngine;

public class Zombie : BaseEnemy
{
    public override void AttackSmt(Entity entity)
    {
        base.AttackSmt(entity);
        Debug.Log("hago cierto daño y enveneno");
    }
    public override void OnDead()
    {
        base.OnDead();
        Debug.Log("suelto carne podrida");
    }
    public override void ChaseEntity(Entity entity)
    {
        base.ChaseEntity(entity);
        Debug.Log("Mientras grito wahhh");
    }
    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
    }
}
