using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Configuración de Raycast Frontal")]
    [SerializeField] private Transform origenRaycast;
    [SerializeField] private float distanciaInteraccion = 4.5f;
    [SerializeField] private LayerMask capaInteractivos;

    private IInteractuable objetoInteractuableActual = null;
    private string promptActual = "";

    private void Awake()
    {
        // Si no se asignó origen, usamos la cámara hija o Camera.main
        if (origenRaycast == null)
        {
            Camera camHija = GetComponentInChildren<Camera>();
            if (camHija != null) origenRaycast = camHija.transform;
            else if (Camera.main != null) origenRaycast = Camera.main.transform;
            else origenRaycast = transform;
        }

        // Si no se asignó en el Inspector, configuramos LayerMask a 'Interactable'
        if (capaInteractivos.value == 0)
        {
            capaInteractivos = LayerMask.GetMask("Interactable");
            if (capaInteractivos.value == 0)
            {
                // Respaldo de seguridad si no existe la capa
                capaInteractivos = ~LayerMask.GetMask("Ignore Raycast");
            }
        }
    }

    private void Update()
    {
        objetoInteractuableActual = null;
        promptActual = "";

        Vector3 origen = origenRaycast != null ? origenRaycast.position : transform.position + Vector3.up * 1.5f;
        Vector3 direccion = origenRaycast != null ? origenRaycast.forward : transform.forward;

        // Proyección del Raycast frontal filtrando EXCLUSIVAMENTE con la LayerMask de objetos interactivos
        bool hit = Physics.Raycast(origen, direccion, out RaycastHit hitInfo, distanciaInteraccion, capaInteractivos, QueryTriggerInteraction.Collide);

        if (hit)
        {
            IInteractuable interactuable = hitInfo.collider.GetComponent<IInteractuable>() 
                                        ?? hitInfo.collider.GetComponentInParent<IInteractuable>();

            if (interactuable != null)
            {
                objetoInteractuableActual = interactuable;
                promptActual = $"Presiona [E] para {interactuable.ObtenerMensaje()}";

                // Al presionar la tecla de interacción
                if (Input.GetKeyDown(KeyCode.E))
                {
                    interactuable.Interactuar(this.gameObject);
                }
            }
        }

        // Gizmo visual en escena para validar el raycast
        Debug.DrawRay(origen, direccion * distanciaInteraccion, objetoInteractuableActual != null ? Color.green : Color.cyan);
    }

    private void OnGUI()
    {
        // Mira central simple
        float crossSize = 4f;
        GUI.Box(new Rect(Screen.width / 2f - crossSize / 2f, Screen.height / 2f - crossSize / 2f, crossSize, crossSize), GUIContent.none);

        // Indicador de interacción cuando el Raycast apunta a un objeto interactivo
        if (!string.IsNullOrEmpty(promptActual))
        {
            GUIStyle style = new GUIStyle(GUI.skin.box);
            style.fontStyle = FontStyle.Bold;
            style.fontSize = 16;
            style.alignment = TextAnchor.MiddleCenter;
            style.normal.textColor = Color.white;

            float width = 360f;
            float height = 36f;
            float x = (Screen.width - width) / 2f;
            float y = Screen.height / 2f + 45f;

            GUI.Box(new Rect(x, y, width, height), promptActual, style);
        }
    }
}

