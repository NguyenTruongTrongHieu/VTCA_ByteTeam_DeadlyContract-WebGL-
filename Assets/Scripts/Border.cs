using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Border : MonoBehaviour
{
    public Vector2Int size;
    public float cellSize = 100;
    public Transform cellContain;

    public int[,] borderArray = null;
    public List<Transform> cells = new List<Transform>();

    /// <summary>
    /// Border[row, col] row: top to bot, col: left to right
    /// </summary>
    private void Awake()
    {
        if (cells.Count == 0)
        {
            for (int i = 0; i < cellContain.childCount; i++)
            {
                cells.Add(cellContain.GetChild(i));
            }
        }

        GenerateBorderArray();
        RenameCell();
    }
    private void GenerateBorderArray()
    {
        if (borderArray != null) return;
        borderArray = new int[size.y, size.x];
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                borderArray[j, i] = 0;
            }
        }
    }
    private int[,] CloneBorder()
    {
        int[,] clone = new int[size.y, size.x];
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                clone[i, j] = borderArray[i,j] + 0;
            }
        }
        return clone;
    }
    private int[,] FirstBorder()
    {
        int[,] clone = new int[size.y, size.x];
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                clone[j, i] = 0;
            }
        }
        return clone;
    }
    Vector2Int preHorvering = new Vector2Int(-1, -1);
    Vector2Int crrHorvering = new Vector2Int(-1, -1);
    public void TurnOffHover()
    {
        Debug.Log("Turn Off Hover");
        for(int i = 0; i < cells.Count; i++)
        {
            //Debug.Log("Turn off: " + i, cells[i].GetChild(0).gameObject);
            cells[i].GetChild(0).gameObject.SetActive(false);
        }
    }
    public void HoverArray(int[,] arrayToInsert, Vector2Int insertionPosition, Item item)
    {
        if (item.isPlaced)
        {
            RemovePlaced(item.placedCells);
            item.isPlaced = false;
        }
        GenerateBorderArray();
        this.crrHorvering = insertionPosition;
        if (this.preHorvering != new Vector2Int(-1,-1) || this.crrHorvering != this.preHorvering)
        {
            Debug.Log("new hover pos: " + insertionPosition);
            this.preHorvering = this.crrHorvering;
            int[,] clone = FirstBorder();
            List<Vector2Int> hoverCells = new List<Vector2Int>();
            int rows = arrayToInsert.GetLength(0);
            int columns = arrayToInsert.GetLength(1);

            int borderX = borderArray.GetLength(0);
            int borderY = borderArray.GetLength(1);

            int k = 2;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    int x = insertionPosition.x - (rows - 1 - i);
                    int y = insertionPosition.y + j;
                    if (x < 0 || y < 0 || x >= borderX || y >= borderY)
                    {
                        continue;
                    }
                    if (i < 0 || j < 0) continue;

                    Debug.Log($"Insert from {i},{j} -> {x},{y}");
                    if (clone[x,y] != 1)
                        clone[x, y] = arrayToInsert[i, j];
                }
            }
            DoHover(clone);
        }
    }
    public bool PlaceToArray(int[,] arrayToInsert, Vector2Int insertionPosition, Item item)
    {
        TurnOffHover();
        GenerateBorderArray();
        Debug.Log("PlaceToArray: " + insertionPosition);
        int[,] clone = CloneBorder();
        List<Vector2Int> hoverCells = new List<Vector2Int>();
        int rows = arrayToInsert.GetLength(0);
        int columns = arrayToInsert.GetLength(1);

        int borderX = borderArray.GetLength(0);
        int borderY = borderArray.GetLength(1);

        int k = 2;
        List<Vector2Int> placedCells = new List<Vector2Int>();
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                int x = insertionPosition.x - (rows - 1 - i);
                int y = insertionPosition.y + j;
                if (x < 0 || y < 0 || x >= borderX || y >= borderY)
                {
                    if (arrayToInsert[i,j] == 1)
                    {
                        Debug.Log($"Out size: {x},{y} | {i},{j}");
                        return false;
                    }
                    continue;
                }
                if (i < 0 || j < 0) continue;

                Debug.Log($"Insert from {i},{j} -> {x},{y} | {clone[x, y]} | {arrayToInsert[i,j]}");
                if (arrayToInsert[i, j] == 1 && clone[x,y] == 1)
                {
                    Debug.Log("Place conflict collider");
                    item.SetPlacedCells(null, false);
                    return false;
                }
                if (clone[x,y] == 0 && arrayToInsert[i,j] != 0)
                {
                    clone[x, y] = arrayToInsert[i, j];
                    placedCells.Add(new Vector2Int(x,y));
                }
                    
            }
        }
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                if (borderArray[i, j] == 0)
                    borderArray[i,j] = clone[i,j];
            }
        }
        DoPlaced(borderArray);
        item.SetPlacedCells(placedCells, true);
        return true;
    }

    private void PrintBorder(int[,] arr)
    {
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                Debug.Log($"{i},{j} = {arr[i, j]}");
            }
        }
    }

    private void DoHover(int[,] pos)
    {
        TurnOffHover();
        for (int i = 0;i < pos.GetLength(0);i++)
        {
            for(int j = 0;j < pos.GetLength(1);j++)
            {
                int value = pos[i,j];
                //Debug.Log($"Hover {j},{i} = {value}");
                Transform cell = GetCell(j, i);
                cell.GetChild(0).gameObject.SetActive(value != 0);
            }
        }
    }
    private void DoPlaced(int[,] pos)
    {
        for (int i = 0; i < pos.GetLength(0); i++)
        {
            for (int j = 0; j < pos.GetLength(1); j++)
            {
                int value = pos[i, j];
                //Debug.Log($"place {i},{j} = {value}");
                Transform cell = GetCell(j, i);
                cell.GetChild(1).gameObject.SetActive(value != 0);
            }
        }
    }
    public void RemovePlaced(List<Vector2Int> v)
    {
        foreach (var cor in v)
        {
            int x = cor.x;
            int y = cor.y;
            GetCell(y, x).GetChild(1).gameObject.SetActive(false);
            borderArray[x, y] = 0;
        }
    }
    private int Cor2Index(int x, int y)
    {
        return size.x * y + x;
    }
    private Vector2Int Index2Cor(int index)
    {
        int row = index / size.y;
        int col = index % size.x;
        return new Vector2Int(row, col);
    }
    public Transform GetCell(int x, int y)
    {
        int index = Cor2Index(x, y);
        if (cellContain.transform.childCount > index)
            return cellContain.transform.GetChild(index);
        return null;
    }
    private void RenameCell()
    {
        int childCount = cellContain.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Transform child = cellContain.GetChild(i);
            Vector2Int cor = Index2Cor(i);
            child.name = $"{cor.x},{cor.y}";
        }
    }
    public (Transform, bool, Vector2Int) GetNearestCell(Vector3 pos)
    {
        Transform cell_0_0 = GetCell(0, 0);
        Transform cell_n_n = GetCell(size.x-1, size.y-1);

        float minX = cell_0_0.position.x - 100;
        float maxX = cell_n_n.position.x + 100;
        float minY = cell_n_n.position.y - 100;
        float maxY = cell_0_0.position.y + 100;

        Debug.Log($"{minX},{maxX},{minY},{maxY} | {pos}");

        bool isInside = false;
        if (pos.x >= minX && pos.x <= maxX && pos.y >= minY && pos.y <= maxY)
            isInside = true;
        if (!isInside)
            return (null, false, Vector2Int.zero);

        int childCount = cellContain.childCount;
        float nearestDistance = -1;
        Transform nearestCell = null;
        int index = 0;
        for (int i = 0; i < childCount; i++)
        {
            Transform child = cellContain.GetChild(i);
            float d = (pos - child.position).magnitude;
            if (nearestDistance == -1 || d < nearestDistance)
            {
                nearestDistance = d;
                nearestCell = child;
                index = i;
            }
        }
        return (nearestCell, isInside, Index2Cor(index));
    }

    public void ResetBorder()
    {
        for (int i = 0; i < borderArray.GetLength(0); i++)
        {
            for (int j = 0; j < borderArray.GetLength(1); j++)
            {
                borderArray[i, j] = 0;
            }
        }
        TurnOffHover();
        TurnOffPlaced();
    }
    public void TurnOffPlaced()
    {
        Debug.Log("Turn Off Placed");
        for (int i = 0; i < cells.Count; i++)
        {
            //Debug.Log("Turn off: " + i, cells[i].GetChild(0).gameObject);
            cells[i].GetChild(1).gameObject.SetActive(false);
        }
    }
}
