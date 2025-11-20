using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 每首诗的数据资产
/// </summary>
[CreateAssetMenu(fileName = "PoemData", menuName = "Poetry/Poem")]
public class PoemData : ScriptableObject
{
    public string poemId;            // e.g. "poem_wlslpf"
    public string title;             // 望庐山瀑布
    [TextArea(3, 10)] public string[] lines;   // 分行显示
    public AudioClip narration;      // 朗诵音频
    public Sprite illustration;      // 插图/卡面（奖励用）
}

