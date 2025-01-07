using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class login : MonoBehaviour
{
    public GameObject valu;
    public TextMeshProUGUI phantram;
    private float lodingtime = 3f;
    private void Start()
    {
        StartCoroutine(loadsene());
    }
    public IEnumerator loadsene()
    {
        float elapsedtim = 0f;
        while (elapsedtim < lodingtime)
        {
            float progr = Mathf.Clamp01(elapsedtim / lodingtime);
            valu.GetComponent<Image>().fillAmount = progr;
            phantram.text = (progr * 100).ToString("0") + "%";
            elapsedtim += Time.deltaTime;
            yield return null;

        }
        SceneManager.LoadScene("PlayScene");
    }
}
