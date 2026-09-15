using UnityEngine;

public class Spider : BaseEnemy
{
    public override void AttackSmt(Entity entity)
    {
        base.AttackSmt(entity);
        Debug.Log("empujo al atacar");
    }
    public override void OnDead()
    {
        base.OnDead();
        Debug.Log("suelto tela de araña");
    }
    public override void ChaseEntity(Entity entity)
    {
        base.ChaseEntity(entity);
        Debug.Log("salto mientras digo pssst");
    }
    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
    }
}
