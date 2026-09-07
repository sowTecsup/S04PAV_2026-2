using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// CASO 05 — Recoger no es chocar. Necesita Play.
///
/// La conclusion primero: el hito 4 parece el facil y es donde se pierden los
/// puntos tontos. Un trigger no distingue: te avisa de TODO lo que entra —el
/// suelo, un disparo, otra pocion— y eres tu quien decide si eso era el Player.
/// Y "solo el Player" no se comprueba por etiqueta, se comprueba pidiendo el
/// componente: la etiqueta es un texto y los textos se escriben mal.
///
/// La segunda mitad es la trampa de verdad, y viene de la semana 3: Destroy no
/// es inmediato. El objeto sigue vivo hasta el final del frame, asi que si dos
/// avisos llegan en el mismo frame, la pocion cura dos veces y la esfera de XP
/// se cobra dos veces. El contador lo dice.
///
/// El arreglo no es Destroy antes: es una bandera. Recoger es una accion que
/// solo puede pasar una vez, y eso lo decide el recolectable, no el reloj.
/// </summary>
public class Caso05_RecogerNoEsChocar : MonoBehaviour
{
    [ContextMenu("Ejecutar caso")]
    public void Ejecutar()
    {
        if (!Application.isPlaying)
        {
            Debug.LogWarning("[Caso05] necesita Play: fuera de Play no existe Destroy diferido " +
                             "y el doble cobro no se puede ensenar.");
            return;
        }

        Debug.Log("=== CASO 05 · recoger no es chocar ===");

        RondaLaBaseComun();
        RondaQuienEntra();
        RondaDobleCobro();
    }

    // ---------------------------------------------------------------- ronda A

    /// La base abstracta hace que el bucle no sepa —ni le importe— si lo que
    /// pisa es una esfera o una pocion. Cada uno sabe lo suyo.
    private void RondaLaBaseComun()
    {
        BolsilloDelCaso bolsillo = new BolsilloDelCaso(20, 30);

        List<RecolectableDelCaso> sueltos = new List<RecolectableDelCaso>
        {
            Crear<EsferaDeXP>("esfera 1"),
            Crear<PocionDeVida>("pocion 1"),
            Crear<EsferaDeXP>("esfera 2")
        };

        Debug.Log("[A] antes:  vida " + bolsillo.vida + "/" + bolsillo.vidaMax +
                  "   xp " + bolsillo.xp);

        foreach (RecolectableDelCaso item in sueltos)
            item.Recoger(bolsillo);      //-> una sola linea para los dos tipos

        Debug.Log("[A] despues: vida " + bolsillo.vida + "/" + bolsillo.vidaMax +
                  "   xp " + bolsillo.xp + "   (y el bucle no ha preguntado de que tipo era)");

        foreach (RecolectableDelCaso item in sueltos)
            if (item != null) DestroyImmediate(item.gameObject);
    }

    // ---------------------------------------------------------------- ronda B

    /// El trigger avisa de todo. Cuatro cosas pisan la pocion, una es el Player.
    private void RondaQuienEntra()
    {
        PocionDeVida pocion = Crear<PocionDeVida>("pocion vigilada");
        BolsilloDelCaso bolsillo = new BolsilloDelCaso(20, 30);

        GameObject[] visitas =
        {
            ConBolsillo("el jugador", bolsillo),
            new GameObject("un disparo"),
            new GameObject("una pared"),
            new GameObject("otro enemigo")
        };

        int cobradas = 0;
        foreach (GameObject visita in visitas)
        {
            bool valia = pocion.IntentarRecoger(visita);
            Debug.Log("[B] entra '" + visita.name + "' -> " + (valia ? "RECOGIDA" : "no era el"));
            if (valia) cobradas++;
        }

        Debug.Log("[B] avisos que llegaron: " + visitas.Length + "   |   recogidas: " + cobradas);
        Debug.Log("[B] la comprobacion es GetComponent<PortadorDeBolsillo>(), no la etiqueta: " +
                  "el componente no se escribe mal, el texto 'Player' si.");

        foreach (GameObject visita in visitas) DestroyImmediate(visita);
    }

