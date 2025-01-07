using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [SerializeField] private GameObject conversationObject;

    public void SimpleWave(GameObject waveObject, Vector2 pointA, Vector2 pointB, float waveHigh, float duration, float frequency = 2f)
    {
        RectTransform rectTransform = waveObject.GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            Debug.LogError("waveObject is not a RectTransform");
            return;
        }

        rectTransform.anchoredPosition = pointA;

        DOTween.To(() => rectTransform.anchoredPosition,
                   x => rectTransform.anchoredPosition = x,
                   pointB,
                   duration)
            .SetEase(Ease.Linear)
            .OnUpdate(() =>
            {
                float progress = (rectTransform.anchoredPosition.x - pointA.x) / (pointB.x - pointA.x);
                float y = Mathf.Sin(progress * Mathf.PI * 2 * frequency) * waveHigh + pointA.y;

                rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, y);
            })
            .OnComplete(() =>
            {
                Debug.Log("Wave motion completed.");
                conversationObject.SetActive(true);
            });
    }
}
