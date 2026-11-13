using UnityEngine;
using System.Collections;

public class powerup : MonoBehaviour
{
    [Header("Configuración del Power-Up")]
    public float multiplicador = 2f; 
    public float duracion = 8f;      

   private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerMovimiento player = other.GetComponent<playerMovimiento>();

            if (player != null)
            {
                StartCoroutine(AplicarBoost(player));

                Collider col = GetComponent<Collider>();
                if (col != null) col.enabled = false;

                MeshRenderer renderizador = GetComponent<MeshRenderer>();
                if (renderizador != null) renderizador.enabled = false;
            }
        }
    }

    private IEnumerator AplicarBoost(playerMovimiento player)
    {

        float velocidadOriginal = player.velocidadMovimiento;


        player.velocidadMovimiento *= multiplicador;
        Debug.Log("¡Velocidad aumentada!");

        yield return new WaitForSeconds(duracion);

        player.velocidadMovimiento = velocidadOriginal;
        Debug.Log("¡El efecto de velocidad ha terminado!");


        Destroy(gameObject);
    }
}