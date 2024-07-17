using System.Collections.Generic;
using UnityEngine;

public class ArrowPointer : MonoBehaviour
{
    [SerializeField] private GameObject _arrowPrefab;
    private List<Arrow> _arrows = new List<Arrow>();

    private void Awake()
    {
        GetComponent<Canvas>().worldCamera = Camera.main;
    }
    public void CreateArrow(Transform target, int id)
    {
        var arrow = Instantiate(_arrowPrefab, transform).GetComponent<Arrow>();
        arrow.Initialize(target, id);
        _arrows.Add(arrow);
    }
    public void DestroyArrow(Transform target)
    { 
        for (int i = 0; i < _arrows.Count; i++) 
        {
            if (_arrows[i].Target == target)
            { 
                _arrows[i].DestroyArrow();
                _arrows.Remove(_arrows[i]);
            }
        }
    }
    public void DestroyAll()
    {
        for (int i = 0; i < _arrows.Count; i++)
        {
            _arrows[i].DestroyArrow();
            _arrows.Remove(_arrows[i]);
        }
    }
}
