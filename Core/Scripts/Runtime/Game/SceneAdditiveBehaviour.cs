using Agate.Starcade.Runtime.Helper;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Agate.Starcade.Scripts.Runtime.Game
{
    public abstract class SceneAdditiveBehaviour: MonoBehaviour
    {
        [SerializeField] protected Button _closeButton;
        internal readonly UnityEvent OnOpenPanel = new UnityEvent();
        internal readonly UnityEvent OnClosePanel = new UnityEvent();
        private object _sceneData;
        public void OnEnable()
        {
            _closeButton.onClick.AddListener(ClosePanel);
            //_sceneData = LoadSceneHelper.PullData();
            OnOpenPanel.Invoke();
        }

        public void ClosePanel()
        {
            OnClosePanel.Invoke();
            LoadSceneHelper.CloseSceneAdditive();
        }
    }
}