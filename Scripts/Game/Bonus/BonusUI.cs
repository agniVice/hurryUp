using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BonusUI : MonoBehaviour
{
    public static BonusUI Instance { get; private set; }

    public List<Sprite> ElementSprites;

    public bool CanSpin { get; private set; } = true;

    [SerializeField] private CanvasGroup _bonusGroup;

    [SerializeField] private TextMeshProUGUI _coinsText;
    [SerializeField] private TextMeshProUGUI _betText;
    [SerializeField] private TextMeshProUGUI _winText;

    [SerializeField] private Image _spinImage;
    [SerializeField] private Image _autoPlayImage;

    [SerializeField] private Button _increaseBetButton;
    [SerializeField] private Button _decreaseBetButton;
    [SerializeField] private Button _return;

    [SerializeField] private Sprite _activeSpin;
    [SerializeField] private Sprite _inactiveSpin;

    [SerializeField] private CanvasGroup _winPanel;

    [SerializeField] private Image[] _winLines;

    [SerializeField] private float _spinButtonTime;


    private int _lastBalance = 0;
    private int _currentBalance;
    private int _currentWin;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;

        GetComponent<Canvas>().worldCamera = Camera.main;
    }
    public void OpenBonus()
    {
        _bonusGroup.blocksRaycasts = true;
        _bonusGroup.alpha = 0f;
        _bonusGroup.DOFade(1, 0.5f).SetLink(_bonusGroup.gameObject);
        UpdateCoins();
        UpdateBet();
    }
    public void CloseBonus()
    {
        _bonusGroup.blocksRaycasts = false;
        _bonusGroup.DOFade(0, 0.3f).SetLink(_bonusGroup.gameObject);
        GameState.Instance.ChangeState(GameState.State.InGame);
    }
    public void UpdateBet()
    {
        _increaseBetButton.interactable = true;
        _decreaseBetButton.interactable = true;

        if (Bonus.Instance.IsCurrentBetMax())
            _increaseBetButton.interactable = false;
        else
            _increaseBetButton.interactable = true;

        if (Bonus.Instance.IsCurrentBetMin())
            _decreaseBetButton.interactable = false;
        else
            _decreaseBetButton.interactable = true;

        _betText.text = Bonus.Instance.FinalBet.ToString();
    }
    public void OnMaxBetButtonClicked()
    {
        Bonus.Instance.MaxBet();
        UpdateBet();
    }
    public void OnPlayerWin(int value, int[] lines, int wildCount)
    {
        int win = 0;
        foreach (int line in lines)
        {
            if (line != 0)
                win++;
        }
        if (win == 0)
            return;
        CanSpin = false;
        for (int l = 0; l < lines.Length; l++)
        {
            _winLines[l].color = new Color32(255, 255, 255, 0);
            if (lines[l] == 1)
            {
                _winLines[l].DOFade(1, 0.2f).SetLink(_winLines[l].gameObject);
                _winLines[l].DOFade(0, 0.2f).SetLink(_winLines[l].gameObject).SetDelay(0.2f);

                _winLines[l].DOFade(1, 0.2f).SetLink(_winLines[l].gameObject).SetDelay(0.4f);
                _winLines[l].DOFade(0, 0.2f).SetLink(_winLines[l].gameObject).SetDelay(0.6f);

                _winLines[l].DOFade(1, 0.2f).SetLink(_winLines[l].gameObject).SetDelay(0.8f);
                _winLines[l].DOFade(0, 0.2f).SetLink(_winLines[l].gameObject).SetDelay(1f);
            }
        }
        AudioManager.Instance.PlaySound(AudioManager.Instance.Win, 1f);
        _currentWin = 0;
        _winPanel.alpha = 0;
        _winPanel.DOFade(1, 0.4f).SetLink(_winPanel.gameObject);
        _winPanel.DOFade(0, 0.4f).SetLink(_winPanel.gameObject).SetDelay(1.1f);

        transform.DOScale(transform.localScale, 1.1f).SetLink(gameObject).OnKill(() => { CanSpin = true; });

        DOTween.To(() => _currentWin, x => _currentWin = x, value, 0.4f).OnUpdate(UpdateWinText).OnKill(() => { UpdateCoins(0.5f); });
    }
    public void UpdateCoins()
    {
        _currentBalance = _lastBalance;
        DOTween.To(() => _currentBalance, x => _currentBalance = x, PlayerBalance.Instance.Coins, 1f).SetEase(Ease.Linear).OnUpdate(UpdateBalanceText);
    }
    public void UpdateCoins(float delay = 0f)
    {
        _currentBalance = _lastBalance;
        DOTween.To(() => _currentBalance, x => _currentBalance = x, PlayerBalance.Instance.Coins, 1f).SetEase(Ease.Linear).OnUpdate(UpdateBalanceText).SetDelay(delay);
    }
    private void UpdateBalanceText()
    {
        _coinsText.text = _currentBalance.ToString();
        _lastBalance = PlayerBalance.Instance.Coins;
    }
    public void OnIncreaseBetButtonClicked()
    {
        Bonus.Instance.IncreaseBet();
        UpdateBet();
    }
    public void OnDecreaseBetButtonClicked()
    {
        Bonus.Instance.DecreaseBet();
        UpdateBet();
    }
    public void OnStartSpin()
    {
        _return.interactable = false;
        _spinImage.sprite = _inactiveSpin;
    }
    public void OnEndSpin()
    {
        _return.interactable = true;
        _spinImage.sprite = _activeSpin;
    }
    public void OnSpinButtonClicked() => Bonus.Instance.Spin();
    private void UpdateWinText() => _winText.text = "+" + _currentWin.ToString();
}
