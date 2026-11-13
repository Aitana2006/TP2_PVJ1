using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Attack : MonoBehaviour
{
    [Header("Raycast Shoot Settings")]
    [SerializeField] private Transform gun;
    [SerializeField] private float rayDistance;
    [SerializeField] private Vector3 offsetGun;

    [Header("Raycast Shoot Settings")]
    [SerializeField] private Transform sword;
    [SerializeField] private float radius;
    [SerializeField] private Vector3 offsetSword;

    [Header("Attack Settings")]
    [SerializeField] private float damage = 10f;

    [SerializeField] private float cooldownAttack;
    private bool canAttack = true;
    private Vector3 origin;
    private Vector3 dir;
    private bool isCharacter;
    private RaycastHit hit;

    [SerializeField] private bool isPlayer;
    [SerializeField] private bool isMelee;

    private playerMovimiento pM;

    private void Awake()
    {
        pM = GetComponent<playerMovimiento>();
    }

    private void Update()
    {
        if (isPlayer)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) && canAttack)
            {
                if (isMelee)
                {
                    Melee();
                }
                else
                {
                    Shoot();
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (!isMelee)
        {
            if (hit.collider == null) return;
            Gizmos.color = isCharacter ? Color.green : Color.red;

            Gizmos.DrawLine(origin, hit.point);
        }
        else
        {
            Gizmos.color = isCharacter ? Color.green : Color.red;

            Gizmos.DrawWireSphere(origin, radius);
        }
    }

    public void Shoot()
    {
        RaycastInfo(offsetGun);

        // Disparo por Raycast: Ignoramos colisionadores propios y usamos máscara de ataque
        int layerMask = ~(1 << gameObject.layer); // Filtramos la capa del propio jugador

        if (Physics.Raycast(origin, dir, out hit, rayDistance, layerMask, QueryTriggerInteraction.Ignore))
        {
            // Verificación estricta de no autodaño
            if (hit.collider.gameObject != this.gameObject && hit.collider.transform.root != transform.root)
            {
                AplyDamage(hit.collider.gameObject);
            }
        }
        StartCoroutine(AttackCooldown());
    }

    public void Melee()
    {
        if (sword == null)
        {
            origin = transform.position + offsetSword;
        }
        else
        {
            RaycastInfo(offsetSword);
        }

        // Colisionador de ataque cuerpo a cuerpo
        Collider[] colliders = Physics.OverlapSphere(origin, radius, ~0, QueryTriggerInteraction.Ignore);
        if (colliders.Length > 0)
        {
            foreach (Collider col in colliders)
            {
                // Garantizar que no colisione ni dañe al propio jugador ni a sus hijos
                if (col.gameObject != this.gameObject && col.transform.root != transform.root)
                {
                    AplyDamage(col.gameObject);
                }
            }
        }
        else
        {
            isCharacter = false;
        }

        StartCoroutine(AttackCooldown());
    }

    IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(cooldownAttack > 0f ? cooldownAttack : 0.4f);
        canAttack = true;
    }

    private void AplyDamage(GameObject hitObj)
    {
        if (hitObj == null) return;

        // SEGURIDAD: Garantizar al 100% que no haya autodaño
        if (hitObj == this.gameObject || hitObj.transform.root == transform.root) return;

        // El jugador solo puede dañar a 'Character' (enemigos), nunca a 'Player'
        if (isPlayer && (hitObj.CompareTag("Player") || hitObj.transform.root.CompareTag("Player"))) return;

        // El enemigo solo puede dañar a 'Player', nunca a otros enemigos
        if (!isPlayer && (hitObj.CompareTag("Character") || hitObj.transform.root.CompareTag("Character"))) return;

        bool targetValid = isPlayer ? hitObj.CompareTag("Character") : hitObj.CompareTag("Player");
        if (targetValid)
        {
            HealthSystem hs = hitObj.GetComponent<HealthSystem>() ?? hitObj.GetComponentInParent<HealthSystem>();
            if (hs != null)
            {
                hs.TakeDamage(damage);
                isCharacter = true;
                Debug.Log($"<color=green>¡IMPACTO! Golpes a {hitObj.name} causándole {damage} de daño.</color>");
                return;
            }
        }

        isCharacter = false;
    }

    private void RaycastInfo(Vector3 offset)
    {
        if (sword != null)
        {
            origin = sword.TransformPoint(offset);
        }
        else
        {
            origin = transform.position + offset;
        }
        dir = transform.forward;
    }

    public void EquipWeapon(Transform newWeapon)
    {
        sword = newWeapon;
        if (newWeapon != null)
        {
            isMelee = true;
        }
        else
        {
            isMelee = false;
        }
    }
}