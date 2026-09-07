using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// CASO 04 — De la etiqueta al collider. ⭐ Necesita Play.
///
/// La conclusion primero: el hito 3 no cambia de tags a colliders por capricho.
/// Buscar por etiqueta pregunta a TODA la escena cada vez y encima te obliga a
/// medir la distancia a mano; el collider te avisa a ti, una sola vez, cuando
/// algo entra. La ronda A lo mide en milisegundos.
///
/// Y trae dos trampas que cuestan la clase entera si no las has visto antes:
///
///   1. Sin Rigidbody2D en uno de los dos, OnTriggerEnter2D NO SE LLAMA NUNCA.
///      No hay error, no hay aviso: simplemente no pasa nada. La ronda C pone
///      los dos sensores al lado para que se vea.
///   2. Cuando el enemigo muere DENTRO del radio no sale andando: desaparece.
///      El aviso de salida no te llega en ese frame, y tu ataque corre en ese
///      frame: la lista todavia sujeta el cadaver y salta
///      MissingReferenceException. Es la misma fuga del caso 07 de la semana 3,
///      ahora con collider. La ronda D la provoca.
///
/// Las rondas B, C y D montan la escena y el informe sale medio segundo
/// despues: las colisiones no ocurren en la linea en que creas el objeto, sino
/// en el siguiente paso de fisicas.
/// </summary>
public class Caso04_DeLaTagAlCollider : MonoBehaviour
{
    private const string ETIQUETA = "Enemy";
    private static readonly Vector3 LEJOS = new Vector3(0f, 60f, 0f);   //-> fuera de camara

    private SensorDelCaso conCuerpo;
    private SensorDelCaso sinCuerpo;
    private GameObject condenado;

    [ContextMenu("Ejecutar caso")]
    public void Ejecutar()
    {
        if (!Application.isPlaying)
        {
            Debug.LogWarning("[Caso04] necesita Play: sin Play no hay pasos de fisicas y " +
                             "ningun OnTrigger se llama. La leccion saldria al reves.");
            return;
        }

        Debug.Log("=== CASO 04 · de la etiqueta al collider ===");

        RondaMedida();
        MontarSensores();
        Invoke("Informe", 0.5f);
    }

    // ---------------------------------------------------------------- ronda A

    /// Lo mismo dos veces: preguntando a la escena y mirando una lista propia.
    private void RondaMedida()
    {
        List<GameObject> mios = new List<GameObject>();
        for (int i = 0; i < 200; i++)
        {
            GameObject go = new GameObject("relleno " + i);
            go.tag = ETIQUETA;
            go.transform.position = LEJOS;
            mios.Add(go);
        }

        System.Diagnostics.Stopwatch reloj = new System.Diagnostics.Stopwatch();

        reloj.Start();
        for (int vuelta = 0; vuelta < 100; vuelta++)
        {
            GameObject[] todos = GameObject.FindGameObjectsWithTag(ETIQUETA);
            foreach (GameObject go in todos)
                Vector3.Distance(go.transform.position, transform.position);
        }
        reloj.Stop();
        long porEtiqueta = reloj.ElapsedMilliseconds;

        reloj.Reset();
        reloj.Start();
        for (int vuelta = 0; vuelta < 100; vuelta++)
        {
            foreach (GameObject go in mios)
                Vector3.Distance(go.transform.position, transform.position);
        }
        reloj.Stop();

        Debug.Log("[A] 100 vueltas sobre 200 enemigos:  por etiqueta " + porEtiqueta +
                  " ms   |   lista propia " + reloj.ElapsedMilliseconds + " ms");
        Debug.Log("[A] y ojo: la etiqueta te devuelve los 200, esten cerca o en la otra " +
                  "punta del mapa. La distancia la sigues midiendo tu.");

        foreach (GameObject go in mios)
            DestroyImmediate(go);
    }

    // ------------------------------------------------------------- rondas B/C/D

