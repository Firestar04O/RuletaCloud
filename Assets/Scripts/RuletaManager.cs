using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RuletaManager : MonoBehaviour
{
    // Lista de participantes (la fuente de verdad)
    private List<string> participantes = new List<string>();

    // Configuración
    private int secciones = 0;
    private float anguloPorSeccion = 0f;

    // Eventos para comunicar cambios
    public delegate void ParticipantesActualizadosHandler(List<string> nuevosParticipantes);
    public event ParticipantesActualizadosHandler OnParticipantesActualizados;

    public delegate void GanadorSeleccionadoHandler(string ganador);
    public event GanadorSeleccionadoHandler OnGanadorSeleccionado;

    // Métodos públicos para modificar participantes
    public void AgregarParticipante(string nombre)
    {
        if (!string.IsNullOrEmpty(nombre) && !participantes.Contains(nombre))
        {
            participantes.Add(nombre);
            ActualizarSecciones();
            OnParticipantesActualizados?.Invoke(new List<string>(participantes));
        }
    }

    public void RemoverParticipante(string nombre)
    {
        if (participantes.Remove(nombre))
        {
            ActualizarSecciones();
            OnParticipantesActualizados?.Invoke(new List<string>(participantes));
        }
    }

    private void ActualizarSecciones()
    {
        secciones = participantes.Count;
        if (secciones > 0)
            anguloPorSeccion = 360f / secciones;
    }

    // Lógica para girar la ruleta
    public string GirarRuleta()
    {
        if (participantes.Count == 0)
            return null;

        // Selección aleatoria del ganador (lógica pura)
        int indiceGanador = Random.Range(0, participantes.Count);
        string ganador = participantes[indiceGanador];

        // Calcular ángulo para la animación (lógica pura)
        float anguloBase = indiceGanador * anguloPorSeccion;
        // Agregar vueltas completas para efecto visual
        float anguloTotal = 360f * 5 + anguloBase;

        // Notificar a los suscriptores
        OnGanadorSeleccionado?.Invoke(ganador);

        return ganador;
    }

    // Método para obtener el ángulo necesario para la animación
    public float CalcularAnguloParaGanador(string nombre)
    {
        int indice = participantes.IndexOf(nombre);
        if (indice < 0) return 0f;

        float anguloBase = indice * anguloPorSeccion;
        return 360f * 5 + anguloBase; // 5 vueltas + posición del ganador
    }

    public List<string> GetParticipantes() => new List<string>(participantes);
    public int GetTotalParticipantes() => participantes.Count;
}