using System.Collections.Generic;
using Agate.Starcade.Scripts.Runtime.Data_Class;
using UnityEngine;

namespace Agate
{
    public class PosterController : MonoBehaviour
    {
        [SerializeField] private Transform _contentTransform;
        [SerializeField] private List<PosterData> _listPosterData;
        [SerializeField] private GameObject _posterPrefab;

        [SerializeField] private GameObject _dummyPoster;
        private float _posterMaxHeight;
        
        void Start()
        {
            //GET MAX POSTER HEIGHT BASED ON DUMMY
            Debug.Log("MAX H = " + _dummyPoster.GetComponent<RectTransform>().sizeDelta.y);
            Debug.Log("MAX H = " + _dummyPoster.GetComponent<RectTransform>().sizeDelta.y);
            _posterMaxHeight = _dummyPoster.GetComponent<RectTransform>().sizeDelta.y;
            //_dummyPoster.gameObject.SetActive(false);
            
            Setup();
        }
        public void Setup()
        {
            foreach (var posterData in _listPosterData)
            {
                GameObject poster = Instantiate(_posterPrefab, _contentTransform);
                    //poster.GetComponent<PosterObject>().SetupPoster(posterData);
            }
        }
    }
}
