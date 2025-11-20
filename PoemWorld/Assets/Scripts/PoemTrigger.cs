using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoemTrigger : MonoBehaviour
{
    public PoemDirector director;   // 拖引用场景里的 PoemDirector
    public PoemData poemData;       // 拖引用那首诗的 PoemData
    public TaskSystem taskSystem;

    private bool triggered = false; // 防止重复触发

    private void OnTriggerEnter(Collider other)
    {
        //如果不是player不触发
        if (!other.CompareTag("Player")) return;
        if (!director || !poemData || !taskSystem) return;

        // 核心：如果已完成则不触发
        if (taskSystem.IsCompleted(poemData.poemId))
            return;

        // 开始播放
        taskSystem.StartTask(poemData.poemId);
        director.PlayPoem(poemData);

        // 标记完成（建议在 PoemDirector 演绎结束时调用）
        taskSystem.CompleteTask(poemData.poemId);
    }
}

