using UnityEngine;

public class ItemPickup : MonoBehaviour, IInteractuable
{
    private void Awake()
    {
        // Aseguramos que pertenezca a la capa Interactable
        int layerIdx = LayerMask.NameToLayer("Interactable");
        if (layerIdx != -1)
        {
            gameObject.layer = layerIdx;
        }
    }

    public void Interactuar(GameObject instigador)
    {
        PickItem pick = instigador.GetComponent<PickItem>() ?? instigador.GetComponentInParent<PickItem>();
        if (pick != null)
        {
            pick.PickDirectly(this.gameObject);
        }
    }

    public string ObtenerMensaje()
    {
        return $"recoger {gameObject.name}";
    }
}

