using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class UIButtonSoundsSetup : EditorWindow
{
    [MenuItem("Tools/Audio/Add UIButtonSounds to All Buttons")]
    public static void AddToAllButtons()
    {
        Button[] buttons = FindObjectsOfType<Button>(true); // true = incluir inactivos
        int added = 0;
        int skipped = 0;

        foreach (Button button in buttons)
        {
            if (button.GetComponent<UIButtonSounds>() == null)
            {
                Undo.AddComponent<UIButtonSounds>(button.gameObject);
                added++;
            }
            else
            {
                skipped++;
            }
        }

        EditorUtility.DisplayDialog(
            "UIButtonSounds Setup",
            $"Completado!\n\nAgregados: {added}\nYa tenían el componente: {skipped}\nTotal botones: {buttons.Length}",
            "OK"
        );
    }

    [MenuItem("Tools/Audio/Remove UIButtonSounds from All Buttons")]
    public static void RemoveFromAllButtons()
    {
        UIButtonSounds[] components = FindObjectsOfType<UIButtonSounds>(true);
        int removed = components.Length;

        foreach (UIButtonSounds comp in components)
        {
            Undo.DestroyObjectImmediate(comp);
        }

        EditorUtility.DisplayDialog(
            "UIButtonSounds Setup",
            $"Removidos: {removed} componentes",
            "OK"
        );
    }
}

