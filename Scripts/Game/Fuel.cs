using DG.Tweening;
using UnityEngine;

public class Fuel : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Collider2D _collider;

    public void DestroyFuel()
    {
        FuelSpawner.Instance.RemoveFuel(gameObject);
        PlayerCar.Instance.OnFuelDestroyed(transform);
        _collider.enabled = false;
        _spriteRenderer.DOFade(0.4f, 0.2f).SetLink(gameObject);
        _spriteRenderer.transform.DOScale(0, 0.4f).SetLink(gameObject).SetEase(Ease.InBack);
        _spriteRenderer.transform.DOMove(PlayerCar.Instance.transform.position, 0.6f).SetLink(gameObject).SetEase(Ease.InOutBack);
        Destroy(gameObject, 1f);
    }
}
