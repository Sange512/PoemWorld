using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableItem : MonoBehaviour, IInteractable
{
    public InventoryItemData itemData;

    public void Interact()
    {
        if (itemData == null) return;
        InventorySystem.Instance.AddItem(itemData);
        gameObject.SetActive(false);
    }
    public string GetPromptText() => $"°´ E Ê°È¡¡¸{itemData.displayName}¡¹";
}

