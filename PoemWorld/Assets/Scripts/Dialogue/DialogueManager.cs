using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    //问题：对话时提示UI仍然出现，DialogueManager 在开始对话时把提示关掉了，但 PlayerInteraction.Update() 每帧又把它重新打开
    //解决方案：给对话系统一个全局只读开关，PlayerInteraction 检查到对话打开就不再显示提示/不再交互
    public static bool IsOpen { get; private set; }  //  全局开关，静态变量

    [Header("交互对象UI")]
    public GameObject dialogueUI;          // 对话界面（一个Panel）
    public TMP_Text dialogueText;          // 显示文字的TMP文本
    public Image portraitImage;            // NPC头像
    public TMP_Text nameText;              // NPC名字

    [Header("选项 UI")]
    public Transform choicesContainer;      // 一个空物体，带 VerticalLayoutGroup
    public Button choiceButtonPrefab;       // 预制按钮（里边放 TMP_Text）

    [Header("打字机设置")]
    public float textSpeed = 0.02f;        // 打字机速度

    [Header("外部数据")]
    public PlayerProfile playerProfile;     // 玩家名字与头像（拖引用）

    [Header("交互提示 UI")]
    [SerializeField] private GameObject interactHintUI; // 引用交互提示UI

    private DialogueData currentData;
    private int nodeIndex;
    private bool isTyping;
    private readonly List<Button> runtimeButtons = new();

    //测试
    //[SerializeField] private PoemDirector director;
    //[SerializeField] private PoemData myPoem;

    void Start()
    {
        //游戏开始时预热UI
        if (dialogueUI)
        {
            dialogueUI.SetActive(true);
            dialogueUI.SetActive(false); // 激活再关闭一次，提前加载字体和Canvas
        }

        if (interactHintUI)
        {
            interactHintUI.SetActive(true);
            interactHintUI.SetActive(false);
        }

        IsOpen = false;//不在对话中
        ClearChoices();//清除所有选项

        // 自动在场景中找到交互提示UI
        //interactHintUI = GameObject.FindWithTag("InteractHint");
        //FindWithTag只会返回**场景中处于激活状态的对象**；如果该对象在层级里或其父物体一开始就是SetActive(false)，它就会返回 null
    }

    /// <summary>
    /// 开始一段新对话，外部入口，（NPC 调 manager.StartDialogue(data)）
    /// 记录数据：currentData、nodeIndex（起始节点）
    /// </summary>
    /// <param name="data"></param>
    public void StartDialogue(DialogueData data)
    {
        //测试：假设你有 PoemDirector director 和 PoemData myPoem 的引用
        //director.PlayPoem(myPoem);

        currentData = data;
        nodeIndex = Mathf.Clamp(data.startIndex, 0, data.nodes.Length - 1);//界限函数

        IsOpen = true;//打开
        // 隐藏交互提示UI
        if (interactHintUI != null)
            interactHintUI.SetActive(false);

        //打开UI并开始打字
        dialogueUI.SetActive(true);
        ShowNode();
    }

    /// <summary>
    /// 节点展示
    /// </summary>
    void ShowNode()
    {
        ClearChoices();//清空旧选项

        if (nodeIndex < 0 || nodeIndex >= currentData.nodes.Length)
        {
            EndDialogue();
            return;
        }

        //var 是 C# 中的隐式类型推断关键字，它允许编译器根据等号右边的值自动推断变量的类型，而无需手动指定类型
        //从一个集合中获取指定索引的元素，并将其赋值给变量 node
        var node = currentData.nodes[nodeIndex];

        // 新增：执行节点命令
        if (!string.IsNullOrEmpty(node.command))
        {
            DialogueCommandRouter.Execute(node.command);
        }

        // 切换说话者的名字与头像
        if (node.speaker == Speaker.NPC)
        {
            if (nameText) nameText.text = currentData.npcName;
            if (portraitImage) portraitImage.sprite = currentData.npcPortrait;
        }
        else // Player
        {
            if (playerProfile != null)
            {
                if (nameText) nameText.text = playerProfile.playerName;
                if (portraitImage) portraitImage.sprite = playerProfile.playerPortrait;
            }
        }

        // 打字机显示台词
        StopAllCoroutines();//停止旧协程
        StartCoroutine(TypeLine(node.line));

        // 如果有选项，生成按钮；没有选项则允许空格/点击继续到下一个节点（nodeIndex+1）
        if (node.choices != null && node.choices.Length > 0)
        {
            // 生成按钮（本帧先渲染台词，不阻塞）
            StartCoroutine(SpawnChoicesNextFrame(node.choices));
        }

    }

    /// <summary>
    /// 打字机协程，只打一行
    /// </summary>
    /// <returns></returns>
    IEnumerator TypeLine(string line)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char c in line)//
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(textSpeed);//实现延迟
        }

        isTyping = false;
    }

    /// <summary>
    /// 按钮切换协程
    /// </summary>
    /// <param name="choices"></param>
    /// <returns></returns>
    IEnumerator SpawnChoicesNextFrame(DialogueChoice[] choices)
    {
        yield return null; // 等一帧，避免和打字机/布局冲突
        ClearChoices();

        for (int i = 0; i < choices.Length; i++)
        {
            var choice = choices[i];
            //实例化button
            var btn = Instantiate(choiceButtonPrefab, choicesContainer);//从预制体实例化一个选项按钮，并设置其父物体
            runtimeButtons.Add(btn);//记录动态生成的按钮，可以通过遍历 runtimeButtons 调用 Destroy(btn) 实现

            //// 显示序号（1-based）
            var label = btn.GetComponentInChildren<TMP_Text>();//获取按钮子物体中的文本组件，用于显示选项文字
            if (label) label.text = $"{i + 1}. {choice.text}";

            int capturedIndex = choice.nextIndex; // 闭包捕获
            //绑定点击事件（点击后跳转到对应对话节点）
            btn.onClick.AddListener(() => OnChoiceSelected(choice.nextIndex));
            btn.gameObject.SetActive(true);
        }
    }


    /// <summary>
    /// 点击某个选项后，跳到 nextIndex 节点并 ShowNode()
    /// </summary>
    /// <param name="nextIndex"></param>
    void OnChoiceSelected(int nextIndex)
    {
        nodeIndex = nextIndex;
        ShowNode();
    }


    void Update()
    {
        if (!IsOpen || currentData == null) return;
        if (!dialogueUI.activeSelf) return;//若对话界面未激活（!dialogueUI.activeSelf），直接退出

        // 当前节点无选项时，允许空格/鼠标继续
        var node = currentData.nodes[nodeIndex];
        bool hasChoices = node.choices != null && node.choices.Length > 0;

        // ============数字键选择 ============
        if (hasChoices)
        {
            int idx = GetPressedChoiceIndex(node.choices.Length); // 0-based；-1 表示未按
            if (idx != -1)
            {
                // 若还在打字，先补全本句（视觉更自然）
                if (isTyping)
                {
                    StopAllCoroutines();
                    if (dialogueText) dialogueText.text = node.line;
                    isTyping = false;
                }
                // 防御：越界忽略
                if (idx >= 0 && idx < node.choices.Length)
                {
                    OnChoiceSelected(node.choices[idx].nextIndex);
                    return;
                }
            }
        }

        if (!hasChoices && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)))
        {
            if (isTyping)
            {
                // 跳过打字机，直接全部显示
                StopAllCoroutines();
                if (dialogueText) dialogueText.text = node.line;
                isTyping = false;
            }
            else
            {
                // 进入nextIndex节点
                int next = node.nextIndex;
                if (next >= 0 && next < currentData.nodes.Length)
                {
                    nodeIndex = next;
                    ShowNode();
                }
                else
                {
                    // 如果 nextIndex 无效或为 -1，就结束
                    EndDialogue();
                }
            }
        }
    }

    public void EndDialogue()
    {
        ClearChoices();
        currentData = null;
        if (dialogueUI) dialogueUI.SetActive(false);
        if (dialogueText) dialogueText.text = "";
        IsOpen = false;

        // 恢复交互提示UI
        //if (interactHintUI)
        //    interactHintUI.SetActive(true);
    }

    void ClearChoices()
    {
        foreach (var b in runtimeButtons)
            if (b) Destroy(b.gameObject);
        runtimeButtons.Clear();
    }

    /// <summary>
    /// 读取用户是否按下了 1..9（主键盘或小键盘），返回 0-based 索引；未按返回 -1
    /// </summary>
    int GetPressedChoiceIndex(int maxChoices)
    {
        // 主键盘 1..9
        for (int i = 1; i <= 9 && i <= maxChoices; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0 + i)) // Alpha1 是 (int)KeyCode.Alpha0 + 1
                return i - 1;
        }
        // 小键盘 1..9
        for (int i = 1; i <= 9 && i <= maxChoices; i++)
        {
            if (Input.GetKeyDown(KeyCode.Keypad1 + (i - 1)))
                return i - 1;
        }
        return -1;
    }
}

