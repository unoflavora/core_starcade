using Agate.Starcade.Runtime.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Agate.Starcade.Runtime.UI.FX
{
    public class CoinRewardFX : MonoBehaviour
    {
        [Serializable]
        public class Target
        {
            public CurrencyTypeEnum CurrencyTarget;
            public Transform TargetTransform;
            public Texture2D TargetVFXTexture;
            public Sprite TargetIcon;
        }

        [SerializeField] private ParticleSystem _particle;
        [SerializeField] private ParticleSystemRenderer _particleRenderer;
        [SerializeField] private Transform _startPos;
        [SerializeField] private Transform _endPos;
        [SerializeField] private Animator _animator;

        [SerializeField] private AnimationClip _moveAnimation;
        
        [SerializeField] private Animation _animation;

        [SerializeField] private float _coinParticleRate;
        [SerializeField] private float _coinParticleMultiplier;

        [Header("Type")]
        [SerializeField] private List<Target> _targets = new List<Target>();

        private Transform _targetPos;

        private bool _useTargetPos;

        private List<ParticleCollisionEvent> collisionEvents;
        private void Awake()
        {
            _animation = gameObject.GetComponent<Animation>();
            gameObject.transform.position = _startPos.position;
        }

        private void OnEnable()
        {
            
        }

        public void StartFX()
        {
            gameObject.transform.position = _startPos.position;
            gameObject.transform.localScale = Vector3.zero;
            //_animation.Play(_moveAnimation.name);
            _animator.SetTrigger("Splash");
            var _particleEmission = _particle.emission;
            _particleEmission.rateOverTime = _coinParticleRate * +_coinParticleMultiplier;
            _particle.Play();
        }

        public void StartFX(float reward)
        {
            gameObject.SetActive(true);
            gameObject.transform.position = _startPos.position;
            gameObject.transform.localScale = Vector3.zero;
            //_animation.Play(_moveAnimation.name);
            _animator.SetTrigger("Splash");
            var _particleEmission = _particle.emission;
            _particleEmission.rateOverTime = (reward / 1000f) * +_coinParticleMultiplier;
            _particle.Play();
        }

        public void StartFX(float reward, CurrencyTypeEnum currencyType)
        {
            Target target = _targets.Find(targetData => targetData.CurrencyTarget == currencyType);
            _targetPos = target.TargetTransform;
            _particleRenderer.sharedMaterial.mainTexture = target.TargetVFXTexture;
            _useTargetPos = true;

            gameObject.SetActive(true);
            gameObject.transform.position = _startPos.position;
            gameObject.transform.localScale = Vector3.zero;
            //_animation.Play(_moveAnimation.name);
            _animator.SetTrigger("Splash");
            var _particleEmission = _particle.emission;
            
            _particleEmission.rateOverTime = (reward / 1000f) * +_coinParticleMultiplier;
            _particle.Play();
        }

        public void StartFX(float reward, Transform targetPos, Texture2D targetTexture)
        {
            if(targetPos != null)
            {
                _useTargetPos = true;
                _targetPos = targetPos;
            }

            gameObject.SetActive(true);
            gameObject.transform.position = _startPos.position;
            gameObject.transform.localScale = Vector3.zero;
            //_animation.Play(_moveAnimation.name);
            _animator.SetTrigger("Splash");
            var _particleEmission = _particle.emission;
            _particleRenderer.sharedMaterial.mainTexture = targetTexture;
            _particleEmission.rateOverTime = (reward / 1000f) * +_coinParticleMultiplier;
            _particle.Play();
        }

        public void StopFX()
        {
            gameObject.SetActive(false);
            _animator.ResetTrigger("Splash");
            gameObject.transform.position = _startPos.position;
            LeanTween.cancel(gameObject);
            _particle.Stop();
        }

        void Move()
        {
            if (_useTargetPos)
            {
                LeanTween.move(gameObject, _targetPos.position, 1.2f).setEaseInOutCubic();
            }
            else
            {
                LeanTween.move(gameObject, _endPos.position, 1.2f).setEaseInOutCubic();
            }
        }
    }
}


