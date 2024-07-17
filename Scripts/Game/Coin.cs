using DG.Tweening;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Collider2D _collider;

    private CoinSet _set;
    public void Spawn(CoinSet set, float delay)
    {
        _set = set;
        _spriteRenderer.DOFade(1, 0.3f).SetLink(gameObject).SetDelay(delay);
        _spriteRenderer.transform.localScale = Vector3.zero;
        _spriteRenderer.transform.DOScale(1, 0.4f).SetLink(gameObject).SetEase(Ease.OutBack).SetDelay(delay);
    }
    public void DestroyCoin()
    {
        _set.RemoveCoin(this);
        _collider.enabled = false;
        _spriteRenderer.DOFade(0.4f, 0.2f).SetLink(gameObject);
        _spriteRenderer.transform.DOScale(0, 0.4f).SetLink(gameObject).SetEase(Ease.InBack);
        _spriteRenderer.transform.DOMove(PlayerCar.Instance.transform.position, 0.6f).SetLink(gameObject).SetEase(Ease.InOutBack);
        Destroy(gameObject, 1f);
    }
}
