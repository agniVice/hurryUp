using DG.Tweening;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;

    [SerializeField] private float _scaleUp = 7f;
    [SerializeField] private float _scaleDown = 5f;

    [SerializeField] private float _scaleWhenPlayerDestroy = 4f;

    private Camera _camera;

    private Tween _upscaler;
    private Tween _downscaler;

    private Tween _turnLeft;
    private Tween _turnRight;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;

        _camera = GetComponent<Camera>();

        _camera.orthographicSize = _scaleDown;
        _upscaler = _camera.DOOrthoSize(_scaleUp, 3f).SetLink(Camera.main.gameObject).SetEase(Ease.OutBack);
    }
    private void FixedUpdate()
    {
        if (PlayerCar.Instance != null)
        {
            transform.position = new Vector3(PlayerCar.Instance.transform.position.x, PlayerCar.Instance.transform.position.y, transform.position.z);
        }
    }
    public void OnPlayerDestroy()
    {
        _downscaler.Kill();
        _upscaler.Kill();

        _camera.DOShakePosition(0.4f, 0.2f, fadeOut: true).SetUpdate(true);
        _camera.DOShakeRotation(0.4f, 0.2f, fadeOut: true).SetUpdate(true);

        _downscaler = _camera.DOOrthoSize(_scaleWhenPlayerDestroy, 3f).SetLink(Camera.main.gameObject).SetEase(Ease.OutBack);
    }
    public void ScaleUp()
    {
        _downscaler.Kill();
        _upscaler.Kill();
        _upscaler = _camera.DOOrthoSize(_scaleUp, 3f).SetLink(Camera.main.gameObject).SetEase(Ease.OutBack);
    }
    public void TurnLeft()
    {
        _turnLeft.Kill();
        _turnRight.Kill();
        _turnLeft = _camera.transform.DOLocalRotate(new Vector3(0, 0, 5f), 1.5f).SetLink(_camera.gameObject);
    }
    public void TurnRight()
    {
        _turnLeft.Kill();
        _turnRight.Kill();
        _turnLeft = _camera.transform.DOLocalRotate(new Vector3(0, 0, -5f), 1.5f).SetLink(_camera.gameObject);
    }
    public void ResetRotation()
    {
        _turnLeft.Kill();
        _turnRight.Kill();
        _turnLeft = _camera.transform.DOLocalRotate(new Vector3(0, 0, 0), 0.3f).SetLink(_camera.gameObject);
    }
    public void ScaleDown()
    {
        _downscaler.Kill();
        _upscaler.Kill();
        _downscaler = _camera.DOOrthoSize(_scaleDown, 5f).SetLink(Camera.main.gameObject).SetEase(Ease.OutBack);
    }
}
