using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Accessibility;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public Button btnStart;
    public Button btnSetting;
    public Button btnExit;
    public Button trve;
    public GameObject plan;

    private void Start()
    {
        btnStart.onClick.AddListener(StartGame);
        btnSetting.onClick.AddListener(OpenSetting);
        btnExit.onClick.AddListener(ExitGame);
        trve.onClick.AddListener(trove);
        plan.SetActive(false);
    }

    public void StartGame()
    {
        AudioManager.Instance.PlayClickSound();

        LoadingManager.NextScene = "PlayScene";
        SceneManager.LoadScene("LoadingScene");
    }

    public void OpenSetting()
        
    {
        plan.SetActive(true);
        AudioManager.Instance.PlayClickSound();
    }

    public void ExitGame()
    {
        AudioManager.Instance.PlayClickSound();
        Application.Quit();
    }
    public void trove()
    {
        plan.SetActive(false);
    }
}
