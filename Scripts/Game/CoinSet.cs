using System.Collections.Generic;
using UnityEngine;

public class CoinSet : MonoBehaviour
{
    [SerializeField] private List<Coin> _coins;

    [SerializeField] private float _maxDistance;
    [SerializeField] private float _checkRate;

    private float _currentTime;

    private void Awake()
    {
        float delay = 1f;
        foreach (var coin in _coins)
        {
            coin.Spawn(this, delay);
            delay += 0.05f;
        }
    }
    private void FixedUpdate()
    {
        if (GameState.Instance.CurrentState != GameState.State.InGame)
            return;
        if (_currentTime < _checkRate)
        {
            _currentTime += Time.fixedDeltaTime;
        }
        else
        {
            _currentTime = 0f;
            Check();
        }
    }
    private void Check()
    {
        if (Vector2.Distance(PlayerCar.Instance.transform.position, transform.position) >= _maxDistance)
        {
            CoinSpawner.Instance.RemoveCoins(gameObject);
            if (PlayerCar.Instance != null)
                PlayerCar.Instance.OnEnemyDestroyed(transform);
            Destroy(gameObject);
        }
        if (_coins.Count == 0)
        {
            CoinSpawner.Instance.RemoveCoins(gameObject);
            if (PlayerCar.Instance != null)
                PlayerCar.Instance.OnEnemyDestroyed(transform);
            Destroy(gameObject);
        }
        _currentTime = _checkRate;
    }
    public void RemoveCoin(Coin coin)
    {
        _coins.Remove(coin);
    }
}
