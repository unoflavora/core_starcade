using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Agate.Starcade
{
    public class ShineUI : MonoBehaviour
    {
        [SerializeField] private float _angle;
        [SerializeField] private float _time;
        [SerializeField] private float _delay;
        [SerializeField] private float _offset;
        

        [SerializeField] private float _parentWidth;
        void Start()
        {
            _parentWidth = gameObject.transform.parent.GetComponent<RectTransform>().sizeDelta.x;
            gameObject.GetComponent<RectTransform>().anchoredPosition  = new Vector2(-_offset,0);
            TweenShine();
        }
        
        void TweenShine()
        {
            LeanTween.value( gameObject, MoveShine, -_offset, _parentWidth + _offset, _time).setEaseInOutCubic().setLoopClamp().delay = _delay;
            
        }
        
        
        void MoveShine(float val)
        {
            gameObject.GetComponent<RectTransform>().anchoredPosition  = new Vector2(val,0);
        }
    }
}
