using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultiTargets : MonoBehaviour
{
    [SerializeField] private GameObject startModel;

    private int modelsCount;
    private int indexCurrentModel;
    private Text modelLabel;
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

        handler.OnTargetFound.AddListener(() => SetSwitcherButtons(true));
        handler.OnTargetLost.AddListener(() => SetSwitcherButtons(false));
    }

    void Start()
    {
        modelsCount = transform.childCount;
        CreateLabel();
        if (modelsCount == 0)
            return;

        indexCurrentModel = startModel != null ? startModel.transform.GetSiblingIndex() : 0;
        if (indexCurrentModel < 0 || indexCurrentModel >= modelsCount)
            indexCurrentModel = 0;

        ShowOnly(indexCurrentModel);
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

        ShowOnly(newIndex);
    }

    void ShowOnly(int index)
    {
        if (transform.childCount == 0)
            return;

        for (int i = 0; i < transform.childCount; i++)
            transform.GetChild(i).gameObject.SetActive(i == index);

        indexCurrentModel = index;
        if (modelLabel != null)
            modelLabel.text = transform.GetChild(index).name;
    }

    void CreateLabel()
    {
        var canvasGo = new GameObject("ModelLabelCanvas", typeof(Canvas), typeof(CanvasScaler));
        var canvas = canvasGo.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 15;

        var scaler = canvasGo.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080f, 1920f);
        scaler.matchWidthOrHeight = 0.5f;

        var textGo = new GameObject("ModelName", typeof(RectTransform), typeof(Text));
        textGo.transform.SetParent(canvasGo.transform, false);
        var rect = textGo.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0f);
        rect.anchorMax = new Vector2(0.5f, 0f);
        rect.pivot = new Vector2(0.5f, 0f);
        rect.anchoredPosition = new Vector2(0f, 40f);
        rect.sizeDelta = new Vector2(900f, 70f);

        modelLabel = textGo.GetComponent<Text>();
        modelLabel.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        modelLabel.fontSize = 36;
        modelLabel.alignment = TextAnchor.MiddleCenter;
        modelLabel.color = Color.white;
        modelLabel.raycastTarget = false;
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
