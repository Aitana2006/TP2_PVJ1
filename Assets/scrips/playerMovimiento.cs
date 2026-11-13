using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class playerMovimiento : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private bool UsarGetAxisRaw = true;
    [SerializeField] private Transform camara;

    [Header("Movimiento")]
    [SerializeField] public float velocidadMovimiento = 10f;
    [SerializeField] private float fuerzaSalto = 7f;
    [SerializeField] private float gravedad = -15f;
    [SerializeField] private int maxSaltos = 2;

    [SerializeField] Vector3 velocidadVertical;
    private int saltosRestantes;
    private CharacterController controlador;

    [Header("Chequeo de Suelo por Raycast y LayerMask")]
    [SerializeField] private LayerMask capaSuelo;
    [SerializeField] private float distanciaRayoSuelo = 0.35f;
    [SerializeField] private Transform puntoPies;
    private bool estaEnSuelo = false;

    private void Awake()
    {
        controlador = GetComponent<CharacterController>();
        saltosRestantes = maxSaltos;

        if (camara == null)
        {
            Camera camHija = GetComponentInChildren<Camera>();
            if (camHija != null)
                camara = camHija.transform;
            else if (Camera.main != null)
                camara = Camera.main.transform;
        }

        // Si no se asignó en el Inspector, configuramos LayerMask por defecto a Ground y Default
        if (capaSuelo.value == 0)
        {
            capaSuelo = LayerMask.GetMask("Ground", "Default");
            if (capaSuelo.value == 0) capaSuelo = ~LayerMask.GetMask("Ignore Raycast");
        }
        // Excluimos la capa del propio jugador para evitar que el rayo colisione consigo mismo
        capaSuelo &= ~(1 << gameObject.layer);
    }

    void Update()
    {
        if (controlador == null || camara == null) return;

        // Validamos si el personaje está en el suelo proyectando un rayo desde sus pies hacia abajo
        estaEnSuelo = CheckGroundRaycast();

        float Horizontal = UsarGetAxisRaw ? Input.GetAxisRaw("Horizontal") : Input.GetAxis("Horizontal");
        float Vertical = UsarGetAxisRaw ? Input.GetAxisRaw("Vertical") : Input.GetAxis("Vertical");

        Vector3 adelanteCamara = camara.forward;
        Vector3 derechaCamara = camara.right;
        adelanteCamara.y = 0f;
        derechaCamara.y = 0f;
        adelanteCamara.Normalize();
        derechaCamara.Normalize();

        Vector3 direccionPlano = (derechaCamara * Horizontal + adelanteCamara * Vertical);
        if (direccionPlano.sqrMagnitude > 0.0001f)
            direccionPlano.Normalize();

        Vector3 movimientoXZ = direccionPlano * velocidadMovimiento;

        // Si el raycast confirma contacto con el suelo
        if (estaEnSuelo && velocidadVertical.y < 0)
        {
            velocidadVertical.y = -2f;
            saltosRestantes = maxSaltos; 
        }

        // Validación de salto limpia
        if (Input.GetButtonDown("Jump") && (estaEnSuelo || saltosRestantes > 0))
        {
            velocidadVertical.y = fuerzaSalto;
            saltosRestantes--;
        }

        velocidadVertical.y += gravedad * Time.deltaTime;
        Vector3 movimientoTotal = movimientoXZ + velocidadVertical;
        controlador.Move(movimientoTotal * Time.deltaTime);
    }

    private bool CheckGroundRaycast()
    {
        // Origen en los pies del jugador
        Vector3 origen = puntoPies != null ? puntoPies.position : (transform.position + Vector3.up * 0.15f);
        float longitudRayo = distanciaRayoSuelo + 0.15f;

        // Proyectamos el rayo hacia el suelo usando LayerMask
        bool hit = Physics.Raycast(origen, Vector3.down, out RaycastHit hitInfo, longitudRayo, capaSuelo, QueryTriggerInteraction.Ignore);

        // Debug visual en la escena: Verde si toca suelo, Rojo si está en el aire
        Debug.DrawRay(origen, Vector3.down * longitudRayo, hit ? Color.green : Color.red);

        // Combinamos con controlador.isGrounded como respaldo de estabilidad
        return hit || controlador.isGrounded;
    }
}
