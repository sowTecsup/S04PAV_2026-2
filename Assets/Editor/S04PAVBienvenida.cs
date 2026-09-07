using UnityEditor;
using UnityEngine;

/// ===========================================================================
/// S04PAVBienvenida — evita el error más fácil de cometer.
/// ---------------------------------------------------------------------------
/// La escena de la semana no se versiona (la construye S04PAVSetup). Si abres
/// el proyecto y le das a Play sin haberla preparado, no pasa nada de nada: no
/// hay MenuDeCasos en la escena, así que no salen los botones y parece que el
/// proyecto está roto.
///
/// Este script se ejecuta al abrir el proyecto. Si la escena todavía no existe,
/// la ofrece. Una vez por sesión de Unity, y nunca más en cuanto está creada.
/// ===========================================================================

[InitializeOnLoad]
public static class S04PAVBienvenida
{
    private const string CLAVE = "S04PAV.bienvenida.mostrada";
    private const string RUTA = "Assets/Scenes/S04PAV_Semana4.unity";

    static S04PAVBienvenida()
    {
        // delayCall: al arrancar, el editor todavía está importando y no se
        // pueden abrir diálogos. Esto lo aplaza al primer frame libre.
        EditorApplication.delayCall += Comprobar;
    }

    private static void Comprobar()
    {
        if (Application.isBatchMode) return;
        if (SessionState.GetBool(CLAVE, false)) return;
        SessionState.SetBool(CLAVE, true);

        if (System.IO.File.Exists(RUTA)) return;

        bool si = EditorUtility.DisplayDialog(
            "S04PAV - Semana 4",
            "Todavía no existe la escena de la Semana 4.\n\n" +
            "Sin ella, darle a Play no muestra nada: los botones de los casos y " +
            "la pista de entrenamiento viven en esa escena.\n\n" +
            "¿La preparo ahora? (también está en el menú Tools > S04PAV)",
            "Sí, preparar la escena", "Ahora no");

        if (si) S04PAVSetup.PrepararEscena(false);
    }
}
