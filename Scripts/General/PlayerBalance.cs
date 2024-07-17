using UnityEngine;

public class PlayerBalance : MonoBehaviour
{
    public static PlayerBalance Instance { get; private set; }
    public int Coins { get; private set; }
    public float Experience { get; private set; }
    public float TargetExperience {  get; private set; }
    public int Level { get; private set; }

    [SerializeField] private int _revivePrice;
    [SerializeField] private float _startTargetExperience;
    [SerializeField] private int _rewardPerLevel;

    private void Awake()
    {
        if (Instance != this && Instance != null)
            Destroy(gameObject);
        else
            Instance = this;

        DontDestroyOnLoad(gameObject);

        Initialize();
    }
    private void Initialize()
    {
        Level = PlayerPrefs.GetInt("Level", 1);
        Coins = PlayerPrefs.GetInt("Coins", 0);
        Experience = PlayerPrefs.GetFloat("Experience", 0);
        TargetExperience = _startTargetExperience * Mathf.Ceil((float)Level/2);
    }
    public void ChangeCoins(int count)
    {
        Coins += count;
        Save();
    }
    public void ChangeExperience(float count)
    {
        TargetExperience = _startTargetExperience * Mathf.Ceil((float)Level / 2);
        Experience += count;
        if (Experience >= TargetExperience)
        {
            Coins += GetReward();
            LevelUp.Instance.OpenLevelUp(GetReward());
            float exp = Experience - TargetExperience;
            Level++;
            Experience = exp;
            TargetExperience = _startTargetExperience * Mathf.Ceil((float)Level / 2);
        }
        Save();
    }
    public int GetReward() => _rewardPerLevel * Level;
    public int GetRevivePrice() => _revivePrice;
    private void Save()
    {
        PlayerPrefs.SetInt("Level", Level);
        PlayerPrefs.SetInt("Coins", Coins);
        PlayerPrefs.SetFloat("Experience", Experience);
    }
}
