using UnityEngine;

public class zonaFinal : MonoBehaviour
{
    [SerializeField] private Transform goal;

    private bool levelCompleted = false;
    private GameObject coinObject;
    private float showHintTime = -10f;

    private void Start()
    {
        coinObject = FindCoin();
    }

    private void Update()
    {
        if (levelCompleted) return;

        if (coinObject == null)
        {
            coinObject = FindCoin();
        }

        // Si la moneda está libre en el mundo y cae o está en la plataforma
        if (coinObject != null)
        {
            Transform p = coinObject.transform.parent;
            bool isCarried = p != null && (p.name.Contains("Hand") || p.name.Contains("mano") || p.CompareTag("Player") || (p.root != null && p.root.CompareTag("Player")));

            if (!isCarried && p != (goal != null ? goal : transform))
            {
                Transform targetGoal = goal != null ? goal : transform;
                float dist = Vector3.Distance(targetGoal.position, coinObject.transform.position);
                if (dist <= 2.5f)
                {
                    TriggerVictory(coinObject);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        HandleTriggerOrCollision(other.gameObject);
    }

    private void OnTriggerStay(Collider other)
    {
        HandleTriggerOrCollision(other.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        HandleTriggerOrCollision(collision.gameObject);
    }

    private void HandleTriggerOrCollision(GameObject obj)
    {
        if (levelCompleted || obj == null) return;

        // 1. Si el objeto que entra en la plataforma es directamente la moneda
        if (IsCoin(obj))
        {
            TriggerVictory(obj);
            return;
        }

        // 2. Si es el jugador quien entra o pisa la plataforma
        if (IsPlayer(obj))
        {
            PickItem pickItem = obj.GetComponent<PickItem>() ?? obj.GetComponentInParent<PickItem>();
            if (pickItem != null)
            {
                GameObject heldItem = pickItem.DropItem();
                if (heldItem != null && IsCoin(heldItem))
                {
                    TriggerVictory(heldItem);
                }
                else
                {
                    // El jugador pisó la plataforma pero no lleva la moneda -> NO gana
                    ShowHintNoCoin();
                }
            }
        }
    }

    private bool IsCoin(GameObject obj)
    {
        if (obj == null) return false;
        if (obj.CompareTag("Item")) return true;
        string n = obj.name.ToLower();
        return n.Contains("moneda") || n.Contains("coin");
    }

    private bool IsPlayer(GameObject obj)
    {
        return obj.CompareTag("Player") || (obj.transform.root != null && obj.transform.root.CompareTag("Player"));
    }

    private void TriggerVictory(GameObject itemObj)
    {
        if (levelCompleted) return;
        levelCompleted = true;

        ItemInZone(itemObj);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.NotifyGoalZone();
        }

        Debug.Log("<color=cyan>¡La moneda está en la plataforma! ¡Victoria!</color>");
    }

    private void ItemInZone(GameObject item)
    {
        Transform targetGoal = goal != null ? goal : transform;

        item.transform.SetParent(targetGoal);
        item.transform.localPosition = new Vector3(0f, 0.8f, 0f);
        item.transform.localRotation = Quaternion.identity;

        Collider col = item.GetComponent<Collider>();
        Rigidbody rb = item.GetComponent<Rigidbody>();

        if (col != null) col.enabled = true;
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }

    private GameObject FindCoin()
    {
        GameObject coin = GameObject.Find("moneda");
        if (coin == null)
        {
            GameObject[] items = GameObject.FindGameObjectsWithTag("Item");
            foreach (var it in items)
            {
                if (it.name.ToLower().Contains("moneda") || it.name.ToLower().Contains("coin"))
                    return it;
            }
            if (items.Length > 0) return items[0];
        }
        return coin;
    }

    private void ShowHintNoCoin()
    {
        showHintTime = Time.time;
        Debug.Log("Llegaste a la plataforma pero la moneda no está aquí. ¡Debes traer la moneda para ganar!");
    }

    private void OnGUI()
    {
        if (levelCompleted) return;

        if (Time.time - showHintTime < 3.0f)
        {
            GUIStyle hintStyle = new GUIStyle(GUI.skin.label);
            hintStyle.fontStyle = FontStyle.Bold;
            hintStyle.fontSize = 20;
            hintStyle.alignment = TextAnchor.MiddleCenter;
            hintStyle.normal.textColor = Color.yellow;

            float w = 600f;
            float h = 40f;
            GUI.Label(new Rect((Screen.width - w) / 2f, Screen.height - 90, w, h), "¡Trae la moneda a la plataforma para ganar el nivel!", hintStyle);
        }
    }
}

// Clase de compatibilidad para evitar discrepancias con el nombre antiguo
public class GoalZone : zonaFinal
{
}
