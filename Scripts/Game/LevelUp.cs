using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelUp : MonoBehaviour
{
    public static LevelUp Instance { get; private set; }

    [SerializeField] private CanvasGroup _levelUpGroup;
    [SerializeField] private List<Transform> _transforms;
    [SerializeField] private TextMeshProUGUI _level;
    [SerializeField] private TextMeshProUGUI _reward;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;

        GetComponent<Canvas>().worldCamera = Camera.main;
    }
    public void OpenLevelUp(int reward)
    {
        AudioManager.Instance.PlaySound(AudioManager.Instance.LevelUp, 1f);

        float delay = 0f;

        foreach (var item in _transforms)
        {
            Vector3 startScale = item.localScale;
            item.localScale = Vector3.zero;
            item.DOScale(startScale, 0.2f).SetLink(item.gameObject).SetEase(Ease.OutBack).SetDelay(delay);
            delay += 0.05f;
        }

        GameState.Instance.ChangeState(GameState.State.Paused);

        _reward.text = reward.ToString();
        _level.text = (PlayerBalance.Instance.Level+1).ToString();

        HUD.Instance.UpdateCoins();

        _levelUpGroup.blocksRaycasts = true;
        _levelUpGroup.alpha = 0f;
        _levelUpGroup.DOFade(1, 0.3f).SetLink(_levelUpGroup.gameObject);
    }
    public void CloseLevelUp() 
    {
        GameState.Instance.ChangeState(GameState.State.InGame);
        _levelUpGroup.blocksRaycasts = false;
        _levelUpGroup.DOFade(0, 0.3f).SetLink(_levelUpGroup.gameObject);
    }
    public void OnBonusGameClicked()
    {
        CloseLevelUp();
        GameState.Instance.ChangeState(GameState.State.Paused);
        BonusUI.Instance.OpenBonus();
    }
}
