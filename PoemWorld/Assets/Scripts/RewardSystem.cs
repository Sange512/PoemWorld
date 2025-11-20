using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RewardSystem : MonoBehaviour
{
    // 用来记录已解锁（内存 + 简易持久化）
    //声明并初始化了一个私有的、存储 string 类型元素的 HashSet 集合，变量名为 unlocked
    //HashSet 是一种不允许重复元素的集合（类似 “无序且唯一的列表”），
    //适合用于存储 “已解锁的物品 / 功能 ID”“已完成的任务标识” 等需要去重的数据。
    private HashSet<string> unlocked = new HashSet<string>();//在内存中记录 已解锁诗卡的 ID
    private const string PREF_KEY = "UnlockedPoems"; // PlayerPrefs 存储用的 key 名，用来取/存解锁诗的列表）

    [Header("UI")]
    public GameObject rewardPanel;   // 一个简单的 Panel，诗卡UI面板
    public Image cardImage;          // 显示诗卡插图
    public TMP_Text titleText;       // 显示诗名
    public float showSeconds = 2.5f; //弹窗展示时间
    public KeyCode clearKey = KeyCode.R;//清空已存储的诗

    void Start()
    {
        Debug.Log("[RewardSystem] 已加载的 PlayerPrefs 数据：" + PlayerPrefs.GetString("UnlockedPoems", "无"));
    }

    void Awake()
    {
        Load();
    }

    private void Update()
    {
        if (Input.GetKeyDown(clearKey))
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            Debug.Log(" 已清空 PlayerPrefs，所有诗重新可解锁！");
        }
    }
    /// <summary>
    /// 检查指定poemID是否解锁，用于外部判断（例如 UI 图鉴、触发逻辑等）
    /// </summary>
    /// <param name="poemId"></param>
    /// <returns></returns>
    public bool IsUnlocked(string poemId)
    {
        return unlocked.Contains(poemId);
    }

    /// <summary>
    /// 解锁诗卡并弹窗显示，解锁逻辑的入口函数，PoemDirector 播完诗后就是调用这个。
    /// </summary>
    /// <param name="data"></param>
    public void GrantPoemCard(PoemData data)
    {
        if (data == null || string.IsNullOrEmpty(data.poemId)) return;
        if (unlocked.Contains(data.poemId)) return;//如果已解锁，则直接退出

        unlocked.Add(data.poemId);//未解锁、写入内存集合
        Save();//Save() → 保存到 PlayerPrefs

        Debug.Log("[RewardSystem] StartCoroutine 正在启动");
        // 弹窗显示
        if (rewardPanel)
        {
            StartCoroutine(ShowRewardCoroutine(data));
        }
            
    }

    /// <summary>
    /// 显示UI弹窗
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    IEnumerator ShowRewardCoroutine(PoemData data)
    {
        Debug.Log("[RewardSystem] ShowRewardCoroutine 已进入协程");
        rewardPanel.SetActive(true);
        if (cardImage && data.illustration) cardImage.sprite = data.illustration;
        if (titleText) titleText.text = $"解锁诗卡：{data.title}";
        yield return new WaitForSeconds(showSeconds);
        rewardPanel.SetActive(false);
    }

    /// <summary>
    /// 把 unlocked 集合存到 PlayerPrefs
    /// PlayerPrefs 是一个用于存储和读取简单键值对数据的工具类，
    /// 主要用于保存玩家的游戏状态、设置或进度等轻量级数据（如高分记录、音量设置、是否完成教程等）
    /// 数据会被持久化到本地设备（根据平台不同，存储路径不同，例如 Windows 通常在注册表，移动设备在应用沙盒目录），
    /// 即使游戏关闭后也不会丢失
    /// 支持的数据类型：
    /// 字符串（string）
    /// 整数（int）
    /// 浮点数（float）
    /// </summary>
    void Save()
    {
        // 简易存到 PlayerPrefs（逗号分隔）
        try
        {
            var joined = string.Join(",", unlocked);
            PlayerPrefs.SetString(PREF_KEY, joined);//存储字符串
            PlayerPrefs.Save();// 立即保存到本地（部分平台会自动保存，但显式调用更安全）
        }
        catch { /* ignore */ }
    }

    /// <summary>
    /// 从 PlayerPrefs 读取已解锁数据到内存
    /// </summary>
    void Load()
    {
        unlocked.Clear();
        var s = PlayerPrefs.GetString(PREF_KEY, "");
        if (string.IsNullOrEmpty(s)) return;
        var parts = s.Split(',');
        foreach (var p in parts)
            if (!string.IsNullOrEmpty(p)) unlocked.Add(p);
    }
}
/*
 启动游戏
↓
Awake → Load() → 将已解锁的 id 读入 unlocked
↓
PoemDirector 播放结束
↓
RewardSystem.GrantPoemCard(poemData)
   ↓
   unlocked.Add(...)   // 记忆
   Save()              // 写入 PlayerPrefs
   ShowRewardCoroutine // 弹窗 UI
 */
