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

        if (Physics.Raycast(origin, dir, out hit, rayDistance))
        {
            AplyDamage(hit.collider.gameObject);
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

        Collider[] colliders = Physics.OverlapSphere(origin, radius);
        if (colliders.Length > 0)
        {
            foreach (Collider col in colliders)
            {
                if (col.gameObject.CompareTag("Character") || col.gameObject.CompareTag("Player"))
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
        float currentvelocidadMovimiento = 0f;
        if (isPlayer)
        {
            currentvelocidadMovimiento = pM.velocidadMovimiento;
            pM.velocidadMovimiento = 0f;
            canAttack = false;

            yield return new WaitForSeconds(cooldownAttack);

            pM.velocidadMovimiento = currentvelocidadMovimiento;
            canAttack = true;
        }
    }

    private void AplyDamage(GameObject hitObj)
    {
        if ((hitObj.CompareTag("Player") || hitObj.CompareTag("Character")) && hitObj != null)
        {
            hitObj.GetComponent<HealthSystem>().TakeDamage(damage);
            isCharacter = true;

            Debug.Log($"<color=green>¡IMPACTO! Golpes a {hitObj.name} causándole {damage} de daño.</color>");
        }
        else
        {
            isCharacter = false;
        }
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