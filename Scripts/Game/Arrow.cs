using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Arrow : MonoBehaviour
{
    public Transform Target;
    [SerializeField] private TextMeshProUGUI _distanceText;
    [SerializeField] private float _distanceFromCenter;
    [SerializeField] private Image _arrowImage;
    [SerializeField] private CanvasGroup _arrowGroup;
    [SerializeField] private Color32 _coinsColor;
    [SerializeField] private Color32 _enemyColor;
    [SerializeField] private Color32 _fuelColor;

    private int _id;

    private void Update()
    {
        if (Target == null)
            return;

        if(_id == 2)
            _distanceText.text = Mathf.Round(Vector2.Distance(PlayerCar.Instance.transform.position, Target.position)) + "m";
        Vector3 direction = Target.position - PlayerCar.Instance.transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        transform.position = PlayerCar.Instance.transform.position + direction.normalized * _distanceFromCenter;
    }
    public void Initialize(Transform target, int id)
    {
        _id = id;
        if (_id == 0)
        {
            _arrowImage.color = _coinsColor;
            _distanceFromCenter *= 1.5f;
        }
        else if (_id == 1)
            _arrowImage.color = _enemyColor;
        else if (_id == 2)
        {
            _arrowImage.color = _fuelColor;
            _distanceFromCenter *= 2.2f;
        }

        _arrowGroup.DOFade(0, 0.01f).SetLink(gameObject);
        _arrowGroup.DOFade(1, 0.5f).SetLink(gameObject).SetDelay(0.05f);
        Target = target;
    }
    public void DestroyArrow()
    {
        Target = null;
        _arrowGroup.DOFade(0, 0.3f).SetLink(gameObject).SetEase(Ease.OutBounce);
        Destroy(gameObject, 0.5f);
    }
}
