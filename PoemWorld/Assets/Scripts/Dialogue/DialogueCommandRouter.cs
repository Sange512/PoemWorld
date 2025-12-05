using UnityEngine;

/// <summary>
/// 对话节点命令路由器：
/// DialogueManager 遇到 DialogueNode.command 时调用这里，
/// 这里再去驱动任务、背包、奖励等系统。
/// </summary>
public static class DialogueCommandRouter
{
    public static void Execute(string command)
    {
        Debug.Log($"[DialogueCommand] Execute: {command}");

        switch (command)
        {
            case "GIVE_BRUSH":
                PoetQuestBridge.GiveBrushToPoet();
                break;

            case "START_PAPER_INK_QUEST":
                PoetQuestBridge.StartPaperInkQuest();
                break;

            case "FINISH_POEM":
                PoetQuestBridge.FinishPoem();
                break;

            case "REWARD_POEM_CARD":
                PoetQuestBridge.RewardPoemCard();
                break;

            default:
                Debug.LogWarning($"未知 Dialogue command: {command}");
                break;
        }
    }
}
