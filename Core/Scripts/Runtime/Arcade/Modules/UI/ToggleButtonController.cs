using UnityEngine;
using UnityEngine.UI;

namespace Agate.Starcade.Scripts.Runtime.Arcade.Global.UI
{
    public class ToggleButtonController : MonoBehaviour
    {
        [SerializeField] private Sprite _offButtonImage;
        [SerializeField] private Sprite _onButtonImage;
        private bool _isActive;

        public bool IsActive
        {
            get => _isActive;
        }

		public Button Button
		{
			get => gameObject.GetComponent<Button>();
		}

		private void Start()
        {
            //gameObject.GetComponent<Toggle>().onValueChanged.AddListener(delegate { ToggleButton(); });
            //gameObject.GetComponent<Button>().onClick.AddListener(ToggleButton);
        }   

        public void SetActive(bool isActive)
        {
            _isActive = isActive;
			gameObject.GetComponent<Image>().sprite = (isActive) ? _onButtonImage : _offButtonImage;
		}
    }
}
