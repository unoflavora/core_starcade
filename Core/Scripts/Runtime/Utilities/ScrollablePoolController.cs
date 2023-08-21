using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Agate.Starcade
{
    public class ScrollablePoolController : MonoBehaviour
    {
        public enum ScrollDirection
        {
            Up,
            Down
        }

        public int PoolSize;
        [SerializeField] private int _row;
        [SerializeField] private int _col;
        [SerializeField] private int _show;
        private double _itemSize;
        private double _itemSpacing;
        private double _itemPadding;

        private int _showRow;
        private int _snapBackAtIndex;

        [SerializeField] private GridLayoutGroup _grid;
        [SerializeField] private RectTransform _content;
        [SerializeField] private Scrollbar _bar;
        [SerializeField] private GameObject _itemPrefab;
        private RectTransform _rectTransform;

        private int _length;
        //private int _totalRowLength;
        private float _lastScrollbarPos;
        private int _currentScrollbarIndex;
        private float _totalLength;
        private float _totalShowLength;
        private CustomScrollRect _scroll;
        private List<GameObject> _pool;
        private bool _canScrollDown = true;
        private bool _canScrollUp = false;
        [HideInInspector] public int StartIndex = 0;
        [HideInInspector] public int EndIndex = 0;

        private ScrollDrag _drag;

        private Vector2 _lastScroll;

        [HideInInspector] public UnityEvent<ScrollDirection, int, int> OnResetScroll;

        [HideInInspector] public UnityEvent<int, GameObject> OnItemActivated;
        [HideInInspector] public UnityEvent<int, GameObject> OnItemDeactivated;

        public void Init(int length)
        {
            _snapBackAtIndex = ((_row / 2) - (_show / _col)/2) - 1;
            _itemSize = _grid.cellSize.y;
            _itemSpacing = _grid.spacing.y;
            _itemPadding = _grid.padding.bottom;
            _showRow = _show / _col;
            _rectTransform = gameObject.GetComponent<RectTransform>();
            _lastScroll = new Vector2(0.5f, 1f);

            _scroll = GetComponent<CustomScrollRect>();
            _scroll.onValueChanged.AddListener(ScrollRectCallBack);

            _drag = _bar.gameObject.GetComponent<ScrollDrag>();
            _bar.onValueChanged.AddListener(ScrollBarCallback);
            _bar.value = 1f;

            _pool = new List<GameObject>();

            ResetPosition(length);

            ScrollRectCallBack(Vector2.one);
        }

        public void ResetPosition(int length)
        {
            _length = length;

            int totalRowLength = (length % _col == 0) ? (length / _col) : (length / _col) + 1;
            _totalLength = (float)(_itemPadding * 2) + ((float)_itemSize * totalRowLength) + ((float)_itemSpacing * (totalRowLength - 1));
            _totalShowLength = (float)(_itemPadding * 2) + ((float)_itemSize * _row) + ((float)_itemSpacing * (_row - 1));
            _lastScrollbarPos = 0;
            _currentScrollbarIndex = 0;
            _bar.size = 0.1f;

            StartIndex = 0;
            EndIndex = 0;

            Debug.Log($"reset pos {_scroll.verticalNormalizedPosition}");
        }

        public void ResetScroll(ScrollDirection direction)
        {
            if (direction == ScrollDirection.Down)
            {
                StartIndex = LimitIndex(EndIndex - ((_row - _snapBackAtIndex) * _col), _length);
                int downRemaining = StartIndex + PoolSize < _length ? StartIndex + PoolSize : _length;
                EndIndex = LimitIndex(downRemaining, _length);
            }
            else
            {
                EndIndex = LimitIndex(StartIndex + (_snapBackAtIndex * _col) + _show, _length);
                int upRemaining = EndIndex - PoolSize > 0 ? EndIndex - PoolSize : 0;
                StartIndex = LimitIndex(upRemaining, _length);
            }
            SetEnableScrollDown(EndIndex < _length);
            SetEnableScrollUp(StartIndex > 0);
            OnResetScroll.Invoke(direction, StartIndex, EndIndex);
            GeneratePool(EndIndex - StartIndex);
        }

        public void GeneratePool(int length)
        {
            if (length <= _pool.Count)
            {
                for (int i = 0; i < length; i++)
                {
                    OnItemActivated.Invoke(i, _pool[i]);
                    _pool[i].gameObject.SetActive(true);
                }
                for (int i = length; i < _pool.Count; i++)
                {
                    OnItemDeactivated.Invoke(i, _pool[i]);
                    _pool[i].gameObject.SetActive(false);
                }
            }
            else
            {
                for (int i = 0; i < _pool.Count; i++)
                {
                    OnItemActivated.Invoke(i, _pool[i]);
                    _pool[i].gameObject.SetActive(true);
                }
                for (int i = _pool.Count; i < length; i++)
                {
                    var gameObject = Instantiate(_itemPrefab);
                    gameObject.GetComponent<RectTransform>().SetParent(_content);
                    gameObject.GetComponent<RectTransform>().localScale = Vector3.one;
                    OnItemActivated.Invoke(i, gameObject);
                    _pool.Add(gameObject);
                }
            }
        }

        private void SetEnableScrollDown(bool enable)
        {
            _canScrollDown = enable;
        }

        private void SetEnableScrollUp(bool enable)
        {
            _canScrollUp = enable;
        }

        public void RemoveAllListeners()
        {
            OnResetScroll.RemoveAllListeners();
            OnItemActivated.RemoveAllListeners();
            OnItemDeactivated.RemoveAllListeners();
        }

        private void ScrollRectCallBack(Vector2 value)
        {
            if (!_drag.isDrag) _bar.value = 1 - ((1 - value.y) * _totalShowLength / _totalLength) - _lastScrollbarPos;

            if (_lastScroll.y > value.y && value.y < 0.1f && _canScrollDown)
            {
                SnapBackScroll(ScrollDirection.Down, _bar.value);
                ResetScroll(ScrollDirection.Down);
            }
            else if (_lastScroll.y < value.y && value.y > 0.9f && _canScrollUp)
            {
                SnapBackScroll(ScrollDirection.Up, _bar.value);
                ResetScroll(ScrollDirection.Up);
            }

            _lastScroll = value;
        }

        private void ScrollBarCallback(float value)
        {
            if (_drag.isDrag)
            {
                float scrollValue = 1 - value;
                float convertedValue = scrollValue * (1 + (1 - _totalShowLength / _totalLength)) - _lastScrollbarPos;
                _scroll.CustomDragSetVerticalNormalizedPosition(1 - convertedValue);
            }
        }

        private void SnapBackScroll(ScrollDirection direction, float lastPos)
        {
            float scrollPos;
            float scrollSize = _scroll.content.rect.size.y;
            if (direction == ScrollDirection.Down)
            {
                float index = _snapBackAtIndex + (_show / _col) - 1;
                scrollPos = (float)_itemPadding + ((index + 0.5f) * (float)_itemSize) + ((index) * (float)_itemSpacing) - (_rectTransform.sizeDelta.y * 0.5f);
                _currentScrollbarIndex++;
                float scrollNextPos = 1 - ((1 - (scrollPos / scrollSize)) * _totalShowLength / _totalLength) - _lastScrollbarPos;
                _lastScrollbarPos += (scrollNextPos - lastPos);
            }
            else
            {
                float index = _snapBackAtIndex;
                scrollPos = (float)_itemPadding + ((index - 1f) * (float)_itemSize) + ((index) * (float)_itemSpacing) + (_rectTransform.sizeDelta.y * 0.5f);
                _currentScrollbarIndex--;
                float scrollNextPos = 1 - ((1 - (scrollPos / scrollSize)) * _totalShowLength / _totalLength) - _lastScrollbarPos;
                if (_currentScrollbarIndex == 0) _lastScrollbarPos = 0;
                else _lastScrollbarPos += (scrollNextPos - lastPos);
            }
            _scroll.CustomDragSetVerticalNormalizedPosition(scrollPos / scrollSize);

            //if (_drag.isDrag)
            //{
            //    _scroll.CustomDragSetVerticalNormalizedPosition(_lastScrollbarPos);
            //}
        }

        private int LimitIndex(int index, int limit)
        {
            if (index > limit) return limit;
            else if (index < 0) return 0;
            else return index;
        }
    }
}
