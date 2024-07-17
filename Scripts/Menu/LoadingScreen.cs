using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private CanvasGroup _loadingGroup;
    [SerializeField] private Image _loadingBar;
    [SerializeField] private TextMeshProUGUI _loadingPercent;

    [SerializeField] private float _loadTime = 2;

    private float _percent;

    private void Start()
    {
        _loadingGroup.alpha = 0f;
        if (GameSettings.Instance.IsFirstPlay)
            Load();
        else
            MenuUI.Instance.OpenMenu();

        GameSettings.Instance.IsFirstPlay = false;
    }
    public void Load()
    {
        _loadingGroup.DOFade(1, 0.2f).SetLink(_loadingBar.gameObject);
        DOTween.To(() => _percent, x => _percent = x, 0.52f, _loadTime * 0.52f).SetEase(Ease.OutCubic).OnUpdate(UpdatePercent);
        DOTween.To(() => _percent, x => _percent = x, 1, _loadTime * 0.3f).SetEase(Ease.OutCubic).OnUpdate(UpdatePercent).SetDelay(_loadTime * 0.7f + 0.5f).OnKill(() => HideLoading());;
    }
    private void UpdatePercent()
    {
        _loadingPercent.text = Mathf.Round(_percent * 100) + "%";
        _loadingBar.fillAmount = _percent;
    }
    private void HideLoading()
    {
        MenuUI.Instance.OpenMenu();
        _loadingGroup.DOFade(0, 0.2f).SetLink(_loadingBar.gameObject);
    }
}
