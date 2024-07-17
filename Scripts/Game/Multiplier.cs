using UnityEngine;

public class Multiplier : MonoBehaviour
{
    public static Multiplier Instance {  get; private set; }

    public int CoinsMultiplier { get; private set; } = 1;
    public float TargetTime { get; private set; }

    [SerializeField] private float _multiplierTime;

    private float _currentTime;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }
    private void Start()
    {
        Calculate();
    }
    private void FixedUpdate()
    {
        if (GameState.Instance.CurrentState != GameState.State.InGame)
            return;
        if (_currentTime < TargetTime)
            _currentTime += Time.fixedDeltaTime;
        else
        {
            CoinsMultiplier++;
            Calculate();
        }
    }
    private void Calculate()
    {
        _currentTime = 0;
        TargetTime += _multiplierTime;
        HUD.Instance.UpdateMultiplier();
    }
    public float GetPercent() => _currentTime / TargetTime;
}
