using GoogleMobileAds.Api;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AdMobManager : MonoBehaviour
{
    public static AdMobManager Instance { get; private set; }
    public bool Acive;

    private BannerView _bannerView;
    private InterstitialAd _frontAd;
    private RewardedAd _rewardAd;

    private void Awake()
    {

        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        gameObject.SetActive(Acive);
    }

    private void Start()
    {
        MobileAds.Initialize((InitializationStatus status) =>
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            LoadFrontAd();
            LoadRewardAd();
        });
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        LoadBannerView();
    }

#if UNITY_EDITOR
    private string _bannerId = "ca-app-pub-3940256099942544/6300978111";
#else
private string _bannerId = "ca-app-pub-7795829488150579/2009377081";
#endif

    private void LoadBannerView()
    {
        if (_bannerView != null)
        {
            _bannerView.Destroy();
            _bannerView = null;
        }
        _bannerView = new BannerView(_bannerId, AdSize.GetLandscapeAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth), AdPosition.Bottom);
        _bannerView.LoadAd(new AdRequest());
    }
#if UNITY_EDITOR
    private string _frontId = "ca-app-pub-3940256099942544/1033173712";
#else
    private string _frontId = "ca-app-pub-7795829488150579/9660404883";
#endif
    private void LoadFrontAd()
    {
        if (_frontAd != null)
        {
            _frontAd.Destroy();
            _frontAd = null;
        }

        InterstitialAd.Load(_frontId, new AdRequest(), (InterstitialAd ad, LoadAdError error) =>
        {
            if (error != null)
            {
                return;
            }

            _frontAd = ad;
            FrontEvent();
        });
    }

    private void FrontEvent()
    {
        _frontAd.OnAdFullScreenContentClosed += () =>
        {
            LoadFrontAd();
        };

        _frontAd.OnAdFullScreenContentFailed += (AdError error) =>
        {
            LoadFrontAd();
        };
    }

    public void ShowFronAd()
    {
        if (_frontAd != null && _frontAd.CanShowAd())
        {
            _frontAd.Show();
            Debug.Log("¡ÿ∫Òµ ");
        }
        else
        {
            Debug.Log("¡ÿ∫Òæ»µ ");
        }
    }
#if UNITY_EDITOR
    private string _rewardId = "ca-app-pub-3940256099942544/5224354917";
#else
    private string _rewardId = "ca-app-pub-7795829488150579/9386779009";
#endif
    private void LoadRewardAd()
    {
        if (_rewardAd != null)
        {
            _rewardAd.Destroy();
            _rewardAd = null;
        }

        RewardedAd.Load(_rewardId, new AdRequest(), (RewardedAd ad, LoadAdError error) =>
        {
            if (error != null)
            {
                return;
            }

            _rewardAd = ad;
            RewardEvent();
        });
    }

    private void RewardEvent()
    {
        _rewardAd.OnAdFullScreenContentClosed += () =>
        {
            LoadRewardAd();
        };

        _rewardAd.OnAdFullScreenContentFailed += (AdError error) =>
        {
            LoadRewardAd();
        };
    }

    public void ShowRewardAd()
    {
        if (_rewardAd != null && _rewardAd.CanShowAd())
        {
            _rewardAd.Show((Reward reward) =>
            {
                StageController.Instance.Revive();
                StageController.Instance.pause = false;
            });
            Debug.Log("¡ÿ∫Òµ ");
        }
        else
        {
            Debug.Log("¡ÿ∫Òæ»µ ");
        }
    }

}
