using UnityEngine;

/// <summary>
/// Los botones de la sesion. Dale a Play y salen a la izquierda; cada uno
/// ejecuta un caso y lo cuenta en la CONSOLA, que es donde hay que mirar esta
/// semana. Ten desactivado el "Clear on Play".
///
/// Tambien va sin Play: boton derecho sobre el componente -> Ejecutar todos.
/// Los casos 04 y 05 avisan y se paran, porque fuera de Play no hay pasos de
/// fisicas ni Destroy diferido y la leccion saldria al reves.
///
/// El boton no ejecuta dentro de OnGUI: apunta cual toca y lo lanza en el
/// Update siguiente, porque varios casos usan DestroyImmediate.
/// </summary>
public class MenuDeCasos : MonoBehaviour
{
    private static readonly string[] TITULOS =
    {
        "01 · El contrato que obliga",
        "02 · Datos o comportamiento  ⭐",
        "03 · El switch que crece  ⭐",
        "04 · De la etiqueta al collider  ⭐",
        "05 · Recoger no es chocar"
    };

    private int pendiente = -1;

    void Update()
    {
        if (pendiente < 0) return;

        int n = pendiente;
        pendiente = -1;
        Ejecutar(n);
    }

    private void Ejecutar(int n)
    {
        switch (n)
        {
            case 0: Crear<Caso01_ElContratoQueObliga>().Ejecutar(); break;
            case 1: Crear<Caso02_DatosOComportamiento>().Ejecutar(); break;
            case 2: Crear<Caso03_ElSwitchQueCrece>().Ejecutar(); break;
            case 3: Crear<Caso04_DeLaTagAlCollider>().Ejecutar(); break;
            case 4: Crear<Caso05_RecogerNoEsChocar>().Ejecutar(); break;
        }
    }

    /// Reutiliza el componente si ya esta puesto; si no, lo anade.
    private T Crear<T>() where T : MonoBehaviour
    {
        T c = GetComponent<T>();
        if (c == null) c = gameObject.AddComponent<T>();
        return c;
    }

    [ContextMenu("Ejecutar todos")]
    public void EjecutarTodos()
    {
        for (int i = 0; i < TITULOS.Length; i++)
            Ejecutar(i);
    }

    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10f, 10f, 330f, 46f + TITULOS.Length * 26f), GUI.skin.box);
        GUILayout.Label("SEMANA 4 · ABSTRACCION Y MODELADO");

        for (int i = 0; i < TITULOS.Length; i++)
        {
            if (GUILayout.Button(TITULOS[i])) pendiente = i;
        }

        GUILayout.EndArea();
    }
}
