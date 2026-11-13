using UnityEngine;

public class TerminalDatos : MonoBehaviour, IInteractuable
{
    [Header("Configuración del Terminal")]
    [SerializeField] private string nombreTerminal = "Consola de Datos";
    [SerializeField] private bool activada = false;
    [SerializeField] private Color colorActivo = Color.cyan;
    [SerializeField] private Color colorInactivo = new Color(1f, 0.5f, 0f);

    private Renderer rend;
    private Material mat;

    private void Awake()
    {
        rend = GetComponent<Renderer>();
        if (rend != null)
        {
            mat = rend.material;
            ActualizarVisual();
        }
    }

    public void Interactuar(GameObject instigador)
    {
        activada = !activada;
        ActualizarVisual();

        if (activada)
        {
            Debug.Log($"<color=cyan>[{nombreTerminal}] ¡Terminal activada con éxito! Acceso concedido.</color>");
        }
        else
        {
            Debug.Log($"<color=yellow>[{nombreTerminal}] Terminal desactivada.</color>");
        }
    }

    public string ObtenerMensaje()
    {
        return activada ? $"Desactivar {nombreTerminal}" : $"Activar {nombreTerminal}";
    }

    private void ActualizarVisual()
    {
        if (mat != null)
        {
            mat.color = activada ? colorActivo : colorInactivo;
        }
    }
}

