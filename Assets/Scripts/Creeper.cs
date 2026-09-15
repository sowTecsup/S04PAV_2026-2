using UnityEngine;

public class Creeper : BaseEnemy
{
    public override void AttackSmt(Entity entity)
    {
        base.AttackSmt(entity);
        Debug.Log("exploto");
    }
    public override void OnDead()
    {
        base.OnDead();
        Debug.Log("suelto polvora");
    }
    public override void ChaseEntity(Entity entity)
    {
        base.ChaseEntity(entity);
        Debug.Log("en silencio!");
    }
    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
    }
}
