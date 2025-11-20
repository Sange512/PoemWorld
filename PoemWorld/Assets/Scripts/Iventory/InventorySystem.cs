using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class InventorySystem : MonoBehaviour
{
    //单例：其他脚本可以直接通过 InventorySystem.Instance 访问到这个唯一的实例
    //无需重新创建对象，方便调用其方法或属性
    public static InventorySystem Instance;  // 单例，方便其他脚本调用
    //HashSet<string> 是 C# 中的一种集合类型，它主要用于存储不重复的元素
    //public List<InventoryItemData> items = new List<InventoryItemData>();//存储字符串类型的数据，如物品ID、名称
    
    //改为数量叠加的背包
    private readonly Dictionary<InventoryItemData, int> bag = new Dictionary<InventoryItemData, int>();

    
    public event Action OnInventoryChanged;


    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    /// <summary>
    /// 背包增加物品
    /// </summary>
    /// <param name="itemData"></param>
    /// <param name="amount"></param>
    public void AddItem(InventoryItemData itemData, int amount = 1)
    {
        if (itemData == null || amount <= 0) return;

        if (bag.TryGetValue(itemData, out int cur))
            bag[itemData] = cur + amount;
        else
            bag[itemData] = amount;

        Debug.Log($"已获得物品：{itemData.displayName} x{amount}（共 x{bag[itemData]}）");
        OnInventoryChanged?.Invoke();
    }

    /// <summary>
    ///移除一定数量的物品，返回是否成功
    /// </summary>
    /// <param name="itemData"></param>
    public bool ConsumeItem(InventoryItemData itemData, int amount = 1)
    {
        if (itemData == null || amount <= 0) return false;
        if (!bag.TryGetValue(itemData, out int cur) || cur < amount) return false;

        cur -= amount;
        if (cur <= 0) bag.Remove(itemData);
        else bag[itemData] = cur;

        OnInventoryChanged?.Invoke();
        return true;
    }

    /// <summary>
    /// 移除该物品的全部堆叠
    /// </summary>
    public void RemoveItemAll(InventoryItemData itemData)
    {
        if (itemData == null) return;
        if (bag.Remove(itemData))
            OnInventoryChanged?.Invoke();
    }

    /// <summary>
    /// 是否拥有指定数量
    /// </summary>
    public bool HasItem(InventoryItemData itemData, int amount = 1)
    {
        return itemData != null &&
               amount > 0 &&
               bag.TryGetValue(itemData, out int cur) &&
               cur >= amount;
    }

    /// <summary>
    /// 获取当前数量（没有则为 0）
    /// </summary>
    public int GetCount(InventoryItemData itemData)
    {
        return (itemData != null && bag.TryGetValue(itemData, out int cur)) ? cur : 0;
    }

    /// <summary>
    /// 遍历所有物品堆叠（UI 用它来生成格子）
    /// </summary>
    public IEnumerable<KeyValuePair<InventoryItemData, int>> AllItems()
    {
        return bag;
    }


    /// <summary>
    /// 清空背包
    /// </summary>
    public void ClearAll()
    {
        bag.Clear();
        Debug.Log("清空背包");
        OnInventoryChanged?.Invoke();
    }
}

