using UnityEngine;

public class BaseStats 
{
    private int health;
    private int damage;
    private int speed;

    public BaseStats(int _health, int _damage,int _speed)
    {
        health = _health;
        damage = _damage;
        speed = _speed;
    }


    public int Health => health;
    public int Damage => damage;
    public int Speed => speed;
}
