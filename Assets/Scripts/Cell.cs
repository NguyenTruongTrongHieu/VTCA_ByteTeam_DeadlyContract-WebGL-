using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    public bool isOccupied = false;

    private RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public RectTransform RectTransform => rectTransform;

    public void Highlight(Color color)
    {
        GetComponent<UnityEngine.UI.Image>().color = color;
    }

    public void ResetHighlight()
    {
        GetComponent<UnityEngine.UI.Image>().color = Color.white;
    }
}
