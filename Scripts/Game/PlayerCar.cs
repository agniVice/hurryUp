using DG.Tweening;
using UnityEngine;

public class PlayerCar : MonoBehaviour, ISubscriber
{
    public static PlayerCar Instance;

    public float Fuel { get; private set; }
    public float MaxFuel { get; private set; }

    [SerializeField] private float _fuel;

    [SerializeField] private GameObject _particlePrefab;
    [SerializeField] private ArrowPointer _arrowPointer;
    [SerializeField] private SpriteRenderer _headlights;

    [SerializeField] private float _speed;
    [SerializeField] private float _rotationSpeed;

    private SpriteRenderer _spriteRenderer;
    private Rigidbody2D _rigidBody;

    private bool _isDestroyed;
    private bool _fuelWarnded;

    private bool _turningRight;
    private bool _turningLeft;

    private int _coinCount;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;

        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rigidBody = GetComponent<Rigidbody2D>();

        _headlights.DOFade(0.6f, 1.5f).SetLink(gameObject).SetEase(Ease.InBounce);
        _spriteRenderer.DOFade(1, 0.5f).SetLink(gameObject).SetEase(Ease.OutBounce);

        MaxFuel = _fuel;
        Fuel = MaxFuel;
    }
    private void Start()
    {
        UpdateSprite();
        AudioManager.Instance.PlaySound(AudioManager.Instance.EngineStart, 0.7f);
        //AudioManager.Instance.PlaySound(AudioManager.Instance.Engine, 1, 1, AudioManager.Instance.EngineStart.length, true);
    }
    private void FixedUpdate()
    {
        if (GameState.Instance.CurrentState != GameState.State.InGame)
        {
            _rigidBody.velocity = Vector2.zero;
            _rigidBody.angularVelocity = 0f;
            return;
        }

        if (_isDestroyed)
            return;

        _rigidBody.velocity = transform.up * _speed;

        if (Fuel >= 0)
        { 
            Fuel -= Time.fixedDeltaTime;
            if (!_fuelWarnded && MaxFuel / Fuel >= 4)
            {
                HUD.Instance.FuelWarning();
                _fuelWarnded = true;
            }

        }
        else
            DestroyPlayer();

        if (_turningLeft)
            _rigidBody.angularVelocity = _rotationSpeed;
        else if (_turningRight)
            _rigidBody.angularVelocity = -_rotationSpeed;
        else
            _rigidBody.angularVelocity = 0;
    }
    public void SubscribeAll()
    {
        PlayerInput.Instance.PlayerMouseDownLeft += StartTurnLeft;
        PlayerInput.Instance.PlayerMouseUpLeft += StopTurnLeft;

        PlayerInput.Instance.PlayerMouseDownRight += StartTurnRight;
        PlayerInput.Instance.PlayerMouseUpRight += StopTurnRight;
    }
    public void UnsubscribeAll()
    {
        PlayerInput.Instance.PlayerMouseDownLeft -= StartTurnLeft;
        PlayerInput.Instance.PlayerMouseUpLeft -= StopTurnLeft;

        PlayerInput.Instance.PlayerMouseDownRight -= StartTurnRight;
        PlayerInput.Instance.PlayerMouseUpRight -= StopTurnRight;
    }
    public void ReviveCar()
    {
        AudioManager.Instance.PlaySound(AudioManager.Instance.Revive, 1);
        AudioManager.Instance.PlaySound(AudioManager.Instance.EngineStart);
        Fuel = MaxFuel;
        _spriteRenderer.DOFade(1, 0.5f).SetLink(gameObject).SetEase(Ease.OutBounce);
        _isDestroyed = false;

        GetComponent<Collider2D>().enabled = true;
        _rigidBody.constraints = RigidbodyConstraints2D.None;
    }
    public void DestroyPlayer()
    {

        _headlights.DOFade(0.1f, 0.3f).SetLink(gameObject).SetEase(Ease.InBounce);
        _spriteRenderer.DOFade(0.3f, 0.5f).SetLink(gameObject).SetEase(Ease.OutBounce);

        GetComponent<Collider2D>().enabled = false;
        _rigidBody.constraints = RigidbodyConstraints2D.FreezeAll;

        _isDestroyed = true;
        _arrowPointer.DestroyAll();
        SpawnParticle();

        GameState.Instance.FinishGame();
        CameraController.Instance.OnPlayerDestroy();
    }
    public void UpdateSprite() => _spriteRenderer.sprite = Garage.Instance.GetPlayerSprite(Garage.Instance.PlayerId);
    public void OnEnemySpawned(Transform enemy) =>_arrowPointer.CreateArrow(enemy, 1);
    public void OnEnemyDestroyed(Transform enemy) => _arrowPointer.DestroyArrow(enemy);
    public void OnCoinSpawned(Transform coins) => _arrowPointer.CreateArrow(coins, 0);
    public void OnCoinsDestroyed(Transform coins) => _arrowPointer.DestroyArrow(coins);
    public void OnFuelSpawned(Transform fuel) => _arrowPointer.CreateArrow(fuel, 2);
    public void OnFuelDestroyed(Transform fuel) => _arrowPointer.DestroyArrow(fuel);
    private void SpawnParticle()
    {
        var particle = Instantiate(_particlePrefab).GetComponent<ParticleSystem>();

        particle.transform.position = new Vector2(transform.position.x, transform.position.y + 0.2f);
        particle.Play();

        Destroy(particle.gameObject, 2f);
    }
    private void StartTurnLeft()
    {
        CameraController.Instance.TurnLeft();
        CameraController.Instance.ScaleDown();
        _turningRight = false;
        _turningLeft = true;
    }
    private void StartTurnRight()
    {
        CameraController.Instance.TurnRight();
        CameraController.Instance.ScaleDown();
        _turningLeft = false;
        _turningRight = true;
    }
    private void StopTurnLeft()
    {
        CameraController.Instance.ResetRotation();
        CameraController.Instance.ScaleUp();
        _turningRight = false;
        _turningLeft = false;
    }
    private void StopTurnRight()
    {
        CameraController.Instance.ResetRotation();
        CameraController.Instance.ScaleUp();
        _turningLeft = false;
        _turningRight = false;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Coin"))
        {
            AudioManager.Instance.PlaySound(AudioManager.Instance.Coin, 0.7f, 0.95f+ _coinCount * 0.01f);
            _coinCount++;
            if (_coinCount == 35)
                _coinCount = 0;
            Missions.Instance.GetCoins(1 * Multiplier.Instance.CoinsMultiplier);
            PlayerBalance.Instance.ChangeCoins(1*Multiplier.Instance.CoinsMultiplier);
            HUD.Instance.UpdateCoins(0.2f);
            collision.GetComponent<Coin>().DestroyCoin();
        }
        if (collision.CompareTag("Fuel"))
        {
            AudioManager.Instance.PlaySound(AudioManager.Instance.Fuel, 1);
            Fuel = MaxFuel;
            _fuelWarnded = false;
            collision.GetComponent<Fuel>().DestroyFuel();
            FuelSpawner.Instance.SpawnFuel();
        }
    }
}
