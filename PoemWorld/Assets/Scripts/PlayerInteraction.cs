using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家交互检测：挂载在相机上，射线检测
/// </summary>
public class PlayerInteraction : MonoBehaviour
{
    public float interactDistance = 3f;
    public LayerMask interactLayer;//所有interactLayer层的内容都为可交互物体，会出现交互提示
    public KeyCode interactKey = KeyCode.E;
    public GameObject interactHintUI;

    [Header("背包UI")]
    public GameObject inventoryUI;
    public KeyCode toggleKey = KeyCode.Tab;

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();

        if (interactHintUI != null)
            interactHintUI.SetActive(false);

        if (inventoryUI != null)
            inventoryUI.SetActive(false);
    }

    void Update()
    {
        //对话内容
        // 对话进行中：强制隐藏提示并直接返回
        if (DialogueManager.IsOpen)
        {
            if (interactHintUI && interactHintUI.activeSelf) interactHintUI.SetActive(false);
            return;
        }

        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactLayer))
        {
            if (interactHintUI != null)
                interactHintUI.SetActive(true);

            if (Input.GetKeyDown(interactKey))
            {
                IInteractable interactable = hit.collider.GetComponent<IInteractable>();
                if (interactable != null)
                    interactable.Interact();//可交互物体的交互方法
            }
        }
        else
        {
            if (interactHintUI != null)
                interactHintUI.SetActive(false);
        }

        //背包UI控制
        if (Input.GetKeyDown(toggleKey))
        {
            if (inventoryUI != null)
                inventoryUI.SetActive(!inventoryUI.activeSelf);
        }
    }
}

