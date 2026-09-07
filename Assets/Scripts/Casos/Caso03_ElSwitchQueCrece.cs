using UnityEngine;

/// <summary>
/// CASO 03 — El switch que crece. ⭐ Cierra la limitacion de la semana 3.
///
/// La conclusion primero: la pregunta no es "que dano hace el fuego", es QUIEN
/// decide. Cuando lo decide el atacante con una cadena de if por tipo de
/// enemigo, cada enemigo nuevo se anade tocando el atacante — y el que se
/// olvida cae en el else y recibe el dano generico sin que nadie avise. Es
/// exactamente la cadena de ifs de ColorDelTipo que quedo escrita y sin
/// parchear en el repositorio de la semana 3.
///
/// Cuando lo decide cada entidad en su propio override, el atacante no conoce
/// tipos: dice "toma esto" y se va. Un enemigo nuevo es un archivo nuevo y
/// CERO archivos tocados. Eso es lo que pide el hito 2.
///
/// La ronda C deja escrita la limitacion que queda: el switch se repite en cada
/// entidad. Se borra con polimorfismo (semana 7) y con datos (semana 13). Hoy
/// no tienes esa herramienta, y esta bien: hoy toca ver por que hace falta.
/// </summary>
public class Caso03_ElSwitchQueCrece : MonoBehaviour
{
    [ContextMenu("Ejecutar caso")]
    public void Ejecutar()
    {
        Debug.Log("=== CASO 03 · quien decide cuanto duele ===");

        RondaAtacanteSabelotodo();
        RondaCadaUnoLoSuyo();
        RondaLoQueQueda();
    }

    // ---------------------------------------------------------------- ronda A

    /// El atacante lo sabe todo. Funciona... hasta que llega un tipo nuevo.
    private void RondaAtacanteSabelotodo()
    {
        Debug.Log("[A] atacante con cadena de if, golpe de FUEGO de 10:");
        Debug.Log("[A]   slime de agua -> " + DanoSegunTipo("agua", 10));
        Debug.Log("[A]   golem         -> " + DanoSegunTipo("golem", 10));
        Debug.Log("[A]   MURCIELAGO    -> " + DanoSegunTipo("murcielago", 10) +
                  "   <-- 10. Nadie escribio su caso y cayo en el else.");
        Debug.Log("[A] archivos que hay que tocar para anadir el murcielago bien: 2 " +
                  "(el suyo y el del atacante).");
    }

    /// La cadena. Fea a proposito: asi es como sale la primera vez.
    private int DanoSegunTipo(string tipo, int cantidad)
    {
        if (tipo == "agua") return cantidad * 2;
        else if (tipo == "golem") return cantidad / 2;
        else return cantidad;      //-> el agujero por donde se cuelan los nuevos
    }

    // ---------------------------------------------------------------- ronda B

    /// Cada entidad decide. El atacante solo reparte.
    private void RondaCadaUnoLoSuyo()
    {
        BichoDelCaso[] bichos = { new SlimeDeAgua(), new GolemDeRoca(), new Murcielago() };

        Debug.Log("[B] el atacante ya no conoce tipos. Golpe de FUEGO de 10:");
        foreach (BichoDelCaso bicho in bichos)
            Debug.Log("[B]   " + bicho.Nombre + " -> " + bicho.TakeDamage(TipoDeGolpe.Fuego, 10));

        Debug.Log("[B] y el mismo golpe de AGUA de 10:");
        foreach (BichoDelCaso bicho in bichos)
            Debug.Log("[B]   " + bicho.Nombre + " -> " + bicho.TakeDamage(TipoDeGolpe.Agua, 10));

        Debug.Log("[B] archivos que hay que tocar para anadir un cuarto bicho: 0. " +
                  "Se anade el suyo y ya.");
    }

    // ---------------------------------------------------------------- ronda C

    /// Lo que NO arregla la abstraccion. Que quede escrito.
    private void RondaLoQueQueda()
    {
        Debug.Log("[C] cuenta los switch: hay uno por entidad, y los cinco casos " +
                  "repetidos en cada uno.");
        Debug.Log("[C] si manana entra el elemento Rayo, hay que abrir las 3 entidades. " +
                  "Con 12 enemigos serian 12.");
        Debug.Log("[C] eso NO se arregla con abstraccion. Se arregla con polimorfismo " +
                  "(semana 7) y sacando los numeros a datos (semana 13). Hoy se queda asi.");
    }
}

/// El enum del hito 2. Se llama TipoDeGolpe y no Elements para que no choque
/// con el que vas a escribir tu en el laboratorio.
public enum TipoDeGolpe
{
    Ninguno,
    Fuego,
    Agua,
    Tierra,
    Aire
}

// ===========================================================================
// La base abstracta: obliga a implementar, y nada mas
// ===========================================================================

public abstract class BichoDelCaso
{
    public abstract string Nombre { get; }

    /// Devuelve el dano que se lleva de verdad. La entidad decide, no quien pega.
    public abstract int TakeDamage(TipoDeGolpe golpe, int cantidad);
}

public class SlimeDeAgua : BichoDelCaso
{
    public override string Nombre => "slime de agua";

    public override int TakeDamage(TipoDeGolpe golpe, int cantidad)
    {
        switch (golpe)
        {
            case TipoDeGolpe.Fuego: return cantidad * 2;    //-> el agua hierve
            case TipoDeGolpe.Agua: return 0;                //-> es de lo mismo
            case TipoDeGolpe.Tierra: return cantidad;
            case TipoDeGolpe.Aire: return cantidad;
            default: return cantidad;
        }
    }
}

public class GolemDeRoca : BichoDelCaso
{
    public override string Nombre => "golem de roca";

    public override int TakeDamage(TipoDeGolpe golpe, int cantidad)
    {
        switch (golpe)
        {
            case TipoDeGolpe.Fuego: return cantidad / 2;
            case TipoDeGolpe.Agua: return cantidad * 3;     //-> la roca se erosiona
            case TipoDeGolpe.Tierra: return 0;
            case TipoDeGolpe.Aire: return cantidad;
            default: return cantidad;
        }
    }
}

public class Murcielago : BichoDelCaso
{
    public override string Nombre => "murcielago";

    public override int TakeDamage(TipoDeGolpe golpe, int cantidad)
    {
        switch (golpe)
        {
            case TipoDeGolpe.Fuego: return cantidad;
            case TipoDeGolpe.Agua: return cantidad;
            case TipoDeGolpe.Tierra: return 0;              //-> vuela, no lo alcanza
            case TipoDeGolpe.Aire: return cantidad * 2;
            default: return cantidad;
        }
    }
}
