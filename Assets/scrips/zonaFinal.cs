using UnityEngine;

public class zonaFinal : MonoBehaviour
{
    [SerializeField] private Transform goal;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PickItem pickItem = other.GetComponent<PickItem>();

            if (pickItem != null)
            {

                GameObject item = pickItem.DropItem();

                if (item != null)
                {
                    ItemInZone(item);

                    if (GameManager.Instance != null)
                    {
                        GameManager.Instance.NotifyGoalZone();
                    }

                    Debug.Log("¡Completaste el nivel y entregaste el ítem!");
                }
                else
                {
                    Debug.Log("Llegaste a la meta, pero no llevas ningún ítem encima.");
                  
                }
            }
        }
    }

    private void ItemInZone(GameObject item)
    {
        item.transform.SetParent(goal);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;

        Collider col = item.GetComponent<Collider>();
        Rigidbody rb = item.GetComponent<Rigidbody>();

        if (col != null) col.enabled = true;
        if (rb != null) rb.isKinematic = false;
    }
}