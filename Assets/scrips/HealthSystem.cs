using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;
    public float CurrentHealth => currentHealth; 

    [SerializeField] private bool isDead;
    public bool IsDead => isDead;

    [SerializeField] private GameObject vfx;

    private Rigidbody rb;
    private Collider col;
    private Attack attack;

    private Renderer characterRenderer;

    private void Awake()
    {
        currentHealth = maxHealth;
        isDead = false;

        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        attack = GetComponent<Attack>();

        characterRenderer = GetComponentInChildren<Renderer>();
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        Debug.Log($"{gameObject.name} recibe daño. Salud actual: {currentHealth}");

        if (currentHealth <= 0f)
        {
            Dead();
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
}