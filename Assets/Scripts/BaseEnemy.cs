using UnityEngine;

public abstract class BaseEnemy : Entity
{
    public override void AttackSmt(Entity entity)
    {
        Debug.Log("ataque base");
    }

    public override void OnDead()
    {
        Debug.Log("no dropeo nada");
    }

    public override void TakeDamage(int damage)
    {
        Debug.Log("ouch");
    }

    public virtual void ChaseEntity(Entity entity)
    {
        Debug.Log("Ven aqui pibble");
    }
}
