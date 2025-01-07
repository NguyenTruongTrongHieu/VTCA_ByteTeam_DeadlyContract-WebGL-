using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum ItemType
{
    I,
    T,
    O
}
public class Item : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerClickHandler
{
    public ItemType itemType;
    public int score;
    public bool isPlaced = false;
    public Transform rootPos;
    public int crrRotation = 0;

    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private Vector3 originalScale;
    public int[,] itemArray = null;

    private Vector3 firstPos = new Vector3(0,0,-999);

    public List<Vector2Int> placedCells = new List<Vector2Int>();
    private Border crrBorder;
    private void Awake()
    {
        itemArray = GenerateItemArray();
    }
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
        originalScale = transform.localScale;

        if (firstPos.z == -999)
            firstPos = transform.position;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("OnBeginDrag");
        if (canvasGroup != null)
            canvasGroup.blocksRaycasts = false;

        transform.localScale = originalScale * 1.5f;
        crrBorder = GamePlayManager.instance.GetCurrentBorder();
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.position = eventData.position;
        if (crrBorder == null)
        {
            Debug.Log("Not found border or GamePlayManager Instance");
            return;
        }
        (Transform nearestCell, bool isInside, Vector2Int cor) = crrBorder.GetNearestCell(this.rootPos.position);
        if (!isInside || nearestCell == null)
        {
            //Debug.Log("Error when finding nearest cell");
            return;
        }
        Debug.Log("Nearest cell: " + cor, nearestCell.gameObject);
        if (itemArray == null)
            itemArray = GenerateItemArray();
        cor = HandleCorToPlace(cor);
        crrBorder.HoverArray(itemArray, cor, this);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Vector2 anchorPos = rectTransform.anchoredPosition;
        Debug.Log($"OnEndDrag at: " + eventData.position + " | root pos: " + rootPos.position + " | " + anchorPos);
        if (canvasGroup != null)
            canvasGroup.blocksRaycasts = true;
        DoPlaceItem();
    }
    private Vector2Int HandleCorToPlace(Vector2Int cor)
    {
        switch (this.crrRotation)
        {
            case 0:
                cor.y -= 1; break;
            case 1:
                cor.x += 1; break;
            case 2:
                cor.y -= 1;
                cor.x += 2; break;
            case 3:
                cor.x += 1;
                cor.y -= 2; break;
        }
        return cor;
    }    

    public void DoPlaceItem()
    {
        if (crrBorder == null)
        {
            Debug.Log("Not found border or GamePlayManager Instance");
            return;
        }
        (Transform nearestCell, bool isInside, Vector2Int cor) = crrBorder.GetNearestCell(this.rootPos.position);
        if (!isInside || nearestCell == null)
        {
            Debug.Log("Error when finding nearest cell");
            crrBorder.TurnOffHover();
            crrBorder.RemovePlaced(this.placedCells);
            ResetState();
            return;
        }
        Debug.Log("Nearest cell: " + cor, nearestCell.gameObject);
        if (itemArray == null)
            itemArray = GenerateItemArray();
        cor = HandleCorToPlace(cor);
        bool isPlaced = crrBorder.PlaceToArray(itemArray, cor, this);
        if (!isPlaced)
        {
            transform.localScale = originalScale;
            transform.position = firstPos;
        }
        else
        {
            Vector3 moveDir = nearestCell.position - rootPos.position;
            Debug.Log($"n: {nearestCell.position} | r: {rootPos.position} | p: {transform.position}");
            Debug.Log("Move Dir: " + moveDir);
            moveDir.z = 0;
            this.transform.position += moveDir;
        }
    }
    private void ResetState()
    {
        this.isPlaced = false;
        this.placedCells = null;
        this.transform.position = firstPos;
        this.transform.localScale = originalScale;
    }    
    public void SetPlacedCells(List<Vector2Int> v, bool isPlaced)
    {
        this.placedCells = v;
        this.isPlaced = isPlaced;
    }

    private static int[,] RotateMatrix(int[,] matrix, int rotations)
    {
        // Đảm bảo số lần xoay nằm trong khoảng từ 0 đến 3 (4 lần là về vị trí ban đầu)
        rotations = rotations % 4;

        // Thực hiện xoay mảng
        for (int r = 0; r < rotations; r++)
        {
            matrix = RotateOnce(matrix);
        }

        return matrix;
    }

    public static int[,] RotateOnce(int[,] matrix)
    {
        int[,] rotated = new int[3, 3]; // Mảng kết quả sau khi xoay

        // Thực hiện xoay chiều kim đồng hồ
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                rotated[j, 2 - i] = matrix[i, j];
            }
        }

        return rotated;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            Debug.Log("Chuột phải click vào Image: " + gameObject.name);
            OnRightClickItem();
        }
    }
    private void OnRightClickItem()
    {
        Debug.Log("Item do rotate", this.gameObject);
        if (!this.isPlaced)
        {
            if (!isRotating)
            {
                crrBorder = GamePlayManager.instance.GetCurrentBorder();
                Debug.Log("Start to rotate");
                this.crrRotation = (this.crrRotation + 1) % 4;
                this.itemArray = RotateOnce(this.itemArray);
                StartCoroutine(RotateOverTime());
            }
        }
        else
        {
            Debug.Log("Remove from Border");
            crrBorder.RemovePlaced(this.placedCells);
            ResetState();
        }    
    }
    private bool isRotating = false;
    private IEnumerator RotateOverTime()
    {
        isRotating = true;
        // Lấy góc hiện tại của đối tượng (trong không gian Euler)
        float currentRotation = transform.eulerAngles.z;

        // Mục tiêu xoay thêm 90 độ ngược chiều kim đồng hồ
        float targetRotation = currentRotation - 90f;

        // Nếu góc hiện tại > 90 độ, sửa mục tiêu để tránh xoay ngược quá
        if (targetRotation < 0)
        {
            targetRotation += 360f;
        }

        // Xoay từ góc hiện tại đến góc mục tiêu
        float rotated = currentRotation;
        if (rotated <= 0)
            rotated += 360;
        // Trong mỗi frame, xoay thêm một chút cho đến khi đạt được mục tiêu
        Debug.Log($"{rotated} {targetRotation}");
        while (rotated > targetRotation)
        {
            float rotationThisFrame = 360 * Time.deltaTime;
            rotated -= rotationThisFrame;

            // Nếu xoay quá mục tiêu, dừng lại ở targetRotation
            if (rotated < targetRotation)
                rotationThisFrame = rotated - targetRotation; // Đảm bảo không vượt quá mục tiêu

            // Xoay Image (ngược chiều kim đồng hồ)
            transform.Rotate(Vector3.back, rotationThisFrame); // Xoay quanh trục Z ngược chiều kim đồng hồ

            yield return null; // Chờ đến frame tiếp theo
        }

        // Đảm bảo kết thúc ở đúng góc mục tiêu
        transform.rotation = Quaternion.Euler(0f, 0f, targetRotation);
        isRotating = false;
    }
    private int[,] GenerateItemArray()
    {
        int[,] arr = new int[3, 3];
        switch (itemType)
        {
            case ItemType.I:
                arr = new int[3, 3]
                {
                    { 0,0,0},
                    { 0,1,0},
                    { 0,1,0}
                };
                break;
            case ItemType.T:
                arr = new int[3, 3]
                {
                    { 0,0,0},
                    { 1,1,1},
                    { 0,1,0}
                };
                break;
            case ItemType.O:
                arr = new int[3, 3]
                {
                    { 0,0,0},
                    { 1,1,0},
                    { 1,1,0}
                };
                break;
            default: return null;
        }
        return arr;
    }
}
