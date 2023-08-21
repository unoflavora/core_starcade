using UnityEngine;

namespace Agate.Starcade.Runtime.Arcade.General.UI
{
    public class TouchVFX : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private float _size = 1f;
        [SerializeField] private float _distance = 1f;
        private ParticleSystem _touchVFX;

        public void Init()
        {
            _touchVFX = gameObject.GetComponent<ParticleSystem>();
        }

        private void TouchAction(Vector3 position)
        {
            if (_touchVFX == null) return;
            
            _touchVFX.gameObject.transform.position = position;
            if (!_touchVFX.gameObject.activeInHierarchy)
            {
                _touchVFX.transform.localScale = new Vector3(_size, _size, _size);
                _touchVFX.gameObject.SetActive(true);
            }

            _touchVFX.Play(true);
        }

        public void TouchRaycast()
        {
            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            Plane xy = new Plane(Vector3.forward, new Vector3(0, 0, _distance));
            xy.Raycast(ray, out var distance);
            var pos = _camera.ScreenToWorldPoint(Input.mousePosition);
            //if (Physics.Raycast(ray, out hit)) 
                TouchAction(ray.GetPoint(distance));
        }
    }
}