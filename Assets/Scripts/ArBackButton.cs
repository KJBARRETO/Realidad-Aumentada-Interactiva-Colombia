using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ArBackButton : MonoBehaviour
{
    [SerializeField] Sprite buttonSprite;
    [SerializeField] Sprite panelSprite;
    [SerializeField] Font labelFont;

    GameObject panel;

    void Awake()
    {
        if (SceneManager.GetActiveScene().name == CambioDeEscena.Menu)
            return;

        EnsureEventSystem();

        var canvasGo = new GameObject("SceneMenuCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        var canvas = canvasGo.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 30;

        var scaler = canvasGo.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080f, 1920f);
        scaler.matchWidthOrHeight = 0.5f;

        var circle = CreateCircle(canvasGo.transform);
        circle.onClick.AddListener(TogglePanel);

        panel = CreatePanel(canvasGo.transform);
        panel.SetActive(false);
    }

    void TogglePanel()
    {
        panel.SetActive(!panel.activeSelf);
    }

    Button CreateCircle(Transform parent)
    {
        var buttonGo = new GameObject("SceneMenuButton", typeof(RectTransform), typeof(Image), typeof(Button));
        buttonGo.transform.SetParent(parent, false);

        var rect = buttonGo.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(1f, 1f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(1f, 1f);
        rect.anchoredPosition = new Vector2(-36f, -36f);
        rect.sizeDelta = new Vector2(120f, 120f);

        var image = buttonGo.GetComponent<Image>();
        image.sprite = CircleSprite();
        image.color = new Color(0.08f, 0.22f, 0.12f, 0.55f);
        image.type = Image.Type.Simple;

        AddBar(buttonGo.transform, 18f);
        AddBar(buttonGo.transform, 0f);
        AddBar(buttonGo.transform, -18f);

        return buttonGo.GetComponent<Button>();
    }

    static void AddBar(Transform parent, float y)
    {
        var bar = new GameObject("Bar", typeof(RectTransform), typeof(Image));
        bar.transform.SetParent(parent, false);
        var rect = bar.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector2(0f, y);
        rect.sizeDelta = new Vector2(52f, 8f);
        bar.GetComponent<Image>().color = Color.white;
        bar.GetComponent<Image>().raycastTarget = false;
    }

    GameObject CreatePanel(Transform parent)
    {
        var panelGo = new GameObject("SceneMenuPanel", typeof(RectTransform), typeof(Image));
        panelGo.transform.SetParent(parent, false);
        var rect = panelGo.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(1f, 1f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(1f, 1f);
        rect.anchoredPosition = new Vector2(-36f, -176f);
        rect.sizeDelta = new Vector2(680f, 680f);
        var panelImage = panelGo.GetComponent<Image>();
        panelImage.sprite = RoundedSprite();
        panelImage.type = Image.Type.Simple;
        panelImage.color = Color.white;
        panelGo.AddComponent<Mask>().showMaskGraphic = false;

        var background = new GameObject("Fondo", typeof(RectTransform), typeof(Image));
        background.transform.SetParent(panelGo.transform, false);
        var backgroundRect = background.GetComponent<RectTransform>();
        backgroundRect.anchorMin = Vector2.zero;
        backgroundRect.anchorMax = Vector2.one;
        backgroundRect.offsetMin = Vector2.zero;
        backgroundRect.offsetMax = Vector2.zero;
        var backgroundImage = background.GetComponent<Image>();
        backgroundImage.sprite = PanelSprite();
        backgroundImage.type = Image.Type.Simple;
        backgroundImage.preserveAspect = true;
        backgroundImage.color = new Color(1f, 1f, 1f, 0.28f);
        backgroundImage.raycastTarget = false;

        AddSceneButton(panelGo.transform, "Menu", CambioDeEscena.Menu, 250f);
        AddSceneButton(panelGo.transform, "ImageTarget", CambioDeEscena.Target, 85f);
        AddSceneButton(panelGo.transform, "GroundPlane", CambioDeEscena.Plano, -80f);
        AddSceneButton(panelGo.transform, "MidAir", CambioDeEscena.Aire, -245f);
        return panelGo;
    }

    void AddSceneButton(Transform parent, string label, string sceneName, float y)
    {
        var buttonGo = new GameObject(label, typeof(RectTransform), typeof(Image), typeof(Button));
        buttonGo.transform.SetParent(parent, false);
        var rect = buttonGo.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector2(0f, y);
        rect.sizeDelta = new Vector2(580f, 145f);

        var image = buttonGo.GetComponent<Image>();
        image.color = Color.white;
        image.sprite = ButtonSprite();
        image.type = Image.Type.Simple;
        image.preserveAspect = true;

        var button = buttonGo.GetComponent<Button>();
        button.onClick.AddListener(() => SceneManager.LoadScene(sceneName));

        var textGo = new GameObject("Label", typeof(RectTransform), typeof(Text));
        textGo.transform.SetParent(buttonGo.transform, false);
        var textRect = textGo.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(110f, 0f);
        textRect.offsetMax = new Vector2(-110f, 0f);

        var text = textGo.GetComponent<Text>();
        text.font = LabelFont();
        text.text = label;
        text.fontSize = 28;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = new Color(0.15f, 0.1f, 0.06f, 1f);
        text.raycastTarget = false;
    }

    Sprite PanelSprite()
    {
        if (panelSprite != null)
            return panelSprite;
#if UNITY_EDITOR
        panelSprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Prefabs/Img/fondo menu pequeño.jpg");
#endif
        return panelSprite;
    }

    Sprite ButtonSprite()
    {
        if (buttonSprite != null)
            return buttonSprite;
#if UNITY_EDITOR
        buttonSprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Prefabs/Banda Blanca con Hojas Tropicales.png");
#endif
        return buttonSprite;
    }

    Font LabelFont()
    {
        if (labelFont != null)
            return labelFont;
#if UNITY_EDITOR
        labelFont = UnityEditor.AssetDatabase.LoadAssetAtPath<Font>("Assets/Prefabs/TextMesh Pro/Sorean Bold.otf");
#endif
        return labelFont != null ? labelFont : Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
    }

    static Sprite roundedSprite;

    static Sprite RoundedSprite()
    {
        if (roundedSprite != null)
            return roundedSprite;

        const int size = 256;
        const float radius = 58f;
        var texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float px = x + 0.5f;
                float py = y + 0.5f;
                float dx = 0f;
                float dy = 0f;
                if (px < radius)
                    dx = radius - px;
                else if (px > size - radius)
                    dx = px - (size - radius);
                if (py < radius)
                    dy = radius - py;
                else if (py > size - radius)
                    dy = py - (size - radius);

                float alpha = Mathf.Clamp01(radius - Mathf.Sqrt(dx * dx + dy * dy) + 0.75f);
                texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
            }
        }

        texture.Apply();
        roundedSprite = Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f);
        return roundedSprite;
    }

    static Sprite circleSprite;

    static Sprite CircleSprite()
    {
        if (circleSprite != null)
            return circleSprite;

        const int size = 128;
        var texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        var center = new Vector2(size * 0.5f, size * 0.5f);
        float radius = size * 0.5f - 1f;
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float distance = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), center);
                texture.SetPixel(x, y, distance <= radius ? Color.white : Color.clear);
            }
        }

        texture.Apply();
        circleSprite = Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect, new Vector4(48f, 48f, 48f, 48f));
        return circleSprite;
    }

    static void EnsureEventSystem()
    {
        if (FindAnyObjectByType<EventSystem>() != null)
            return;

        var eventSystem = new GameObject("EventSystem");
        eventSystem.AddComponent<EventSystem>();
        eventSystem.AddComponent<InputSystemUIInputModule>();
    }
}
