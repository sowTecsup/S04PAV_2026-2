using UnityEngine;

/// <summary>
/// El maniqui del gimnasio. NO es una entidad de tu dominio y no hereda de
/// nada: es un saco que recibe golpes y los canta por consola.
///
/// Esta aqui por un motivo concreto: el laboratorio te pide que el Player
/// ataque solo cada cierto tiempo a lo que tenga cerca, y eso no se puede
/// probar hasta tener enemigos. Con el maniqui lo pruebas desde el minuto uno,
/// antes de escribir tu primera clase abstracta.
///
/// [ENCHUFE] En cuanto tu Enemy funcione, BORRA este archivo y el
/// PistaDeEntrenamiento. No forman parte de la entrega.
/// </summary>
public class ManiquiDeEntrenamiento : MonoBehaviour
{
    public int vida = 40;
    public bool cantaCadaGolpe = true;

    private int golpesRecibidos;

    /// Firma a proposito tonta —un int y un texto— para que NO se parezca a tu
    /// TakeDamage(BaseEntity). El maniqui no participa en tu sistema.
    public void Golpear(int cantidad, string dequien)
    {
        golpesRecibidos++;
        vida -= cantidad;

        if (vida <= 0)
        {
            vida = 0;
            Debug.Log("[Maniqui] " + name + " cae tras " + golpesRecibidos + " golpes. " +
                      "Ultimo de '" + dequien + "'.");
            Destroy(gameObject);
            return;
        }

        if (cantaCadaGolpe)
            Debug.Log("[Maniqui] " + name + " recibe " + cantidad + " de '" + dequien +
                      "'. Le quedan " + vida + ".");
    }

    /// Para probarlo sin Player todavia: boton derecho sobre el componente.
    [ContextMenu("Recibir un golpe de prueba")]
    public void GolpeDePrueba()
    {
        Golpear(7, "el boton derecho");
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0.55f, 0.2f, 0.9f);
        Gizmos.DrawWireSphere(transform.position, 0.45f);
    }
}
