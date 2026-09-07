using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

/// ===========================================================================
/// S04PAVSetup — arma la escena de la Semana 4 con un clic.
/// ---------------------------------------------------------------------------
/// Por qué un script y no un .unity versionado: los GUID y el formato de una
/// escena dependen de la versión exacta del editor. Un .unity escrito a mano
/// revienta en cuanto una máquina abre otra versión. Esto se comporta igual en
/// las 20 máquinas del laboratorio.
///
/// USO:  menú  Tools > S04PAV > Preparar escena Semana 4
/// ===========================================================================

public static class S04PAVSetup
{
    private const string CARPETA = "Assets/Scenes";
    private const string RUTA = CARPETA + "/S04PAV_Semana4.unity";
    private const string ETIQUETA = "Enemy";

    [MenuItem("Tools/S04PAV/Preparar escena Semana 4")]
    public static void PrepararEscena()
    {
        PrepararEscena(true);
    }

    /// Punto de entrada para la línea de comandos:
    ///   Unity.exe -batchmode -quit -projectPath ... -executeMethod S04PAVSetup.PrepararEscenaSinPreguntar
    public static void PrepararEscenaSinPreguntar()
    {
        PrepararEscena(false);
    }

    public static void PrepararEscena(bool preguntar)
    {
        if (preguntar && !EditorUtility.DisplayDialog(
                "Preparar escena Semana 4",
                "Se va a crear (o sobrescribir) la escena:\n\n" + RUTA +
                "\n\nSe pierde lo que tengas sin guardar en la escena actual. ¿Seguimos?",
                "Sí, prepararla", "Cancelar"))
            return;

        AsegurarEtiqueta(ETIQUETA);

        var escena = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // ---- cámara 2D, con hueco para la pista ----
        var goCam = new GameObject("Main Camera");
        goCam.tag = "MainCamera";
        var cam = goCam.AddComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = 10f;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.11f, 0.12f, 0.16f);
        goCam.transform.position = new Vector3(0f, 0f, -10f);

        Sprite cuadro = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");

        // ---- suelo, solo para ver dónde cae todo ----
        var suelo = CrearFicha("Arena", Vector3.zero, new Color(0.16f, 0.17f, 0.22f), cuadro);
        suelo.transform.localScale = new Vector3(18f, 18f, 1f);
        suelo.GetComponent<SpriteRenderer>().sortingOrder = -10;

        // ---- el centro: aquí va TU Player, con tu collider ----
        var centro = CrearFicha("Centro (aquí va tu Player)", Vector3.zero,
                                new Color(0.30f, 0.68f, 1f), cuadro);
        centro.transform.localScale = new Vector3(1.2f, 1.2f, 1f);

        // ---- la pista de entrenamiento, colgada del centro ----
        var pista = centro.AddComponent<PistaDeEntrenamiento>();
        pista.radioDeAtaque = 3f;
        pista.cuantosManiquies = 8;

        // ---- la lanzadera de los cinco casos ----
        var goCasos = new GameObject("MenuDeCasos");
        goCasos.AddComponent<MenuDeCasos>();

        if (!AssetDatabase.IsValidFolder(CARPETA))
            AssetDatabase.CreateFolder("Assets", "Scenes");

        EditorSceneManager.SaveScene(escena, RUTA);
        AssetDatabase.Refresh();

        RegistrarEnBuildSettings(RUTA);

        Debug.Log("Escena lista: " + RUTA +
                  "\nDale a Play. Los botones de la izquierda lanzan los cinco casos y " +
                  "cuentan lo que pasa en la consola. Para las dianas: botón derecho sobre " +
                  "PistaDeEntrenamiento (en 'Centro') > Colocar maniquíes.");
    }

    // ------------------------------------------------------------------ utils

    private static GameObject CrearFicha(string nombre, Vector3 pos, Color color, Sprite sprite)
    {
        var go = new GameObject(nombre);
        go.transform.position = pos;
        go.transform.localScale = new Vector3(1.6f, 1.6f, 1f);

        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.color = color;
        sr.drawMode = SpriteDrawMode.Sliced;
        sr.size = new Vector2(1f, 1f);

        return go;
    }

    /// El caso 04 compara la búsqueda por etiqueta con el collider, y el hito 3
    /// habla de dejar las etiquetas atrás. Si 'Enemy' no existe en el proyecto,
    /// FindGameObjectsWithTag lanza una excepción y 21 alumnos se quedan
    /// clavados en el minuto uno. Se crea aquí.
    private static void AsegurarEtiqueta(string etiqueta)
    {
        Object[] assets = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");
        if (assets == null || assets.Length == 0) return;

        SerializedObject tagManager = new SerializedObject(assets[0]);
        SerializedProperty tags = tagManager.FindProperty("tags");
        if (tags == null) return;

        for (int i = 0; i < tags.arraySize; i++)
        {
            if (tags.GetArrayElementAtIndex(i).stringValue == etiqueta)
                return;
        }

        tags.InsertArrayElementAtIndex(tags.arraySize);
        tags.GetArrayElementAtIndex(tags.arraySize - 1).stringValue = etiqueta;
        tagManager.ApplyModifiedPropertiesWithoutUndo();

        Debug.Log("Etiqueta '" + etiqueta + "' creada en el proyecto.");
    }

    private static void RegistrarEnBuildSettings(string ruta)
    {
        var escenas = new System.Collections.Generic.List<EditorBuildSettingsScene>();
        escenas.Add(new EditorBuildSettingsScene(ruta, true));
        EditorBuildSettings.scenes = escenas.ToArray();
    }
}
