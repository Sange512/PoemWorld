using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;  // 引入 Unity UI

//控制背包面板的开关和锁定
public class InventoryToggle : MonoBehaviour
{
    [Header("UI")]
    public GameObject inventoryPanel;//背包面板
    public Button closeButton;  // 用来关闭背包的按钮

    [Header("控制开关")]
    public KeyCode toggleKey = KeyCode.Tab;
    public KeyCode closeKey = KeyCode.Escape;
    public bool pauseOnOpen = true;

    [Header("打开背包时要禁用的脚本（相机/角色控制等）")]
    public MonoBehaviour[] disableWhileOpen;

    bool isOpen = false;
    float prevTimeScale = 1f;

    void Start()
    {
        if (inventoryPanel) inventoryPanel.SetActive(false);

        // 绑定关闭按钮点击事件
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseInventory);
        }

        LockMouseToGame(); // 初始锁定
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            if (!isOpen) OpenInventory();
            else CloseInventory();
        }
        else if (isOpen && Input.GetKeyDown(closeKey))
        {
            CloseInventory();
        }
    }

    void OpenInventory()
    {
        isOpen = true;
        if (inventoryPanel) inventoryPanel.SetActive(true);

        // 暂停游戏
        if (pauseOnOpen)
        {
            prevTimeScale = Time.timeScale;
            Time.timeScale = 0f;
        }

        // 禁用相机/角色控制
        foreach (var mb in disableWhileOpen)
            if (mb) mb.enabled = false;

        // 显示鼠标
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseInventory()
    {
        isOpen = false;
        if (inventoryPanel) inventoryPanel.SetActive(false);

        // 恢复时间
        if (pauseOnOpen) Time.timeScale = prevTimeScale;

        // 恢复相机/角色控制
        foreach (var mb in disableWhileOpen)
            if (mb) mb.enabled = true;

        // 锁回鼠标
        LockMouseToGame();
    }

    void LockMouseToGame()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}


