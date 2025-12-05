using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 这里放与“诗人任务”有关的具体逻辑，
/// 供 DialogueCommandRouter 调用。
/// 你也可以把这些逻辑挪到 PoetQuestNPC 中，
/// 这里只是为了让结构清晰。
/// </summary>
public static class PoetQuestBridge
{
    // 你可以把这些引用做成更稳妥的方式（比如单例/场景查找）
    private static PoetQuestNPC poet;

    static PoetQuestBridge()
    {
        poet = Object.FindObjectOfType<PoetQuestNPC>();
    }

    public static void GiveBrushToPoet()
    {
        if (poet == null) poet = Object.FindObjectOfType<PoetQuestNPC>();
        if (poet == null) return;

        poet.TryGiveBrush();
    }

    public static void StartPaperInkQuest()
    {
        if (poet == null) poet = Object.FindObjectOfType<PoetQuestNPC>();
        if (poet == null) return;

        poet.EnterNeedPaperInkStage();
    }

    public static void FinishPoem()
    {
        if (poet == null) poet = Object.FindObjectOfType<PoetQuestNPC>();
        if (poet == null) return;

        poet.MarkFinished();
    }

    public static void RewardPoemCard()
    {
        if (poet == null) poet = Object.FindObjectOfType<PoetQuestNPC>();
        if (poet == null) return;

        poet.GivePoemCardReward();
    }
}

