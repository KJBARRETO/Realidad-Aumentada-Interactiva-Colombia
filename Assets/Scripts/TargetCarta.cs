using UnityEngine;
using UnityEngine.SceneManagement;
using Vuforia;

public class TargetCarta : MonoBehaviour
{
    bool tracked;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Registrar()
    {
        SceneManager.sceneLoaded += AlCargarEscena;
    }

    static void AlCargarEscena(Scene escena, LoadSceneMode modo)
    {
        if (escena.name != CambioDeEscena.Target)
            return;

        var targets = FindObjectsByType<ImageTargetBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var target in targets)
        {
            if (target.GetComponent<MultiTargets>() != null)
                continue;
            if (target.GetComponent<TargetCarta>() != null)
                continue;

            target.gameObject.AddComponent<TargetCarta>();
        }
    }

    void OnEnable()
    {
        var handler = GetComponent<DefaultObserverEventHandler>();
        if (handler != null)
        {
            handler.OnTargetFound.AddListener(OnFound);
            handler.OnTargetLost.AddListener(OnLost);
        }

        MultiTargets.PosterVisibilityChanged += OnPosterChanged;
    }

    void OnDisable()
    {
        var handler = GetComponent<DefaultObserverEventHandler>();
        if (handler != null)
        {
            handler.OnTargetFound.RemoveListener(OnFound);
            handler.OnTargetLost.RemoveListener(OnLost);
        }

        MultiTargets.PosterVisibilityChanged -= OnPosterChanged;
    }

    void Start()
    {
        Apply();
    }

    void OnFound()
    {
        tracked = true;
        Apply();
    }

    void OnLost()
    {
        tracked = false;
        Apply();
    }

    void OnPosterChanged(bool posterVisible)
    {
        Apply();
    }

    void Apply()
    {
        bool show = tracked && !MultiTargets.PosterVisible;

        for (int i = 0; i < transform.childCount; i++)
        {
            var model = transform.GetChild(i).gameObject;
            model.SetActive(show);
            if (!show)
                continue;

            foreach (var renderer in model.GetComponentsInChildren<Renderer>(true))
                renderer.enabled = true;
        }
    }
}
