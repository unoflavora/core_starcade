using Agate.Starcade.Core.Runtime.UI;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Agate.Starcade.Core.Runtime.Pet.Fragment.Object
{
	public class PetFragmentVFX : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _mainParticle;
        [SerializeField] private ParticleSystem _puzzleParticle;
        [SerializeField] private ParticleSystemRenderer _puzzleParticleRenderer;
        [SerializeField] private Transform _container;
        [SerializeField] private Image _puzzleImage;
        [SerializeField] private Image _petImage;

        public async void StartVFX(Sprite spritePet, Sprite spritePetFragment, UnityAction afterVFX)
        {
            _container.gameObject.SetActive(true);

            _petImage.gameObject.SetActive(false);

            _container.GetComponent<CanvasTransition>().TriggerTransition();

            _puzzleImage.sprite = spritePetFragment;
            _puzzleParticleRenderer.material.mainTexture = spritePetFragment.texture;

            _mainParticle.Play();

            UpdatePetImage(spritePet);
            await Task.Delay(6000);
            afterVFX.Invoke();
            _container.gameObject.SetActive(false);
        }

        private async void UpdatePetImage(Sprite sprite)
        {
            await Task.Delay(4000);
            _petImage.gameObject.SetActive(true);

            var tempColor = _petImage.color;
            tempColor.a = 0f;
            _petImage.color = tempColor;

            LeanTween.value(0,1,0.2f).setOnUpdate((float value) =>
            {
                tempColor.a = value;
                _petImage.color = tempColor;
            });

            _petImage.sprite = sprite;
        }

        private Texture2D GetTexture2D(Sprite texture)
        {
            Texture2D texture2D = new Texture2D((int)texture.rect.width, (int)texture.rect.height);
            var pixel = texture.texture.GetPixels((int)texture.textureRect.x, (int)texture.textureRect.y, (int)texture.textureRect.width, (int)texture.textureRect.height);
            texture2D.SetPixels(pixel);
            texture2D.Apply();
            return texture2D;
        }
    }
}