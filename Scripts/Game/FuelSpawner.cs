using System.Collections.Generic;
using UnityEngine;

public class FuelSpawner : MonoBehaviour
{
    public static FuelSpawner Instance { get; private set; }

    [SerializeField] private GameObject _fuelPrefab;

    [SerializeField] private float _minDistance = 2f;
    [SerializeField] private float _maxDistance = 5f;

    private List<GameObject> _fuels = new List<GameObject>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }
    private void Start()
    {
        SpawnFuel();
    }
    public void SpawnFuel()
    {
        float randomAngle = Random.Range(0, 2f * Mathf.PI);
        float randomDistance = Random.Range(_minDistance, _maxDistance);

        Vector3 spawnPosition = PlayerCar.Instance.transform.position + new Vector3(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle), 0f) * randomDistance;
        var fuel = Instantiate(_fuelPrefab, spawnPosition, Quaternion.identity);
        _fuels.Add(fuel);

        if (PlayerCar.Instance != null)
            PlayerCar.Instance.OnFuelSpawned(fuel.transform);
    }
    public void DestroyAllFuels()
    {
        for (int i = 0; i < _fuels.Count; i++)
        {
            PlayerCar.Instance.OnFuelDestroyed(_fuels[i].transform);
            Destroy(_fuels[i]);
        }
        _fuels.Clear();
    }
    public void RemoveFuel(GameObject fuel)
    {
        _fuels.Remove(fuel);
    }
}
