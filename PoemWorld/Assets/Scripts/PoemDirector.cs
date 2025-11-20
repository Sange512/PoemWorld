using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// 统一入口 PlayPoem(PoemData, poemId)，按行显示字幕，并播放配音。结束后标记完成。
/// </summary>
public class PoemDirector : MonoBehaviour
{
    [Header("UI")]
    public GameObject subtitleUI;   // Canvas 下的面板（先隐藏）
    public TMP_Text subtitleText;   // 面板里的 TMP_Text

    [Header("Audio (可选)")]
    public AudioSource audioSource; // 场景里随便一个 AudioSource

    [Header("播放参数")]
    public float perLineSeconds = 2f;

    [Header("任务完成奖励")]
    public RewardSystem rewardSystem;//任务奖励诗卡
    public TaskSystem taskSystem; // 如果你还想在这里标记完成

    Coroutine running;

    public void PlayPoem(PoemData data)
    {
        if (running != null) StopCoroutine(running);
        running = StartCoroutine(PlaySequence(data));
    }

    IEnumerator PlaySequence(PoemData data)
    {
        if (subtitleUI) subtitleUI.SetActive(true);

        if (data.narration && audioSource)
            audioSource.PlayOneShot(data.narration);

        foreach (var line in data.lines)
        {
            if (subtitleText) subtitleText.text = line;
            yield return new WaitForSeconds(perLineSeconds);
        }

        if (subtitleUI) subtitleUI.SetActive(false);
        if (subtitleText) subtitleText.text = "";
        running = null;

        // 播放结束后标记任务完成（如果需要）
        if (taskSystem != null)
            taskSystem.CompleteTask(data.poemId);

        Debug.Log("尝试解锁诗卡：" + data.poemId);
        // 发奖励（解锁诗卡）
        if (rewardSystem != null)
            rewardSystem.GrantPoemCard(data);
    }

    // 方便你在 Inspector 里右键测试
    [ContextMenu("Test Play (需要在 Inspector 里拖入 PoemData)")]
    void TestPlayContext() { /* 占位：通过其他脚本或按钮调用 PlayPoem */ }
}

