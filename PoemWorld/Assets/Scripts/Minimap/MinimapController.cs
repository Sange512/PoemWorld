using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MinimapController : MonoBehaviour
{
    [Header("玩家引用")]
    public Transform player;
    private FirstPersonController fpsController;

    [Header("小地图组件")]
    public RawImage minimapRawImage;
    private RectTransform minimapRect;
    public Camera minimapCamera;

    [Header("缩放设置")]
    public float minZoom;
    public float maxZoom;
    public float zoomSpeed = 1f;

    [Header("传送点设置")]
    public GameObject teleportIconPrefab; // 需包含Image（图标）和Text（名称）组件
    public List<TeleportPoint> teleportPoints = new List<TeleportPoint>(); // 存储带名称的传送点
    private List<GameObject> spawnedIcons = new List<GameObject>();
    private Teleportation teleportation;

    private bool isExpanded = false;
    private Vector2 originalPosition;
    private Vector2 originalSize;
    private float cameraSize;

    void Start()
    {
        minimapRect = minimapRawImage.GetComponent<RectTransform>();
        originalPosition = minimapRect.anchoredPosition;
        originalSize = minimapRect.sizeDelta;
        cameraSize = minimapCamera.orthographicSize;

        teleportation = FindAnyObjectByType<Teleportation>();
        fpsController = player.GetComponent<FirstPersonController>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            ToggleMinimapSize();
        }

        if (isExpanded)
        {
            HandleZoom();
        }
        else
        {
            minimapRect.anchoredPosition = originalPosition;
            minimapRect.sizeDelta = originalSize;
            minimapCamera.orthographicSize = cameraSize;
        }
    }

    private void LateUpdate()
    {
        // 小地图跟随玩家逻辑
        Vector3 newPosition = player.position;
        newPosition.y = transform.position.y;
        transform.position = newPosition;
        transform.rotation = Quaternion.Euler(90f, player.eulerAngles.y, 0f);

        // 更新传送点图标位置（仅展开时）
        if (isExpanded)
        {
            UpdateTeleportIconPositions();
        }
    }

    private void ToggleMinimapSize()
    {
        isExpanded = !isExpanded;

        if (isExpanded)
        {
            minimapRect.anchoredPosition = Vector2.zero;
            minimapRect.sizeDelta = new Vector2(1200, 1200);
            SpawnTeleportIcons(); // 生成带名称的图标
            if (fpsController != null)
                fpsController.LockMovement(true);
        }
        else
        {
            minimapRect.anchoredPosition = originalPosition;
            minimapRect.sizeDelta = originalSize;
            ClearTeleportIcons(); // 清除图标
            if (fpsController != null)
                fpsController.LockMovement(false);
        }
    }

    // 生成传送点图标（包含名称）
    private void SpawnTeleportIcons()
    {
        ClearTeleportIcons();

        foreach (var point in teleportPoints)
        {
            GameObject icon = Instantiate(teleportIconPrefab, minimapRawImage.transform);
            spawnedIcons.Add(icon);

            // 设置图标位置
            UpdateIconPosition(icon, point.pointTransform);

            // 设置传送点名称
            Text nameText = icon.GetComponentInChildren<Text>(); 
            if (nameText != null)
            {
                nameText.text = point.pointName; // 显示当前传送点的名称
            }

            // 绑定点击传送事件
            Button btn = icon.GetComponent<Button>();
            if (btn != null)
            {
                Transform target = point.pointTransform;
                btn.onClick.AddListener(() => OnTeleportIconClicked(target));
            }
        }
    }

    // 更新图标位置
    private void UpdateIconPosition(GameObject icon, Transform target)
    {
        Vector3 viewportPos = minimapCamera.WorldToViewportPoint(target.position);
        RectTransform iconRect = icon.GetComponent<RectTransform>();

        // 判断传送点是否在小地图相机的可见范围内
        // 视口坐标 x/y 在 [0,1] 之间，且 z>0（在相机前方）表示在范围内
        bool isWithinViewport = viewportPos.x >= 0 && viewportPos.x <= 1
                            && viewportPos.y >= 0 && viewportPos.y <= 1
                            && viewportPos.z > 0;

        if (iconRect != null)
        {
            // 只有在范围内时才更新位置并显示，否则隐藏
            if (isWithinViewport)
            {
                float x = viewportPos.x * minimapRect.sizeDelta.x - minimapRect.sizeDelta.x / 2;
                float y = viewportPos.y * minimapRect.sizeDelta.y - minimapRect.sizeDelta.y / 2;
                iconRect.anchoredPosition = new Vector2(x, y);
                icon.SetActive(true); // 显示图标
            }
            else
            {
                icon.SetActive(false); // 隐藏范围外图标
            }
        }
    }

    // 更新所有图标位置
    private void UpdateTeleportIconPositions()
    {
        for (int i = 0; i < spawnedIcons.Count && i < teleportPoints.Count; i++)
        {
            UpdateIconPosition(spawnedIcons[i], teleportPoints[i].pointTransform);
        }
    }

    // 清除图标
    private void ClearTeleportIcons()
    {
        foreach (var icon in spawnedIcons)
        {
            Destroy(icon);
        }
        spawnedIcons.Clear();
    }

    // 点击传送
    private void OnTeleportIconClicked(Transform target)
    {
        if (teleportation != null)
        {
            teleportation.TeleportTo(target); // 调用传送功能（需修改Teleportation.cs支持动态目标）
            ToggleMinimapSize();
        }
    }

    private void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            minimapCamera.orthographicSize = Mathf.Clamp(
                minimapCamera.orthographicSize - scroll * zoomSpeed,
                minZoom,
                maxZoom
            );
        }
    }
}