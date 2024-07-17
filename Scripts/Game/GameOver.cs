using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    public static GameOver Instance { get; private set; }

    [SerializeField] private CanvasGroup _gameOverGroup;
    [SerializeField] private TextMeshProUGUI _revivePrice;
    [SerializeField] private Image _blackScreen;

    private void Awake()
    {
        if (Instance != this && Instance != null)
            Destroy(gameObject);
        else
            Instance = this;

        GetComponent<Canvas>().worldCamera = Camera.main;
    }
    public void ShowGameOver()
    {
        AudioManager.Instance.PlaySound(AudioManager.Instance.GameOver, 1f);
        _revivePrice.text = PlayerBalance.Instance.GetRevivePrice().ToString();
        _gameOverGroup.blocksRaycasts = true;
        _gameOverGroup.alpha = 0f;
        _gameOverGroup.DOFade(1, 0.4f).SetLink(_gameOverGroup.gameObject);
    }
    public void HideGameOver(string level = "None")
    {
        _gameOverGroup.blocksRaycasts = false;
        _gameOverGroup.DOFade(0, 0.3f).SetLink(_gameOverGroup.gameObject);
        if (level != "None")
        {
            _blackScreen.DOFade(1, 0.6f);
            SceneLoader.Instance.LoadScene(level, 0.6f);
        }
    }
    public void OnRestartClicked()
    {
        HideGameOver("Game");
    }
    public void OnMenuClicked()
    {
        HideGameOver("Menu");
    }
    public void OnReviveClicked()
    {
        if (PlayerBalance.Instance.Coins >= PlayerBalance.Instance.GetRevivePrice())
        {
            HideGameOver();
            GameState.Instance.ChangeState(GameState.State.InGame);
            CameraController.Instance.ScaleUp();
            PlayerCar.Instance.ReviveCar();
            PlayerInput.Instance.EnableInput();
            EnemySpawner.Instance.DestroyAllEnemys();
            FuelSpawner.Instance.DestroyAllFuels();
            CoinSpawner.Instance.DestroyAllCoins();
            FuelSpawner.Instance.SpawnFuel();
            PlayerBalance.Instance.ChangeCoins(-PlayerBalance.Instance.GetRevivePrice());
            HUD.Instance.UpdateCoins();
        }
        else
        {
            Error.Instance.ShowError(PlayerBalance.Instance.GetRevivePrice() - PlayerBalance.Instance.Coins);
        }
    }
}