/*
外部触发：NPC.Interact()
        │
        ▼
StartDialogue(data)
  ├─ 设置 IsOpen = true
  ├─ 隐藏交互提示 UI
  ├─ 打开对话面板
  └─ nodeIndex = data.startIndex → ShowNode()
                         │
                         ▼
                    ShowNode()
     ├─ ClearChoices()
     ├─ 根据 speaker 切换名字/头像（NPC/Player）
     ├─ 停旧协程 → 启动 TypeLine(line)（打字机）
     └─ 若节点含 choices → SpawnChoicesNextFrame(choices)
                         │
                         │（并行：一边打字机，一边等待输入）
                         ▼
                 玩家输入（Update）
        ┌────────────────────────────────────────────┐
        │ A. 线性节点（无 choices）                   │
        │    空格/左键：                             │
        │     ├─ 若 isTyping == true →               │
        │     │      停协程，直接显示完整文本        │
        │     └─ 若 isTyping == false →              │
        │            nodeIndex++ → ShowNode()        │
        └────────────────────────────────────────────┘
        ┌────────────────────────────────────────────┐
        │ B. 分支节点（有 choices）                   │
        │    等待玩家点击按钮：                       │
        │     → OnChoiceSelected(nextIndex)          │
        │          ├─ nextIndex 为有效节点 → ShowNode│
        │          └─ nextIndex 为结束（-1/越界） →  │
        │                EndDialogue()               │
        └────────────────────────────────────────────┘
                         │
                         ▼
                   EndDialogue()
    ├─ 关闭对话 UI、清空文字与按钮
    ├─ IsOpen = false
    └─ 恢复交互提示 UI（可继续与场景交互）

 */

