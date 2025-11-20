using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    [Header("Layout")]
    public Transform slotParent;        // 指到 Grid 节点
    public GameObject slotPrefab;       // 指到 SlotPrefab

    [Header("Info")]
    public TMP_Text nameText;           // 指到 NameText
    public TMP_Text descText;           // 指到 DescText


    private void OnEnable()
    {
        var inv = InventorySystem.Instance ?? FindObjectOfType<InventorySystem>();
        if (inv == null)
        {
            Debug.LogError("[InventoryUI] 未找到 InventorySystem。请在场景放置一个挂有 InventorySystem 的对象。");
            return;
        }
        inv.OnInventoryChanged += RefreshUI;
        RefreshUI();
    }

    private void OnDisable()
    {
        var inv = InventorySystem.Instance ?? FindObjectOfType<InventorySystem>();
        if (inv != null) inv.OnInventoryChanged -= RefreshUI;
    }

    public void RefreshUI()
    {
        if (slotPrefab == null || slotParent == null)
        {
            Debug.LogError("[InventoryUI] slotPrefab 或 slotParent 未绑定。");
            return;
        }

        // 清空旧格子
        for (int i = slotParent.childCount - 1; i >= 0; i--)
            Destroy(slotParent.GetChild(i).gameObject);

        // 生成新格子并绑定点击
        var inv = InventorySystem.Instance;
        if (inv == null) return;

        foreach (var kv in InventorySystem.Instance.AllItems())
        {
            var data = kv.Key;
            var count = kv.Value;

            var go = Instantiate(slotPrefab, slotParent);
            var ui = go.GetComponent<InventorySlotUI>();
            ui.Bind(data, OnSlotClicked, count); // 多传一个数量
        }

        // 可选：清空详情
        if (nameText) nameText.text = "";
        if (descText) descText.text = "";
    }

    void OnSlotClicked(InventoryItemData data)
    {
        Debug.Log($"点击物品：{data.displayName}");
        if (nameText) nameText.text = data.displayName;
        if (descText) descText.text = data.description;
    }
}

