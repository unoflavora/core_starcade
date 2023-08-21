using System.Collections.Generic;
using Starcade.Arcades.MT02.Scripts.Runtime;
using UnityEngine;

namespace Agate.Starcade.Scripts.Runtime.Arcade.Monstamatch.Hexacore.Symbols
{
    
    public class MonstamatchSymbol : MonoBehaviour
    {
        // The points on the path that the object will move between
        [SerializeField] public List<Vector3> Paths;
        [SerializeField] SpriteRenderer spriteRenderer;
        [SerializeField] private SpriteRenderer coinRenderer;
        [SerializeField] private Animator _animator;
        // The speed at which the object moves along the path
        [SerializeField] float speedFactor = 2f;
        [SerializeField] private AnimationCurve _animationCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        // The total distance traveled along the path
        public float distanceTraveled;
        
        public string Id => Data == null ? null : Data.Id;
        public Vector3 startingPos;
        public bool isMoving = false;
        private Sprite _sprite;
        private MonstamatchSymbolData _data;
        public MonstamatchSymbolData Data => _data;

        public bool IsMovable;
        private bool _resetPositionAfterFinish;
        
        public void SetupData(MonstamatchSymbolData data = null, List<Vector3> paths = null)
        {
            _data = data;
            Paths = paths == null ? new List<Vector3>() : paths;

            spriteRenderer.sprite = _data == null ? null : _data.sprite;
            SetSymbolSpecial(data != null && data.IsSpecial);
        }

        private void Update()
        {
            Move();
        }

        public void SetPosition(Vector3 position)
        {
            // StartCoroutine(LerpPos(transform.position, position));
            if (isMoving) return;
            transform.position = position;
        }

        public void AddPath(Vector3 fromCoord, Vector3 toCoord, bool resetPositionAfterFinish = false)
        {
            if (Paths.Count == 0)
            {
                transform.position = fromCoord;
                startingPos = fromCoord;
                Paths.Add(fromCoord);
            }

            _resetPositionAfterFinish = resetPositionAfterFinish;
            Paths.Add(toCoord);
        }

        private void Move()
        {
            if(!IsMovable) return;

            // If we haven't reached the end of the path yet
            if (distanceTraveled < GetPathLength())
            {
                isMoving = true;
                // TODO Use a different easing curve that is more similar to Blossom Saga
                float t = _animationCurve.Evaluate(distanceTraveled / GetPathLength());
                transform.position = GetPointOnPath(t);

                // Update the distance traveled along the path, relative to the length of the path
                distanceTraveled += Time.deltaTime * GetPathLength() * speedFactor;
            }
            else
            {
                // reset paths and distance travelled
                if (Paths.Count > 0) Paths = new List<Vector3>();
                if(_resetPositionAfterFinish) transform.localPosition = Vector3.zero;
                distanceTraveled = 0;
                isMoving = false;
            }
        }

        // Returns the total length of the path
        float GetPathLength()
        {
            float length = 0f;
            var index = 0;
            foreach (var path in Paths)
            {
                if (index + 1 < Paths.Count)
                {
                    length += Vector3.Distance(path, Paths[index + 1]);
                    length++;
                }
                index++;
            }
            
            
            return length;
        }

        // Returns the point on the path at the specified normalized distance
        Vector3 GetPointOnPath(float t)
        {
            t = Mathf.Clamp01(t);
            int numPoints = Paths.Count;
            
            int p0 = (int)(t * (numPoints - 1));
            int p1 = p0 + 1;
            
            if (p1 >= numPoints) p1 = p0;
            var res = Vector3.Lerp(Paths[p0], Paths[p1], t * (numPoints - 1) - p0);
            return res;
        }

        public void SetSymbolSpecial(bool isSpecial)
        {
            if (_data != null) _data.IsSpecial = isSpecial;
            
            coinRenderer.enabled = _data != null && isSpecial;
        }

        public void ScaleUp()
        {
            _animator.ResetTrigger("ScaleDown");
            _animator.SetTrigger("ScaleUp");
        }
        
        public void ScaleDown()
        {
            _animator.ResetTrigger("ScaleUp");
            _animator.SetTrigger("ScaleDown");
        }
    }
}
