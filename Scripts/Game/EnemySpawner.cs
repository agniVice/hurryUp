using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance;

    [Header("Other")]

    [SerializeField] private GameObject _enemyPrefab;

    [SerializeField] private float _minDistance = 2f;
    [SerializeField] private float _maxDistance = 5f;

    [SerializeField] private float _minTime = 2f;
    [SerializeField] private float _maxTime = 5f;

    [SerializeField] private float _spawnRate;

    private float _currentTime;

    private Sprite _enemySprite;
    private List<GameObject> _enemys = new List<GameObject>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else Instance = this;
    }
    private void Start()
    {
        _enemySprite = Garage.Instance.EnemySprites[Garage.Instance.EnemyId];
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
            SpawnEnemy();
        }
    }
    public void SetSprite(Sprite sprite)
    {
        _enemySprite = sprite;
    }
    public void SpawnEnemy()
    {
        float randomAngle = Random.Range(0, 2f * Mathf.PI);
        float randomDistance = Random.Range(_minDistance, _maxDistance);

        Vector3 spawnPosition = PlayerCar.Instance.transform.position + new Vector3(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle), 0f) * randomDistance;

        var enemy = Instantiate(_enemyPrefab, spawnPosition, Quaternion.identity);
        _enemys.Add(enemy);
        if (PlayerCar.Instance != null)
            PlayerCar.Instance.OnEnemySpawned(enemy.transform);
    }
    public void DestroyAllEnemys()
    {
        for (int i = 0; i < _enemys.Count; i++)
        {
            PlayerCar.Instance.OnEnemyDestroyed(_enemys[i].transform);
            _enemys[i].GetComponent<EnemyCar>().DestroyCar();
        }
        _enemys.Clear();
    }
    public void RemoveEnemy(GameObject enemy)
    {
        _enemys.Remove(enemy);
    }
}
