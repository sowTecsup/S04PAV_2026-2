using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// La pista: coloca maniquies en un anillo alrededor del centro y DIBUJA el
/// radio de ataque, que es lo que no se ve y por lo que se pierde media hora.
///
/// Deliberadamente la mitad de los maniquies quedan DENTRO del radio y la otra
/// mitad fuera. Cuando enchufes tu deteccion por collider, la respuesta correcta
/// se ve de un vistazo: pega a los de dentro y a ninguno de los de fuera. Si les
/// pega a todos, tu radio no es el que crees.
///
/// Esto no resuelve ningun hito: no crea entidades, no reparte dano y no sabe
/// nada de elementos. Solo pone dianas y pinta el circulo.
///
/// [ENCHUFE] Pon aqui el radio del CircleCollider2D de tu Player para verlo.
/// </summary>
public class PistaDeEntrenamiento : MonoBehaviour
{
    public float radioDeAtaque = 3f;      //-> [ENCHUFE] el mismo que tu CircleCollider2D
    public int cuantosManiquies = 8;
    public int vidaPorManiqui = 40;

    private readonly List<GameObject> puestos = new List<GameObject>();

    [ContextMenu("Colocar maniquies")]
    public void ColocarManiquies()
    {
        QuitarManiquies();

        for (int i = 0; i < cuantosManiquies; i++)
        {
            bool dentro = i % 2 == 0;
            float radio = dentro ? radioDeAtaque * 0.6f : radioDeAtaque * 1.7f;
            float angulo = i * Mathf.PI * 2f / cuantosManiquies;

            Vector3 sitio = transform.position +
                            new Vector3(Mathf.Cos(angulo) * radio, Mathf.Sin(angulo) * radio, 0f);

            GameObject go = new GameObject("Maniqui " + (i + 1) + (dentro ? " (dentro)" : " (fuera)"));
            go.transform.position = sitio;
            go.transform.SetParent(transform);

            ManiquiDeEntrenamiento maniqui = go.AddComponent<ManiquiDeEntrenamiento>();
            maniqui.vida = vidaPorManiqui;

            // Collider para que tu deteccion del hito 3 tenga a que agarrarse.
            // Sin Rigidbody2D en ninguno de los dos, tu OnTriggerEnter2D no se
            // llamara nunca: el cuerpo va en tu Player, no aqui.
            go.AddComponent<CircleCollider2D>().radius = 0.45f;

            puestos.Add(go);
        }

        Debug.Log("[Pista] " + cuantosManiquies + " maniquies colocados: la mitad dentro del " +
                  "radio " + radioDeAtaque + " y la mitad fuera.");
    }

    [ContextMenu("Quitar maniquies")]
    public void QuitarManiquies()
    {
        foreach (GameObject go in puestos)
        {
            if (go == null) continue;
            if (Application.isPlaying) Destroy(go); else DestroyImmediate(go);
        }

        puestos.Clear();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.30f, 0.68f, 1f, 0.9f);
        Gizmos.DrawWireSphere(transform.position, radioDeAtaque);   //-> el radio de ataque
    }
}
