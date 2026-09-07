using UnityEngine;

/// <summary>
/// CASO 02 — Datos o comportamiento. ⭐ El que decide como te queda el hito 1.
///
/// La conclusion primero: una entidad es lo que HACE; sus numeros son otra
/// cosa. Cuando los cinco enteros viven sueltos dentro del Player, y otra vez
/// dentro del Enemy, y otra vez dentro del Boss, la validacion tambien vive
/// tres veces — y se corrige dos.
///
/// BaseStats existe para eso: es una clase PURA (no MonoBehaviour) que guarda
/// los numeros y es la unica que sabe curarlos y restarlos. La entidad la tiene
/// dentro, la construye con constructor parametrizado y le pide las cosas.
///
/// Y la trampa que se lleva media clase: BaseStats NO puede ser MonoBehaviour.
/// Un MonoBehaviour no se crea con new — eso ya se vio en la semana 3 — asi que
/// "new BaseStats(10, 10, ...)" solo existe si BaseStats es una clase normal.
/// La ronda C lo ensena con el error de Unity delante, y con la trampa fina:
/// el objeto PARECE bueno —sus campos tienen los valores del inicializador—
/// pero para Unity no existe. Es null y no lo es al mismo tiempo.
/// </summary>
public class Caso02_DatosOComportamiento : MonoBehaviour
{
    [ContextMenu("Ejecutar caso")]
    public void Ejecutar()
    {
        Debug.Log("=== CASO 02 · los numeros no son la entidad ===");

        RondaSuelta();
        RondaConStats();
        RondaMonoBehaviour();
    }

    // ---------------------------------------------------------------- ronda A

    /// Los campos sueltos. Cada entidad repite los cinco enteros y repite —o se
    /// olvida de repetir— la regla de que la vida no pasa del maximo.
    private void RondaSuelta()
    {
        EnemigoSuelto malo = new EnemigoSuelto();
        malo.health = 30;
        malo.healthMax = 30;

        malo.health += 50;   //-> una pocion. Nadie mira el maximo: es un campo publico
        Debug.Log("[A] vida 30 de 30, se cura 50 -> " + malo.health + " de " + malo.healthMax +
                  "   <-- 80 de 30. El campo publico no defiende nada.");

        malo.health -= 200;
        Debug.Log("[A] le pegan 200 -> " + malo.health + "   <-- vida negativa, y sigue vivo");
    }

    // ---------------------------------------------------------------- ronda B

    /// La clase pura. Una sola puerta para entrar (el constructor) y dos para
    /// mover la vida (TakeDamage y Curar). Los campos son privados y las
    /// propiedades solo leen.
    private void RondaConStats()
    {
        EnemigoConStats malo = new EnemigoConStats(30, 7, 3, 1, 5);

        malo.Stats.Curar(50);
        Debug.Log("[B] vida 30 de 30, se cura 50 -> " + malo.Stats.Health + " de " +
                  malo.Stats.HealthMax + "   <-- topada en 30, y la regla esta UNA vez");

        malo.Stats.TakeDamage(200);
        Debug.Log("[B] le pegan 200 -> " + malo.Stats.Health + "   |  vivo: " + malo.Stats.Vivo);

        Debug.Log("[B] descomenta la linea marcada NO COMPILA dentro de este metodo: " +
                  "'health' es privado y la propiedad no tiene set.");
        // malo.Stats.Health = 999;   //-> NO COMPILA. Esta es la proteccion de verdad.

        EnemigoConStats otro = new EnemigoConStats(-40, 7, 3, 1, 5);
        Debug.Log("[B] uno construido con vida -40 -> " + otro.Stats.Health +
                  "   (el constructor pasa por el set y el set corrige)");
    }

    // ---------------------------------------------------------------- ronda C

    /// Por que BaseStats no lleva : MonoBehaviour. Se ve mejor rompiendolo.
    ///
    /// Y ojo con lo que se ve: el objeto parece bueno. El 30 esta puesto, porque
    /// el inicializador del campo lo pone C#, no Unity. Lo que falta es todo lo
    /// de Unity: no hay GameObject, no hay transform y Awake no corre nunca.
    private void RondaMonoBehaviour()
    {
        Debug.Log("[C] ahora sale un error rojo de Unity. Es el del caso, no lo has roto tu:");
        StatsComoComponente malo = new StatsComoComponente();

        Debug.Log("[C] new StatsComoComponente() -> health = " + malo.health +
                  "   <-- el 30 esta. Enganoso: eso lo puso C#, no Unity.");
        Debug.Log("[C] pero malo == null da " + (malo == null) +
                  "   <-- existe para C# y NO existe para Unity, a la vez.");

        try
        {
            GameObject suyo = malo.gameObject;
            Debug.Log("[C] y su gameObject es " + (suyo == null ? "null" : suyo.name));
        }
        catch (System.Exception e)
        {
            Debug.Log("[C] y pedirle su gameObject lanza " + e.GetType().Name +
                      ": no esta en ninguna escena.");
        }

        Debug.Log("[C] por eso BaseStats es una clase normal: para poder decir " +
                  "new BaseStats(10, 10, 10, 10, 10) en el Awake de la entidad.");
    }
}

// ===========================================================================
// A · los cinco enteros sueltos, uno por entidad, repetidos por todas
// ===========================================================================

public class EnemigoSuelto
{
    public int health;
    public int healthMax;
    public int power;
    public int speed;
    public int xp;
}

// ===========================================================================
// B · la clase pura de datos, con la regla dentro
// ===========================================================================

public class StatsDeEjemplo
{
    private int health;
    private int healthMax;
    private int power;
    private int speed;
    private int knockback;
    private int xp;

    /// Constructor parametrizado: no hay forma de tener stats a medias.
    /// Y asigna pasando por los set, no por el campo, para que la validacion
    /// tambien valga en el nacimiento.
    public StatsDeEjemplo(int health, int power, int speed, int knockback, int xp)
    {
        this.healthMax = health < 0 ? 0 : health;
        SetHealth(health);
        SetPower(power);
        SetSpeed(speed);
        SetKnockback(knockback);
        this.xp = xp;
    }

    public void SetHealth(int valor)
    {
        if (valor < 0) valor = 0;
        if (valor > healthMax) valor = healthMax;
        health = valor;
    }

    public void SetPower(int valor) { power = valor < 0 ? 0 : valor; }
    public void SetSpeed(int valor) { speed = valor < 0 ? 0 : valor; }
    public void SetKnockback(int valor) { knockback = valor < 0 ? 0 : valor; }

    public void TakeDamage(int cantidad) { SetHealth(health - cantidad); }
    public void Curar(int cantidad) { SetHealth(health + cantidad); }

    public int Health => health;
    public int HealthMax => healthMax;
    public int Power => power;
    public int Speed => speed;
    public int Knockback => knockback;
    public int XP => xp;
    public bool Vivo => health > 0;
}

public class EnemigoConStats
{
    private StatsDeEjemplo stats;

    public EnemigoConStats(int health, int power, int speed, int knockback, int xp)
    {
        stats = new StatsDeEjemplo(health, power, speed, knockback, xp);
    }

    public StatsDeEjemplo Stats => stats;
}

// ===========================================================================
// C · lo que pasa si los datos se hacen componente
// ===========================================================================

public class StatsComoComponente : MonoBehaviour
{
    public int health = 30;   //-> este 30 SI se pone. Lo que no llega es nada de Unity.
}
