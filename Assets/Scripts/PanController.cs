using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PanController : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    public Text numberOfFood;

    private float minX = -273f;
    private float maxX = 273f;

    public int countItem;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {

    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 newPosition = rectTransform.anchoredPosition + eventData.delta / canvas.scaleFactor;

        float clampedX = Mathf.Clamp(newPosition.x, minX, maxX);

        rectTransform.anchoredPosition = new Vector2(clampedX, rectTransform.anchoredPosition.y);
    }

    public void OnEndDrag(PointerEventData eventData)
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Hamburger")
        {
            Destroy(collision.gameObject);
            countItem += 1;
            numberOfFood.text = $"Nguyên liệu: {countItem}/10";
           

            AudioManager.Instance.PlayCatchSound();//////////////////////////

            //if (countItem == 10)
            //{
            //    Debug.Log("Xong");
            //    GamePlayManager.instance.FinishCooking();

            //    // Reset
            //    countItem = 0;
            //}
        }
    }
}
