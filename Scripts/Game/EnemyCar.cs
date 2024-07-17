using DG.Tweening;
using UnityEngine;

public class EnemyCar : MonoBehaviour
{
    [SerializeField] private GameObject _particlePrefab;
    [SerializeField] private TrailRenderer[] _trailRenderers;
    [SerializeField] private Sprite[] _carSprites;

    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _rotationSpeed = 45f;

    [SerializeField] private SpriteRenderer _headlights;

    private SpriteRenderer _spriteRenderer;
    private Transform _playerTransform;
    private Rigidbody2D _rigidBody;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _playerTransform = PlayerCar.Instance.transform;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.DOFade(1, 0.5f).SetLink(gameObject).SetEase(Ease.OutBounce);
        _headlights.DOFade(0.6f, 1f).SetLink(gameObject).SetEase(Ease.InBounce);
        UpdateSprite();
        AudioManager.Instance.PlaySound(AudioManager.Instance.EngineStart, 0.5f, Random.Range(0.9f, 1.1f));
    }
    private void FixedUpdate()
    {
        if (GameState.Instance.CurrentState != GameState.State.InGame)
        {
            _rigidBody.velocity = Vector2.zero;
            _rigidBody.angularVelocity = 0f;
            return;
        }

        if (_playerTransform != null)
        {
            Vector2 directionToPlayer = _playerTransform.position - transform.position;

            directionToPlayer.Normalize();

            float angle = Vector2.SignedAngle(transform.up, directionToPlayer);

            _rigidBody.angularVelocity = angle * _rotationSpeed;
        }
        else
        {
            _speed *= 0.95f;
            _rigidBody.angularVelocity = 0;
        }
        _rigidBody.velocity = transform.up * _speed;
    }
    public void SetSprite(Sprite sprite)
    { 
        _spriteRenderer.sprite = sprite;
    }
    private void SpawnParticle()
    {
        var particle = Instantiate(_particlePrefab).GetComponent<ParticleSystem>();

        particle.transform.position = new Vector2(transform.position.x, transform.position.y + 0.2f);
        particle.Play();

        Destroy(particle.gameObject, 2f);
    }
    public void DestroyCar()
    {
        foreach (var trail in _trailRenderers)
            trail.transform.SetParent(null);

        SpawnParticle();
        _headlights.DOFade(0, 0.3f).SetLink(gameObject);
        _spriteRenderer.DOFade(0, 0.5f).SetLink(gameObject).SetEase(Ease.OutBounce);

        GetComponent<Collider2D>().enabled = false;
        _rigidBody.constraints = RigidbodyConstraints2D.FreezeAll;

        EnemySpawner.Instance.RemoveEnemy(gameObject);

        if (PlayerCar.Instance != null)
            PlayerCar.Instance.OnEnemyDestroyed(transform);

        Destroy(gameObject, 0.6f);
    } 
    public void UpdateSprite() => _spriteRenderer.sprite = Garage.Instance.GetEnemySprite(Garage.Instance.EnemyId);
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            DestroyCar();
            PlayerCar.Instance.DestroyPlayer();
            AudioManager.Instance.PlaySound(AudioManager.Instance.CarCrash, 0.6f);
        }
        if (collision.gameObject.CompareTag("Enemy"))
        {
            AudioManager.Instance.PlaySound(AudioManager.Instance.CarCrash, 0.6f);
            DestroyCar();

            collision.gameObject.GetComponent<Collider2D>().enabled = false;
            collision.gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;

            EnemySpawner.Instance.RemoveEnemy(collision.gameObject);

            if (PlayerCar.Instance != null)
                PlayerCar.Instance.OnEnemyDestroyed(collision.transform);

            Destroy(collision.gameObject, 0.6f);
        }
    }
}
