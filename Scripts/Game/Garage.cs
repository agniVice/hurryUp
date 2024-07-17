using DG.Tweening;
using System;
using System.Collections.Generic;using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Garage : MonoBehaviour
{
    public static Garage Instance { get; private set; }

    public int PlayerId { get; private set; }
    public int EnemyId { get; private set; }

    [SerializeField] private CanvasGroup _garageGroup;
    [SerializeField] private TextMeshProUGUI _pricePlayer;
    [SerializeField] private TextMeshProUGUI _priceEnemy;

    [SerializeField] private Image _selectedPlayer;
    [SerializeField] private Image _selectedEnemy;

    [SerializeField] private GameObject _buyPlayerButton;
    [SerializeField] private GameObject _buyEnemyButton;

    [SerializeField] private Button _nextPlayer;
    [SerializeField] private Button _nextEnemy;

    [SerializeField] private Button _prevPlayer;
    [SerializeField] private Button _prevEnemy;

    [SerializeField] private List<int> _playerPrices;
    [SerializeField] private List<int> _enemyPrices;

    public List<Sprite> PlayerSprites;
    public List<Sprite> EnemySprites;

    private int _playerSelected;
    private int _enemySelected;

    private void Awake()
    {
        if (Instance != this && Instance != null)
            Destroy(gameObject);
        else
            Instance = this;

        GetComponent<Canvas>().worldCamera = Camera.main;

        Initialize();
    }
    private void Initialize()
    {
        PlayerPrefs.SetInt("0PlayerCar", 1);
        PlayerPrefs.SetInt("0EnemyCar", 1);

        PlayerId = PlayerPrefs.GetInt("PlayerId", 0);
        EnemyId = PlayerPrefs.GetInt("EnemyId", 0);

        _playerSelected = PlayerId;
        _enemySelected = EnemyId;
    }
    private void Save()
    {
        PlayerPrefs.SetInt("PlayerId", PlayerId);
        PlayerPrefs.SetInt("EnemyId", EnemyId);
    }
    public void ShowGarage()
    {
        _playerSelected = PlayerId;
        _enemySelected = EnemyId;

        UpdateGarage();

        _garageGroup.blocksRaycasts = true;
        _garageGroup.alpha = 0f;
        _garageGroup.DOFade(1, 0.3f).SetLink(_garageGroup.gameObject);
    }
    public void HideGarage()
    {
        GameState.Instance.ChangeState(GameState.State.InGame);
        _garageGroup.blocksRaycasts = false;
        _garageGroup.DOFade(0, 0.3f).SetLink(_garageGroup.gameObject);
        PlayerCar.Instance.UpdateSprite();
        EnemySpawner.Instance.SetSprite(EnemySprites[EnemyId]);
    }
    public void OnPrevPlayer()
    {
        if (_playerSelected != 0)
            _playerSelected--;
        UpdateGarage();
    }
    public void OnNextPlayer()
    {
        if (_playerSelected != PlayerSprites.Count-1)
            _playerSelected++;
        UpdateGarage();
    }
    public void OnPrevEnemy()
    {
        if (_enemySelected != 0)
            _enemySelected--;
        UpdateGarage();
    }
    public void OnNextEnemy()
    {
        if (_enemySelected != EnemySprites.Count - 1)
            _enemySelected++;
        UpdateGarage();
    }
    private void UpdateGarage()
    {
        if (_playerSelected == 0)
            _prevPlayer.interactable = false;
        else
            _prevPlayer.interactable = true;

        if (_playerSelected == PlayerSprites.Count - 1)
            _nextPlayer.interactable = false;
        else
            _nextPlayer.interactable = true;

        if (_enemySelected == 0)
            _prevEnemy.interactable = false;
        else
            _prevEnemy.interactable = true;

        if (_enemySelected == EnemySprites.Count - 1)
            _nextEnemy.interactable = false;
        else
            _nextEnemy.interactable = true;


        _pricePlayer.text = _playerPrices[_playerSelected].ToString();
        if (1 == PlayerPrefs.GetInt(_playerSelected + "PlayerCar", 0))
        {
            PlayerId = _playerSelected;
            _buyPlayerButton.SetActive(false);
            Save();
        }
        else
            _buyPlayerButton.SetActive(true);

        _priceEnemy.text = _enemyPrices[_enemySelected].ToString();
        if (1 == PlayerPrefs.GetInt(_enemySelected + "EnemyCar", 0))
        {
            EnemyId = _enemySelected;
            _buyEnemyButton.SetActive(false);
            Save();
        }
        else
            _buyEnemyButton.SetActive(true);

        _selectedPlayer.sprite = PlayerSprites[_playerSelected];
        _selectedEnemy.sprite = EnemySprites[_enemySelected];
    }
    public void BuyPlayerCar()
    {
        if (PlayerBalance.Instance.Coins >= _playerPrices[_playerSelected])
        {
            AudioManager.Instance.PlaySound(AudioManager.Instance.Buy, 0.7f);
            PlayerBalance.Instance.ChangeCoins(-_playerPrices[_playerSelected]);
            HUD.Instance.UpdateCoins();
            PlayerCar.Instance.UpdateSprite();
            PlayerPrefs.SetInt(_playerSelected + "PlayerCar", 1);
            PlayerId = _playerSelected;
            Save();
            UpdateGarage();
        }
        else
            Error.Instance.ShowError(_playerPrices[_playerSelected] - PlayerBalance.Instance.Coins);
    }
    public void BuyEnemyCar()
    {
        if (PlayerBalance.Instance.Coins >= _enemyPrices[_enemySelected])
        {
            AudioManager.Instance.PlaySound(AudioManager.Instance.Buy, 0.7f);
            PlayerBalance.Instance.ChangeCoins(-_enemyPrices[_enemySelected]);
            HUD.Instance.UpdateCoins();
            EnemySpawner.Instance.SetSprite(EnemySprites[_enemySelected]);
            PlayerPrefs.SetInt(_enemySelected + "EnemyCar", 1);
            EnemyId = _enemySelected;
            Save();
            UpdateGarage();
        }
        else
            Error.Instance.ShowError(_enemyPrices[_enemySelected] - PlayerBalance.Instance.Coins);

    }
    public bool GetPlayerOpen(int id) => Convert.ToBoolean(PlayerPrefs.GetInt(id + "PlayerCar", 0));
    public bool GetEnemyOpen(int id) => Convert.ToBoolean(PlayerPrefs.GetInt(id + "EnemyCar", 0));
    public int GetPlayerPrice(int id) => _playerPrices[id];
    public int GetEnemyPrice(int id) => _enemyPrices[id];
    public Sprite GetPlayerSprite(int id) => PlayerSprites[id];
    public Sprite GetEnemySprite(int id) => EnemySprites[id];
}
