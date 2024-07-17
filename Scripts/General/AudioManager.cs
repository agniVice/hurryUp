using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    public bool IsSoundEnabled { get; private set; }
    public bool IsMusicEnabled { get; private set; }

    public AudioClip Engine;
    public AudioClip EngineStart;
    public AudioClip CarCrash;
    public AudioClip Coin;
    public AudioClip Fuel;
    public AudioClip MissionComplete;
    public AudioClip FuelWarning;
    public AudioClip LevelUp;
    public AudioClip GameOver;
    public AudioClip Revive;
    public AudioClip Buy;

    public AudioClip Spin;
    public AudioClip DrumSpin;
    public AudioClip Win;

    [SerializeField] private AudioMixer _audioMixer;
    [SerializeField] private GameObject _sfxPrefab;

    private void Awake()
    {
        if (Instance != this && Instance != null)
            Destroy(gameObject);
        else
            Instance = this;

        DontDestroyOnLoad(gameObject);

        Initialize();
    }
    private void Start()
    {
        UpdateAudio();
    }
    private void Initialize()
    {
        IsSoundEnabled = Convert.ToBoolean(PlayerPrefs.GetInt("IsSoundEnabled", 1));
        IsMusicEnabled = Convert.ToBoolean(PlayerPrefs.GetInt("IsMusicEnabled", 0));
    }
    private void UpdateAudio()
    {
        _audioMixer.SetFloat("Music", -80 * Convert.ToInt32(!IsMusicEnabled));
        _audioMixer.SetFloat("Sound", -80 * Convert.ToInt32(!IsSoundEnabled));
    }
    public void ChangeSound(bool enabled)
    {
        IsSoundEnabled = enabled;
        UpdateAudio();
        Save();
    }
    public void ChangeMusic(bool enabled) 
    {
        IsMusicEnabled = enabled;
        UpdateAudio();
        Save();
    }
    public void PlaySound(AudioClip clip, float volume = 1f, float pitch = 1f, float delay = 0f, bool looped = false)
    {
        var sound = Instantiate(_sfxPrefab).GetComponent<Sound>();
        sound.PlaySound(clip, volume, pitch, delay, looped);
        if(!looped)
            Destroy(sound.gameObject, clip.length + 0.2f);
    }
    private void Save()
    {
        PlayerPrefs.SetInt("IsSoundEnabled", Convert.ToInt32(IsSoundEnabled));
        PlayerPrefs.SetInt("IsMusicEnabled", Convert.ToInt32(IsMusicEnabled));
    }
}
