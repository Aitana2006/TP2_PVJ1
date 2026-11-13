using UnityEngine;

public class PickItem : MonoBehaviour
{
    [Header("Manos del Personaje")]
    [SerializeField] private Transform rightHand;
    [SerializeField] private Transform leftHand;  

    private GameObject currentLeftItem = null;
    private GameObject currentRightItem = null;

    private Attack attackScript;

    private void Awake()
    {
        attackScript = GetComponent<Attack>();
    }

    private void Update()
    {
 
        if (Input.GetKeyDown(KeyCode.E))
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, 2.5f);
            foreach (var col in colliders)
            {
                if (col.CompareTag("Item") && currentLeftItem == null)
                {
                    Pick(col.gameObject, leftHand, true);
                    break;
                }
                else if (col.CompareTag("Weapon") && currentRightItem == null)
                {
                    Pick(col.gameObject, rightHand, false);
                    break;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            GameObject dropped = DropItem();
            if (dropped != null)
            {
                dropped.transform.SetParent(null);
                Rigidbody rb = dropped.GetComponent<Rigidbody>();
                Collider col = dropped.GetComponent<Collider>();
                if (rb != null)
                {
                    rb.isKinematic = false;
                    rb.useGravity = true;
                }
                if (col != null) col.enabled = true;
                Debug.Log($"Soltaste {dropped.name}");
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (other.CompareTag("Item"))
            {

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

    public void PickDirectly(GameObject item)
    {
        if (item == null) return;
        if (item.CompareTag("Item"))
        {
            if (currentLeftItem == null)
            {
                Pick(item, leftHand, true);
            }
            else
            {
                Debug.Log("La mano izquierda ya tiene un ítem.");
            }
        }
        else if (item.CompareTag("Weapon"))
        {
            if (currentRightItem == null)
            {
                Pick(item, rightHand, false);
            }
            else
            {
                Debug.Log("La mano derecha ya tiene un arma.");
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

        if (isLeftHand)
        {
            currentLeftItem = item;
        }
        else
        {
            currentRightItem = item;

            if (attackScript != null)
            {
                attackScript.EquipWeapon(item.transform);
            }
        }

        item.transform.SetParent(targetHand);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;

        Rigidbody rb = item.GetComponent<Rigidbody>();
        Collider col = item.GetComponent<Collider>();

        if (rb != null) rb.isKinematic = true;
        if (col != null) col.enabled = false;
    }

    public GameObject DropItem()
    {
        if (currentLeftItem != null)
        {
            return DropLeftItem();
        }
        else if (currentRightItem != null)
        {
            return DropRightItem();
        }
        return null;
    }

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

        if (attackScript != null)
        {
            attackScript.EquipWeapon(null);
        }

        return temp;
    }
}