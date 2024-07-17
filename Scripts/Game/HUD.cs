using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public static HUD Instance { get; private set; }

    public Transform CoinsPosition;
    [SerializeField] private CanvasGroup _hudGroup;

    [SerializeField] private TextMeshProUGUI _levelCount;
    [SerializeField] private TextMeshProUGUI _coinsCount;
    [SerializeField] private TextMeshProUGUI _experience;
    [SerializeField] private Image _experienceBar;

    [SerializeField] private Image _fuelBar;

    [SerializeField] private TextMeshProUGUI _multiplier;
    [SerializeField] private Image _multiplierBar;

    [SerializeField] private TextMeshProUGUI _missionExperience;
    [SerializeField] private TextMeshProUGUI _missionProgress;
    [SerializeField] private Image _missionBar;
    [SerializeField] private CanvasGroup _missionCompleted;
    [SerializeField] private Image _fuelWarning;

    private int _startCoins;
    private int _currentCoins;

    private void Awake()
    {
        if (Instance != this && Instance != null)
            Destroy(gameObject);
        else
            Instance = this;

        GetComponent<Canvas>().worldCamera = Camera.main;

        ShowHUD();
    }
    private void Update()
    {
        if (PlayerCar.Instance != null)
            _fuelBar.fillAmount = PlayerCar.Instance.Fuel / PlayerCar.Instance.MaxFuel;
        if (Multiplier.Instance != null)
            _multiplierBar.fillAmount = Multiplier.Instance.GetPercent();
    }
    public void ShowHUD()
    {
        UpdateCoins();
        UpdateLevel();
        UpdateMission();

        _hudGroup.blocksRaycasts = true;
        _hudGroup.alpha = 0f;
        _hudGroup.DOFade(1, 0.8f).SetLink(_hudGroup.gameObject).SetDelay(0.2f);
    }
    public void HideHUD()
    {
        _hudGroup.blocksRaycasts = false;
        _hudGroup.DOFade(0, 0.3f).SetLink(_hudGroup.gameObject);
    }
    public void UpdateCoins(float time = 1f)
    {
        _currentCoins = _startCoins;
        DOTween.To(() => _currentCoins, x => _currentCoins = x, PlayerBalance.Instance.Coins, 1f).SetEase(Ease.Linear).OnUpdate(UpdateCoinsText);
    }
    private void UpdateCoinsText()
    {
        _coinsCount.text = _currentCoins.ToString();
        _startCoins = PlayerBalance.Instance.Coins;
    }
    public void UpdateLevel()
    {
        _levelCount.text = PlayerBalance.Instance.Level.ToString();
        _experience.text = Mathf.Round(PlayerBalance.Instance.Experience) + "/" + Mathf.Round(PlayerBalance.Instance.TargetExperience);
        _experienceBar.fillAmount = PlayerBalance.Instance.Experience / PlayerBalance.Instance.TargetExperience;
    }
    public void UpdateMission()
    {
        _missionExperience.text = Missions.Instance.CurrentExperience.ToString();
        _missionProgress.text = Missions.Instance.CoinsCount + "/" + Missions.Instance.Target;
        _missionBar.fillAmount = (float)Missions.Instance.CoinsCount / Missions.Instance.Target;
    }
    public void FuelWarning()
    {
        AudioManager.Instance.PlaySound(AudioManager.Instance.FuelWarning, 1f);
        _fuelWarning.DOFade(0.5f, 0.3f).SetLink(_fuelWarning.gameObject);
        _fuelWarning.DOFade(0f, 0.3f).SetLink(_fuelWarning.gameObject).SetDelay(0.3f);
        _fuelWarning.DOFade(0.5f, 0.3f).SetLink(_fuelWarning.gameObject).SetDelay(0.6f);
        _fuelWarning.DOFade(0f, 0.3f).SetLink(_fuelWarning.gameObject).SetDelay(0.9f);
        _fuelWarning.DOFade(0.5f, 0.3f).SetLink(_fuelWarning.gameObject).SetDelay(1.2f);
        _fuelWarning.DOFade(0f, 0.3f).SetLink(_fuelWarning.gameObject).SetDelay(1.5f);
    }
    public void OnMissionCompleted()
    {
        _missionCompleted.DOFade(1, 0.8f).SetLink(gameObject);
        _missionCompleted.DOFade(0, 0.8f).SetLink(gameObject).SetDelay(1.5f);
    }
    public void UpdateMultiplier()
    {
        _multiplier.text = Multiplier.Instance.CoinsMultiplier + "x";
    }
    public void OnGarageClicked()
    {
        Garage.Instance.ShowGarage();
        GameState.Instance.ChangeState(GameState.State.Paused);
    }
}
