using System;
using UnityEngine;

public class Bonus : MonoBehaviour
{
    public static Bonus Instance { get; private set;}

    public int FinalBet { get; private set; }
    public int PlayBet { get; private set; }

    [SerializeField] private int _wildChance;
    [SerializeField] private int _defaultWinChance;

    [SerializeField] private int[] _bets;
    [SerializeField] private int _defaultWin;
    [SerializeField] private int _wildWin;

    [SerializeField] private Drum[] _drums;
    [SerializeField] private int _lines = 3;

    private int _currentBet;

    private int _winChance;

    private bool _isSpinning;
    private bool[] _canSpin = { true, true, true };

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;

        FinalBet = _bets[_currentBet];
    }
    private void FixedUpdate()
    {
        CheckForSpin();
    }
    private void CheckForSpin()
    {
        if (_isSpinning)
        {
            if (_canSpin[0] && _canSpin[1] && _canSpin[2])
                OnEndSpin();
        }
    }
    public void Spin()
    {
        if (_isSpinning)
            return;
        if (PlayerBalance.Instance.Coins < FinalBet)
            return;
        if (_canSpin[0] && _canSpin[1] && _canSpin[2] && BonusUI.Instance.CanSpin)
        {
            AudioManager.Instance.PlaySound(AudioManager.Instance.Spin, 1f);
            PlayBet = FinalBet;

            for (int i = 0; i < _canSpin.Length; i++)
                _canSpin[i] = false;

            int winGame = WinGame();

            if (winGame != -1)
            {
                ElementType[] types = new ElementType[3];
                for (int i = 0; i < types.Length; i++)
                {
                    types[i] = (ElementType)UnityEngine.Random.Range(0, 9);
                }
                float spinDelay = 0f;
                for (int i = 0; i < _drums.Length; i++)
                {
                    _drums[i].OnDrumStartWinSpin(spinDelay, types, winGame);
                    spinDelay += 0.15f;
                }
            }
            else
            {
                float spinDelay = 0f;
                for (int i = 0; i < _drums.Length; i++)
                {
                    _drums[i].OnDrumStartSpin(spinDelay);
                    spinDelay += 0.15f;
                }
            }

            PlayerBalance.Instance.ChangeCoins(-FinalBet);
            BonusUI.Instance.UpdateCoins();

            _isSpinning = true;

            BonusUI.Instance.OnStartSpin();
        }
    }
    private void CheckForWin()
    {
        int reward = 0;
        int wildCount = 0;
        int[] win = { 0, 0, 0 };
        for (int j = 0; j < _lines; j++)
        {
            ElementType type = _drums[0].GetLineElement(j + 1);
            bool elementsSame = true;
            for (int i = 0; i < _drums.Length; i++)
            {
                if (_drums[i].GetLineElement(j + 1) != type)
                {
                    elementsSame = false;
                }
            }
            if (elementsSame)
            {
                if (type == ElementType.Wild)
                {
                    wildCount++;
                    reward += PlayBet * _wildWin;
                }
                else
                    reward += PlayBet * _defaultWin;

                foreach (var drum in _drums)
                    drum.PlayWinFX(j + 1);

                win[j]++;
                PlayerBalance.Instance.ChangeCoins(reward);
            }
        }
        BonusUI.Instance.OnPlayerWin(reward, win, wildCount);
    }
    private void OnEndSpin()
    {
        CheckForWin();
        _isSpinning = false;
        BonusUI.Instance.OnEndSpin();
    }
    public void MaxBet()
    {
        _currentBet = _bets.Length - 1;
        FinalBet = _bets[_currentBet];
    }
    public void IncreaseBet()
    {
        if (_currentBet != _bets.Length - 1)
            _currentBet++;
        FinalBet = _bets[_currentBet];
    }
    public void DecreaseBet()
    {
        if (_currentBet != 0)
            _currentBet--;
        FinalBet = _bets[_currentBet];
    }
    private int WinGame()
    {
        float isWin = UnityEngine.Random.Range(0, 100f);
        if (isWin <= _winChance)
        {
            float randomNumber = UnityEngine.Random.Range(0, 110f);
            if (randomNumber < 33)
                return 0;
            else if (randomNumber > 33 && randomNumber < 66)
                return 1;
            else if (randomNumber > 66 && randomNumber < 99)
                return 2;
            else
                return 3;
        }
        else
            return -1;
    }

    public bool IsCurrentBetMax() => _currentBet == (_bets.Length - 1);
    public bool IsCurrentBetMin() => _currentBet == 0;
    public float GetWildChance() => _wildChance;
    public void ReadyForSpin(int id) => _canSpin[id] = true;
    public bool CanOpenMenu() => (_canSpin[0] && _canSpin[1] && _canSpin[2]);
    public int GetDrumIndex(Drum drum) => Array.IndexOf(_drums, drum);
}
