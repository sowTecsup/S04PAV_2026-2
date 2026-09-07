using UnityEngine;

/// <summary>
/// CASO 01 — El contrato que obliga.
///
/// La conclusion primero: "virtual" es una SUGERENCIA y "abstract" es un
/// CONTRATO. Con virtual, la clase hija puede olvidarse de implementar su
/// version y el proyecto compila igual; el fallo aparece en clase, jugando,
/// cuando el slime de fuego recibe el dano generico y nadie entiende por que.
/// Con abstract el compilador no te deja llegar hasta ahi.
///
/// Esa es toda la diferencia, y es la razon de que el hito 1 diga "obligar a
/// que cada clase hija implemente su propio TakeDamage()". Obligar es la
/// palabra: no se pide que puedan, se pide que tengan que.
///
/// Tres rondas: la tibia (virtual, y el olvido pasa), la estricta (abstract, y
/// el olvido no compila) y la que reaparte trabajo con base.TakeDamage().
/// </summary>
public class Caso01_ElContratoQueObliga : MonoBehaviour
{
    [ContextMenu("Ejecutar caso")]
    public void Ejecutar()
    {
        Debug.Log("=== CASO 01 · virtual sugiere, abstract obliga ===");

        RondaTibia();
        RondaEstricta();
        RondaConBase();
    }

    // ---------------------------------------------------------------- ronda A

    /// El olvido silencioso. SlimeOlvidadizo hereda de una base con TakeDamage
    /// virtual y NO lo sobrescribe. Compila, corre, y hace lo que no toca.
    private void RondaTibia()
    {
        EntidadTibia agua = new SlimeDeAguaTibio();
        EntidadTibia fuego = new SlimeOlvidadizo();

        Debug.Log("[A] slime de agua, 10 de dano de fuego -> " + agua.TakeDamage(10) +
                  "   (el agua arde: doble)");
        Debug.Log("[A] slime OLVIDADIZO, 10 de dano de fuego -> " + fuego.TakeDamage(10) +
                  "   <-- 10, el dano generico. Nadie aviso de nada.");
    }

    // ---------------------------------------------------------------- ronda B

    /// El contrato. La base no trae cuerpo, asi que la hija esta obligada.
    private void RondaEstricta()
    {
        EntidadEstricta piedra = new GolemDePiedra();
        EntidadEstricta viento = new ElementalDeViento();

        Debug.Log("[B] golem de piedra, 10 de dano de fuego -> " + piedra.TakeDamage(10) +
                  "   (la piedra aguanta: mitad)");
        Debug.Log("[B] elemental de viento, 10 de dano de fuego -> " + viento.TakeDamage(10) +
                  "   (el viento lo esquiva: cero)");
        Debug.Log("[B] descomenta 'GolemOlvidadizo' al final del archivo: NO COMPILA. " +
                  "Ese error rojo es el que te salva la partida.");
    }

    // ---------------------------------------------------------------- ronda C

    /// Obligar no es duplicar. Lo comun se escribe UNA vez en la base y la hija
    /// la llama con base.TakeDamage(...) antes de poner lo suyo.
    private void RondaConBase()
    {
        EntidadEstricta blindado = new GolemBlindado();
        Debug.Log("[C] golem BLINDADO, 10 de dano de fuego -> " + blindado.TakeDamage(10) +
                  "   (mitad por piedra, y encima -2 de armadura, desde base)");
    }
}

// ===========================================================================
// A · virtual: hay cuerpo por defecto, y por eso el olvido no se nota
// ===========================================================================

public class EntidadTibia
{
    public virtual int TakeDamage(int cantidad)
    {
        return cantidad;   //-> el dano generico, el que se cuela cuando nadie sobrescribe
    }
}

public class SlimeDeAguaTibio : EntidadTibia
{
    public override int TakeDamage(int cantidad) { return cantidad * 2; }
}

public class SlimeOlvidadizo : EntidadTibia
{
    // Aqui faltaba el override. El proyecto compila igual: ese es el problema.
}

// ===========================================================================
// B · abstract: no hay cuerpo, asi que la hija TIENE que escribirlo
// ===========================================================================

public abstract class EntidadEstricta
{
    /// Sin llaves y sin cuerpo. Esta linea es el contrato.
    public abstract int TakeDamage(int cantidad);
}

public class GolemDePiedra : EntidadEstricta
{
    public override int TakeDamage(int cantidad) { return cantidad / 2; }
}

public class ElementalDeViento : EntidadEstricta
{
    public override int TakeDamage(int cantidad) { return 0; }
}

// ===========================================================================
// C · lo comun vive en la base y la hija lo reutiliza
// ===========================================================================

public abstract class EntidadConArmadura : EntidadEstricta
{
    public int armadura = 2;

    /// Deja de ser abstract: aqui ya hay algo que decir para todos.
    public override int TakeDamage(int cantidad)
    {
        int tras = cantidad - armadura;
        return tras < 0 ? 0 : tras;
    }
}

public class GolemBlindado : EntidadConArmadura
{
    public override int TakeDamage(int cantidad)
    {
        int mitad = cantidad / 2;              //-> lo suyo, de piedra
        return base.TakeDamage(mitad);         //-> y lo de la base, sin copiarlo
    }
}

// ---------------------------------------------------------------------------
// DESCOMENTA ESTO EN CLASE. Es el error del que va la semana:
//   "GolemOlvidadizo no implementa el miembro abstracto heredado
//    EntidadEstricta.TakeDamage(int)"
// ---------------------------------------------------------------------------
// public class GolemOlvidadizo : EntidadEstricta
// {
// }
