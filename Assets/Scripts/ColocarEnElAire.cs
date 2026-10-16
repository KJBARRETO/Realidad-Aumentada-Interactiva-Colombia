using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Vuforia;

public class ColocarEnElAire : MonoBehaviour
{
    ContentPositioningBehaviour contenido;
    float distancia = 1f;
    Camera camara;
    GameObject modelo;
    readonly List<RaycastResult> toquesEnUi = new List<RaycastResult>();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Registrar()
    {
        SceneManager.sceneLoaded += AlCargarEscena;
    }

    static void AlCargarEscena(Scene escena, LoadSceneMode modo)
    {
        if (escena.name != CambioDeEscena.Aire)
            return;

        var posicionador = FindAnyObjectByType<MidAirPositionerBehaviour>();
        if (posicionador == null || posicionador.GetComponent<ColocarEnElAire>() != null)
            return;

        posicionador.gameObject.AddComponent<ColocarEnElAire>();
    }

    void Awake()
    {
        contenido = GetComponent<ContentPositioningBehaviour>();
        var posicionador = GetComponent<MidAirPositionerBehaviour>();
        if (posicionador != null)
        {
            distancia = posicionador.DistanceToCamera;
            posicionador.enabled = false;
        }

        var entrada = GetComponent<AnchorInputListenerBehaviour>();
        if (entrada != null)
            entrada.enabled = false;

        if (contenido != null && contenido.AnchorStage != null)
            contenido.AnchorStage.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!TryGetTap(out Vector2 pantalla))
            return;

        if (TocaInterfaz(pantalla))
            return;

        if (camara == null)
            camara = Camera.main;
        if (camara == null || contenido == null || contenido.AnchorStage == null)
            return;

        var rayo = camara.ScreenPointToRay(pantalla);
        var adelante = Vector3.ProjectOnPlane(camara.transform.forward, Vector3.up);
        if (adelante.sqrMagnitude < 0.0001f)
            adelante = Vector3.forward;

        var posicion = rayo.GetPoint(distancia);
        var rotacion = Quaternion.LookRotation(adelante.normalized, Vector3.up);

        if (modelo == null)
        {
            modelo = Instantiate(contenido.AnchorStage.gameObject);
            modelo.name = contenido.AnchorStage.name;

            var handler = modelo.GetComponent<DefaultObserverEventHandler>();
            if (handler != null)
                handler.enabled = false;

            var ancla = modelo.GetComponent<AnchorBehaviour>();
            if (ancla != null)
                ancla.enabled = false;
        }

        modelo.transform.SetPositionAndRotation(posicion, rotacion);
        modelo.SetActive(true);

        foreach (var renderer in modelo.GetComponentsInChildren<Renderer>(true))
            renderer.enabled = true;
    }

    static bool TryGetTap(out Vector2 pantalla)
    {
        var tactil = Touchscreen.current;
        if (tactil != null && tactil.primaryTouch.press.wasPressedThisFrame)
        {
            pantalla = tactil.primaryTouch.position.ReadValue();
            return true;
        }

        var mouse = Mouse.current;
        if (mouse != null && mouse.leftButton.wasPressedThisFrame)
        {
            pantalla = mouse.position.ReadValue();
            return true;
        }

        pantalla = default;
        return false;
    }

    bool TocaInterfaz(Vector2 pantalla)
    {
        if (EventSystem.current == null)
            return false;

        var datos = new PointerEventData(EventSystem.current) { position = pantalla };
        toquesEnUi.Clear();
        EventSystem.current.RaycastAll(datos, toquesEnUi);
        return toquesEnUi.Count > 0;
    }
}
