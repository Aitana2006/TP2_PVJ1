using UnityEngine;

public class PickItem : MonoBehaviour
{
    [Header("Manos del Personaje")]
    [SerializeField] private Transform rightHand; // Para armas ("Weapon")
    [SerializeField] private Transform leftHand;  // Para ítems ("Item")

    // Variables para controlar cada mano de forma independiente
    private GameObject currentLeftItem = null;
    private GameObject currentRightItem = null;

    // Referencia opcional al script de ataque
    private Attack attackScript;

    private void Awake()
    {
        attackScript = GetComponent<Attack>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (other.CompareTag("Item"))
            {
                // Si la mano izquierda está libre, agarramos el ítem
                if (currentLeftItem == null)
                {
                    Pick(other.gameObject, leftHand, true);
                }
                else
                {
                    Debug.Log("La mano izquierda ya tiene un ítem.");
                }
            }
            else if (other.CompareTag("Weapon"))
            {
                // Si la mano derecha está libre, agarramos el arma
                if (currentRightItem == null)
                {
                    Pick(other.gameObject, rightHand, false);
                }
                else
                {
                    Debug.Log("La mano derecha ya tiene un arma.");
                }
            }
        }
    }

    private void Pick(GameObject item, Transform targetHand, bool isLeftHand)
    {
        if (targetHand == null)
        {
            Debug.LogError("¡La mano seleccionada no está asignada en el Inspector!");
            return;
        }

        // Asignamos el objeto a la mano correspondiente
        if (isLeftHand)
        {
            currentLeftItem = item;
        }
        else
        {
            currentRightItem = item;

            // Si es un arma, le avisamos automáticamente al script Attack que la equipe
            if (attackScript != null)
            {
                attackScript.EquipWeapon(item.transform);
            }
        }

        // Emparentar y acomodar en la mano
        item.transform.SetParent(targetHand);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;

        // Desactivar físicas y colisiones mientras se lleva equipado
        Rigidbody rb = item.GetComponent<Rigidbody>();
        Collider col = item.GetComponent<Collider>();

        if (rb != null) rb.isKinematic = true;
        if (col != null) col.enabled = false;
    }

    // Método de compatibilidad general para que GoalZone.cs no dé error
    public GameObject DropItem()
    {
        if (currentRightItem != null)
        {
            return DropRightItem();
        }
        else if (currentLeftItem != null)
        {
            return DropLeftItem();
        }
        return null;
    }

    // Métodos para soltar por separado si los necesitas en el futuro
    public GameObject DropLeftItem()
    {
        GameObject temp = currentLeftItem;
        currentLeftItem = null;
        return temp;
    }

    public GameObject DropRightItem()
    {
        GameObject temp = currentRightItem;
        currentRightItem = null;

        // Opcional: si suelta el arma, le avisamos al script Attack que ya no tiene espada
        if (attackScript != null)
        {
            attackScript.EquipWeapon(null);
        }

        return temp;
    }
}