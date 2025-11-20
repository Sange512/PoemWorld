using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 接口，统一所有可交互物体的交互入口
/// </summary>
public interface IInteractable
{
    void Interact();//// 所有可交互物体必须实现这个方法，定义具体交互逻辑
}
/*
NPC 的脚本实现 IInteractable，Interact() 方法中写 “开始对话” 逻辑；
宝箱的脚本实现 IInteractable，Interact() 方法中写 “打开宝箱并获得物品” 逻辑；
门的脚本实现 IInteractable，Interact() 方法中写 “开门 / 关门” 逻辑。
 */