    // ---------------------------------------------------------------- ronda C

    /// Dos avisos en el mismo frame. Con Destroy no basta; con bandera si.
    private void RondaDobleCobro()
    {
        BolsilloDelCaso sinBandera = new BolsilloDelCaso(20, 30);
        EsferaDeXP floja = Crear<EsferaDeXP>("esfera sin bandera");
        floja.usaBandera = false;

        floja.Recoger(sinBandera);
        Debug.Log("[C] esfera de 5 xp, primer aviso -> xp " + sinBandera.xp +
                  "   |  tras Destroy, == null da " + (floja == null) + " (sigue viva)");
        floja.Recoger(sinBandera);
        Debug.Log("[C] segundo aviso en el MISMO frame -> xp " + sinBandera.xp +
                  "   <-- 10 por una sola esfera. Ahi van tus puntos.");

        BolsilloDelCaso conBandera = new BolsilloDelCaso(20, 30);
        EsferaDeXP buena = Crear<EsferaDeXP>("esfera con bandera");

        buena.Recoger(conBandera);
        buena.Recoger(conBandera);
        Debug.Log("[C] la misma jugada con bandera -> xp " + conBandera.xp +
                  "   (el segundo aviso se va por la puerta de atras)");
    }

    // ------------------------------------------------------------------ utils

    private T Crear<T>(string nombre) where T : RecolectableDelCaso
    {
        return new GameObject(nombre).AddComponent<T>();
    }

    private GameObject ConBolsillo(string nombre, BolsilloDelCaso bolsillo)
    {
        GameObject go = new GameObject(nombre);
        go.AddComponent<PortadorDeBolsillo>().bolsillo = bolsillo;
        return go;
    }
}

/// Lo que lleva encima el jugador. Clase pura: son datos, como BaseStats.
public class BolsilloDelCaso
{
    public int vida;
    public int vidaMax;
    public int xp;

    public BolsilloDelCaso(int vida, int vidaMax)
    {
        this.vidaMax = vidaMax;
        this.vida = vida > vidaMax ? vidaMax : vida;
    }

    public void Curar(int cantidad)
    {
        vida += cantidad;
        if (vida > vidaMax) vida = vidaMax;
    }
}

/// El componente que dice "esto es el jugador". Preguntar por el es la forma
/// exacta de comprobarlo; la etiqueta es una aproximacion.
public class PortadorDeBolsillo : MonoBehaviour
{
    public BolsilloDelCaso bolsillo;
}

// ===========================================================================
// La base de los recolectables: obliga a decir QUE hace cada uno al recogerse
// ===========================================================================

public abstract class RecolectableDelCaso : MonoBehaviour
{
    public bool usaBandera = true;
    private bool recogido;

    /// Lo unico que cada hijo esta obligado a escribir.
    protected abstract void AlRecoger(BolsilloDelCaso bolsillo);

    public void Recoger(BolsilloDelCaso bolsillo)
    {
        if (usaBandera && recogido) return;   //-> LA linea, y no depende de Destroy
        recogido = true;

        AlRecoger(bolsillo);
        Destroy(gameObject);                  //-> se va al final del frame, no ahora
    }

    /// Devuelve si quien entro valia. El trigger llamaria a esto.
    public bool IntentarRecoger(GameObject quienEntra)
    {
        PortadorDeBolsillo portador = quienEntra.GetComponent<PortadorDeBolsillo>();
        if (portador == null) return false;

        Recoger(portador.bolsillo);
        return true;
    }
}

public class EsferaDeXP : RecolectableDelCaso
{
    public int valor = 5;

    protected override void AlRecoger(BolsilloDelCaso bolsillo)
    {
        bolsillo.xp += valor;
    }
}

public class PocionDeVida : RecolectableDelCaso
{
    public int cura = 8;

    protected override void AlRecoger(BolsilloDelCaso bolsillo)
    {
        bolsillo.Curar(cura);
    }
}
