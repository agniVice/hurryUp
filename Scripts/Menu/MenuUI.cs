using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{
    public static MenuUI Instance { get; private set; }

    [SerializeField] private GameObject _canvas;

    [SerializeField] private CanvasGroup _menuGroup;
    [SerializeField] private CanvasGroup _settingsGroup;

    [SerializeField] private TextMeshProUGUI _levelCount;
    [SerializeField] private TextMeshProUGUI _expCount;
    [SerializeField] private Image _progressLevel;

    [SerializeField] private TextMeshProUGUI _coinsCount;

    [SerializeField] private Image _soundOffImage;
    [SerializeField] private Image _soundOnImage;

    [SerializeField] private Image _musicOffImage;
    [SerializeField] private Image _musicOnImage;

    [SerializeField] private Sprite _onEnabled;
    [SerializeField] private Sprite _offEnabled;

    [SerializeField] private Sprite _onDisabled;
    [SerializeField] private Sprite _offDisabled;

    private int _startCoins;
    private int _currentCoins;

    private void Awake()
    {
        if(Instance != this && Instance != null)
            Destroy(gameObject);
        else
            Instance = this;

        OpenAll();
    }
    public void OpenAll()
    {
        _canvas.GetComponent<CanvasGroup>().alpha = 0f;
        _canvas.GetComponent<CanvasGroup>().DOFade(1, 0.3f).SetLink(_canvas);
        _canvas.gameObject.SetActive(true);
        _menuGroup.blocksRaycasts = false;
        _settingsGroup.blocksRaycasts = false;
    }
    private void CloseAll()
    {
        _canvas.GetComponent<CanvasGroup>().DOFade(0, 0.5f).SetLink(_canvas);
        SceneLoader.Instance.LoadScene("Game", 0.6f);
        _menuGroup.DOFade(0, 0.4f).SetLink(_menuGroup.gameObject);
        _menuGroup.blocksRaycasts = false;
        _settingsGroup.blocksRaycasts = false;
    }
    public void OpenMenu()
    {
        UpdateInterface();
        _settingsGroup.blocksRaycasts = false;
        _settingsGroup.DOFade(0, 0.2f).SetLink(_settingsGroup.gameObject);
        _menuGroup.blocksRaycasts = true;
        _menuGroup.alpha = 0f;
        _menuGroup.DOFade(1, 0.2f).SetLink(_menuGroup.gameObject);
    }
    private void OpenSettings()
    {
        UpdateSettings();
        _menuGroup.blocksRaycasts = false;
        _menuGroup.alpha = 0f;

        _settingsGroup.blocksRaycasts = true;
        _settingsGroup.alpha = 0f;
        _settingsGroup.DOFade(1, 0.2f).SetLink(_settingsGroup.gameObject);
    }
    public void OnPlayClicked()
    {
        CloseAll();
    }
    public void OnReturnClicked() => OpenMenu();
    public void OnSettingsClicked() => OpenSettings();
    public void OnExitClicked()
    {
        Application.Quit();
    }
    public void UpdateInterface()
    {
        _currentCoins = _startCoins;
        DOTween.To(() => _currentCoins, x => _currentCoins = x, PlayerBalance.Instance.Coins, 1f).SetEase(Ease.Linear).OnUpdate(UpdateCoinsText);
        _levelCount.text = PlayerBalance.Instance.Level.ToString();
        _expCount.text = PlayerBalance.Instance.Experience + "/" + PlayerBalance.Instance.TargetExperience;
        _progressLevel.fillAmount = PlayerBalance.Instance.Experience / PlayerBalance.Instance.TargetExperience;
    }
    private void UpdateCoinsText()
    {
        _coinsCount.text = _currentCoins.ToString();
        _startCoins = PlayerBalance.Instance.Coins;
    }
    public void UpdateSettings()
    {
        if (AudioManager.Instance.IsSoundEnabled)
        {
            _soundOffImage.sprite = _offDisabled;
            _soundOnImage.sprite = _onEnabled;
        }
        else
        {
            _soundOffImage.sprite = _offEnabled;
            _soundOnImage.sprite = _onDisabled;
        }
        if (AudioManager.Instance.IsMusicEnabled)
        {
            _musicOffImage.sprite = _offDisabled;
            _musicOnImage.sprite = _onEnabled;
        }
        else
        {
            _musicOffImage.sprite = _offEnabled;
            _musicOnImage.sprite = _onDisabled;
        }
    }
    public void MusicOff() => AudioManager.Instance.ChangeMusic(false);
    public void MusicOn() => AudioManager.Instance.ChangeMusic(true);
    public void SoundOff() => AudioManager.Instance.ChangeSound(false);
    public void SoundOn() => AudioManager.Instance.ChangeSound(true);
}
