using UnityEngine;

public class Teleportation : MonoBehaviour
{
    public GameObject player;
    private CharacterController controller;

    void Start()
    {
        controller = player.GetComponent<CharacterController>();
    }

    // 新增：指定目标位置传送
    public void TeleportTo(Transform target)
    {
        if (target == null) return;

        if (controller != null)
        {
            controller.enabled = false;
            player.transform.position = target.position;
            controller.enabled = true;
            FirstPersonController fpsController = player.GetComponent<FirstPersonController>();
            if (fpsController != null)
            {
                fpsController.velocity = Vector3.zero;
            }
        }
        else
        {
            player.transform.position = target.position;
        }
    }
}