    /// Dos sensores identicos salvo por el Rigidbody2D, y tres enemigos dentro
    /// del radio de cada uno.
    private void MontarSensores()
    {
        conCuerpo = CrearSensor("sensor CON Rigidbody2D", LEJOS, true);
        sinCuerpo = CrearSensor("sensor SIN Rigidbody2D", LEJOS + new Vector3(20f, 0f, 0f), false);

        for (int i = 0; i < 3; i++)
        {
            CrearEnemigo(conCuerpo.transform.position + new Vector3(i * 0.6f, 0f, 0f));
            CrearEnemigo(sinCuerpo.transform.position + new Vector3(i * 0.6f, 0f, 0f));
        }

        // El de la ronda D: entra en el radio y se muere ahi dentro.
        condenado = CrearEnemigo(conCuerpo.transform.position + new Vector3(0f, 0.6f, 0f));
        condenado.name = "el condenado";
    }

    private void Informe()
    {
        Debug.Log("[B] sensor CON Rigidbody2D: " + conCuerpo.dentro.Count +
                  " enemigos dentro, y no he medido ni una distancia.");
        Debug.Log("[C] sensor SIN Rigidbody2D: " + sinCuerpo.dentro.Count +
                  "   <-- CERO. Mismo collider, mismo isTrigger, misma posicion. " +
                  "Sin cuerpo, Unity no simula: los dos son colliders estaticos.");

        // --- ronda D: matarlo dentro del radio -----------------------------
        int antes = conCuerpo.dentro.Count;
        DestroyImmediate(condenado);

        int muertos = 0;
        foreach (GameObject go in conCuerpo.dentro)
            if (go == null) muertos++;

        Debug.Log("[D] mato a uno DENTRO del radio. Antes " + antes + ", ahora " +
                  conCuerpo.dentro.Count + ", y de esos hay " + muertos + " que ya no existen.");
        Debug.Log("[D] el que no salio andando no genera OnTriggerExit2D en este frame, " +
                  "y si Unity acaba avisando lo hace en el siguiente paso de fisicas: tarde.");
        Debug.Log("[D] tu ataque corre AHORA. En cuanto toque go.transform de ese, " +
                  "MissingReferenceException. Es la fuga del caso 07 de la semana 3.");
        Debug.Log("[D] el arreglo es una linea en el enemigo: en su OnDestroy se da de " +
                  "baja del sensor. Lo escribe el que se muere, que es el unico que lo sabe.");

        Limpiar();
    }

    private void Limpiar()
    {
        if (conCuerpo != null) DestroyImmediate(conCuerpo.gameObject);
        if (sinCuerpo != null) DestroyImmediate(sinCuerpo.gameObject);
    }

    // ------------------------------------------------------------------ utils

    private SensorDelCaso CrearSensor(string nombre, Vector3 pos, bool conRigidbody)
    {
        GameObject go = new GameObject(nombre);
        go.transform.position = pos;

        CircleCollider2D coll = go.AddComponent<CircleCollider2D>();
        coll.isTrigger = true;
        coll.radius = 3f;

        if (conRigidbody)
        {
            Rigidbody2D cuerpo = go.AddComponent<Rigidbody2D>();
            cuerpo.gravityScale = 0f;                              //-> que no se caiga solo
            cuerpo.constraints = RigidbodyConstraints2D.FreezeAll; //-> ni se mueva
            // Dinamico a proposito. Uno Kinematic tambien vale, pero entonces hay que
            // encender "Use Full Kinematic Contacts" o no ve los colliders estaticos.
        }

        return go.AddComponent<SensorDelCaso>();
    }

    private GameObject CrearEnemigo(Vector3 pos)
    {
        GameObject go = new GameObject("enemigo");
        go.tag = ETIQUETA;
        go.transform.position = pos;
        go.AddComponent<CircleCollider2D>().radius = 0.3f;
        return go;
    }
}

/// El sensor. Lo unico que hace es apuntar quien esta dentro; no mide nada.
public class SensorDelCaso : MonoBehaviour
{
    public List<GameObject> dentro = new List<GameObject>();

    void OnTriggerEnter2D(Collider2D otro)
    {
        if (!dentro.Contains(otro.gameObject)) dentro.Add(otro.gameObject);
    }

    void OnTriggerExit2D(Collider2D otro)
    {
        dentro.Remove(otro.gameObject);   //-> no llega a tiempo para el que muere dentro
    }
}
