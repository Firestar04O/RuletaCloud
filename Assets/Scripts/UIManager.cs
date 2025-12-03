using UnityEngine;
using UnityEngine.UI;
using TMPro;  // Para InputField con TMP

public class UIManager : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private RuletaManager ruletaManager;
    [SerializeField] private RuletaVisual ruletaVisual;

    [Header("UI Elements")]
    [SerializeField] private TMP_InputField inputNombre; // Si usas TMP
                                                         // O usar esto si usas UI normal:
                                                         // [SerializeField] private InputField inputNombre;

    [SerializeField] private Button btnAgregar;
    [SerializeField] private Button btnRemover;
    [SerializeField] private Button btnGirar;
    [SerializeField] private Button btnQuit;

    [Header("Mensaje de Estado")]
    [SerializeField] private TMP_Text textoEstado; // Opcional: para mostrar mensajes

    private void Start()
    {

        // Configurar botones
        btnAgregar.onClick.AddListener(AgregarParticipante);
        btnRemover.onClick.AddListener(RemoverParticipante);
        btnGirar.onClick.AddListener(GirarRuleta);
        btnQuit.onClick.AddListener(QuitGame);
        // Opcional: deshabilitar botón girar si no hay participantes
        ActualizarEstadoUI();
    }

    private void AgregarParticipante()
    {
        if (!string.IsNullOrEmpty(inputNombre.text))
        {
            ruletaVisual.OnClickAgregar(inputNombre.text);
            inputNombre.text = "";
            inputNombre.Select();
            inputNombre.ActivateInputField();

            MostrarMensaje($"Agregado: {inputNombre.text}", Color.green);
            ActualizarEstadoUI();
        }
        else
        {
            MostrarMensaje("Ingresa un nombre primero", Color.yellow);
        }
    }

    private void RemoverParticipante()
    {
        if (!string.IsNullOrEmpty(inputNombre.text))
        {
            ruletaVisual.OnClickRemover(inputNombre.text);
            inputNombre.text = "";
            inputNombre.Select();
            inputNombre.ActivateInputField();

            MostrarMensaje($"Removido: {inputNombre.text}", Color.red);
            ActualizarEstadoUI();
        }
        else
        {
            MostrarMensaje("Ingresa un nombre para remover", Color.yellow);
        }
    }

    private void GirarRuleta()
    {
        ruletaVisual.OnClickGirar();
        MostrarMensaje("¡Girando la ruleta!", Color.cyan);
    }

    private void ActualizarEstadoUI()
    {
        // Opcional: deshabilitar botón girar si no hay participantes
        if (ruletaManager != null)
        {
            int total = ruletaManager.GetTotalParticipantes();
            btnGirar.interactable = total > 0;
        }
    }

    private void MostrarMensaje(string mensaje, Color color)
    {
        if (textoEstado != null)
        {
            textoEstado.text = mensaje;
            textoEstado.color = color;
        }
        Debug.Log(mensaje);
    }
    public void QuitGame()
    {
        Debug.Log("Ha salido del programa");
        Application.Quit();
    }
}