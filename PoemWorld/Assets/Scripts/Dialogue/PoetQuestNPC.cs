using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 诗人 NPC 的任务与对话逻辑：
//// - 初次见面：说明需要毛笔
/// - 玩家拿到毛笔后再次对话：交出毛笔，进入找纸和砚台阶段
/// - 玩家把纸和砚台放到指定 PlacePoint 后：再次对话触发“写诗”
/// - 完成后进入闲聊阶段
/// </summary>
public class PoetQuestNPC : MonoBehaviour, IInteractable
{
    [Header("对话数据（按阶段配置）")]
    public DialogueData firstMeetDialogue;        // 第一次见面
    public DialogueData needBrushDialogue;        // 没有毛笔时的对话（催促去找毛笔）
    public DialogueData afterGiveBrushDialogue;   // 交出毛笔后的对话（说明还需要纸和砚台）
    public DialogueData needPaperInkDialogue;     // 找纸和砚台过程中（物品未放齐时）
    public DialogueData allReadyDialogue;         // 所有物品齐全，准备写诗
    public DialogueData finishedDialogue;         // 任务完成后的闲聊

    [Header("任务所需物品（InventoryItemData）")]
    public InventoryItemData brushItem;           // 毛笔
    public InventoryItemData paperItem;           // 纸
    public InventoryItemData inkstoneItem;        // 砚台
    [SerializeField] private InventoryItemData poemCardItem; // 诗卡奖励

    [Header("纸与砚台的放置点")]
    public PlacePoint paperPlacePoint;            // 放纸的位置
    public PlacePoint inkstonePlacePoint;         // 放砚台的位置

    private DialogueManager manager;

    private enum QuestStage
    {
        NotMet,         // 未正式对话过
        NeedBrush,      // 诗人缺毛笔
        NeedPaperInk,   // 已拿到毛笔，缺纸 & 砚台
        AllReady,       // 所有物品齐了，可以写诗
        Finished        // 整个任务已经完成
    }

    [SerializeField] private QuestStage stage = QuestStage.NotMet;

    void Awake()
    {
        manager = FindObjectOfType<DialogueManager>();
        if (manager == null)
        {
            Debug.LogWarning($"[{name}] 场景中没有找到 DialogueManager，诗人无法对话。");
        }
    }

    public void Interact()
    {
        if (DialogueManager.IsOpen) return;
        if (manager == null) return;

        DialogueData dataToPlay = null;

        switch (stage)
        {
            case QuestStage.NotMet:
                dataToPlay = firstMeetDialogue ?? needBrushDialogue;
                stage = QuestStage.NeedBrush;
                break;

            case QuestStage.NeedBrush:
                if (InventorySystem.Instance.HasItem(brushItem, 1))
                {
                    InventorySystem.Instance.ConsumeItem(brushItem, 1);
                    dataToPlay = afterGiveBrushDialogue ?? needPaperInkDialogue;
                    stage = QuestStage.NeedPaperInk;
                }
                else
                {
                    dataToPlay = needBrushDialogue ?? firstMeetDialogue;
                }
                break;

            case QuestStage.NeedPaperInk:
                bool paperOK = paperPlacePoint != null && paperPlacePoint.IsPlaced;
                bool inkstoneOK = inkstonePlacePoint != null && inkstonePlacePoint.IsPlaced;

                if (paperOK && inkstoneOK)
                {
                    dataToPlay = allReadyDialogue;
                    stage = QuestStage.AllReady;
                }
                else
                {
                    dataToPlay = needPaperInkDialogue;
                }
                break;

            case QuestStage.AllReady:
                dataToPlay = allReadyDialogue ?? finishedDialogue;
                stage = QuestStage.Finished;
                break;

            case QuestStage.Finished:
                dataToPlay = finishedDialogue ?? allReadyDialogue;
                break;
        }

        if (dataToPlay != null)
        {
            manager.StartDialogue(dataToPlay);
        }
        else
        {
            Debug.LogWarning($"[{name}] 当前阶段 {stage} 没有配置对应的 DialogueData。");
        }
    }

    public void MarkFinished()
    {
        stage = QuestStage.Finished;
    }

    public void TryGiveBrush()
    {
        if (InventorySystem.Instance.HasItem(brushItem, 1))
        {
            InventorySystem.Instance.ConsumeItem(brushItem, 1);
            Debug.Log("毛笔已通过对话节点交给诗人。");
        }
        else
        {
            Debug.Log("玩家背包里没有毛笔，无法交付。");
        }
    }

    public void EnterNeedPaperInkStage()
    {
        stage = QuestStage.NeedPaperInk;
    }

    public void GivePoemCardReward()
    {
        if (InventorySystem.Instance != null && poemCardItem != null)
        {
            InventorySystem.Instance.AddItem(poemCardItem, 1);
            Debug.Log("获得诗卡奖励！");
        }
    }
}
