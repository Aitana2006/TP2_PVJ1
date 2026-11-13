using UnityEngine;
using System.Collections;

public class powerup : MonoBehaviour
{
    [Header("Configuración del Power-Up")]
    public float multiplicador = 2f;
    public float duracion = 8f;

    private bool yaRecogido = false;
    private bool isBoosted = false;
    private float tiempoRestante = 0f;

    private void Update()
    {
        if (!yaRecogido)
        {
            transform.Rotate(Vector3.up * 90f * Time.deltaTime, Space.World);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (yaRecogido) return;

        if (other.CompareTag("Player") || other.transform.root.CompareTag("Player"))
        {
            playerMovimiento player = other.GetComponent<playerMovimiento>() ?? other.GetComponentInParent<playerMovimiento>();

            if (player != null)
            {
                yaRecogido = true;
                StartCoroutine(AplicarBoost(player));

                foreach (Collider c in GetComponentsInChildren<Collider>())
                {
                    c.enabled = false;
                }
                foreach (Renderer r in GetComponentsInChildren<Renderer>())
                {
                    r.enabled = false;
                }
            }
        }
    }

    private IEnumerator AplicarBoost(playerMovimiento player)
    {
        isBoosted = true;
        tiempoRestante = duracion;
        float velocidadOriginal = player.velocidadMovimiento;

        player.velocidadMovimiento *= multiplicador;
        Debug.Log($"<color=cyan>¡Power-Up recogido! Velocidad aumentada a {player.velocidadMovimiento}</color>");

        while (tiempoRestante > 0)
        {
            yield return new WaitForSeconds(0.1f);
            tiempoRestante -= 0.1f;
        }

        if (player != null)
        {
            player.velocidadMovimiento = velocidadOriginal;
            Debug.Log("<color=cyan>¡El efecto de velocidad ha terminado!</color>");
        }

        Destroy(gameObject);
    }

    private void OnGUI()
    {
        if (isBoosted && tiempoRestante > 0)
        {
            GUIStyle boxStyle = new GUIStyle(GUI.skin.box);
            GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.fontSize = 15;
            labelStyle.fontStyle = FontStyle.Bold;
            labelStyle.normal.textColor = Color.cyan;

            GUI.Box(new Rect(20, 80, 240, 32), "", boxStyle);
            GUI.Label(new Rect(28, 85, 230, 24), $"⚡ BOOST VELOCIDAD: {tiempoRestante:F1}s", labelStyle);
        }
    }
}