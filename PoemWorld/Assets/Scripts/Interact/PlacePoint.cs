using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacePoint : MonoBehaviour, IInteractable
{
    [Header("放置所需的物品数据（InventoryItemData）")]
    public InventoryItemData requiredItem;

    [Header("放置后生成的Prefab（拖 Project 面板里的预制体）")]
    public GameObject placedPrefab;

    [Header("生成位置偏移（可选）")]
    public Vector3 spawnOffset = Vector3.zero;

    [Header("是否作为当前PlacePoint的子物体（可选）")]
    public bool parentToThis = false;

    private bool isPlaced = false;

    //对外暴露一个只读属性
    public bool IsPlaced => isPlaced;

    public void Interact()
    {
        if (isPlaced) return;

        if (requiredItem == null)
        {
            Debug.LogWarning("PlacePoint: requiredItem 未设置！");
            return;
        }

        if (InventorySystem.Instance.HasItem(requiredItem, 1))
        {
            // 消耗背包物品
            InventorySystem.Instance.ConsumeItem(requiredItem, 1);

            // 标记已放置
            isPlaced = true;

            // 动态生成新物体
            if (placedPrefab != null)
            {
                Vector3 spawnPos = transform.position + spawnOffset;
                Quaternion spawnRot = transform.rotation;

                GameObject obj = Instantiate(placedPrefab, spawnPos, spawnRot);

                if (parentToThis)
                    obj.transform.SetParent(transform);

                obj.SetActive(true);
            }
            else
            {
                Debug.LogWarning("PlacePoint: placedPrefab 未设置，无法生成放置物体！");
            }

            Debug.Log($"成功放置 {requiredItem.displayName}");
        }
        else
        {
            Debug.Log("缺少所需物品：" + requiredItem.displayName);
        }
    }

    public string GetPromptText()
    {
        if (requiredItem == null) return "未设置 requiredItem";
        return isPlaced ? "已放置" : $"按 E 放置「{requiredItem.displayName}」";
    }
}
