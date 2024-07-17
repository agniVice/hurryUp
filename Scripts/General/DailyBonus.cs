using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DailyBonus : MonoBehaviour
{
    [SerializeField] private int _bonus;
    [SerializeField] private TextMeshProUGUI _bonusText;
    [SerializeField] private Image _lockImage;

    private bool _isLocked;
    private int _streak;

    private DateTime _lastDate;
    private DateTime _currentDate;

    private void Start()
    {
        _streak = PlayerPrefs.GetInt("LoginStreak", 1);
        _bonusText.text = (_bonus * _streak).ToString();
        CheckBonus();
    }

    private void CheckBonus()
    {
        DateTime lastLoginDate = GetLastDate();
        DateTime currentDate = DateTime.Now.Date;

        _lastDate = lastLoginDate;
        _currentDate = currentDate;

        if (_lastDate < _currentDate)
            Unlock();
        else
            Lock();
    }

    private DateTime GetLastDate()
    {
        string lastLoginDateString = PlayerPrefs.GetString("LastLoginDate", string.Empty);

        if (DateTime.TryParse(lastLoginDateString, out DateTime lastLoginDate))
            return lastLoginDate;

        return DateTime.MinValue;
    }

    private void Unlock()
    {
        _isLocked = false;
        _lockImage.gameObject.SetActive(false);
    }

    private void Lock()
    {
        _isLocked = true;
        _lockImage.gameObject.SetActive(true);
    }

    public void OnGet()
    {
        if (!_isLocked)
        {
            AudioManager.Instance.PlaySound(AudioManager.Instance.Buy, 0.7f);
            PlayerBalance.Instance.ChangeCoins(_bonus * _streak);
            MenuUI.Instance.UpdateInterface();

            if (_lastDate == _currentDate.AddDays(-1))
                _streak++;
            else
                _streak = 1;

            PlayerPrefs.SetInt("LoginStreak", _streak);

            _currentDate = DateTime.Now.Date;
            PlayerPrefs.SetString("LastLoginDate", _currentDate.ToString());

            _bonusText.text = (_bonus * _streak).ToString();

            Lock();
        }
    }
}
