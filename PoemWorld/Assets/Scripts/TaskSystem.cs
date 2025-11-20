using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TaskState { Locked, Available, InProgress, Completed }

[System.Serializable]
public class PoemTask
{
    public string poemId;
    public TaskState state = TaskState.Available; // demo 先设为可开始
}

/// <summary>
/// 控制一首诗是否可开始、进行中、已完成，避免重复触发
/// </summary>
public class TaskSystem : MonoBehaviour
{
    public PoemTask current;//当前状态

    public bool CanStart(string poemId)
        => current != null && current.poemId == poemId && current.state == TaskState.Available;

    public void StartTask(string poemId)
    {
        if (CanStart(poemId)) current.state = TaskState.InProgress;
    }

    public void CompleteTask(string poemId)
    {
        if (current != null && current.poemId == poemId && current.state == TaskState.InProgress)
            current.state = TaskState.Completed;
    }

    /// <summary>
    /// 判断该诗是否已完成
    /// </summary>
    /// <param name="poemId"></param>
    /// <returns></returns>
    public bool IsCompleted(string poemId)
    {
        return current != null
               && current.poemId == poemId
               && current.state == TaskState.Completed;
    }
}

