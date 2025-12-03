using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RuletaVisual : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private RuletaManager manager;
    [SerializeField] private Transform ruedaTransform;
    [SerializeField] private TMP_Text textoGanador;

    [Header("Configuración Visual")]
    [SerializeField] private float radio = 0.25f;
    [SerializeField] private float duracionGiro = 3f;
    [SerializeField] private Ease tipoEase = Ease.OutCubic;
    [SerializeField] private float fontSize = 36;

    [Header("Colores (opcional)")]
    [SerializeField] private Color[] coloresTexto;

    // Canvas para los textos
    private Canvas canvasRuleta;
    private List<TMP_Text> textosRuleta = new List<TMP_Text>();

    private void Start()
    {
        // Crear Canvas en World Space
        CrearCanvasRuleta();

        manager.OnParticipantesActualizados += ActualizarVisualizacion;
        ActualizarVisualizacion(manager.GetParticipantes());
    }

    private void CrearCanvasRuleta()
    {
        GameObject canvasObj = new GameObject("CanvasRuleta");
        canvasObj.transform.SetParent(ruedaTransform);
        canvasObj.transform.localPosition = Vector3.zero;
        canvasObj.transform.localRotation = Quaternion.identity;
        canvasObj.transform.localScale = Vector3.one;

        canvasRuleta = canvasObj.AddComponent<Canvas>();
        canvasRuleta.renderMode = RenderMode.WorldSpace;

        // CanvasScaler para escalado
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.dynamicPixelsPerUnit = 10;

        // GraphicRaycaster (opcional, para interacción)
        canvasObj.AddComponent<GraphicRaycaster>();

        // Ajustar tamaño del Canvas
        RectTransform rect = canvasObj.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(1, 1);
    }

    private void ActualizarVisualizacion(List<string> participantes)
    {
        Debug.Log($"Actualizando visualización con {participantes.Count} participantes");

        int total = participantes.Count;

        if (total == 0)
        {
            foreach (var texto in textosRuleta)
            {
                if (texto != null)
                    texto.gameObject.SetActive(false);
            }
            return;
        }

        // Crear más textos si faltan
        while (textosRuleta.Count < total)
        {
            CrearNuevoTexto();
        }

        // Posicionar textos en círculo
        for (int i = 0; i < textosRuleta.Count; i++)
        {
            if (i < total)
            {
                textosRuleta[i].text = participantes[i];
                textosRuleta[i].gameObject.SetActive(true);

                if (coloresTexto != null && coloresTexto.Length > 0)
                {
                    textosRuleta[i].color = coloresTexto[i % coloresTexto.Length];
                }

                // Calcular posición en círculo
                float angulo = (360f / total) * i;
                float anguloRad = angulo * Mathf.Deg2Rad;

                Vector3 posicion = new Vector3(
                    Mathf.Sin(anguloRad) * radio,
                    Mathf.Cos(anguloRad) * radio,
                    0
                );

                textosRuleta[i].transform.localPosition = posicion;

                // Rotación para que el texto mire hacia afuera
                textosRuleta[i].transform.localRotation = Quaternion.Euler(0, 0, -angulo + 90);

                textosRuleta[i].fontSize = fontSize;
                textosRuleta[i].alignment = TextAlignmentOptions.Center;

                Debug.Log($"Texto '{participantes[i]}' en posición: {posicion}");
            }
            else
            {
                textosRuleta[i].gameObject.SetActive(false);
            }
        }
    }

    private void CrearNuevoTexto()
    {
        GameObject nuevoTextoObj = new GameObject($"Seccion_{textosRuleta.Count}");
        nuevoTextoObj.transform.SetParent(canvasRuleta.transform);
        nuevoTextoObj.transform.localPosition = Vector3.zero;
        nuevoTextoObj.transform.localRotation = Quaternion.identity;
        nuevoTextoObj.transform.localScale = Vector3.one;

        // Usar TextMeshProUGUI (para Canvas)
        TMP_Text nuevoTexto = nuevoTextoObj.AddComponent<TextMeshProUGUI>();

        // Configuración básica
        nuevoTexto.alignment = TextAlignmentOptions.Center;
        nuevoTexto.fontSize = fontSize;
        nuevoTexto.color = Color.white;
        nuevoTexto.text = "Texto";

        // Configurar RectTransform
        RectTransform rect = nuevoTextoObj.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(100, 50); // Tamaño adecuado
        rect.pivot = new Vector2(0.5f, 0.5f); // Centro

        textosRuleta.Add(nuevoTexto);
    }

    public void IniciarGiro()
    {
        if (manager == null) return;

        if (manager.GetTotalParticipantes() == 0)
        {
            if (textoGanador != null)
                textoGanador.text = "Agrega participantes primero!";
            return;
        }

        string ganador = manager.GirarRuleta();

        if (string.IsNullOrEmpty(ganador)) return;

        float anguloFinal = manager.CalcularAnguloParaGanador(ganador);

        ruedaTransform.DORotate(new Vector3(0, 0, -anguloFinal), duracionGiro, RotateMode.LocalAxisAdd)
            .SetEase(tipoEase)
            .OnStart(() => {
                if (textoGanador != null)
                {
                    textoGanador.text = "Girando...";
                }
            })
            .OnComplete(() => {
                if (textoGanador != null)
                {
                    textoGanador.text = $"¡Ganador: {ganador}!";
                }
            });
    }

    // Resto de métodos igual...
    public void OnClickAgregar(string nombre) => manager?.AgregarParticipante(nombre);
    public void OnClickRemover(string nombre) => manager?.RemoverParticipante(nombre);
    public void OnClickGirar() => IniciarGiro();

    private void OnDestroy()
    {
        if (manager != null)
            manager.OnParticipantesActualizados -= ActualizarVisualizacion;
    }
}