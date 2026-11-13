using UnityEngine;

public class HealingStation : MonoBehaviour
{
    [Header("Configuración de Curación")]
    [SerializeField] private float healRate = 25f; // Salud recuperada por segundo
    [SerializeField] private float healRadius = 3.5f; // Radio de la zona de curación
    [SerializeField] private float rotationSpeed = 35f;

    private Transform playerTransform;
    private HealthSystem playerHealth;
    private bool isPlayerInside = false;

    private void Start()
    {
        FindPlayer();
    }

    private void Update()
    {
        // Rotación continua decorativa
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);

        if (playerTransform == null || playerHealth == null)
        {
            FindPlayer();
            if (playerTransform == null) return;
        }

        // Verificamos por distancia
        float dist = Vector3.Distance(transform.position, playerTransform.position);
        isPlayerInside = dist <= healRadius;

        if (isPlayerInside && playerHealth != null && !playerHealth.IsDead)
        {
            if (playerHealth.CurrentHealth < 100f)
            {
                playerHealth.Heal(healRate * Time.deltaTime);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") || (other.transform.root != null && other.transform.root.CompareTag("Player")))
        {
            isPlayerInside = true;
            if (playerHealth == null) FindPlayer();
            if (playerHealth != null && !playerHealth.IsDead && playerHealth.CurrentHealth < 100f)
            {
                playerHealth.Heal(healRate * Time.deltaTime);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || (other.transform.root != null && other.transform.root.CompareTag("Player")))
        {
            isPlayerInside = false;
        }
    }

    private void FindPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
            playerHealth = playerObj.GetComponent<HealthSystem>() ?? playerObj.GetComponentInChildren<HealthSystem>();
        }
    }

    private void OnGUI()
    {
        if (isPlayerInside && playerHealth != null && !playerHealth.IsDead)
        {
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.fontStyle = FontStyle.Bold;
            style.fontSize = 18;
            style.alignment = TextAnchor.MiddleCenter;
            style.normal.textColor = new Color(0.2f, 1f, 0.4f, 1f);

            float width = 450f;
            float height = 35f;
            float x = (Screen.width - width) / 2f;
            float y = Screen.height - 75f;

            string text = playerHealth.CurrentHealth >= 100f 
                ? "[ Estación de Curación: Salud al Máximo ]" 
                : $"[ Estación de Curación: Regenerando salud (+{Mathf.RoundToInt(healRate)} HP/s) ]";

            GUI.Label(new Rect(x, y, width, height), text, style);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, healRadius);
    }
}

