using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening;

public class UIController : MonoBehaviour
{
    [Header("Common")]
    [SerializeField] private StageController stageController;

    [Header("InGame")]
    [SerializeField] private TextMeshProUGUI textCurrentScore;
    [SerializeField] private TextMeshProUGUI textHighScore;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Text judgmentText;

    [Header("GameOver")]
    [SerializeField]
    private GameObject adView;

    [SerializeField]
    private GameObject gameOverView;

    [SerializeField] private GameObject panelGameOver;
    [SerializeField] private Screenshot screenshot;
    [SerializeField] private Image imageScreenshot;
    [SerializeField] private Text resultScore;
    [SerializeField] private Text highScore;

    private int reviveCount;

    private void Update()
    {
        textCurrentScore.text = stageController.CurrentScore.ToString();
        textHighScore.text = stageController.HighScore.ToString();
    }

    public void BtnClickPause()
    {
        // 일시정지 Panel 활성화, 등장 애니메이션 재생
        StageController.Instance.pause = true;
        SoundManager.Instance.PlayButtonSound();
        StageController.Instance.Pause();
        pausePanel.SetActive(true);
    }

    public void BtnClickHome()
    {
        SoundManager.Instance.PlayButtonSound();
        SceneManager.LoadScene("01Main");
    }

    public void BtnClickRestart()
    {
        SceneManager.LoadScene("02Game");
        SoundManager.Instance.PlayButtonSound();
        // 현재 활성화되어 있는 씬이 "02Game"이기 때문에 아래와 같이 써도 됨
        // SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void BtnClickClose()
    {
        // 일시정지 Panel 퇴장 애니메이션 재생, 비활성화
        StageController.Instance.pause = false;
        SoundManager.Instance.PlayButtonSound();
        pausePanel.SetActive(false);
    }

    public void GameOver()
    {
        // 게임오버 될 떄 게임화면 스크린샷을 촬영하고,
        // 블록이 배치된 부분만 잘라내서 결과 화면에 출력

        /* imageScreenshot.sprite = screenshot.ScreenshotToSprite();
         textResultScore.text = stageController.CurrentScore.ToString();

         panelGameOver.SetActive(true);*/

        if (stageController.CurrentScore >= 100 && reviveCount < 1)
        {
            reviveCount++;
            adView.SetActive(true);
        }
        else
        {
            AdMobManager.Instance.ShowFronAd();
            ShowGameOverView();
        }
    }

    public void ShowAd()
    {
        StageController.Instance.pause = true;
        SoundManager.Instance.PlayButtonSound();
        AdMobManager.Instance.ShowRewardAd();
        adView.SetActive(false);
    }

    public void CloseAdView()
    {
        SoundManager.Instance.PlayButtonSound();
        adView.SetActive(false);
        ShowGameOverView();
    }

    private void ShowGameOverView()
    {
        gameOverView.SetActive(true);
        resultScore.text = stageController.CurrentScore.ToString();
        highScore.text = stageController.HighScore.ToString();
    }

    public void Judgment(string message, Color color)
    {
        judgmentText.text = message;
        judgmentText.color = color;
        judgmentText.transform.DOKill();
        judgmentText.transform.localScale = Vector3.zero;
        judgmentText.transform.DOScale(2.5f, 0.4f).SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                judgmentText.transform.localScale = Vector3.zero;
            });
    }
}
