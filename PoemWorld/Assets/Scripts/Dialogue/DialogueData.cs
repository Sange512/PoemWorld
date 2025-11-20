using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 一个 ScriptableObject 保存一段对话图；节点可指定说话者、台词、以及分支选项
/// </summary>

public enum Speaker { NPC, Player }//枚举变量

[System.Serializable]
public class DialogueChoice
{
    public string text;         // 选项文字（玩家看到的）
    public int nextIndex = -1;  // 下一个节点索引；-1 表示结束
}

[System.Serializable]
public class DialogueNode
{
    public Speaker speaker;                 // 说话者
    [TextArea(3, 6)] public string line;    // 台词
    public DialogueChoice[] choices;        // 可为空；有则显示选项按钮

    [Header("下一句（自动跳转用， -1 表示结束，若为分支节点不用设置）")]
    public int nextIndex = -1;
}

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue/NPC Dialogue")]
public class DialogueData : ScriptableObject
{
    [Header("NPC 信息")]
    public string npcName;
    public Sprite npcPortrait;

    [Header("对话图")]
    public int startIndex = 0;
    public DialogueNode[] nodes;
}

