using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class InventorySlotUI : MonoBehaviour
{
    [Header("Refs")]
    public Image icon;               // 显示物品图标
    public TextMeshProUGUI nameText; // 显示物品名称
    public TextMeshProUGUI countText;  //物品数量

    private InventoryItemData itemData;  // 物品数据
    private int count;
    private Action<InventoryItemData> onClick;

    // 用来绑定物品数据，并设置点击事件
    public void Bind(InventoryItemData data, Action<InventoryItemData> clickHandler, int itemCount = 1)
    {
        itemData = data;
        count = itemCount;
        //把从外部传进来的「点击时要执行的回调函数」保存到当前对象（this）中的 onClick 变量里
        this.onClick = clickHandler;

        // 显示
        if (icon) icon.sprite = data.icon;
        if (nameText) nameText.text = data.displayName;
        if (countText) countText.text = (itemCount >= 1) ? $"{itemCount}" : "";
        
        GetComponent<Button>().onClick.RemoveAllListeners();  // 移除旧的监听事件
        GetComponent<Button>().onClick.AddListener(() => onClick?.Invoke(itemData)); // 点击后触发事件
    }

    // （可选）当背包数量变化时也可单独刷新显示
    public void SetCount(int newCount)
    {
        count = newCount;
        if (countText) countText.text = (newCount > 1) ? $"{newCount}" : "";
    }
}

