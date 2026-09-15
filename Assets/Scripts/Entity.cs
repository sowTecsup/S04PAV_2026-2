using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    private BaseStats stats;

    private void Awake()
    {
        stats = new(10, 10, 10);
    }

    public abstract void TakeDamage(int damage);
    public abstract void AttackSmt(Entity entity);
    public abstract void OnDead();



    public BaseStats Stats => stats;
}
