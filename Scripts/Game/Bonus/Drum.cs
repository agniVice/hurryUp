using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drum : MonoBehaviour
{
    [SerializeField] private List<Element> _elements;
    private Vector2[] _elementPositions;

    private bool _isSpinning;
    private float _delayStart;
    private float _delayEnd;

    private int _countOfSpins;

    private bool _isWinGame;

    private ElementType[] _winTypes = new ElementType[3];

    private int _lineType;

    private void Start()
    {
        InitializeElements();
        Generate();
    }
    private void FixedUpdate()
    {
        if (_isSpinning)
        {
            foreach (var element in _elements)
                element.transform.position += Vector3.down * Time.fixedDeltaTime * 30;
        }
    }
    private void InitializeElements()
    {
        foreach (var element in _elements)
            element.Initialize(this);

        _elementPositions = new Vector2[_elements.Count];
        for (int i = 0; i < _elements.Count; i++)
            _elementPositions[i] = _elements[i].transform.localPosition;
    }
    private void Generate()
    {
        foreach (var element in _elements)
            SetRandomElementType(element);
    }
    private void SetRandomElementType(Element element)
    {
        if (Random.Range(0, 100) <= Bonus.Instance.GetWildChance())
            element.SetElementType(this, ElementType.Wild);
        else
            element.SetElementType(this, (ElementType)Random.Range(0, 9));
    }
    private IEnumerator StartSpin()
    {
        yield return new WaitForSeconds(_delayStart);
        _isSpinning = true;
    }
    private IEnumerator EndSpin()
    {
        yield return new WaitForSeconds(_delayEnd);
        for (int i = 0; i < _elements.Count; i++)
        {
            _elements[i].transform.DOLocalMove(_elementPositions[i], 0.1f)
                .SetLink(_elements[i].gameObject)
                .SetEase(Ease.OutBack)
                .OnKill(() => { Bonus.Instance.ReadyForSpin(Bonus.Instance.GetDrumIndex(this)); });
        }
        _isSpinning = false;
    }
    public void PlayWinFX(int line)
    {
        _elements[line].PlayWinFX();
    }
    public void OnDrumStartWinSpin(float delay, ElementType[] types, int lineType)
    {
        _lineType = lineType;
        _winTypes = types;
        _isWinGame = true;
        OnDrumStartSpin(delay);
    }
    public void OnDrumStartSpin(float delay)
    {
        _delayStart = delay;
        StartCoroutine("StartSpin");
    }
    public void OnDrumEndSpin(float delay)
    {
        _delayEnd = delay;
        StartCoroutine("EndSpin");
    }
    public void OnElementSpin(Element element)
    {
        if (_countOfSpins % 5 == 0)
            AudioManager.Instance.PlaySound(AudioManager.Instance.DrumSpin, 0.6f, Random.Range(0.95f, 1.05f));
        _countOfSpins++;
        _countOfSpins++;
        SetRandomElementType(element);
        if (_countOfSpins == 18 && _isWinGame && (_lineType == 3 || _lineType == 0))
            _elements[0].SetElementType(this, _winTypes[0]);
        if (_countOfSpins == 19 && _isWinGame && (_lineType == 3 || _lineType == 1))
            _elements[0].SetElementType(this, _winTypes[1]);
        if (_countOfSpins == 20 && _isWinGame && (_lineType == 3 || _lineType == 2))
        {
            _elements[0].SetElementType(this, _winTypes[2]);
        }
        if (_countOfSpins == 20)
        {
            _isWinGame = false;
            _countOfSpins = 0;
            _isSpinning = false;
            _delayEnd = 0;
            StartCoroutine("EndSpin");
        }
    }
    public void RemoveElement(Element element) => _elements.Remove(element);
    public void AddElement(Element element) => _elements.Insert(0, element);
    public Vector2 GetLastElementPosition() => _elements[0].transform.localPosition;
    public ElementType GetLineElement(int line) => _elements[line].GetElementType();
}
