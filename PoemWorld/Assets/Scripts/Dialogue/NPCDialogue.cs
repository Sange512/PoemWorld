using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// NPC对话触发脚本，让NPC实现IInteractable接口
/// </summary>
public class NPCDialogue : MonoBehaviour, IInteractable
{
    public DialogueData dialogueData; // 拖入ScriptableObject

    void Start()
    {
        //提前加载避免卡顿
        DialogueManager manager = FindObjectOfType<DialogueManager>();
    }

    public void Interact()//npc定义的接口方法
    {
        DialogueManager manager = FindObjectOfType<DialogueManager>();
        if (manager != null)
        {
            manager.StartDialogue(dialogueData);
        }
    }
}

