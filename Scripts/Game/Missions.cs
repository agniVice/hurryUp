using UnityEngine;

public class Missions : MonoBehaviour
{
    public static Missions Instance {  get; private set; }

    public int CoinsCount {get; private set;}
    public int Target { get; private set;}
    public int CurrentExperience { get; private set;}

    [SerializeField] private int _targetCount;
    [SerializeField] private int _experience;

    private int _missionNumber;

    private void Awake()
    {
        if (Instance != this && Instance != null)
            Destroy(gameObject);
        else
            Instance = this;

        Initialize();
    }
    private void Initialize()
    {
        _missionNumber = PlayerPrefs.GetInt("MissionNumber", 1);
        CoinsCount = PlayerPrefs.GetInt("CoinsCount", 0);

        Target = _targetCount * _missionNumber;
        CurrentExperience = _experience * Mathf.CeilToInt((float)_missionNumber / 2);
    }
    public void GetCoins(int count)
    { 
        CoinsCount += count;
        if (CoinsCount >= Target)
        {
            PlayerBalance.Instance.ChangeExperience(CurrentExperience);
            HUD.Instance.UpdateLevel();
            HUD.Instance.OnMissionCompleted();
            _missionNumber++;
            GetMission();
            AudioManager.Instance.PlaySound(AudioManager.Instance.MissionComplete, 1);
        }
        HUD.Instance.UpdateMission();
        Save();
    }
    private void GetMission()
    {
        CoinsCount = 0;
        Target = _targetCount * _missionNumber;
        CurrentExperience = _experience * Mathf.CeilToInt((float)_missionNumber / 2);
        HUD.Instance.UpdateMission();
    }
    private void Save()
    {
        PlayerPrefs.SetInt("MissionNumber", _missionNumber);
        PlayerPrefs.SetInt("CoinsCount", CoinsCount);
    }    
}
