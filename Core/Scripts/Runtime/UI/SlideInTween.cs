using UnityEngine;
using UnityEngine.Events;

namespace Agate.Starcade
{
    [ExecuteInEditMode]
    public class SlideInTween : MonoBehaviour
    {
        enum MoveDirection
        {
            X,
            Y,
            Both
        }

        [SerializeField] private bool _ignore;
        
        [SerializeField] private MoveDirection moveDirection;
        [SerializeField] public Vector2 StartPosition;
        [SerializeField] public bool UseCurrentPosAsStart;
        [SerializeField] public Vector2 EndPosition;
        [SerializeField] public float duration;

        [SerializeField] public float padding;
        [SerializeField] public Transform _safeArea;

        private UnityAction _onComplete = () => { };

        private Vector3 _padding;

        private bool _isIn = true;

        private void Start()
        {
            if (UseCurrentPosAsStart) gameObject.transform.localPosition = StartPosition;
        }

        private void Update()
        {
            if (_ignore) return;
            if (_safeArea != null)
            {
                _padding = _safeArea.localPosition;
            }
        }

        public void setOnComplete(UnityAction onComplete)
        {
            _onComplete = onComplete;
        }

        public void SlideIn()
        {
            if (_ignore) return;
            _isIn = true;
            switch (moveDirection)
            {
                case MoveDirection.X:
                    this.transform.LeanMoveLocalX(EndPosition.x + _padding.x, duration).setEaseOutExpo().delay = 0.1f;
                    break;
                case MoveDirection.Y:
                    this.transform.LeanMoveLocalY(EndPosition.y + _padding.y, duration).setEaseOutExpo().delay = 0.1f;
                    break;
                case MoveDirection.Both:
                    this.transform.LeanMoveLocal(EndPosition + (Vector2)_padding, duration).setEaseOutExpo().delay = 0.1f;
                    break;
            }
            
        }

        public void SlideOut()
        {
            if (_ignore) return;
            _isIn = false;
            switch (moveDirection)
            {
                case MoveDirection.X:
                    this.transform.LeanMoveLocalX(StartPosition.x + _padding.x, duration).setEaseOutExpo().setOnComplete(() => _onComplete()).delay = 0.1f;
                    break;
                case MoveDirection.Y:
                    this.transform.LeanMoveLocalY(StartPosition.y + _padding.y, duration).setEaseOutExpo().setOnComplete(() => _onComplete()).delay = 0.1f;
                    break;
                case MoveDirection.Both:
                    this.transform.LeanMoveLocal(StartPosition + (Vector2)_padding, duration).setEaseOutExpo().setOnComplete(() => _onComplete()).delay = 0.1f;
                    break;
            }
        }

        public void Slide()
        {
            if (_ignore) return;
            if ((Vector2)transform.localPosition == StartPosition) SlideIn();
            else if((Vector2)transform.localPosition == EndPosition) SlideOut();
            else return;
        }

        public bool IsOnStartPosition()
        {
            return (Vector2)this.gameObject.transform.localPosition == StartPosition;
        }

        public bool IsMoving()
        {
            return LeanTween.isTweening(this.gameObject);
        }

        public void SlideInOut()
        {

        }
        
    }
}
