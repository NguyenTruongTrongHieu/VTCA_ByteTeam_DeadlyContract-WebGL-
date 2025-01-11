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

    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    private void Start()
    {
        btnStart.onClick.AddListener(StartGame);
        btnSetting.onClick.AddListener(OpenSetting);
        btnExit.onClick.AddListener(ExitGame);
        trve.onClick.AddListener(trove);
        plan.SetActive(false);
        //AudioManager.Instance.PlayBackground();

        //Kiem tra 2 slider chinh am luong co null khong
        if (musicSlider != null)
        {
            musicSlider.value = AudioManager.Instance.musicVolume;
            musicSlider.onValueChanged.AddListener(AudioManager.Instance.SetMusicVolume);
        }

        if (sfxSlider != null)
        {
            sfxSlider.value = AudioManager.Instance.sfxVolume;
            sfxSlider.onValueChanged.AddListener(AudioManager.Instance.SetSFXVolume);
        }

        AudioManager.Instance.UpdateVolume();
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
        AudioManager.Instance.PlayClickSound();
        plan.SetActive(false);
    }
}
