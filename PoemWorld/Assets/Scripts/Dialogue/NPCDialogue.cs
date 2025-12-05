using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// NPC对话触发脚本，让NPC实现IInteractable接口
/// </summary>
public class NPCDialogue : MonoBehaviour, IInteractable
{
    [Header("对话数据")]
    public DialogueData dialogueData; // 拖入ScriptableObject

    //提前加载避免卡顿
    private DialogueManager manager;// 缓存的 DialogueManager 引用

    void Awake()
    {
        manager = FindObjectOfType<DialogueManager>();

        if (manager == null)
        {
            Debug.LogWarning($"[{name}] 场景中没有找到 DialogueManager，NPC 无法触发对话。");
        }

        if (dialogueData == null)
        {
            Debug.LogWarning($"[{name}] 未设置 DialogueData，对话内容为空。");
        }
    }

    public void Interact()//npc定义的接口方法
    {
        if (DialogueManager.IsOpen) return; // 避免对话嵌套
        if (manager == null || dialogueData == null) return;
        if (manager != null)
        {
            manager.StartDialogue(dialogueData);
        }
    }
}

