using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour
{
    public static string NextScene = "PlayScene";

    public Text textLoading;
    private Vector3 initialPosition;

    private void Start()
    {
        if (textLoading != null)
        {
            initialPosition = textLoading.rectTransform.anchoredPosition;

            StartWaveEffect();
        }

        LoadingToNextScene();
    }

    public void LoadingToNextScene()
    {
        StartCoroutine(DelayedSceneLoad());
    }

    private IEnumerator DelayedSceneLoad()
    {
        yield return new WaitForSeconds(5);

        SceneManager.LoadScene(NextScene);
    }

    void StartWaveEffect()
    {
        textLoading.rectTransform.DOAnchorPosY(initialPosition.y + 30, 4 / 2)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);

        Debug.Log("Wave effect started for Loading text.");
    }
}
