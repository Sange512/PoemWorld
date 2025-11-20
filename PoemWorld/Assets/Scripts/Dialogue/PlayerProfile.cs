using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家资料，用来显示玩家的名字与头像
/// </summary>
[CreateAssetMenu(fileName = "PlayerProfile", menuName = "Dialogue/Player Profile")]
public class PlayerProfile : ScriptableObject
{
    public string playerName;
    public Sprite playerPortrait;
}

