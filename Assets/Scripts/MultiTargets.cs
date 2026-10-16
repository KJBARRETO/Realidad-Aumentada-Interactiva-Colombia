using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultiTargets : MonoBehaviour
{
    public static bool PosterVisible { get; private set; }
    public static event Action<bool> PosterVisibilityChanged;

    [SerializeField] private GameObject startModel;

    private int modelsCount;
    private int indexCurrentModel;
    private readonly List<GameObject> models = new List<GameObject>();
    private readonly List<GameObject> switcherButtons = new List<GameObject>();

    void Awake()
    {
        foreach (var button in FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (button.name == "Anterior" || button.name == "Siguiente")
                switcherButtons.Add(button.gameObject);
        }

        SetSwitcherButtons(false);

        var handler = GetComponent<DefaultObserverEventHandler>();
        if (handler == null)
            return;

        handler.OnTargetFound.AddListener(() => SetPosterVisible(true));
        handler.OnTargetLost.AddListener(() => SetPosterVisible(false));
    }

    void OnDestroy()
    {
        if (PosterVisible)
            SetPosterVisible(false);
    }

    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
            models.Add(transform.GetChild(i).gameObject);

        modelsCount = models.Count;
        if (modelsCount == 0)
            return;

        indexCurrentModel = 0;
        if (startModel != null)
        {
            int startIndex = models.IndexOf(startModel);
            if (startIndex >= 0)
                indexCurrentModel = startIndex;
        }

        ShowCurrentOnly();
    }

    public void ChangeARModel(int delta)
    {
        if (modelsCount <= 0)
            return;

        int newIndex = indexCurrentModel + delta;
        if (newIndex < 0)
            newIndex = modelsCount - 1;
        else if (newIndex >= modelsCount)
            newIndex = 0;

        indexCurrentModel = newIndex;
        ShowCurrentOnly();
    }

    void SetPosterVisible(bool visible)
    {
        PosterVisible = visible;
        SetSwitcherButtons(visible);
        if (visible)
            ShowCurrentOnly();
        PosterVisibilityChanged?.Invoke(visible);
    }

    void ShowCurrentOnly()
    {
        if (modelsCount <= 0)
            return;

        if (indexCurrentModel < 0 || indexCurrentModel >= modelsCount)
            indexCurrentModel = 0;

        for (int i = 0; i < models.Count; i++)
        {
            if (models[i] == null)
                continue;

            bool show = i == indexCurrentModel;
            models[i].SetActive(show);
            if (show)
                SetRenderersEnabled(models[i], true);
        }
    }

    static void SetRenderersEnabled(GameObject model, bool enabled)
    {
        foreach (var renderer in model.GetComponentsInChildren<Renderer>(true))
            renderer.enabled = enabled;
    }

    void SetSwitcherButtons(bool visible)
    {
        foreach (var button in switcherButtons)
        {
            if (button != null)
                button.SetActive(visible);
        }
    }
}
