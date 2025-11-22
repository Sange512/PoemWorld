using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    //在对话中金庸移动/视角
    //if (DialogueManager.IsOpen) return;

    [Header("移动参数")]
    public float walkSpeed = 5f;        // 普通行走速度
    public float runSpeed = 8f;         // 加速跑步速度
    public float gravity = -9.8f;       // 重力加速度
    public float jumpHeight = 2f;       // 跳跃高度
    public bool canMove = true; // 是否可以移动
    public bool canLook = true; // 是否可以转动视角

    [Header("鼠标视角参数")]
    public float mouseSensitivity = 100f;  // 初始鼠标灵敏度
    public Transform cameraTransform;      // 摄像机引用（拖Main Camera）

    [Header("灵敏度调整")]
    public float sensitivityStep = 10f;    // 每次调整灵敏度步长
    public float minSensitivity = 50f;     // 最低灵敏度
    public float maxSensitivity = 300f;    // 最高灵敏度

    [Header("地面检测")]
    public float groundedGraceTime = 0.2f; // 离地多少秒内仍可跳（“土狼时间”）

    private CharacterController controller;
    public Vector3 velocity;
    private float xRotation = 0f;
    private float groundedTimer; // 着地缓冲时间（避免跳跃卡顿）

    void Start()
    {
        controller = GetComponent<CharacterController>();

        // 锁定并隐藏鼠标
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if(canLook)
        {
            HandleMouseLook();       // 控制视角
        }
        if(canMove)
        {
            HandleMovement();
        }
        AdjustMouseSensitivity();// 动态调节灵敏度
    }

    /// <summary>
    /// 控制移动、重力与跳跃
    /// </summary>
    void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // 是否在地面上
        if (controller.isGrounded)
            groundedTimer = groundedGraceTime;
        else
            groundedTimer -= Time.deltaTime;

        // 计算移动方向
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float currentSpeed = isRunning ? runSpeed : walkSpeed;
        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        controller.Move(move * currentSpeed * Time.deltaTime);

        // 跳跃（带地面缓冲）
        if (groundedTimer > 0 && Input.GetButtonDown("Jump"))
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            groundedTimer = 0; // 防止多次跳
        }

        // 重力
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // 如果着地，轻微下压，防止悬浮
        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;
    }

    /// <summary>
    /// 控制鼠标视角旋转
    /// </summary>
    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // 上下视角旋转（摄像机）
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // 左右旋转（角色）
        transform.Rotate(Vector3.up * mouseX);
    }

    /// <summary>
    /// 动态调整鼠标灵敏度
    /// </summary>
    void AdjustMouseSensitivity()
    {
        if (Input.GetKeyDown(KeyCode.Equals) || Input.GetKeyDown(KeyCode.Plus))
        {
            mouseSensitivity = Mathf.Clamp(mouseSensitivity + sensitivityStep, minSensitivity, maxSensitivity);
            Debug.Log($"鼠标灵敏度增加至: {mouseSensitivity}");
        }

        if (Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.Underscore))
        {
            mouseSensitivity = Mathf.Clamp(mouseSensitivity - sensitivityStep, minSensitivity, maxSensitivity);
            Debug.Log($"鼠标灵敏度减少至: {mouseSensitivity}");
        }
    }

    public void LockMovement(bool isLocked)
    {
        canMove = !isLocked;
        canLook = !isLocked;
        
        Cursor.lockState = isLocked ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isLocked; // 锁定移动时显示鼠标，反之隐藏
    }
}
