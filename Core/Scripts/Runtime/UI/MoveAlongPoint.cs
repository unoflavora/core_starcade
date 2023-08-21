using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Agate
{
    [ExecuteInEditMode]
    public class MoveAlongPoint : MonoBehaviour
    {
        public List<Transform> ListTarget;
        private int _currentTarget;
        private Vector3[] _vector3s;
        private float distance;
        private void OnEnable()
        {
            _currentTarget = 0;
            gameObject.transform.localPosition = ListTarget[_currentTarget].transform.localPosition;
            Move();
        }

        private void OnDisable()
        {
            LeanTween.cancel(this.gameObject);
            gameObject.GetComponent<TrailRenderer>().Clear();
        }

        private void Move()
        {
            if (_currentTarget == ListTarget.Count - 1)
            {
                distance = Vector3.Distance(this.gameObject.transform.localPosition,
                    ListTarget[0].localPosition);
                _currentTarget = 0;
            }
            else
            {
                distance = Vector3.Distance(this.gameObject.transform.localPosition,
                    ListTarget[_currentTarget+1].localPosition);
                _currentTarget++;
            }
            
            float time = distance / 500;
            LeanTween.moveLocal(gameObject, ListTarget[_currentTarget].localPosition, time).setOnComplete(() =>
            {
                Move();
            });
        }
    }
}
