using UnityEngine;

namespace Agate.Starcade.Runtime.UI
{
	public class UITapController : MonoBehaviour
	{

#if UNITY_EDITOR
		private Vector2 _screenRes
		{
			get
			{
				return new Vector2(Screen.width, Screen.height);
			}
		}
#else
	private readonly Vector2 _screenRes = new Vector2 (Screen.width, Screen.height);
#endif

		[SerializeField]
		private RectTransform _rtHolder;
		[SerializeField]
		private RectTransform _rtVFXTap;
		[SerializeField]
		private ParticleSystem _vfxTap;

		private Camera _cam;

		void Awake()
		{
			//GameInstance.Instance.UIVFXTap = this;
		}

		public void Emit()
		{
			if (!IsValidCam())
				return;

			Vector2 mousePos = Input.mousePosition;

			Vector2 ratio = _rtHolder.rect.size;
			ratio.x /= _screenRes.x;
			ratio.y /= _screenRes.y;

			Vector2 screenPos = mousePos;
			screenPos.x *= ratio.x;
			screenPos.y *= ratio.y;

			_rtVFXTap.anchoredPosition = screenPos - (_rtHolder.rect.size / 2f);

			_vfxTap.Emit(1);

		}

		private bool IsValidCam()
		{
			if (_cam == null)
			{
				_cam = Camera.main;

				if (Camera.allCameras.Length > 0 && _cam == null)
					_cam = Camera.allCameras[0];
			}

			return _cam != null;
		}
	}

}
