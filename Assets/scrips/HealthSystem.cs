using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;
    public float CurrentHealth => currentHealth; 

    [SerializeField] private bool isDead;
    public bool IsDead => isDead;

    [Header("UI (Opcional si usas Slider de Canvas)")]
    [SerializeField] private Slider healthSlider;

    [SerializeField] private GameObject vfx;

    private Rigidbody rb;
    private Collider col;
    private Attack attack;
    private Renderer characterRenderer;

    // Control para mostrar daño y efectos en pantalla
    private float lastDamageTime = -10f;
    private float lastDamageAmount = 0f;
    private float lastHealTime = -10f;
    private float lastHealAmount = 0f;
    private static Texture2D whiteTexture;

    private void Awake()
    {
        currentHealth = maxHealth;
        isDead = false;

        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        attack = GetComponent<Attack>();
        characterRenderer = GetComponentInChildren<Renderer>();

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        if (gameObject.CompareTag("Player"))
        {
            lastDamageTime = Time.time;
            lastDamageAmount = amount;
            Debug.Log($"<color=red>¡El Jugador recibió {amount} de daño! Vida actual: {currentHealth}/{maxHealth}</color>");
        }
        else
        {
            Debug.Log($"{gameObject.name} recibe daño. Salud actual: {currentHealth}");
            // Si el enemigo es atacado, reacciona y persigue al jugador
            EnemyAI enemyAI = GetComponent<EnemyAI>();
            if (enemyAI != null)
            {
                enemyAI.OnAttackedByPlayer();
            }
        }

        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }

        if (currentHealth <= 0f)
        {
            Dead();
        }
    }

    public void Heal(float amount)
    {
        if (isDead) return;
        if (currentHealth >= maxHealth) return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        lastHealTime = Time.time;
        lastHealAmount = amount;

        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }
    }

    private void Dead()
    {
        isDead = true;

        if (gameObject.CompareTag("Player"))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.NotifyPlayerDead();
            }
            Debug.Log("¡El Jugador ha muerto! Game Over.");
        }
        else if (gameObject.CompareTag("Character"))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.NotifyEnemyDefeat();
            }
            Debug.Log("Enemigo derrotado.");
        }

        StartCoroutine(DestroyTransition());
    }

    IEnumerator DestroyTransition()
    {
        if (col != null) col.enabled = false;
        if (rb != null) rb.isKinematic = true;

        if (attack != null) attack.enabled = false;

        if (characterRenderer != null) characterRenderer.enabled = false;
          
        yield return new WaitForSeconds(5f);

        Destroy(this.gameObject);
    }

    private void OnGUI()
    {
        // Solo mostramos la barra de vida en pantalla para el Jugador
        if (!gameObject.CompareTag("Player")) return;

        // Flash rojo en la pantalla cuando el enemigo ataca al jugador
        if (Time.time - lastDamageTime < 0.35f && !isDead)
        {
            float flashAlpha = (0.35f - (Time.time - lastDamageTime)) / 0.35f * 0.35f;
            DrawQuad(new Rect(0, 0, Screen.width, Screen.height), new Color(1f, 0f, 0f, flashAlpha));
        }

        // Resplandor verde suave en la pantalla al curarse
        if (Time.time - lastHealTime < 0.35f && !isDead)
        {
            float greenAlpha = (0.35f - (Time.time - lastHealTime)) / 0.35f * 0.2f;
            DrawQuad(new Rect(0, 0, Screen.width, Screen.height), new Color(0.1f, 1f, 0.3f, greenAlpha));
        }

        // Posición y dimensiones de la barra de vida
        float barX = 20f;
        float barY = 20f;
        float barWidth = 240f;
        float barHeight = 24f;

        // Borde y fondo negro
        DrawQuad(new Rect(barX - 3, barY - 3, barWidth + 6, barHeight + 6), new Color(0f, 0f, 0f, 0.8f));
        DrawQuad(new Rect(barX, barY, barWidth, barHeight), new Color(0.2f, 0.2f, 0.2f, 0.85f));

        // Relleno de la barra según la vida restante
        float healthPercent = currentHealth / maxHealth;
        Color barColor = Color.green;
        if (healthPercent <= 0.25f) barColor = Color.red;
        else if (healthPercent <= 0.5f) barColor = Color.yellow;

        DrawQuad(new Rect(barX, barY, barWidth * healthPercent, barHeight), barColor);

        // Texto de vida sobre la barra
        GUIStyle textStyle = new GUIStyle(GUI.skin.label);
        textStyle.fontStyle = FontStyle.Bold;
        textStyle.fontSize = 14;
        textStyle.alignment = TextAnchor.MiddleCenter;
        textStyle.normal.textColor = Color.white;

        GUI.Label(new Rect(barX, barY, barWidth, barHeight), $"VIDA: {Mathf.CeilToInt(currentHealth)} / {maxHealth}", textStyle);

        // Notificación de curación
        if (Time.time - lastHealTime < 0.8f && !isDead)
        {
            GUIStyle healStyle = new GUIStyle(GUI.skin.label);
            healStyle.fontStyle = FontStyle.Bold;
            healStyle.fontSize = 15;
            healStyle.normal.textColor = new Color(0.2f, 1f, 0.4f, 1f);
            GUI.Label(new Rect(barX, barY + barHeight + 5, barWidth, 24), "+ ¡Curando salud!", healStyle);
        }
        // Notificación de ataque recibido
        else if (Time.time - lastDamageTime < 2.0f && !isDead)
        {
            GUIStyle damageStyle = new GUIStyle(GUI.skin.label);
            damageStyle.fontStyle = FontStyle.Bold;
            damageStyle.fontSize = 15;
            damageStyle.normal.textColor = new Color(1f, 0.25f, 0.25f, 1f);
            GUI.Label(new Rect(barX, barY + barHeight + 5, barWidth, 24), $"¡Ataque recibido! -{lastDamageAmount} HP", damageStyle);
        }

        // Mensaje en caso de muerte (fallback si no hay GameManager)
        if (isDead && GameManager.Instance == null)
        {
            GUIStyle deadStyle = new GUIStyle(GUI.skin.label);
            deadStyle.fontStyle = FontStyle.Bold;
            deadStyle.fontSize = 32;
            deadStyle.alignment = TextAnchor.MiddleCenter;
            deadStyle.normal.textColor = Color.red;
            GUI.Label(new Rect(0, Screen.height / 2 - 40, Screen.width, 50), "¡GAME OVER!", deadStyle);
        }
    }

    private static void DrawQuad(Rect rect, Color color)
    {
        if (whiteTexture == null)
        {
            whiteTexture = new Texture2D(1, 1);
            whiteTexture.SetPixel(0, 0, Color.white);
            whiteTexture.Apply();
        }

        Color prevColor = GUI.color;
        GUI.color = color;
        GUI.DrawTexture(rect, whiteTexture);
        GUI.color = prevColor;
    }
}