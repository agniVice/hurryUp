using DG.Tweening;
using UnityEngine;

public class Tutorial : MonoBehaviour, IInitializable
{
    public static Tutorial Instance { get; private set; }

    [SerializeField] private CanvasGroup _tutorialGroup;

    private void Awake()
    {
        if (Instance != this && Instance != null)
            Destroy(gameObject);
        else
            Instance = this;

        GetComponent<Canvas>().worldCamera = Camera.main;
    }
    public void Initialize()
    {
        if (PlayerPrefs.GetInt("IsTutorialComplete", 0) == 0)
            OpenTutorial();
    }
    private void OpenTutorial()
    {
        GameState.Instance.ChangeState(GameState.State.Paused);
        _tutorialGroup.blocksRaycasts = true;
        _tutorialGroup.alpha = 1f;
    }
    public void CloseTutorial()
    {
        GameState.Instance.ChangeState(GameState.State.InGame);
        PlayerPrefs.SetInt("IsTutorialComplete", 1);
        _tutorialGroup.blocksRaycasts = false;
        _tutorialGroup.DOFade(0, 0.4f).SetLink(_tutorialGroup.gameObject);
    }
}
