using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainScenario : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textHighScore;

    private void Awake()
    {
        //저장되어 잇는 최고 점수 데이터를 불러와서 출력
        textHighScore.text = PlayerPrefs.GetInt("HighScore").ToString();
    }

    public void BtnClickGameStart()
    {
        SceneManager.LoadScene("02Game");
    }

    public void BtnClickGameExit()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.ExitPlaymode();
        #else
        Application.Quit();
        #endif
    }
}
