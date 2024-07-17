using DG.Tweening;
using TMPro;
using UnityEngine;

public class Error : MonoBehaviour
{
    public static Error Instance { get; private set; }

    [SerializeField] private CanvasGroup _errorGroup;
    [SerializeField] private TextMeshProUGUI _countCoins;

    private void Awake()
    {
        if (Instance != this && Instance != null)
            Destroy(gameObject);
        else
            Instance = this;
        GetComponent<Canvas>().worldCamera = Camera.main;
    }
    public void ShowError(int count)
    { 
        _countCoins.text = count.ToString();
        _errorGroup.blocksRaycasts = true;
        _errorGroup.alpha = 0f;
        _errorGroup.DOFade(1, 0.3f).SetLink(gameObject);
    }
    public void HideError()
    {
        _errorGroup.blocksRaycasts = false;
        _errorGroup.DOFade(0, 0.3f).SetLink(gameObject);
    }
}
