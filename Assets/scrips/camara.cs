using System.Threading;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class camara : MonoBehaviour
{
    [SerializeField] public float Sensibilidad = 50f;
    public Transform player;

    [Header("limites de la camara")]
    [SerializeField] float limiteArriba = -25f;
    [SerializeField] float limiteAbajo = 60f;
    float rotacionHorinzontal = 0;
    float rotacionVertical = 0;

    void Start()
    {
        //el lockState bloquea en el centro de la pantalla el cursor y el visible lo oculta mientras se juega
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        //nos dan los valores del mouse para moverlo
        float valorX = Input.GetAxis("Mouse X") * Sensibilidad * Time.deltaTime;
        float valorY = Input.GetAxis("Mouse Y") * Sensibilidad * Time.deltaTime;

        //guarda el valor y queda en el valor para seguir
        rotacionHorinzontal += Input.GetAxis("Mouse X") * Sensibilidad * Time.deltaTime;
        rotacionVertical -= valorY;

        //lo va a limitar a mover solo a 80 grados y no maree
        rotacionVertical = Mathf.Clamp(rotacionVertical, limiteArriba, limiteAbajo);
        transform.localRotation = Quaternion.Euler(rotacionVertical, 0f, 0f);

        //hace la rotacion horizontal
        if (player != null)
        {
            player.Rotate(Vector3.up * valorX);
        }
        else
        {
            //si no tiene asignado nada
            Debug.LogWarning("Por favor asigna el objeto 'player' en el Inspector del componente camara.");
        }

    }
}
