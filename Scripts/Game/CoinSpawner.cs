using System.Collections.Generic;
using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    public static CoinSpawner Instance { get; private set; }

    [SerializeField] private List<GameObject> _coinsPrefabs = new List<GameObject>();

    [SerializeField] private float _minDistance = 2f;
    [SerializeField] private float _maxDistance = 5f;

    [SerializeField] private float _minTime;
    [SerializeField] private float _maxTime;

    [SerializeField] private float _spawnRate;

    private List<GameObject> _coins = new List<GameObject>();

    private float _currentTime;

    private void Awake()
    {
        if(Instance != this && Instance != null)
            Destroy(gameObject);
        else
            Instance = this;
    }
    private void FixedUpdate()
    {
        if (GameState.Instance.CurrentState != GameState.State.InGame)
            return;
        if (_currentTime < _spawnRate)
        {
            _currentTime += Time.fixedDeltaTime;
        }
        else
        {
            _currentTime = 0f;
            _spawnRate = Random.Range(_minTime, _maxTime);
            SpawnCoins();
        }
    }
    private void SpawnCoins()
    {
        float randomAngle = Random.Range(0, 2f * Mathf.PI);
        float randomDistance = Random.Range(_minDistance, _maxDistance);

        Vector3 spawnPosition = PlayerCar.Instance.transform.position + new Vector3(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle), 0f) * randomDistance;
        var coins = Instantiate(_coinsPrefabs[Random.Range(0, _coinsPrefabs.Count)], spawnPosition, Quaternion.identity);
        _coins.Add(coins);

        if (PlayerCar.Instance != null)
            PlayerCar.Instance.OnCoinSpawned(coins.transform);
    }
    public void DestroyAllCoins()
    {
        for (int i = 0; i < _coins.Count; i++)
        {
            PlayerCar.Instance.OnCoinsDestroyed(_coins[i].transform);
            Destroy(_coins[i]);
        }
        _coins.Clear();
    }
    public void RemoveCoins(GameObject coins)
    {
        _coins.Remove(coins);
    }
}
