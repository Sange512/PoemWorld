using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Inventory Item")]
public class InventoryItemData : ScriptableObject
{
    public string itemId;//物品id
    public string displayName;//物品名称
    [TextArea(2, 6)] public string description;//物品描述
    public Sprite icon;//物品图标
}
