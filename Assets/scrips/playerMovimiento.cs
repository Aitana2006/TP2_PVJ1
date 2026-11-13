using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class playerMovimiento : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private bool UsarGetAxisRaw = true;
    [SerializeField] private Transform camara;

    [Header("Movimiento")]
    [SerializeField] public float velocidadMovimiento = 10f;
    [SerializeField] private float fuerzaSalto = 6f;
    [SerializeField] private float gravedad = -9f;
    [SerializeField] private int maxSaltos = 2;

    [SerializeField] Vector3 velocidadVertical;
    private int saltosRestantes;
    private CharacterController controlador;

    private void Awake()
    {
        controlador = GetComponent<CharacterController>();

        if (camara == null)
        {
            Camera camHija = GetComponentInChildren<Camera>();
            if (camHija != null)
                camara = camHija.transform;
            else if (Camera.main != null)
                camara = Camera.main.transform;
        }
    }

    void Update()
    {
        if (controlador == null || camara == null) return;

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

        if (controlador.isGrounded && velocidadVertical.y < 0)
        {
            velocidadVertical.y = -2f;
            saltosRestantes = maxSaltos; 
        }

        if (Input.GetButtonDown("Jump") && saltosRestantes > 0)
        {
            velocidadVertical.y = fuerzaSalto;
            saltosRestantes--;
        }

        velocidadVertical.y += gravedad * Time.deltaTime;
        Vector3 movimientoTotal = movimientoXZ + velocidadVertical;
        controlador.Move(movimientoTotal * Time.deltaTime);
    }
}
