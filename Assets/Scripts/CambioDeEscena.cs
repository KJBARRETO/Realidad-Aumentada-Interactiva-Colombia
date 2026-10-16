using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class CambioDeEscena : MonoBehaviour
{
    public const string Menu = "Escena 1 Menu";
    public const string Plano = "Escena 2 Plano";
    public const string Target = "Escena 3 Target";
    public const string Aire = "Escena 4 Aire";

    void Awake()
    {
        var buttons = FindObjectsByType<Button>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        foreach (var button in buttons)
        {
            if (IsSalir(button))
                button.onClick.AddListener(QuitApp);
        }
    }

    public void QuitApp()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    static bool IsSalir(Button button)
    {
        if (button.gameObject.name.IndexOf("Salir", System.StringComparison.OrdinalIgnoreCase) >= 0)
            return true;

        var legacy = button.GetComponentInChildren<Text>(true);
        if (legacy != null && string.Equals(legacy.text.Trim(), "Salir", System.StringComparison.OrdinalIgnoreCase))
            return true;

        var tmp = button.GetComponentInChildren<TMP_Text>(true);
        return tmp != null && string.Equals(tmp.text.Trim(), "Salir", System.StringComparison.OrdinalIgnoreCase);
    }

    public void OpenMenu()
    {
        SceneManager.LoadScene(Menu);
    }

    public void OpenGroundPlane()
    {
        SceneManager.LoadScene(Plano);
    }

    public void OpenImageTargets()
    {
        SceneManager.LoadScene(Target);
    }

    public void OpenMidAir()
    {
        SceneManager.LoadScene(Aire);
    }
}
