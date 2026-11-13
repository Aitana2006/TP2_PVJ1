using UnityEngine;

public class HealingStation : MonoBehaviour
{
    [Header("Configuración de Curación")]
    [SerializeField] private float healAmount = 10f; // +10 HP por ciclo
    [SerializeField] private float healInterval = 1.5f; // Cada 1.5 segundos
    [SerializeField] private float maxCharges = 50f; // Límite de carga máxima antes de recargar
    [SerializeField] private float cooldownTime = 5f; // Tiempo de recarga (cooldown)

    [Header("Retroalimentación Visual")]
    [SerializeField] private Light stationLight; // Luz de la estación
    [SerializeField] private Renderer stationRenderer; // Malla para cambiar color

    private float currentCharges;
    private bool isCoolingDown = false;
    private float cooldownTimer = 0f;
    private float nextHealTime = 0f;
    private bool isPlayerInside = false;

    private HealthSystem playerHealth;
    private Material stationMat;
    private Color activeColor = new Color(0.15f, 0.95f, 0.35f);
    private Color cooldownColor = new Color(0.35f, 0.35f, 0.35f);

    private void Awake()
    {
        currentCharges = maxCharges;
        if (stationLight == null) stationLight = GetComponentInChildren<Light>();
        if (stationRenderer == null) stationRenderer = GetComponent<Renderer>();
        if (stationRenderer != null) stationMat = stationRenderer.material;
    }

    private void Update()
    {
        // Rotación continua decorativa
        transform.Rotate(Vector3.up * 30f * Time.deltaTime, Space.World);

        // Control del tiempo de enfriamiento (Cooldown)
        if (isCoolingDown)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0f)
            {
                ReactivateStation();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsPlayer(other.gameObject))
        {
            isPlayerInside = true;
            ResolvePlayerHealth(other.gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (isCoolingDown) return;

        if (IsPlayer(other.gameObject))
        {
            isPlayerInside = true;
            ResolvePlayerHealth(other.gameObject);

            if (playerHealth != null && !playerHealth.IsDead && playerHealth.CurrentHealth < 100f)
            {
                if (Time.time >= nextHealTime)
                {
                    ApplyProgressiveHeal();
                    nextHealTime = Time.time + healInterval;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (IsPlayer(other.gameObject))
        {
            isPlayerInside = false;
        }
    }

    private void ApplyProgressiveHeal()
    {
        if (currentCharges <= 0f || isCoolingDown) return;

        float amountToHeal = Mathf.Min(healAmount, currentCharges);
        playerHealth.Heal(amountToHeal);
        currentCharges -= amountToHeal;

        Debug.Log($"<color=green>[Estación de Curación] Curó +{amountToHeal} HP. Carga restante: {currentCharges}/{maxCharges}</color>");

        // Límite alcanzado: entra en estado deshabilitado (Cooldown)
        if (currentCharges <= 0f)
        {
            StartCooldown();
        }
    }

    private void StartCooldown()
    {
        isCoolingDown = true;
        cooldownTimer = cooldownTime;
        Debug.Log("<color=yellow>[Estación de Curación] Estación en Recarga. Carga agotada, enfriando...</color>");

        // 3. Retroalimentación: Apagar luz y atenuar la estación
        if (stationLight != null) stationLight.enabled = false;
        if (stationMat != null) stationMat.color = cooldownColor;
    }

    private void ReactivateStation()
    {
        isCoolingDown = false;
        currentCharges = maxCharges;
        Debug.Log("<color=green>[Estación de Curación] Estación reactivada y lista para curar.</color>");

        // Encender luz y restaurar color activo
        if (stationLight != null) stationLight.enabled = true;
        if (stationMat != null) stationMat.color = activeColor;
    }

    private void ResolvePlayerHealth(GameObject obj)
    {
        if (playerHealth == null)
        {
            playerHealth = obj.GetComponent<HealthSystem>() ?? obj.GetComponentInParent<HealthSystem>() ?? obj.GetComponentInChildren<HealthSystem>();
            if (playerHealth == null)
            {
                GameObject p = GameObject.FindGameObjectWithTag("Player");
                if (p != null) playerHealth = p.GetComponent<HealthSystem>();
            }
        }
    }

    private bool IsPlayer(GameObject obj)
    {
        return obj.CompareTag("Player") || (obj.transform.root != null && obj.transform.root.CompareTag("Player"));
    }

    private void OnGUI()
    {
        if (!isPlayerInside) return;

        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.fontStyle = FontStyle.Bold;
        style.fontSize = 18;
        style.alignment = TextAnchor.MiddleCenter;

        float width = 520f;
        float height = 35f;
        float x = (Screen.width - width) / 2f;
        float y = Screen.height - 80f;

        if (isCoolingDown)
        {
            style.normal.textColor = new Color(1f, 0.7f, 0.2f);
            GUI.Label(new Rect(x, y, width, height), $"[ Estación en Recarga ] - Lista en {Mathf.CeilToInt(cooldownTimer)}s", style);
        }
        else if (playerHealth != null && playerHealth.CurrentHealth >= 100f)
        {
            style.normal.textColor = Color.white;
            GUI.Label(new Rect(x, y, width, height), "[ Estación de Curación: Salud al Máximo ]", style);
        }
        else
        {
            style.normal.textColor = new Color(0.2f, 1f, 0.4f);
            GUI.Label(new Rect(x, y, width, height), $"[ Estación de Curación: +{healAmount} HP cada {healInterval}s ] (Carga: {Mathf.CeilToInt(currentCharges)}/{maxCharges})", style);
        }
    }
}
